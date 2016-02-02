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

            //TODO: write directly 
            string validatingTest = Console.Properties.Resources.ValidatingTest;
            string validatingIllustation = Console.Properties.Resources.ValidatingIllustration;
            string creatingArchive = Console.Properties.Resources.CreatingArchive;

            IFileValidator fileValidator = new FileValidator();
            IArchiver archiver = new Archiver();
            IGlossaryAugmenter augmenter = new GlossaryAugmenter();

            System.Console.WriteLine(validatingTest);

            try
            {
                fileValidator.ValidateTestPackage(testPackageFilePath);
            }
            catch (Exception e)
            {
                exitWithErrorString("Error: Test package is invalid: " + e.Message);
            }

            System.Console.WriteLine(validatingIllustation + Environment.NewLine);

            try
            {
                fileValidator.ValidateIllustrationSpreadsheet(csvFilePath);
            }
            catch (Exception e)
            {
                exitWithErrorString("Error: Illustration spreadsheet is invalid: " + e.Message);
            }

            System.Console.WriteLine(creatingArchive + Environment.NewLine);

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

            string finished = Console.Properties.Resources.Finished;
            System.Console.WriteLine(finished);
            System.Console.Read();
        }

        static void exitWithErrorString(string errorString)
        {
            System.Console.WriteLine(errorString);
            System.Console.Read();
            Environment.Exit(1);
        }
    }
}
