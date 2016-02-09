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
using IllustrationGlossaryPackage.Core.Exceptions;

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
                ExitWithErrorString("Error: Two command line arguments are required. Format: ");
            }

            string testPackageFilePath = args[0];
            string csvFilePath = args[1];

            Console.WriteLine("Validating test package..." + Environment.NewLine);
            ValidateFiles(testPackageFilePath, csvFilePath);

            Console.WriteLine("Creating archive of test package..." + Environment.NewLine);
            CreateArchive(testPackageFilePath);

            Console.WriteLine("Adding illustrations to test package..." + Environment.NewLine);
            AddIllustrationToTestPackage(testPackageFilePath, csvFilePath);

            Console.WriteLine("Finished!");
            Console.Read();
        }

        static void ValidateFiles(string testPackageFilePath, string csvFilePath)
        {
            IFileValidator fileValidator = new FileValidator();
            try
            {
                fileValidator.ValidateTestPackage(testPackageFilePath);
            }
            catch (FileNotFoundException e)
            {
                ExitWithErrorString("Error: Test package not found: " + e.Message);
            }
            catch (InvalidFileException e)
            {
                ExitWithErrorString("Error: Test package is invalid: " + e.Message);
            }

            Console.WriteLine("Validating illustration list..." + Environment.NewLine);

            try
            {
                fileValidator.ValidateIllustrationSpreadsheet(csvFilePath);
            }
            catch (FileNotFoundException e)
            {
                ExitWithErrorString("Error: Illustration spreadsheet not found: " + e.Message);
            }
            catch (InvalidFileException e)
            {
                ExitWithErrorString("Error: Illustration spreadsheet is invalid: " + e.Message);
            }
        }

        static void CreateArchive(string testPackageFilePath)
        {
            IArchiver archiver = new Archiver();
            try
            {
                archiver.CreateArchive(testPackageFilePath);
            }
            catch (ArchiveAlreadyExistsException e)
            {
                ExitWithErrorString("Error: Archive already exists: " + e.Message);
            }
        }

        static void AddIllustrationToTestPackage(string testPackageFilePath, string csvFilePath)
        {
            IGlossaryAugmenter augmenter = new GlossaryAugmenter();
            try
            {
                augmenter.AddItemsToGlossary(testPackageFilePath, csvFilePath);
            }
            catch (Exception e)  // TODO only catch certain exceptions
            {
                ExitWithErrorString("Error: Failed while adding items to glossary: " + e.Message);
            }
        }


        static void ExitWithErrorString(string errorString)
        {
            Console.WriteLine(errorString);
            Console.Read();
            Environment.Exit(1);
        }
    }
}
