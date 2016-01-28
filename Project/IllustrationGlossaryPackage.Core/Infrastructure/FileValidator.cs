using IllustrationGlossaryPackage.Core.Exceptions;
using IllustrationGlossaryPackage.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IllustrationGlossaryPackage.Core.Infrastructure
{
    public class FileValidator : IFileValidator
    {
        /// <summary>
        /// Validates that the test package parameter is a valid test package
        /// </summary>
        /// <param name="testPackage"></param>
        public void ValidateTestPackage(string testPackage)
        {
            if (!File.Exists(testPackage))
            {
                throw new FileNotFoundException("File does not exist: " + testPackage);
            }

            else if (!testPackage.EndsWith(".zip"))
            {
                throw new InvalidFileException("Test package must be a zip file");
            }
        }

        /// <summary>
        /// Validates that the illustration glossary csv is valid
        /// </summary>
        /// <param name="csvPath"></param>
        public void ValidateIllustrationSpreadsheet(string csvPath)
        {
            if (!File.Exists(csvPath))
            {
                throw new FileNotFoundException("File does not exist: " + csvPath);
            }

            else if (!csvPath.EndsWith(".csv"))
            {
                throw new InvalidFileException("Illustration Glossary Spreadsheet must be a csv");
            }
        }
    }
}
