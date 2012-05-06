using System;
using System.Xml.Serialization;

namespace Downloader
{
    [Serializable]
    public class BoardTimer
    {
        [XmlAttribute(AttributeName = "days")]
        public int Days { get; set; }

        [XmlAttribute(AttributeName = "hours")]
        public int Hours { get; set; }

        [XmlAttribute(AttributeName = "minutes")]
        public int Minutes { get; set; }

        [XmlAttribute(AttributeName = "seconds")]
        public int Seconds { get; set; }

        [XmlAttribute(AttributeName = "disabled")]
        public bool Disabled { get; set; }
    }
}