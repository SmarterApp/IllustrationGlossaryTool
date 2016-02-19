using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IllustrationGlossaryPackage.Core.Interfaces;
using IllustrationGlossaryPackage.Core.Infrastructure;
using System.IO;
using System.IO.Compression;
using IllustrationGlossaryPackage.App;

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
    }
}
