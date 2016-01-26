using IllustrationGlossaryPackage.Dal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IllustrationGlossaryPackage.Dal.Interfaces
{
    public interface IIllustrationGlossaryParser
    {
        IEnumerable<Illustration> GetIllustrationsFromSpreadsheet(string csvFilePath);
    }
}
