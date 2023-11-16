using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Denon.DataClasses
{

    // using System.Xml.Serialization;
    // XmlSerializer serializer = new XmlSerializer(typeof(SourceList));
    // using (StringReader reader = new StringReader(xml))
    // {
    //    var test = (SourceList)serializer.Deserialize(reader);
    // }

    [XmlRoot(ElementName = "Source")]
    public class Source
    {

        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "index")]
        public int Index { get; set; }

    }

    [XmlRoot(ElementName = "Zone")]
    public class Zone
    {

        [XmlElement(ElementName = "Source")]
        public List<Source> Source { get; set; }

        [XmlAttribute(AttributeName = "zone")]
        public int ZoneName { get; set; }

        [XmlAttribute(AttributeName = "index")]
        public int Index { get; set; }
    }

    [XmlRoot(ElementName = "SourceList")]
    public class SourceList
    {

        [XmlElement(ElementName = "Zone")]
        public List<Zone> Zone { get; set; }
    }


}
