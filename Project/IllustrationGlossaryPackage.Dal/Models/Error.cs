using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IllustrationGlossaryPackage.Dal.Models
{
    public class Error
    {
        public enum Type { FileDNE, InvalidCsvLine, ItemDNE };
        public Type type;
        public string Message;
        public int CsvLine;

        public Error() { }

        public Error(Type t, string m) : this(t, m, 0) { }

        public Error(Type t, string m, int i)
        {
            this.CsvLine = i;
            this.type = t;
            this.Message = m;
        }

    }
}
