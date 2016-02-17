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

        public IEnumerable<KeywordListItem> GetKeywordListItems(string testPackageFilePath, string itemsFilePath)
        {
            IEnumerable<AssessmentItem> assessmentItems = GetAssessmentItems(testPackageFilePath, itemsFilePath);
            IEnumerable<string> keywordListItemIds = assessmentItems.Select(i => i.KeywordListItemId);
            IEnumerable<ItemDocument> keywordListDocuments = GetItemsXml(testPackageFilePath, keywordListItemIds);
            IList<KeywordListItem> keywordListItems = new List<KeywordListItem>();
            foreach (AssessmentItem assessmentItem in assessmentItems.ToList())
            {
                string keywordListId = assessmentItem.KeywordListItemId;
                if (!keywordListItems.Any(x => x.ItemId.Equals(keywordListId)))
                {
                    keywordListItems.Add(new KeywordListItem { ItemId = keywordListId,
                        AssessmentItems = new List<AssessmentItem>(),
                        Document = SelectByID(keywordListDocuments, keywordListId).Document,
                        FullPath = SelectByID(keywordListDocuments, keywordListId).FullPath
                    });
                }
                keywordListItems.First(x => x.ItemId.Equals(keywordListId))
                    .AssessmentItems.Add(assessmentItem);
            }
            return keywordListItems;
        }

        private IEnumerable<AssessmentItem> GetAssessmentItems(string testPackageFilePath, string itemsFilePath)
        {
            IEnumerable<Illustration> illustrations = glossaryParser.GetIllustrationsFromSpreadsheet(itemsFilePath);
            IEnumerable<string> assessmentItemIds = illustrations.Select(i => i.ItemId);
            IEnumerable<ItemDocument> contentItems = GetItemsXml(testPackageFilePath, assessmentItemIds);
            IEnumerable<AssessmentItem> assessmentItems =
                illustrations.GroupBy(x => x.ItemId, (key, g) => new AssessmentItem
                {
                    ItemId = key,
                    Illustrations = g.ToList(),
                    Document = SelectByID(contentItems, key).Document,
                    FullPath = SelectByID(contentItems, key).FullPath,
                    KeywordListItemId = GetKeywordListItemId(SelectByID(contentItems, key).Document)
                });
            return assessmentItems;
        }

        private string GetKeywordListItemId(XDocument d)
        {
            IEnumerable<XElement> resources = d.Element("itemrelease").Element("item").Element("resourceslist").Elements("resource");
            XElement wordlist = resources.First(y => GetAttribute(y, "type") == "wordList");
            string keywordListId = wordlist.Attribute("id").Value;
            return keywordListId;
        }

        private ItemDocument SelectByID(IEnumerable<ItemDocument> items, string id)
        {
            return items.FirstOrDefault(x => x.Document.Element("itemrelease")
                        .Element("item").Attribute("id").ToString()
                        .Contains(id));
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

        private static IEnumerable<ItemDocument> GetItemsXml(string testPackageFilePath, IEnumerable<string> itemsIds)
        {
            // Get list of all archive entries, select XML files w/ regex, load each XML file into XDocument
            IList<ItemDocument> itemXmls = new List<ItemDocument>();
            using (ZipArchive testPackageArchive = ZipFile.Open(testPackageFilePath, ZipArchiveMode.Update))
            {
                Regex xmlFileRegex = new Regex("(item)-([0-9])*-([0-9])*(.xml)", RegexOptions.IgnoreCase);
                IEnumerable<ZipArchiveEntry> itemXmlEntries = testPackageArchive.Entries
                    .Where(i => itemsIds.Any(s => i.Name.Contains(s)))
                    .Where(i => xmlFileRegex.IsMatch(i.Name));
       
                foreach (ZipArchiveEntry itemXmlEntry in itemXmlEntries)
                {
                    XDocument itemXml = XDocument.Load(itemXmlEntry.Open());
                    //itemXmls.Add(new ItemDocument { FullPath = (new FileInfo(testPackageFilePath)).Directory.FullName + "\\" + itemXmlEntry.Name, Document = itemXml });
                    itemXmls.Add(new ItemDocument { FullPath = itemXmlEntry.FullName, Document = itemXml });
                }
            }
            return itemXmls;
        }

        private static void MoveMediaFileForIllustrationToPath(Illustration illustration, string path)
        {

        }

        private static void UpdateXmlForIllustration(Illustration illustration)
        {

        }

        public string GetAttribute(XElement e, string attributeName)
        {
            XAttribute attribute = e.Attribute(attributeName);
            return attribute == null ? string.Empty : attribute.Value;
        }
    }
}
