using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IllustrationGlossaryPackage.Core.Exceptions
{
    class ArchiveAlreadyExistsException : Exception
    {
        public ArchiveAlreadyExistsException() : base()
        {
        }

        public ArchiveAlreadyExistsException(string message)
            : base(Properties.Resources.ArchiveAlreadyExists + message)
        {
        }

        public ArchiveAlreadyExistsException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
