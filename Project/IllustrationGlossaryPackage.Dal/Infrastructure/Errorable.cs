using IllustrationGlossaryPackage.Dal.Interfaces;
using IllustrationGlossaryPackage.Dal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IllustrationGlossaryPackage.Dal.Infrastructure
{
    public class Errorable : IErrorable
    {
        protected List<Error> errors = new List<Error>();
        public IEnumerable<Error>  GetErrors()
        {
            return errors;
        }
    }
}
