using IllustrationGlossaryPackage.Dal.Models;
using System.Collections.Generic;
using System.Xml.Linq;

namespace IllustrationGlossaryPackage.Dal.Interfaces
{
    public interface IItemsModifier
    {
        void AddIllustrationsToItems(IEnumerable<Illustration> illustrations, string testPackageFilePath);
        IEnumerable<XDocument> GetContentItems(string testPackageFilePath);
        IEnumerable<XDocument> GetKeywordListItems(string testPackageFilePath);
    }
}
