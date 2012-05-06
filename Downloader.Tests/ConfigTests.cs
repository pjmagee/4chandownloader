using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using NUnit.Framework;

namespace Downloader.Tests
{
    [TestFixture]
    public class ConfigTests
    {

        [Test]
        public void SerialiseTest()
        {
            BoardSetup a = new BoardSetup
            {
                Name = "a",
                Timer = null,
                Create = true,
                Disabled = false,
                Pages = 5,
                Repeat = true,
                SaveDirectory = @"C:\Temp\",
                Sleep = 20
            };

            BoardSetup b = new BoardSetup
            {
                Name = "b",
                Timer = new BoardTimer { Days = 0, Hours = 0, Minutes = 0, Seconds = 0, Disabled = true },
                Create = true,
                Disabled = false,
                Pages = 5,
                Repeat = true,
                SaveDirectory = @"C:\Temp\",
                Filter = new BoardThreadFilter { ImageCount = 3 },
                Sleep = 30
            };

            BoardSetup c = new BoardSetup
            {
                Name = "c",
                Timer = new BoardTimer { Days = 0, Hours = 0, Minutes = 0, Seconds = 0, Disabled = true },
                Create = true,
                Disabled = true,
                Pages = 10,
                Repeat = false,
                SaveDirectory = @"C:\Temp2\",
                Filter = new BoardThreadFilter { ImageCount = 10 },
                Sleep = 30
            };

            BoardSetup d = new BoardSetup
            {
                Name = "d",
                Timer = new BoardTimer { Days = 0, Hours = 0, Minutes = 20, Seconds = 0, Disabled = false },
                Create = false,
                Disabled = false,
                Pages = 5,
                Repeat = true,
                SaveDirectory = @"C:\Temp2\",
                Filter = new BoardThreadFilter { ImageCount = 3 },
                Sleep = 60
            };

            BoardSetup e = new BoardSetup
            {
                Name = "e",
                Timer = new BoardTimer { Days = 0, Hours = 0, Minutes = 20, Seconds = 0, Disabled = true },
                Create = false,
                Disabled = false,
                Pages = 15,
                Repeat = true,
                SaveDirectory = @"C:\Temp2\",
                Filter = new BoardThreadFilter { ImageCount = 3 },
                Sleep = 10
            };

            var config = new BoardConfig { Boards = new List<BoardSetup>() { a, b, c, d } };

            using (var writer = new StreamWriter(Environment.CurrentDirectory + "\\config.xml"))
            {
                new XmlSerializer(typeof(BoardConfig)).Serialize(writer, config);
            }

            using (var reader = new StreamReader(Environment.CurrentDirectory + "\\config.xml"))
            {
                object deserialize = new XmlSerializer(typeof(BoardConfig)).Deserialize(reader);

                config = deserialize as BoardConfig;

                Assert.NotNull(config);
            }

            foreach(var board in config.Boards)
            {
                Assert.NotNull(board.Timer);
            }
        }
    }
}
