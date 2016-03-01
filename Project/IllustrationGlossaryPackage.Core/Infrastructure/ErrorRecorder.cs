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

            WriteErrorsToFile(errorsAndWarnings.Where(x => x.type == Error.Type.Error), errorsFileName);
            WriteErrorsToFile(errorsAndWarnings.Where(x => x.type == Error.Type.Warning), warningsFileName);
        }

        private void WriteErrorsToFile(IEnumerable<Error> errors, string file)
        {
            errors = errors.OrderBy(x => x.CsvLine);

            List<string> errorText = new List<string>();
            errorText.Add("Line, Type, Message");
            errorText.AddRange(errors.Select(e => e.CsvLine + "," + e.exception.ToString() + "," + e.Message));

            File.WriteAllLines(file, errorText);
        }
    }
}
