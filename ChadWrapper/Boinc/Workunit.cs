using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ChadWrapper.Boinc
{
    [XmlType("workunit")]
    public class Workunit
    {
        [XmlElement("command_line")]
        public string CommandLine { get; set; }

        public string Serialize()
        {
            XmlSerializer xml = new XmlSerializer(typeof(Workunit));
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
