using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ChadWrapper.Boinc
{
    [XmlType("app_version")]
    public class AppVersion
    {
        [XmlElement("app_name")]
        public string Name { get; set; }
        [XmlElement("version_num")]
        public int VersionNumber { get; set; }
        [XmlElement("file_ref")]
        public FileReference Reference { get; set; }

        public string Serialize()
        {
            XmlSerializer xml = new XmlSerializer(typeof(AppVersion));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            using StringWriter writer = new StringWriter();

            ns.Add("", "");

            xml.Serialize(writer, this, ns);

            string ret = writer.ToString();

            //remove XML version from data
            ret = ret.Substring(ret.IndexOf(Environment.NewLine) + Environment.NewLine.Length);

            if (Environment.NewLine != "\n")
                ret.Replace(Environment.NewLine, "\n");
            return ret;
        }
    }
}
