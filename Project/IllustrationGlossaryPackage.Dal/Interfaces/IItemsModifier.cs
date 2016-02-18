using IllustrationGlossaryPackage.Core.Infrastructure;
using IllustrationGlossaryPackage.Dal.Models;
using System.Collections.Generic;
using System.IO.Compression;
using System.Xml.Linq;

namespace IllustrationGlossaryPackage.Dal.Interfaces
{
    public interface IItemsModifier
    {
        IEnumerable<KeywordListItem> GetKeywordListItems(string testPackageFilePath, string itemsFilePath);
        string GetAttribute(XElement e, string attributeName);
        void SaveItem(KeywordListItem keywordListItem, ZipArchive testPackageArchive);
        void MoveMediaFileForIllustration(Illustration illustration, KeywordListItem keywordListItem, ZipArchive testPackageArchive);
    }
}
