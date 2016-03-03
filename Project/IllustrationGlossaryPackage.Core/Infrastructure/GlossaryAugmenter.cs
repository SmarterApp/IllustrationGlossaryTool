using IllustrationGlossaryPackage.Core.Interfaces;
using IllustrationGlossaryPackage.Dal.Infrastructure;
using IllustrationGlossaryPackage.Dal.Interfaces;
using IllustrationGlossaryPackage.Dal.Models;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Threading.Tasks;

namespace IllustrationGlossaryPackage.Core.Infrastructure
{
#pragma warning disable CS1998
#pragma warning disable CS4014
    public class GlossaryAugmenter : Errorable, IErrorable, IGlossaryAugmenter
    {
        private IIllustrationGlossaryParser glossaryParser;
        private IItemsModifier itemsModifier;
        private IManifestModifier manifestModifier;
        private IItemsProcessor itemsProcessor;
        public GlossaryAugmenter() : this(new IllustrationGlossaryParser(), new ItemsModifier(), new ManifestModifier(), new ItemsProcessor()) { }
        public GlossaryAugmenter(IllustrationGlossaryParser glossaryParser, ItemsModifier itemsModifier, ManifestModifier manifestModifier, ItemsProcessor itemsProcessor)
        {
            this.glossaryParser = glossaryParser;
            this.itemsModifier = itemsModifier;
            this.manifestModifier = manifestModifier;
            this.itemsProcessor = itemsProcessor;
        }

        /// <summary>
        /// Adds items in csv file to glossary
        /// </summary>
        /// <param name="testPackageFilePath"></param>
        /// <param name="itemsFilePath"></param>
        public void AddItemsToGlossary(string testPackageFilePath, string itemsFilePath)
        {
            XDocument manifest = manifestModifier.GetManifestXml(testPackageFilePath);
            IList<KeywordListItem> keywordListItems = itemsProcessor.GetKeywordListItems(testPackageFilePath, itemsFilePath).ToList();
            errors.AddRange(itemsProcessor.GetErrors());
            using (ZipArchive testPackageArchive = ZipFile.Open(testPackageFilePath, ZipArchiveMode.Update))
            {
                UpdateKeywordListItems(keywordListItems, testPackageArchive);
                AddKeywordListItemsToManifest(keywordListItems, testPackageArchive, manifest);
            }
            errors.AddRange(itemsModifier.GetErrors());
        }

        private void AddKeywordListItemsToManifest(IList<KeywordListItem> keywordListItems, ZipArchive testPackageArchive, XDocument manifest)
        {
            XNamespace ns = "http://www.imsglobal.org/xsd/apip/apipv1p0/imscp_v1p1";
            IList<XElement> resources = manifest
                .ElementOrException(ns + "manifest")
                .ElementOrException(ns + "resources")
                .ElementsOrException(ns + "resource").ToList();
            foreach (KeywordListItem keywordListItem in keywordListItems)
            {
                foreach(AssessmentItem assessmentItem in keywordListItem.AssessmentItems)
                {
                    foreach(Illustration illustration in assessmentItem.Illustrations)
                    {
                        XElement assessmentItemResource = resources.FirstOrDefault(x =>
                             itemsModifier.GetAttribute(x, "identifier") == assessmentItem.Identifier);
                        ClearElements(resources, illustration, assessmentItemResource);
                        assessmentItemResource.AddAfterSelf(GetManifestResourceElement(illustration, ns));
                        assessmentItemResource.Add(GetManifestDependencyElement(illustration, ns));
                    }
                }
            }

            manifestModifier.SaveManifest(manifest, testPackageArchive);
        }

        private void ClearElements(IList<XElement> resources, Illustration illustration, XElement assessmentItemResource)
        {
            XElement existingIllResource = resources.FirstOrDefault(x =>
                            itemsModifier.GetAttribute(x, "identifier") == illustration.Identifier);
            XElement existingIllDependency = assessmentItemResource.Descendants().FirstOrDefault(x =>
                itemsModifier.GetAttribute(x, "identifierref") == illustration.Identifier);

            RemoveTag(existingIllResource, "identifier", illustration);
            RemoveTag(existingIllDependency, "identifierref", illustration);
        }

        private void RemoveTag(XElement existingElt, string idAttribute, Illustration illustration)
        {
            if (existingElt != null)
            {
                string msg = string.Format("In manifest file: Overwriting <{0}> tag with identifier {1}",
                                existingElt.Name.LocalName, itemsModifier.GetAttribute(existingElt, idAttribute));
                errors.Add(new Error(Error.Exception.OverwriteWarning, msg, illustration.LineNumber, Error.Type.Warning));
                existingElt.Remove();
            }
        }

