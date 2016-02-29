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
        public void RecordErrors(IEnumerable<Error> errorsAndWarnings, string directory)
        {
            string errorsFileName = directory + "\\GlossaryUtilityErrors.csv";
            string warningsFileName = directory + "\\GlossaryUtilityWarnings.csv";

            IEnumerable<Error> errors = errorsAndWarnings
                .Where(x => x.type == Error.Type.Error)
                .OrderBy(x => x.CsvLine);
            IEnumerable<Error> warnings = errorsAndWarnings
                .Where(x => x.type == Error.Type.Warning);

            List<string> errorText = new List<string>();
            errorText.Add("Line, Type, Message");
            errorText.AddRange(errors.Select(e => e.CsvLine + "," + e.exception.ToString() + "," + e.Message));

            List<string> warningText = new List<string>();
            warningText.Add("Type, Message");
            warningText.AddRange(warnings.Select(e => e.exception.ToString() + "," + e.Message));

            File.WriteAllLines(errorsFileName, errorText);
            File.WriteAllLines(warningsFileName, warningText);
        }
    }
}
