using IllustrationGlossaryPackage.Core.Infrastructure;
using IllustrationGlossaryPackage.Dal.Models;
using System.Collections.Generic;
using System.Xml.Linq;

namespace IllustrationGlossaryPackage.Dal.Interfaces
{
    public interface IItemsModifier
    {
        void AddIllustrationsToItems(IEnumerable<Illustration> illustrations, string testPackageFilePath);
        IEnumerable<KeywordListItem> GetKeywordListItems(string testPackageFilePath, string itemsFilePath);
        string GetAttribute(XElement e, string attributeName);
    }
}
