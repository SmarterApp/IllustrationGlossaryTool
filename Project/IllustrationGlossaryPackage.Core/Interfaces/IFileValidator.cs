using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IllustrationGlossaryPackage.Core.Interfaces
{
    public interface IFileValidator
    {
        void ValidateTestPackage(string testPackage);
        void ValidateIllustrationSpreadsheet(string csvPath);
    }
}
