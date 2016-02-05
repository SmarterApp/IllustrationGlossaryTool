using IllustrationGlossaryPackage.Core;
using IllustrationGlossaryPackage.Core.Infrastructure;
using IllustrationGlossaryPackage.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;

namespace IllustrationGlossaryPackage.App
{
    class Program
    {
        /// <summary>
        /// Add illustrations to a test package from a csv of items to add
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                exitWithErrorString("Error: Two command line arguments are required. Format: ");
            }

            string testPackageFilePath = args[0];
            string csvFilePath = args[1];

            IFileValidator fileValidator = new FileValidator();
            IArchiver archiver = new Archiver();
            IGlossaryAugmenter augmenter = new GlossaryAugmenter();

            Console.WriteLine("Validating test package...");

            try
            {
                fileValidator.ValidateTestPackage(testPackageFilePath);
            }
            catch (Exception e)
            {
                exitWithErrorString("Error: Test package is invalid: " + e.Message);
            }

            Console.WriteLine("Validating illustration list..." + Environment.NewLine);

            try
            {
                fileValidator.ValidateIllustrationSpreadsheet(csvFilePath);
            }
            catch (Exception e)
            {
                exitWithErrorString("Error: Illustration spreadsheet is invalid: " + e.Message);
            }

            Console.WriteLine("Creating archive of test package..." + Environment.NewLine);

            try
            {
                archiver.CreateArchive(testPackageFilePath);
            }
            catch (Exception e)
            {
                exitWithErrorString("Error: Failed to create archive: " + e.Message);
            }

            try
            {
                augmenter.AddItemsToGlossary(testPackageFilePath, csvFilePath);
            }
            catch (Exception e)
            {
                exitWithErrorString("Error: Failed while adding items to glossary: " + e.Message);
            }

            Console.WriteLine("Finished!");
            Console.Read();
        }

        static void exitWithErrorString(string errorString)
        {
            Console.WriteLine(errorString);
            Console.Read();
            Environment.Exit(1);
        }
    }
}
