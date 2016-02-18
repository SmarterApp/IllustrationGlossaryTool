using IllustrationGlossaryPackage.Core.Infrastructure;
using IllustrationGlossaryPackage.Dal.Interfaces;
using IllustrationGlossaryPackage.Dal.Models;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace IllustrationGlossaryPackage.Dal.Infrastructure
{
    public class ItemsModifier : IItemsModifier
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
            StreamWriter writer = new StreamWriter(itemXmlEntry.Open());
            writer.BaseStream.Seek(0, SeekOrigin.Begin);
            keywordListItem.Document.Save(writer);
        }

        /// <summary>
        /// Moves an illustration to the appropreate keywordlist item in the archive
        /// </summary>
        /// <param name="illustration"></param>
        /// <param name="keywordListItem"></param>
        /// <param name="testPackageArchive"></param>
        public void MoveMediaFileForIllustration(Illustration illustration, KeywordListItem keywordListItem, ZipArchive testPackageArchive)
        {
            ZipArchiveEntry entry = SelectItemZipEntry(keywordListItem.FullPath, testPackageArchive);
            string fullpath = entry.FullName;
            string filename = entry.Name;
            string directory = fullpath.Remove(fullpath.Length - filename.Length);
            string illustrationFileName = (new FileInfo(illustration.FileName)).Name;
            testPackageArchive.CreateEntryFromFile(illustration.FileName, directory + illustrationFileName);
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

        /// <summary>
        /// null safe way to get an attributes value from an xml element
        /// </summary>
        /// <param name="e"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public string GetAttribute(XElement e, string attributeName)
        {
            XAttribute attribute = e.Attribute(attributeName);
            return attribute == null ? string.Empty : attribute.Value;
        }
    }
}
