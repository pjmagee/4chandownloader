using System;
using System.Xml.Serialization;

namespace Downloader
{
    [Serializable]
    public class BoardThreadFilter
    {
        [XmlAttribute(AttributeName = "minimum")]
        public int ImageCount { get; set; }

        [XmlAttribute(AttributeName = "disabled")]
        public bool Disabled { get; set; }
    }
}
