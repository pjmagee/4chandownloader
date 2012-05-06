using System.IO;
using NUnit.Framework;

namespace Downloader.Tests
{
    [TestFixture]
    public class BoardTests
    {
        private Board board;

        [SetUp]
        public void SetUp()
        {
            board = new Board
            {
                Name = "a",
                MaxPage = 1,
                CreateFolders = true,
                DirectoryInfo = new DirectoryInfo(@"C:\Temp\a")
            };
        }

        [Test]
        public void BoardTest()
        {
            Assert.NotNull(board.DirectoryInfo);
            Assert.IsEmpty(board.BoardThreads);
            Assert.IsEmpty(board.Threads);
        }

        [Test]
        public void GatherThreadsTest()
        {
            Assert.IsEmpty(board.Threads);
            board.GatherThreads();
            Assert.IsNotEmpty(board.Threads);
            Assert.AreEqual(board.Threads.Count, 10);
        }

        [Test]
        public void ProcessThreads()
        {
           foreach(var thread in board.BoardThreads)
           {
               Assert.False(thread.DirectoryInfo.Exists);
           }

            board.GatherThreads();

            foreach(var thread in board.BoardThreads)
            {
                Assert.False(thread.DirectoryInfo.Exists);
            }

            board.ProcessThreads();

            foreach(var thread in board.BoardThreads)
            {
                if(board.CreateFolders)
                {
                    Assert.True(thread.DirectoryInfo.Exists);
                    Assert.AreNotSame(board.DirectoryInfo, thread.DirectoryInfo);
                }
                else
                {
                    Assert.AreSame(board.DirectoryInfo, thread.DirectoryInfo);
                }     
            }

        }

        [TearDown]
        public void TearDown()
        {
            if(board.DirectoryInfo.Exists)
            {
                board.DirectoryInfo.Delete(true);
            }
        }

    }
}