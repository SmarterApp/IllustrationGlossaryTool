using IllustrationGlossaryPackage.Dal.Interfaces;
using IllustrationGlossaryPackage.Dal.Models;
using System.Collections.Generic;

namespace IllustrationGlossaryPackage.Core.Infrastructure
{
    public interface IItemsProcessor : IErrorable
    {
        IEnumerable<KeywordListItem> GetKeywordListItems(string testPackageFilePath, string itemsFilePath);
    }
}