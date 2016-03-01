using IllustrationGlossaryPackage.Dal.Interfaces;
using IllustrationGlossaryPackage.Dal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IllustrationGlossaryPackage.Core.Interfaces
{
    public interface IGlossaryAugmenter : IErrorable
    {
        void AddItemsToGlossary(string testPackageFilePath, string itemsFilePath);
    }
}
