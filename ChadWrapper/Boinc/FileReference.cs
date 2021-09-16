using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ChadWrapper.Boinc
{
    [XmlType("file_ref")]
    public class FileReference
    {
        [XmlElement("file_name")]
        public string FileName { get; set; }
        [XmlElement("main_program")]
        public string IsMainProgram = "";
    }
}
