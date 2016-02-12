using IllustrationGlossaryPackage.Core.Interfaces;
using IllustrationGlossaryPackage.Dal.Infrastructure;
using IllustrationGlossaryPackage.Dal.Interfaces;
using IllustrationGlossaryPackage.Dal.Models;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

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
            IEnumerable<Illustration> illustrations = glossaryParser.GetIllustrationsFromSpreadsheet(itemsFilePath);
            XDocument manifest = manifestModifier.GetManifestXml(testPackageFilePath);
            IEnumerable<XDocument> itemXmls = itemsModifier.GetItemsXml(testPackageFilePath);

            AddIllustrationInfoToItemXml(itemXmls, illustrations.First());

            itemsModifier.AddIllustrationsToItems(illustrations, testPackageFilePath);
            manifestModifier.AddIllustrationsToManifest(illustrations, testPackageFilePath);
        }

        private void AddIllustrationInfoToItemXml(IEnumerable<XDocument> itemXmls, Illustration illustration)
        {
            XDocument itemXml = GetXDocumentForIllustration(illustration, itemXmls);

            // Assume the item element exists, then work from there to add illustration resource list if necessary
            XElement rootElement = itemXml
                .Element("itemrelease")
                .Element("item");
            XElement resourceslistElement = rootElement.ElementOrCreate("resourceslist");
            XElement resourceElement = resourceslistElement.ElementOrCreate("resource");
            XElement itemElement = resourceElement.ElementOrCreate("item");
            XElement keywordListElement = itemElement.ElementOrCreate("keywordList");

            XElement keyword = NewIllustrationXmlElementFromIllustrationClass(illustration);
        }

        static XElement NewIllustrationXmlElementFromIllustrationClass(Illustration illustration)
        {
            XElement keyword = 
                new XElement("keyword",
                    new XAttribute("text", illustration.Term),
                    new XAttribute("index", "1"),
                        new XElement("html",
                            new XAttribute("listType", "illustration"),
                            new XAttribute("listCode", "TDS_WL_Illustration")
                        )
                );
            //keyword.Element("html").Value = "<![CDATA[<p style="">spun<a href="item_1881_spun_svg.svg" type="illustration / svg" visible="True"></a></p>]]>"
            return keyword;
        }

        static XDocument GetXDocumentForIllustration(Illustration illustration, IEnumerable<XDocument> documents)
        {
            return documents.First(
                    x => x.Element("itemrelease")
                    .Element("item")
                    .Attribute("id")
                    .Value == illustration.ItemId);
        }
    }
}
