using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Downloader
{
    public class Board
    {
        public string Name { get; set; }
        public bool Repeat { get; set; }
        public int MaxPage { get; set; }
        public bool CreateFolders { get; set; }
        public long? Ticks { get; set; }
        public List<BoardThread> BoardThreads { get; set; }
        public ConcurrentBag<string> Threads { get; set; }
        public int? MinImages { get; set; }
        public int? Sleep { get; set; }

        private DirectoryInfo _directoryInfo;
        public DirectoryInfo DirectoryInfo
        {
            get
            {
                if(!_directoryInfo.Exists)
                {
                    _directoryInfo.Create();
                }
                return _directoryInfo;
            }
            set { _directoryInfo = value; }
        }

        private const string ThreadImageRegex = "\\d{13}.(jpg|png|gif)"; // contains 13 digits followed by a . and ends in a jpg or png or gif
        private const string ThreadLinkRegex = "<a href=\"res/\\d{1,12}\" class=\"replylink\" >Reply</a>";
        private const string ThreadNameRegex = "\\d{1,12}"; // contains digits 1 to 12 long
        private string ImageLinkRegex
        {
            get { return "<a href=\"//images.4chan.org/" + Name + "/src/\\d{10,16}.(jpg|png|gif)\" target=\"_blank\">"; }
        }

        public Board(string name = "", bool repeat = false, int maxPage = 5, bool createFolders = true)
        {
            Name = name;
            Repeat = repeat;
            MaxPage = maxPage;
            CreateFolders = createFolders;
            BoardThreads = new List<BoardThread>();
            Threads = new ConcurrentBag<string>();
        }

        public void GatherThreads()
        {
            // Page 0 -> Page 15
            Parallel.For(fromInclusive: 0, toExclusive: MaxPage, body: (i, a) =>
            {
                try
                {
                    Console.WriteLine(string.Format("STGR http://boards.4chan.org/{0}/{1}", Name, i));
                    string html = new WebClient().DownloadString(string.Format("http://boards.4chan.org/{0}/{1}", Name, i));
                    var matchCollection = Regex.Matches(html, ThreadLinkRegex);
                    matchCollection.Cast<Match>().ToList().ForEach(thread => Threads.Add(Regex.Match(thread.Value, ThreadNameRegex).Value));
                    Console.WriteLine(string.Format("FNGT http://boards.4chan.org/{0}/{1}", Name, i));
                }
                catch(Exception e)
                {
                    Console.WriteLine(string.Format("FTGT http://boards.4chan.org/{0}/{1}][{2}", Name, i, e.Message));
                }     
            });
        }

        public void ProcessThreads()
        {
            var threadFolders = Threads
                .Select(thread => new BoardThread()
                                      {
                                          Parent = this,
                                          Name = thread,
                                          DirectoryInfo = CreateFolders ? DirectoryInfo.CreateSubdirectory(thread) : DirectoryInfo,
                                      });

            BoardThreads = new List<BoardThread>(threadFolders);

            var groups = BoardThreads.Select((x, i) => new { Index = i, Value = x })
                    .GroupBy(x => x.Index / 5) // Sets of 5 would be nice...
                    .Select(x => x.Select(v => v.Value).ToList()).ToList();

            foreach(var group in groups)
            {
                Parallel.ForEach(source: group, body: ProcessThread);
            }

            
        }


        private void ProcessThread(BoardThread thread)
        {
            try
            {
                string html = new WebClient().DownloadString(address: new Uri(string.Format(@"http://boards.4chan.org/{0}/res/{1}", Name, thread.Name)));

                MatchCollection links = Regex.Matches(html, ImageLinkRegex, RegexOptions.IgnoreCase);

                if(MinImages.HasValue)
                {
                    if(links.Count < MinImages.Value)
                    {
                        Console.WriteLine(string.Format(@"DNMF http://boards.4chan.org/{0}/res/{1}", Name, thread.Name));

                        if(!thread.DirectoryInfo.Equals(DirectoryInfo))
                            thread.DirectoryInfo.Delete();
                            
                        return;
                    }
                }

                var groups = links.Cast<Match>().AsParallel().Select((x, i) => new { Index = i, Value = x })
                    .GroupBy(x => x.Index / 5) // Sets of 10 would be nice...
                    .Select(x => x.Select(v => v.Value).ToList()).ToList();

                foreach (var group in groups)
                {
                    Console.WriteLine("Processing the next 5 images...");
                    // Send in a set of 10 at once...
                    Parallel.ForEach(group, link =>
                    {
                        string image = Regex.Match(link.Value, ThreadImageRegex).Value;

                        try
                        {
                            Console.WriteLine(string.Format(@"DLNG http://images.4chan.org/{0}/src/{1}", Name, image));
                            new WebClient().DownloadFile(new Uri(string.Format(@"http://images.4chan.org/{0}/src/{1}", Name, image)), thread.DirectoryInfo.FullName + "\\" + image);
                        }
                        catch (WebException e)
                        {
                            var httpWebResponse = e.Response as HttpWebResponse;

                            if (httpWebResponse != null)
                            {
                                Console.WriteLine(string.Format(@"FTDL http://images.4chan.org/{0}/src/{1} - {2}", Name, image, (int)(httpWebResponse).StatusCode));
                            }
                        }
                    });
                }
            }
            catch (WebException e)
            {
                var httpWebResponse = e.Response as HttpWebResponse;

                if (httpWebResponse != null)
                {
                    Console.WriteLine(string.Format(@"FTDL http://boards.4chan.org/{0}/res/{1} - {2}", Name, thread.Name, (int)(httpWebResponse).StatusCode));
                }
            }

        }

    }
}
