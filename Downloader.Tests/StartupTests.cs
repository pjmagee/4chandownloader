using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using NUnit.Framework;

namespace Downloader.Tests
{
    [TestFixture]
    public class StartupTests
    {
        [Test]
        public void Test()
        {
            BoardConfig config;

            using (var reader = new StreamReader(Environment.CurrentDirectory + "\\config.xml"))
            {
                config = (BoardConfig)new XmlSerializer(typeof(BoardConfig)).Deserialize(reader);
            }

            var boards = new List<Board>();

            foreach (var configBoard in config.Boards)
            {
                Board board = new Board();
                board.Name = configBoard.Name;
                board.Repeat = configBoard.Repeat;
                board.CreateFolders = configBoard.Create;
                board.DirectoryInfo = new DirectoryInfo(configBoard.SaveDirectory + "\\" + configBoard.Name);
                board.Sleep = configBoard.Sleep;
                board.MaxPage = configBoard.Pages;

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
    }
}
