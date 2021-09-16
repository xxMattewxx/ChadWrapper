using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ChadWrapper.Boinc
{
    [XmlType("file_info")]
    public class BoincFileInfo
    {
        [XmlElement("name")]
        public string Name { get; set; }
        [XmlElement("url")]
        public string URL { get; set; }
        [XmlElement("executable")]
        public string IsExecutable { get; set; }
        [XmlElement("file_signature")]
        public string FileSignature { get; set; }
        [XmlElement("nbytes")]
        public Int64 ByteCount { get; set; }

        public string Serialize()
        {
            XmlSerializer xml = new XmlSerializer(typeof(BoincFileInfo));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            using StringWriter writer = new StringWriter();

            ns.Add("", "");

            xml.Serialize(writer, this, ns);

            string ret = writer.ToString();

            //remove XML version from data
            ret = ret.Substring(ret.IndexOf(Environment.NewLine) + Environment.NewLine.Length);

            if(Environment.NewLine != "\n")
                ret.Replace(Environment.NewLine, "\n");
            return ret;
        }
    }
}
