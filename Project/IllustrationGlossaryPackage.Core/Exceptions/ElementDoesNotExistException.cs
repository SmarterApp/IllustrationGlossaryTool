using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IllustrationGlossaryPackage.Core.Exceptions
{
    public class ElementDoesNotExistException : Exception
    {
        public ElementDoesNotExistException() : base() { }
        public ElementDoesNotExistException(string message)
            : base(message) { }
    }
}