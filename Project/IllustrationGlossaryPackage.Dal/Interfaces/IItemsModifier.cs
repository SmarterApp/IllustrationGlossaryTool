using IllustrationGlossaryPackage.Core.Infrastructure;
using IllustrationGlossaryPackage.Dal.Infrastructure;
using IllustrationGlossaryPackage.Dal.Models;
using System.Collections.Generic;
using System.IO.Compression;
using System.Xml.Linq;

namespace IllustrationGlossaryPackage.Dal.Interfaces
{
    public interface IItemsModifier : IErrorable
    {
        void SaveItem(KeywordListItem keywordListItem, ZipArchive testPackageArchive);
        void SaveItem(AssessmentItem ai, ZipArchive testPackageArchive);
        void SaveItem(XDocument document, ZipArchiveEntry zipEntry);
        void MoveMediaFileForIllustration(Illustration illustration, AssessmentItem assessmentItem, ZipArchive testPackageArchive);
    }
}
