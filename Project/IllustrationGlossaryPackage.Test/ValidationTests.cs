using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IllustrationGlossaryPackage.Core.Interfaces;
using IllustrationGlossaryPackage.Core.Infrastructure;
using System.IO;
using System.Collections.Generic;
using System.IO.Compression;
using IllustrationGlossaryPackage.App;
using IllustrationGlossaryPackage.Dal.Models;
using IllustrationGlossaryPackage.Dal.Infrastructure;
using IllustrationGlossaryPackage.Dal.Interfaces;
using System.Linq;

namespace IllustrationGlossaryPackage.Test
{
    [TestClass]
    public class ValidationTests
    {
        private string badCsv = "not-a-file";
        private string goodCsv = "../../TestData/illustrations-test-1.csv";
        private string badZip = "also-not-a-file";
        private string goodZip = "../../TestData/IrpContentPackage.zip";

        //tests error reporting if the specified illustration list (csv) is not found
        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void IllustrationListNotFoundError()
        {
            IIllustrationGlossaryParser glossaryParser = new IllustrationGlossaryParser();
            IEnumerable<Illustration> actual = glossaryParser.GetIllustrationsFromSpreadsheet(badCsv);
        }

        //tests error reporting if a specified package is not found
        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void PackageNotFoundError()
        {
            IItemsProcessor itemsProcessor = new ItemsProcessor();
            IList<KeywordListItem> actual = itemsProcessor.GetKeywordListItems(badZip, goodCsv).ToList();
        }

        //test that we are parsing the illustrations from the xml correctly
        [TestMethod]
        public void TestGetIllustrationsFromSpreadsheet()
        {
            IIllustrationGlossaryParser glossaryParser = new IllustrationGlossaryParser();
            IEnumerable<Illustration> actual = glossaryParser.GetIllustrationsFromSpreadsheet(goodCsv);
            //not too few
            Assert.AreEqual(actual.ToList().Count(), 5);
            //not too many
            foreach (Illustration illustration in actual)
            {
                Assert.IsTrue(illustration.FileExists);
            }
        }

        //test that we get the correct list of keyword items
        //TODO: change these filepaths to relative, using the testdata folder
        [TestMethod]
        public void TestGetKeywordList()
        {
            IItemsProcessor itemsProcessor = new ItemsProcessor();
            IList<KeywordListItem> actual = itemsProcessor.GetKeywordListItems(goodZip, goodCsv).ToList();
            //make sure we found the correct assessment items
            Assert.AreEqual(actual.Count(), 2);

            //ensure that all the item ids match the keywordlistitem ids
            foreach (KeywordListItem kitem in actual)
            {
                foreach (AssessmentItem aitem in kitem.AssessmentItems)
                {
                    Assert.AreEqual(kitem.ItemId, aitem.KeywordListItemId);
                    Assert.IsTrue(aitem.FullPath.Contains(aitem.ItemId));
                }
            }
            IGlossaryAugmenter ga = new GlossaryAugmenter();
            ga.AddItemsToGlossary(goodZip, goodCsv);
        }

        [TestMethod]
        public void TestGetItemsXml()
        {
            //probably we should make the 'getitemsxml' function public, because it might need to be unit tested
            //(doesn't seem to be working properly)
        }
    }
}