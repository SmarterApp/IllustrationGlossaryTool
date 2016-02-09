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

            UpdateItemXmls(itemXmls, illustrations);

            itemsModifier.AddIllustrationsToItems(illustrations, testPackageFilePath);
            manifestModifier.AddIllustrationsToManifest(illustrations, testPackageFilePath);
        }

        private void UpdateItemXmls(IEnumerable<XDocument> itemXmls, IEnumerable<Illustration> illustrations)
        {
            foreach(Illustration illustration in illustrations)
            {
                XDocument itemXml = GetXDocumentForIllustration(illustration, itemXmls);
            }
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
