using System.Xml.Serialization;

namespace Pandv.DataAccess
{
    public class DataConnection
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string ConnectionString { get; set; }
    }
}