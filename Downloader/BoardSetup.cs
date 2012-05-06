using System;
using System.Xml.Serialization;

namespace Downloader
{
    [Serializable]
    public class BoardSetup
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "pages")]
        public int Pages { get; set; }

        [XmlAttribute(AttributeName = "folders")]
        public bool Create { get; set; }

        [XmlAttribute(AttributeName = "continuous")]
        public bool Repeat { get; set; }

        [XmlAttribute(AttributeName = "directory")]
        public string SaveDirectory { get; set; }

        [XmlAttribute(AttributeName = "disabled")]
        public bool Disabled { get; set; }
   
        [XmlAttribute(AttributeName = "snooze")]
        public int Sleep { get; set; }

        [XmlElement(ElementName = "timer")]
        public BoardTimer Timer { get; set; }

        [XmlElement(ElementName = "constraint")]
        public BoardThreadFilter Filter { get; set; }

        public BoardSetup()
        {
            Timer = new BoardTimer() {Days = 0, Hours = 0, Minutes = 0, Seconds = 0, Disabled = true};
            Filter = new BoardThreadFilter() {ImageCount = 0, Disabled = true};
        }

    }
}