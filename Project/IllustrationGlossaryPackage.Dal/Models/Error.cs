using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IllustrationGlossaryPackage.Dal.Models
{
    public class Error
    {
        public enum Exception { FileDNE, InvalidCsvLine, ItemDNE, OverwriteWarning, IllustrationSize};
        public enum Type { Error, Warning };
        public Exception exception;
        public Type type;
        public string Message;
        public int CsvLine;

        public Error() { }

        public Error(Exception e, string m, int i) : this(e, m, i, Type.Error) { }
        public Error(Exception e, string m, int i, Type t)
        {
            this.CsvLine = i;
            this.exception = e;
            this.Message = m;
            this.type = t;
        }

    }
}
