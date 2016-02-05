using IllustrationGlossaryPackage.Dal.Models;
using System.Collections.Generic;

namespace IllustrationGlossaryPackage.Dal.Interfaces
{
    public interface IItemsModifier
    {
        void AddIllustrationsToItems(IEnumerable<Illustration> illustrations, string testPackageFilePath);
    }
}
