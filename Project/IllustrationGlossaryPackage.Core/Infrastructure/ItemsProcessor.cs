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
    public class ItemsProcessor : Errorable, IErrorable, IItemsProcessor
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
            AddNonexistantWordlistResource(assessmentItems, testPackageFilePath);
            IEnumerable<string> keywordListItemIds = assessmentItems.Select(i => i.KeywordListItemId);
            IEnumerable<ItemDocument> keywordListDocuments = GetItemsXml(testPackageFilePath, keywordListItemIds);
            IList<KeywordListItem> keywordListItems = new List<KeywordListItem>();
            foreach (AssessmentItem assessmentItem in assessmentItems.ToList())
            {
                string keywordListId = assessmentItem.KeywordListItemId;
                ItemDocument itDoc = SelectByID(keywordListDocuments, keywordListId);
                if (!keywordListItems.Any(x => x.ItemId.Equals(keywordListId)))
                {
                    keywordListItems.Add(new KeywordListItem(keywordListId, itDoc));
                }
                assessmentItem.Illustrations.ToList().ForEach(x => 
                    x.CopiedToPath = GetCopyToLocation(x, itDoc.FullPath));
                keywordListItems.First(x => x.ItemId.Equals(keywordListId))
                    .AssessmentItems.Add(assessmentItem);
            }

            return keywordListItems;
        }

        private string GetCopyToLocation(Illustration illustration, string path)
        {
            string directory = Path.GetDirectoryName(path);
            string illPath = directory + "\\" + illustration.FileName;
            return illPath;
        }


        private void AddNonexistantWordlistResource(IEnumerable<AssessmentItem> assessmentItems, string testPackageFilePath)
        {
            using (ZipArchive testPackageArchive = ZipFile.Open(testPackageFilePath, ZipArchiveMode.Update))
            {
                foreach (AssessmentItem assessmentItem in assessmentItems)
                {
                    XDocument d = assessmentItem.Document;
                    XElement resourcesList = GetResourcesListElement(d);
                    if (!resourcesList.Descendants().Any(y => y.GetAttribute("type") == "wordList"))
                    {
                        assessmentItem.KeywordListItemId = Convert.ToString(Convert.ToInt32(assessmentItem.ItemId) + 1000000);
                        XElement e = CreateNewWordlistResource(assessmentItem);
                        resourcesList.Add(e);
                        itemsModifier.SaveItem(d, assessmentItem.GetZipArchiveEntry(testPackageArchive));
                        CreateNonexistantKeywordlistItem(assessmentItem, testPackageArchive);
                        CreateMetaData(assessmentItem, testPackageArchive);
                    }
                }
            }
        }

        private void CreateNonexistantKeywordlistItem(AssessmentItem assessmentItem, ZipArchive testPackageArchive)
        {
            ZipArchiveEntry entry = testPackageArchive.CreateEntry(assessmentItem.KeywordListFullPath);
            XDocument document = new XDocument(CreateEmptyKeywordlistXml(assessmentItem));
            itemsModifier.SaveItem(document, entry);
        }

        private void CreateMetaData(AssessmentItem assessmentItem, ZipArchive testPackageArchive)
        {
            ZipArchiveEntry entry = testPackageArchive.CreateEntry(assessmentItem.KeywordListMetadataFullPath);
            XDocument document = new XDocument(CreateEmptyMetaData(assessmentItem.KeywordListItemId, assessmentItem.ItemId));
            itemsModifier.SaveItem(document, entry);
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
            errors.AddRange(glossaryParser.GetErrors());
            IEnumerable<string> assessmentItemIds = illustrations.Select(i => i.ItemId);
            IEnumerable<ItemDocument> contentItems = GetItemsXml(testPackageFilePath, assessmentItemIds);
            IEnumerable<AssessmentItem> assessmentItems =
                illustrations.GroupBy(x => x.ItemId, (key, g) => CreateAssessmentItem(contentItems, key, g));
            IEnumerable<AssessmentItem> nonExistingAssessmentItems = 
                    assessmentItems.Where(x => x.Document == null);
            IEnumerable<Error> nonExistingItemIdErrors = nonExistingAssessmentItems
                    .Select(x => x.Illustrations)
                    .SelectMany(x => x).ToList()
                    .Select(x => new Error(Error.Exception.ItemDNE, "No item exists with id " + x.ItemId, x.LineNumber));
            errors.AddRange(nonExistingItemIdErrors);
            assessmentItems = assessmentItems.Where(x => x.Document != null);
            return assessmentItems;
        }

        private AssessmentItem CreateAssessmentItem(IEnumerable<ItemDocument> contentItems, string key, IEnumerable<Illustration> illustrations)
        {
            ItemDocument document = SelectByID(contentItems, key);
            if(document != null)
            {
                string keywordlistItemId = GetKeywordListItemId(document.Document);
                string bankKey = document.Document.Element("itemrelease")
                                    .Element("item").GetAttribute("bankkey");
                return new AssessmentItem(key, keywordlistItemId, bankKey, illustrations, document);
            }
            else
            {
                return new AssessmentItem(key, illustrations);
            }
        }

        /// <summary>
        /// gets the keywordlist item id from an assessment item xml document
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private string GetKeywordListItemId(XDocument d)
        {
            IEnumerable<XElement> resourceElements = GetResourceElements(d);
            XElement wordlist = resourceElements.FirstOrDefault(y => y.GetAttribute("type") == "wordList");
            string keywordListId = null;
            if (wordlist != null)
            {
                keywordListId = wordlist.GetAttribute("id");
            }
             
            return keywordListId;
        }

        private IEnumerable<XElement> GetResourceElements(XDocument d)
        {
            return GetResourcesListElement(d).Elements("resource");
        }

        private XElement GetResourcesListElement(XDocument d)
        {
            XElement resourcesList = d.ElementOrException("itemrelease")
                .ElementOrCreate("item")
                .ElementOrCreate("resourceslist");
            return resourcesList;
        }

        /// <summary>
        /// Selects an item by its id from a list of item documents
        /// </summary>
        /// <param name="items"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private ItemDocument SelectByID(IEnumerable<ItemDocument> items, string id)
        {
            return items.FirstOrDefault(x => x.Document.ElementOrException("itemrelease")
                        .ElementOrException("item").Attribute("id").ToString()
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

        private XElement CreateNewWordlistResource(AssessmentItem assessmentItem)
        {
            return new XElement("resource",
                        new XAttribute("type", "wordList"),
                        new XAttribute("id", assessmentItem.KeywordListItemId),
                        // TODO: Make index actually do stufff
                        new XAttribute("index", 1),
                        new XAttribute("bankkey", assessmentItem.Bankkey));
        }

        private XDocument CreateEmptyKeywordlistXml(AssessmentItem assessmentItem)
        {
            return new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                        new XElement("itemrelease",
                            new XAttribute("version", "2.0"),
                            new XElement("item",
                                new XAttribute("type", "wordList"),
                                new XAttribute("id", assessmentItem.KeywordListItemId),
                                new XAttribute("version", "10"),
                                new XAttribute("bankkey", assessmentItem.Bankkey),
                                new XElement("keywordList", string.Empty))));
        }

        private XDocument CreateEmptyMetaData(string id, string assessmentItemId)
        {
            XNamespace ns = ManifestModifier.Namespace;
            return new XDocument(new XDeclaration("1.0", "iso-8859-1", "yes"),
                        new XElement("metadata",
                            new XElement(ns + "smarterAppMetadata",
                                new XElement(ns + "Identifier", id),
                                new XElement(ns + "SmarterAppItemDescriptor", assessmentItemId),
                                new XElement(ns + "ItemAuthorIdentifier", "IllustrationGlossaryPackage"),
                                new XElement(ns + "LastModifiedBy", "IllustrationGlossaryPackage"))));
        }
    }
}
