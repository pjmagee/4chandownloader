using System.IO;

namespace Downloader
{
    public class BoardThread
    {
        public string Name { get; set; }
        public DirectoryInfo DirectoryInfo { get; set; }
        public Board Parent { get; set; }
    }
}
