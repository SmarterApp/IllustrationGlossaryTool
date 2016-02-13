using IllustrationGlossaryPackage.Dal.Interfaces;
using IllustrationGlossaryPackage.Dal.Models;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace IllustrationGlossaryPackage.Dal.Infrastructure
{
    public class ItemsModifier : IItemsModifier
    {
        /// <summary>
        /// Returns an IEnumerable of XDocuments that are Smarter Balance Content Items
        /// </summary>
        /// <param name="testPackageFilePath"></param>
        /// <returns>IEnumerable of XDocuments that are Smarter Balance Content Items</returns>
        public IEnumerable<XDocument> GetContentItems(string testPackageFilePath)
        {
            return GetItemsXml(testPackageFilePath).Where(x => x.IsContentItem());
        }

        /// <summary>
        /// Returns an IEnumerable of XDocuments that are Smarter Balance Keyword List Items
        /// </summary>
        /// <param name="testPackageFilePath"></param>
        /// <returns>IEnumerable of XDocuments that are Smarter Balance Keyword List Items</returns>
        public IEnumerable<XDocument> GetKeywordListItems(string testPackageFilePath)
        {
            return GetItemsXml(testPackageFilePath).Where(x => x.IsKeywordListItem());
        }

        /// <summary>
        /// Adds each illustration to the corresponding item in the test package
        /// </summary>
        /// <param name="illustrations"></param>
        /// <param name="testPackageFilePath"></param>
        public void AddIllustrationsToItems(IEnumerable<Illustration> illustrations, string testPackageFilePath)
        {

        }

        // MARK: Internal methods

        private static IEnumerable<XDocument> GetItemsXml(string testPackageFilePath)
        {
            // Get list of all archive entries, select XML files w/ regex, load each XML file into XDocument
            ZipArchive testPackageArchive = ZipFile.Open(testPackageFilePath, ZipArchiveMode.Update);
            Regex xmlFileRegex = new Regex("(item)-([0-9])*-([0-9])*(.xml)", RegexOptions.IgnoreCase);
            IEnumerable<ZipArchiveEntry> itemXmlEntries = testPackageArchive.Entries.Where(i => xmlFileRegex.IsMatch(i.Name));
            List<XDocument> itemXmls = new List<XDocument>();
            foreach(ZipArchiveEntry itemXmlEntry in itemXmlEntries)
            {
                XDocument itemXml = XDocument.Load(itemXmlEntry.Open());
                itemXmls.Add(itemXml);
            }
            testPackageArchive.Dispose();
            return itemXmls;
        }

        private static void MoveMediaFileForIllustrationToPath(Illustration illustration, string path)
        {

        }

        private static void UpdateXmlForIllustration(Illustration illustration)
        {

        }
    }
}
