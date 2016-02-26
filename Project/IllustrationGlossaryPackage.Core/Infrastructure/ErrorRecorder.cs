using IllustrationGlossaryPackage.Core.Interfaces;
using IllustrationGlossaryPackage.Dal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IllustrationGlossaryPackage.Core.Infrastructure
{
    public class ErrorRecorder : IErrorRecorder
    {
        public void RecordErrors(IEnumerable<Error> errors, string directory)
        {
            string fileName = directory + "\\GlossaryUtilityErrors.csv";
            errors = errors.OrderBy(x => x.CsvLine);
            List<string> errorText = new List<string>();
            errorText.Add("Line, Type, Message");
            errorText.AddRange(errors.Select(e => e.CsvLine + "," + e.type.ToString() + "," + e.Message));
            File.WriteAllLines(fileName, errorText);
        }
    }
}
