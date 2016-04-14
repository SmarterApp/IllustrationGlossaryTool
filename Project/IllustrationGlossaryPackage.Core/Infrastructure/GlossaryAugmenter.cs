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
using System.Text.RegularExpressions;

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
            IList<XElement> resources = manifestModifier.GetResourcesList(manifest).ToList();
            foreach (KeywordListItem keywordListItem in keywordListItems)
            {
                foreach(AssessmentItem assessmentItem in keywordListItem.AssessmentItems)
                {
                    XElement assessmentItemResource = resources.FirstOrDefault(x =>
                             x.GetAttribute("identifier") == assessmentItem.Identifier);
                    AddNonexistingKeywordlist(keywordListItem, assessmentItem, assessmentItemResource, resources);
                    AddNonexistingKeywordlistToAssessmentItem(keywordListItem, assessmentItem, assessmentItemResource, resources);

                    foreach (Illustration illustration in assessmentItem.Illustrations)
                    {
                        ClearElements(resources, illustration, assessmentItemResource);
                        assessmentItemResource.AddAfterSelf(GetManifestResourceElement(illustration));
                        assessmentItemResource.Add(GetManifestDependencyElement(illustration));
                    }
                }
            }

            manifestModifier.SaveManifest(manifest, testPackageArchive);
        }

        private void AddNonexistingKeywordlist(KeywordListItem keywordListItem, AssessmentItem assessmentItem, XElement assessmentItemResource, IList<XElement> resources)
        {
            if(!resources.Any(x => x.GetAttribute("identifier") == keywordListItem.Identifier))
            {
                assessmentItemResource.Parent.Add(GetKeywordlistItemResource(keywordListItem));
            }
            if (!resources.Any(x => x.GetAttribute("identifier") == keywordListItem.MetadataIdentifier))
            {
                assessmentItemResource.Parent.Add(GetKeywordlistItemMetadataResource(keywordListItem, assessmentItem.KeywordListMetadataFullPath));
            }
        }

        private void AddNonexistingKeywordlistToAssessmentItem(KeywordListItem keywordListItem, AssessmentItem assessmentItem, XElement assessmentItemResource, IList<XElement> resources)
        {
            IEnumerable<XElement> dependencies = assessmentItemResource.Descendants("dependency");
            if (!dependencies.Any(x => x.GetAttribute("identifierref") == keywordListItem.Identifier))
            {
                assessmentItemResource.Add(GetKeywordlistItemDepency(keywordListItem));
            }
        }

        private void ClearElements(IList<XElement> resources, Illustration illustration, XElement assessmentItemResource)
        {
            XElement existingIllResource = resources.FirstOrDefault(x =>
                            x.GetAttribute("identifier") == illustration.Identifier);
            XElement existingIllDependency = assessmentItemResource.Descendants().FirstOrDefault(x =>
                            x.GetAttribute("identifierref") == illustration.Identifier);

            RemoveTag(existingIllResource, "identifier", illustration);
            RemoveTag(existingIllDependency, "identifierref", illustration);
        }

        private void RemoveTag(XElement existingElt, string idAttribute, Illustration illustration)
        {
            if (existingElt != null)
            {
                string msg = string.Format("In manifest file: Overwriting <{0}> tag with identifier {1}",
                                existingElt.Name.LocalName, existingElt.GetAttribute(idAttribute));
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
                .OptionalElement("itemrelease")
                .OptionalElement("item");
            XElement keywordListElt = rootElement.ElementOrCreate("keywordList");
            foreach (AssessmentItem assessmentItem in keywordListItem.AssessmentItems)
            {
                foreach(Illustration illustration in assessmentItem.Illustrations)
                {
                    AddIllustrationToKeywordListItem(illustration, keywordListElt, keywordListItem.ItemId);
                    itemsModifier.MoveMediaFileForIllustration(illustration, assessmentItem, testPackageArchive);
                }

                if(assessmentItem.Illustrations.Any(x => x.KeywordAdded))
                {
                    AddNewKeywordsToAssessmentItemContent(assessmentItem);
                    itemsModifier.SaveItem(assessmentItem, testPackageArchive);
                }
            }

            itemsModifier.SaveItem(keywordListItem, testPackageArchive);
            return 0;
        }

        private void AddIllustrationToKeywordListItem(Illustration illustration, XElement keywordListElt, string KeywordListItemId)
        {
            // TODO REFACTOR THIS METHOD
            IEnumerable<XElement> keywords = keywordListElt.Elements("keyword");
            XElement keyword = null;
            if (keywords != null && keywords.Count() > 0)
            {
                keyword = keywords.FirstOrDefault(
                        x => x.GetAttribute("text").ToLower() == illustration.Term.ToLower());
                if(keyword == null)
                {
                    keyword = keywords.FirstOrDefault(
                        x => isInexactTextMatch(x.GetAttribute("text"), illustration.Term));
                }
            }
            if (keyword == null)
            {
                IEnumerable<int> indicies = keywords.Select(x =>
                {
                    string index = x.GetAttribute("index");
                    return index == string.Empty ? 0 : int.Parse(index);
                });
                int maxIndex = indicies == null || indicies.Count() < 1 ? 0 : indicies.Max();
                keywordListElt.Add(GetKeywordXElementForFile(illustration, maxIndex));
                string msg = string.Format("Added new keyword \"{0}\" to keywordlist item {1}", illustration.Term, KeywordListItemId);
                errors.Add(new Error(Error.Exception.NewKeywordWarning, msg, illustration.LineNumber, Error.Type.Warning));
                illustration.KeywordAdded = true;
            }
            else
            {
                IEnumerable<XElement> existingHtmlElt = keyword.ElementsOrException("html")
                                        .Where(x => x.GetAttribute("listType") == "illustration"
                                           && x.GetAttribute("listCode") == "TDS_WL_Illustration");
                if(existingHtmlElt != null && existingHtmlElt.Count() > 0)
                {
                    string msg = string.Format("In item {0}: Overwriting illustration <html> tag under keyword {1}",
                                                KeywordListItemId, illustration.Term);
                    errors.Add(new Error(Error.Exception.OverwriteWarning, msg, illustration.LineNumber, Error.Type.Warning));
                    existingHtmlElt.Remove();
                }
                string textAttribute = keyword.GetAttribute("text");
                if (textAttribute.ToLower() != illustration.Term.ToLower())
                {
                    string msg = string.Format("In item {0}: Matched illustration term \"{1}\" to keyword \"{2}\"",
                                               KeywordListItemId, illustration.Term, textAttribute);
                    errors.Add(new Error(Error.Exception.KeywordNotExactMatchWarning, msg, illustration.LineNumber, Error.Type.Warning));
                }
                keyword.Add(GetHtmlXElementForFile(illustration.FileName, illustration.Width, illustration.Height));
            }
        }

        private bool isInexactTextMatch(string textAttribute, string illustrationTerm)
        {
            illustrationTerm = illustrationTerm.ToLower();
            textAttribute = textAttribute.ToLower();
            return textAttribute.Split().Any(s => s == illustrationTerm);
        }

        private void AddNewKeywordsToAssessmentItemContent(AssessmentItem assessmentItem)
        {
            foreach(Illustration i in assessmentItem.Illustrations)
            {
                if (i.KeywordAdded)
                {
                    AddKeywordToContent(assessmentItem, i);
                }
            }
        }

        private void AddKeywordToContent(AssessmentItem assessmentItem, Illustration i)
        {
            IEnumerable<XElement> contents = assessmentItem.Document.OptionalElement("itemrelease")
                                    .OptionalElement("item").Elements("content");
            foreach(XElement content in contents)
            {
                XElement stem = content.ElementOrException("stem");
                stem.ReplaceNodes(new XRaw(AddKeywordToStem(stem, i, assessmentItem)));
            }
        }

        private string AddKeywordToStem(XElement stemElt, Illustration illustration, AssessmentItem assessmentItem)
        {
            string stem = stemElt.FirstNode.ToString();
            IEnumerable<string> matches = Regex.Matches(stem, illustration.Term, RegexOptions.IgnoreCase)
                                            .Cast<Match>()
                                            .Select(x => x.Value);
            foreach (string s in matches)
            {
                stem = stem.Replace(s, GetSpan(s, illustration, assessmentItem));
            }

            return stem;
        }

        private string GetSpan(string termWithCase, Illustration illustration, AssessmentItem assessmentItem)
        {
            string termIndex = "TODO";
            string datawordIndex = "TODO";
            return string.Format(Properties.Resources.SpanString, assessmentItem.ItemId, termIndex, datawordIndex, termWithCase);
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
                            GetHtmlXElementForFile(illustration.FileName, illustration.Width, illustration.Height));
        }

        /// <summary>
        /// Gets the <html> element to be added to the keywordlist xml
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private XElement GetHtmlXElementForFile(string fileName, string width, string height)
        {
            return new XElement("html",
                        new XAttribute("listType", Properties.Resources.listType),
                        new XAttribute("listCode", Properties.Resources.listCode),
                        new XRaw(string.Format(Properties.Resources.html, fileName, width, height)));
        }

        private XElement GetManifestResourceElement(Illustration illustration)
        {
            XNamespace ns = ManifestModifier.Namespace;
            return new XElement(ns + "resource",
                        new XAttribute("identifier", illustration.Identifier),
                        new XAttribute("type", Properties.Resources.IllustrationResourceTypeInManifest),
                        new XElement(ns + "file",
                            new XAttribute("href", illustration.CopiedToPath))); 
        }

        private XElement GetManifestDependencyElement(Illustration illustration)
        {
            XNamespace ns = ManifestModifier.Namespace;
            return new XElement(ns + "dependency",
                        new XAttribute("identifierref", illustration.Identifier));
        }
        
        private XElement GetKeywordlistItemDepency(KeywordListItem keywordlistItem)
        {
            XNamespace ns = ManifestModifier.Namespace;
            return new XElement(ns + "dependency",
                        new XAttribute("identifierref", keywordlistItem.Identifier));
        }

        private XElement GetKeywordlistItemResource(KeywordListItem keywordlistItem)
        {
            XNamespace ns = ManifestModifier.Namespace;
            return new XElement(ns + "resource",
                        new XAttribute("identifier", keywordlistItem.Identifier),
                        new XAttribute("type", Properties.Resources.WordlistResourceTypeInManifest), 
                        new XElement(ns + "file", 
                            new XAttribute("href", keywordlistItem.FullPath)),
                        new XElement(ns + "dependency", 
                            new XAttribute("identifierref", keywordlistItem.MetadataIdentifier)));
        }

        private XElement GetKeywordlistItemMetadataResource(KeywordListItem keywordlistItem, string metaPath)
        {
            XNamespace ns = ManifestModifier.Namespace;
            return new XElement(ns + "resource",
                        new XAttribute("identifier", keywordlistItem.MetadataIdentifier),
                        new XAttribute("type", Properties.Resources.MetadataResourceTypeInManifest),
                        new XElement(ns + "file",
                            new XAttribute("href", metaPath)));
        }  
    }
#pragma warning restore CS4014
#pragma warning restore CS1998
}
