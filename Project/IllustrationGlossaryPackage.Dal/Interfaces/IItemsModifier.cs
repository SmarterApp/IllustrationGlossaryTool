using IllustrationGlossaryPackage.Core.Infrastructure;
using IllustrationGlossaryPackage.Dal.Models;
using System.Collections.Generic;
using System.IO.Compression;
using System.Xml.Linq;

namespace IllustrationGlossaryPackage.Dal.Interfaces
{
    public interface IItemsModifier
    {
        string GetAttribute(XElement e, string attributeName);
        string GetAttribute(XElement e, XName attributeName);
        void SaveItem(KeywordListItem keywordListItem, ZipArchive testPackageArchive);
        void SaveItem(XDocument document, ZipArchiveEntry zipEntry);
        void MoveMediaFileForIllustration(Illustration illustration, AssessmentItem assessmentItem, ZipArchive testPackageArchive);
        string GetIllustrationCopyToLocation(Illustration illustration, AssessmentItem assessmentItem, ZipArchive testPackageArchive);
    }
}
