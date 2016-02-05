using IllustrationGlossaryPackage.Dal.Models;
using System.Collections.Generic;

namespace IllustrationGlossaryPackage.Dal.Interfaces
{
    public interface IIllustrationGlossaryParser
    {
        IEnumerable<Illustration> GetIllustrationsFromSpreadsheet(string csvFilePath);
    }
}
