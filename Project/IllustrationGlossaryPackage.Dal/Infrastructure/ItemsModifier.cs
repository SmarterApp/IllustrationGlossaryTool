using IllustrationGlossaryPackage.Core.Infrastructure;
using IllustrationGlossaryPackage.Dal.Extensions;
using IllustrationGlossaryPackage.Dal.Interfaces;
using IllustrationGlossaryPackage.Dal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace IllustrationGlossaryPackage.Dal.Infrastructure
{
    public class ItemsModifier : Errorable, IErrorable, IItemsModifier
    {
        private IIllustrationGlossaryParser glossaryParser;
        public ItemsModifier() : this(new IllustrationGlossaryParser()) { }
        public ItemsModifier(IllustrationGlossaryParser glossaryParser)
        {
            this.glossaryParser = glossaryParser;
        }


        /// <summary>
        /// Saves a keywordlist item to a zip archive
        /// </summary>
        /// <param name="keywordListItem"></param>
        /// <param name="testPackageArchive"></param>
        public void SaveItem(KeywordListItem keywordListItem, ZipArchive testPackageArchive)
        {
            ZipArchiveEntry itemXmlEntry = SelectItemZipEntry(keywordListItem.FullPath, testPackageArchive);
            SaveItem(keywordListItem.Document, itemXmlEntry);
        }

        public void SaveItem(XDocument document, ZipArchiveEntry zipEntry)
        {
            using (StreamWriter writer = new StreamWriter(zipEntry.Open()))
            {
                document.Document.Save(writer);
            }
        }

        /// <summary>
        /// Moves an illustration to the appropreate keywordlist item in the archive
        /// </summary>
        /// <param name="illustration"></param>
        /// <param name="keywordListItem"></param>
        /// <param name="testPackageArchive"></param>
        public void MoveMediaFileForIllustration(Illustration illustration, AssessmentItem assessmentItem, ZipArchive testPackageArchive)
        {
            ZipArchiveEntry existingIll = illustration.GetZipArchiveEntry(testPackageArchive);
            if(existingIll != null)
            {
                errors.Add(new Error(Error.Exception.OverwriteWarning, "Overwriting image file: " + illustration.CopiedToPath, illustration.LineNumber, Error.Type.Warning));
                existingIll.Delete();
            }

            testPackageArchive.CreateEntryFromFile(illustration.OriginalFilePath, illustration.CopiedToPathForCreate);
        }
        
        /// <summary>
        /// selects and archive entry from an archive
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="testPackageArchive"></param>
        /// <returns></returns>
        private ZipArchiveEntry SelectItemZipEntry(string filePath, ZipArchive testPackageArchive)
        {
            return testPackageArchive.Entries.FirstOrDefault(x => x.FullName == filePath);
        }

    }
}
