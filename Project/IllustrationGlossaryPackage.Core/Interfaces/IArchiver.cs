using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IllustrationGlossaryPackage.Core.Interfaces
{
    public interface IArchiver
    {
        void CreateArchive(string zipFile);
    }
}
