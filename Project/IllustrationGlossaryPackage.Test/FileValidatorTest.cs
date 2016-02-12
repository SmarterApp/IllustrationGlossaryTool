using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IllustrationGlossaryPackage.Core.Interfaces;
using IllustrationGlossaryPackage.Core.Infrastructure;
using System.IO;
using IllustrationGlossaryPackage.Core.Exceptions;

namespace IllustrationGlossaryPackage.Test
{
    [TestClass]
    public class FileValidatorTest
    {
        [TestMethod]
        public void TestPackageValidatorTest()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string fileName = "testFile";
            string filePath = currentDirectory + "\\" + fileName;

            IFileValidator fileValidator = new FileValidator();
            try
            {
                fileValidator.ValidateTestPackage(filePath);
                Assert.Fail();
            }
            catch(FileNotFoundException e) { }

            File.Create(filePath).Close();
            try
            {
                fileValidator.ValidateTestPackage(filePath);
                Assert.Fail();
            }
            catch (InvalidFileException e) { }

            File.Delete(filePath);

            fileName = "testFile.zip";
            filePath = currentDirectory + "\\" + fileName;
            File.Create(filePath).Close();
            fileValidator.ValidateTestPackage(filePath);
            File.Delete(filePath);
        }

        /* TODO: Test method for illustration csv file vaildator:
                Note: Want to validate headers of csv */
    }
}
