using System.Collections.Generic;

namespace IllustrationGlossaryPackage.Core.Infrastructure
{
    public interface IItemsProcessor
    {
        IEnumerable<KeywordListItem> GetKeywordListItems(string testPackageFilePath, string itemsFilePath);
    }
}