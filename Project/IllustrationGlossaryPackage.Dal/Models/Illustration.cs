using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IllustrationGlossaryPackage.Dal.Models
{
    public class Illustration
    {
        public string ItemId { get; set; }
        public string Term { get; set; }
        public string FileName { get; set; }
        public bool FileExists { get; set; }
        public string CopiedToPath { get; set; }
        public string Identifier { get; set; }
        public int LineNumber { get; set; }
    }
}
