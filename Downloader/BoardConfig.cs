using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Downloader
{
    [Serializable]
    [XmlRoot(ElementName = "Config")]
    public class BoardConfig
    {
        [XmlArray(ElementName = "boards")]
        [XmlArrayItem(ElementName = "board")]
        public List<BoardSetup> Boards { get; set; }

        public BoardConfig()
        {
            Boards = new List<BoardSetup>();
        }
    }
}
