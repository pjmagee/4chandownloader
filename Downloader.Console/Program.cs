using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Downloader.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
        theTop:

            var boards = new List<Board>();

            try
            {
                BoardConfig config;
                using (var reader = new StreamReader(Environment.CurrentDirectory + "\\config.xml"))
                {
                    config = (BoardConfig)new XmlSerializer(typeof(BoardConfig)).Deserialize(reader);
                }

                foreach (var configBoard in config.Boards.Where(b => !b.Disabled))
                {
                    var board = new Board
                    {
                        Name = configBoard.Name,
                        Repeat = configBoard.Repeat,
                        CreateFolders = configBoard.Create,
                        DirectoryInfo = new DirectoryInfo(configBoard.SaveDirectory + "\\" + configBoard.Name),
                        Sleep = configBoard.Sleep * 1000,
                        MaxPage = configBoard.Pages
                    };

                    if (!configBoard.Filter.Disabled)
                    {
                        board.MinImages = configBoard.Filter.ImageCount;
                    }

                    if (!configBoard.Timer.Disabled)
                    {
                        board.Ticks = DateTime.Now.Ticks +
                                      new TimeSpan(configBoard.Timer.Days, configBoard.Timer.Hours,
                                                   configBoard.Timer.Minutes, configBoard.Timer.Seconds).Ticks;
                    }

                    boards.Add(board);
                }
            }
            catch(FileNotFoundException e)
            {
                MessageBox.Show(e.Message);
            }

            var processed = new ConcurrentDictionary<string, long>();

            Parallel.ForEach(boards, board =>
            {
                if (!processed.ContainsKey(board.Name))
                {
                    // we store this as a base point to know how many images this board had before beginning its sweep
                    // then we compare this value to its 'current value'
                    processed.TryAdd(board.Name, board.DirectoryInfo.GetFiles("*.*", SearchOption.AllDirectories).Count());
                }

                do
                {
                    board.GatherThreads();
                    board.ProcessThreads();

                    if (board.Ticks.HasValue)
                    {
                        if (DateTime.Now.Ticks > board.Ticks.Value)
                            board.Repeat = false;

                        System.Console.WriteLine("TRFT " + board.Name + ": " + TimeSpan.FromTicks(board.Ticks.Value - DateTime.Now.Ticks).Duration().ToString("G"));
                    }

                    // Clean out any corrupt images
                    ParallelQuery<FileInfo> parallelQuery = board.DirectoryInfo.GetFiles("*.*").AsParallel().Where(f => f.Length == 0);
                    Parallel.ForEach(parallelQuery, file => file.Delete());

                    if (board.Sleep.HasValue)
                    {
                        System.Console.WriteLine("SNZ " + board.Name);
                        Thread.Sleep(board.Sleep.Value);
                        System.Console.WriteLine("AWK " + board.Name);
                    }

                } while (board.Repeat);

            });

            foreach (var board in boards)
            {
                System.Console.WriteLine("Board " + board.Name.ToUpper() + " processed " + (board.DirectoryInfo.GetFiles("*.*", SearchOption.AllDirectories).Count() - processed[board.Name] + " images"));
            }

            System.Console.WriteLine("Downloader tasks finished!");

            System.Console.WriteLine("Repeat? y/n");
            ConsoleKeyInfo consoleKeyInfo = System.Console.ReadKey();
            System.Console.WriteLine("\n");

            // Repeat from the top!
            if (consoleKeyInfo.Key == ConsoleKey.Y)
                goto theTop;

            System.Console.WriteLine("\r\nDownloader closing!");
            System.Console.WriteLine("May the force be with you ~");
            Thread.Sleep(5000);

        }
    }
}
