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
        //tests error reporting if the specified illustration list (csv) is not found
        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void IllustrationListNotFoundError()
        {
            string[] args = { "not-a-file", "also-not-a-file" };
            Program.Main(args);
        }

        //tests error reporting if a specified package is not found
        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void PackageNotFoundError()
        {
            string[] args = { "~/IllustrationGlossaryPackage.Test/TestData/illustrations-test-1.csv", "for-sure-not-a-file" };
            Program.Main(args);
        }

        //tests error reporting if a specified test item is not found
        [TestMethod]
        public void ItemNotFoundError()
        {
            //ensure that we are enforcing the goldilocks principle - not too many or too few arguments
            string[] tooManyArgs = { "1", "2", "3" };
            string[] tooFewArgs = { "1" };
            try {
                Program.Main(tooManyArgs);
                Assert.Fail("program accepted an incorrect number of arguments (" + tooManyArgs.Length.ToString() + ")");
            } catch (ArgumentException e)
            {
                //ensure that the user is prompted with the correct usage
                Assert.IsTrue(e.Message.Contains("usage"));
            }

            try
            {
                Program.Main(tooManyArgs);
                Assert.Fail("program accepted an incorrect number of arguments (" + tooFewArgs.Length.ToString() + ")");
            }
            catch (ArgumentException e)
            {
                //ensure that the user is prompted with the correct usage
                Assert.IsTrue(e.Message.Contains("usage"));
            }
        }

        //test that we are parsing the illustrations from the xml correctly
        [TestMethod]
        public void TestGetIllustrationsFromSpreadsheet()
        {
            IIllustrationGlossaryParser glossaryParser = new IllustrationGlossaryParser();
            IEnumerable<Illustration> actual = glossaryParser.GetIllustrationsFromSpreadsheet("C:\\Users\\Miles\\Source\\Repos\\smarter-balanced-glossary-service\\Project\\IllustrationGlossaryPackage.Test\\TestData\\illustrations-test-1.csv");
            //not too few
            Assert.AreEqual(actual.ToList().Count(), 5);
            //not too many
            foreach(Illustration illustration in actual) {
                Assert.IsTrue(illustration.FileExists);
            }
        }

        //test that we get the correct list of keyword items
        //TODO: change these filepaths to relative, using the testdata folder
        [TestMethod]
        public void TestGetKeywordList()
        {
            IItemsProcessor itemsProcessor = new ItemsProcessor();
            IList<KeywordListItem> actual = itemsProcessor.GetKeywordListItems("C:\\Users\\Miles\\Source\\Repos\\smarter-balanced-glossary-service\\Project\\IllustrationGlossaryPackage.Test\\TestData\\IrpContentPackage.zip", "C:\\Users\\Miles\\Source\\Repos\\smarter-balanced-glossary-service\\Project\\IllustrationGlossaryPackage.Test\\TestData\\illustrations-test-1.csv").ToList();
            //make sure we found the correct assessment items
            Assert.AreEqual(actual.Count(), 2);

            //ensure that all the item ids match the keywordlistitem ids
            foreach(KeywordListItem kitem in actual)
            {
                foreach(AssessmentItem aitem in kitem.AssessmentItems)
                {
                    Assert.AreEqual(kitem.ItemId, aitem.KeywordListItemId);
                    Assert.IsTrue(aitem.FullPath.Contains(aitem.ItemId));
                }
            }
            IGlossaryAugmenter ga = new GlossaryAugmenter();
            ga.AddItemsToGlossary("C:\\Users\\Miles\\Source\\Repos\\smarter-balanced-glossary-service\\Project\\IllustrationGlossaryPackage.Test\\TestData\\IrpContentPackage.zip", "C:\\Users\\Miles\\Source\\Repos\\smarter-balanced-glossary-service\\Project\\IllustrationGlossaryPackage.Test\\TestData\\illustrations-test-1.csv");
        }

        [TestMethod]
        public void TestGetItemsXml()
        {
            //probably we should make the 'getitemsxml' function public, because it might need to be unit tested
            //(doesn't seem to be working properly)
        }
    }
}
