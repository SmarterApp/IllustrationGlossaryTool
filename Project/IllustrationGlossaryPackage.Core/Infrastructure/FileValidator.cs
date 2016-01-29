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
            string zipFileExt = Properties.Resources.ZipFileExtention;

            if (!File.Exists(testPackage))
            {
                string msg = string.Format(Properties.Resources.FileDNE, testPackage);
                throw new FileNotFoundException(msg);
            }

            else if (!testPackage.EndsWith(zipFileExt))
            {
                throw new InvalidFileException(Properties.Resources.TestMustZip);
            }
        }

        /// <summary>
        /// Validates that the illustration glossary csv is valid
        /// </summary>
        /// <param name="csvPath"></param>
        public void ValidateIllustrationSpreadsheet(string csvPath)
        {
            string csvFileExt = Properties.Resources.CsvFileExtention;

            if (!File.Exists(csvPath))
            {
                string msg = string.Format(Properties.Resources.FileDNE, csvPath);
                throw new FileNotFoundException(msg);
            }

            else if (!csvPath.EndsWith(csvFileExt))
            {
                throw new InvalidFileException(Properties.Resources.IllMustCsv);
            }
        }
    }
}
