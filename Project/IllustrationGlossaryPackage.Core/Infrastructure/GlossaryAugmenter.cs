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

namespace IllustrationGlossaryPackage.Core.Infrastructure
{
    public class GlossaryAugmenter : IGlossaryAugmenter
    {
        private IIllustrationGlossaryParser glossaryParser;
        private IItemsModifier itemsModifier;
        private IManifestModifier manifestModifier;
        public GlossaryAugmenter() : this(new IllustrationGlossaryParser(), new ItemsModifier(), new ManifestModifier()) { }
        public GlossaryAugmenter(IllustrationGlossaryParser glossaryParser, ItemsModifier itemsModifier, ManifestModifier manifestModifier)
        {
            this.glossaryParser = glossaryParser;
            this.itemsModifier = itemsModifier;
            this.manifestModifier = manifestModifier;
        }

        /// <summary>
        /// Add items in csv file to glossary
        /// </summary>
        /// <param name="testPackageFilePath"></param>
        /// <param name="itemsFilePath"></param>
        public void AddItemsToGlossary(string testPackageFilePath, string itemsFilePath)
        {
            XDocument manifest = manifestModifier.GetManifestXml(testPackageFilePath);
            IList<KeywordListItem> keywordListItems = itemsModifier.GetKeywordListItems(testPackageFilePath, itemsFilePath).ToList();
            using (ZipArchive testPackageArchive = ZipFile.Open(testPackageFilePath, ZipArchiveMode.Update))
            {
                UpdateKeywordListItems(keywordListItems, testPackageArchive);
            }
            //manifestModifier.AddIllustrationsToManifest(illustrations, testPackageFilePath);*/
        }

        private void UpdateKeywordListItems(IList<KeywordListItem> keywordListItems, ZipArchive testPackageArchive)
        {
            // TODO: Asyncronously
            foreach(KeywordListItem keywordListItem in keywordListItems)
            {
                AddIllustrationInfoToKeywordListItemXml(keywordListItem, testPackageArchive);
            }
        }

        private void AddIllustrationInfoToKeywordListItemXml(KeywordListItem keywordListItem, ZipArchive testPackageArchive)
        {
            XDocument itemXml = keywordListItem.Document;
            XElement rootElement = itemXml
                .Element("itemrelease")
                .Element("item");
            XElement keywordListElt = rootElement.ElementOrCreate("keywordList");
            IEnumerable<XElement> keywords = keywordListElt.Elements("keyword");
            foreach(AssessmentItem assessmentItem in keywordListItem.AssessmentItems)
            {
                foreach(Illustration illustration in assessmentItem.Illustrations)
                {
                    XElement keyword = keywords.FirstOrDefault(
                        x => itemsModifier.GetAttribute(x, "text") == illustration.Term);
                    if(keyword == null)
                    {
                        int maxIndex = keywords.Select(x => int.Parse(itemsModifier.GetAttribute(x, "index"))).Max();
                        keywordListElt.Add(GetKeywordXElementForFile(illustration, maxIndex));
                    }
                    else
                    {
                        keyword.Elements("html").Where(x => itemsModifier.GetAttribute(x, "listType") == "illustration"
                                                    && itemsModifier.GetAttribute(x, "listCode") == "TDS_WL_Illustration")
                                                    .Remove();
                        keyword.Add(GetHtmlXElementForFile(illustration.FileName));
                    }
                    itemsModifier.MoveMediaFileForIllustration(illustration, keywordListItem, testPackageArchive);
                }
            }
            itemsModifier.SaveItem(keywordListItem, testPackageArchive);
        }

        private XElement GetKeywordXElementForFile(Illustration illustration, int maxIndex)
        {
            return new XElement("keyword",
                        new XAttribute("text", illustration.Term),
                        new XAttribute("index", (maxIndex + 1).ToString()),
                            GetHtmlXElementForFile(illustration.FileName));
        }

        private XElement GetHtmlXElementForFile(string fileName)
        {
            return new XElement("html",
                        new XAttribute("listType", "illustration"),
                        new XAttribute("listCode", "TDS_WL_Illustration"),
                        new XRaw(string.Format("<![CDATA[<p style=\"\"><img src=\"{0}\" width=\"100\" height=\"200\" /></p>]]>", fileName)));
        }
    }
}
