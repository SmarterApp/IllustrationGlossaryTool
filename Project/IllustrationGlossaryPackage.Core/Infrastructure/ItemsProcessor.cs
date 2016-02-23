using IllustrationGlossaryPackage.Dal.Infrastructure;
using IllustrationGlossaryPackage.Dal.Interfaces;
using IllustrationGlossaryPackage.Dal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IllustrationGlossaryPackage.Core.Infrastructure
{
    public class ItemsProcessor : IItemsProcessor
    {
        private IIllustrationGlossaryParser glossaryParser;
        private IItemsModifier itemsModifier;
        public ItemsProcessor() : this(new IllustrationGlossaryParser(), new ItemsModifier()) { }
        public ItemsProcessor(IllustrationGlossaryParser glossaryParser, ItemsModifier itemsModifier)
        {
            this.glossaryParser = glossaryParser;
            this.itemsModifier = itemsModifier;
        }

        /// <summary>
        /// public method that retrevies list of keyworditems to be modified from 
        ///     in the given test package that are in the items csv
        /// </summary>
        /// <param name="testPackageFilePath"></param>
        /// <param name="itemsFilePath"></param>
        /// <returns></returns>
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
                    keywordListItems.Add(new KeywordListItem
                    {
                        ItemId = keywordListId,
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

        /// <summary>
        /// Gets list of assessment items in preparation for creating keywordlist items
        /// </summary>
        /// <param name="testPackageFilePath"></param>
        /// <param name="itemsFilePath"></param>
        /// <returns></returns>
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
                    KeywordListItemId = GetKeywordListItemId(SelectByID(contentItems, key).Document),
                    Name = SelectByID(contentItems, key).Name,
                    Identifier = Path.GetFileNameWithoutExtension(SelectByID(contentItems, key).FullPath)
                });
            return assessmentItems;
        }

        /// <summary>
        /// gets the keywordlist item id from an assessment item xml document
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private string GetKeywordListItemId(XDocument d)
        {
            IEnumerable<XElement> resources = d.Element("itemrelease").Element("item").Element("resourceslist").Elements("resource");
            XElement wordlist = resources.First(y => itemsModifier.GetAttribute(y, "type") == "wordList");
            string keywordListId = wordlist.Attribute("id").Value;
            return keywordListId;
        }

        /// <summary>
        /// Selects an item by its id from a list of item documents
        /// </summary>
        /// <param name="items"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private ItemDocument SelectByID(IEnumerable<ItemDocument> items, string id)
        {
            return items.FirstOrDefault(x => x.Document.Element("itemrelease")
                        .Element("item").Attribute("id").ToString()
                        .Contains(id));
        }

        /// <summary>
        /// Gets xml docs for all relevant items
        /// </summary>
        /// <param name="testPackageFilePath"></param>
        /// <param name="itemsIds"></param>
        /// <returns></returns>
        private static IEnumerable<ItemDocument> GetItemsXml(string testPackageFilePath, IEnumerable<string> itemsIds)
        {
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
                    itemXmls.Add(new ItemDocument { FullPath = itemXmlEntry.FullName, Document = itemXml, Name = itemXmlEntry.Name });
                }
            }
            return itemXmls;
        }
    }
}
