using IllustrationGlossaryPackage.Dal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IllustrationGlossaryPackage.Core.Interfaces
{
    public interface IErrorRecorder
    {
        void RecordErrors(IEnumerable<Error> errors, string directory);
        void RemoveExistingErrors(string errorsDirectory);
    }
}
