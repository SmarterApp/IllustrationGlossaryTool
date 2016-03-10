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
        static string errorsFileName = "\\GlossaryUtilityErrors.csv";
        static string warningsFileName = "\\GlossaryUtilityWarnings.csv";

        public void RecordErrors(IEnumerable<Error> errorsAndWarnings, string directory)
        {
            WriteErrorsToFile(errorsAndWarnings.Where(x => x.type == Error.Type.Error), directory + errorsFileName);
            WriteErrorsToFile(errorsAndWarnings.Where(x => x.type == Error.Type.Warning), directory + warningsFileName);
        }

        private void WriteErrorsToFile(IEnumerable<Error> errors, string file)
        {
            errors = errors.OrderBy(x => x.CsvLine);

            List<string> errorText = new List<string>();
            errorText.Add("Line, Type, Message");
            errorText.AddRange(errors.Select(e => e.CsvLine + "," + e.exception.ToString() + "," + e.Message));

            File.WriteAllLines(file, errorText);
        }

        public void RemoveExistingErrors(string errorsDirectory)
        {
            File.Delete(errorsDirectory + errorsFileName);
            File.Delete(errorsDirectory + warningsFileName);
        }
    }
}