        /// <summary>
        /// Asyncronously updates the keywordlist items with keywords
        /// </summary>
        /// <param name="keywordListItems"></param>
        /// <param name="testPackageArchive"></param>
        /// <returns></returns>
        private async Task UpdateKeywordListItems(IList<KeywordListItem> keywordListItems, ZipArchive testPackageArchive)
        {
            //IList<Task<int>> tasks = new List<Task<int>>();
            foreach (KeywordListItem keywordListItem in keywordListItems)
            {
                //Task<int> task = Task.Run(() => AddIllustrationInfoToKeywordListItemXml(keywordListItem, testPackageArchive));
                //tasks.Add(task);
                AddIllustrationInfoToKeywordListItemXml(keywordListItem, testPackageArchive);
            }
            //Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// Adds a keyword to a keywordlist item xml and saves and xml and 
        ///     copies the illustration to the zip archive
        /// </summary>
        /// <param name="keywordListItem"></param>
        /// <param name="testPackageArchive"></param>
        /// <returns></returns>
        private async Task<int> AddIllustrationInfoToKeywordListItemXml(KeywordListItem keywordListItem, ZipArchive testPackageArchive)
        {
            XDocument itemXml = keywordListItem.Document;
            XElement rootElement = itemXml
                .ElementOrException("itemrelease")
                .ElementOrException("item");
            XElement keywordListElt = rootElement.ElementOrCreate("keywordList");
            foreach (AssessmentItem assessmentItem in keywordListItem.AssessmentItems)
            {
                foreach(Illustration illustration in assessmentItem.Illustrations)
                {
                    illustration.CopiedToPath = 
                        itemsModifier.GetIllustrationCopyToLocation(illustration, keywordListItem, testPackageArchive);
                    AddIllustrationToKeywordListItem(illustration, keywordListElt, keywordListItem.ItemId);
                    itemsModifier.MoveMediaFileForIllustration(illustration, assessmentItem, testPackageArchive);
                }
            }

            itemsModifier.SaveItem(keywordListItem, testPackageArchive);
            return 0;
        }

        private void AddIllustrationToKeywordListItem(Illustration illustration, XElement keywordListElt, string KeywordListItemId)
        {
            IEnumerable<XElement> keywords = keywordListElt.ElementsOrException("keyword");
            XElement keyword = keywords.FirstOrDefault(
                        x => itemsModifier.GetAttribute(x, "text") == illustration.Term);
            if (keyword == null)
            {
                int maxIndex = keywords.Select(x => int.Parse(itemsModifier.GetAttribute(x, "index"))).Max();
                keywordListElt.Add(GetKeywordXElementForFile(illustration, maxIndex));
            }
            else
            {
                IEnumerable<XElement> existingHtmlElt = keyword.ElementsOrException("html")
                                        .Where(x => itemsModifier.GetAttribute(x, "listType") == "illustration"
                                           && itemsModifier.GetAttribute(x, "listCode") == "TDS_WL_Illustration");
                if(existingHtmlElt != null && existingHtmlElt.Count() > 0)
                {
                    string msg = string.Format("In item {0}: Overwriting illustration <html> tag under keyword {1}",
                                                KeywordListItemId, illustration.Term);
                    errors.Add(new Error(Error.Exception.OverwriteWarning, msg, illustration.LineNumber, Error.Type.Warning));
                    existingHtmlElt.Remove();
                }
                keyword.Add(GetHtmlXElementForFile(illustration.FileName));
            }
        }

        /// <summary>
        /// Gets the full keyword xml if the keyword is not already in the keywordlist
        /// </summary>
        /// <param name="illustration"></param>
        /// <param name="maxIndex"></param>
        /// <returns></returns>
        private XElement GetKeywordXElementForFile(Illustration illustration, int maxIndex)
        {
            return new XElement("keyword",
                        new XAttribute("text", illustration.Term),
                        new XAttribute("index", (maxIndex + 1).ToString()),
                            GetHtmlXElementForFile(illustration.FileName));
        }

        /// <summary>
        /// Gets the <html> element to be added to the keywordlist xml
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private XElement GetHtmlXElementForFile(string fileName)
        {
            return new XElement("html",
                        new XAttribute("listType", "illustration"),
                        new XAttribute("listCode", "TDS_WL_Illustration"),
                        new XRaw(string.Format("<![CDATA[<p style=\"\"><img src=\"{0}\" width=\"100\" height=\"200\" /></p>]]>", fileName)));
        }

        private XElement GetManifestResourceElement(Illustration illustration, XNamespace ns)
        {
            return new XElement(ns + "resource",
                        new XAttribute("identifier", illustration.Identifier),
                        new XAttribute("type", "associatedcontent/apip_xmlv1p0/learning-application-resource"),
                        new XElement(ns + "file",
                            new XAttribute("href", illustration.CopiedToPath))); 
        }

        private XElement GetManifestDependencyElement(Illustration illustration, XNamespace ns)
        {
            return new XElement(ns + "dependency",
                        new XAttribute("identifierref", illustration.Identifier));
        }
    }
#pragma warning restore CS4014
#pragma warning restore CS1998
}
