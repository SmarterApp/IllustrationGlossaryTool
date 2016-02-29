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
using IllustrationGlossaryPackage.Dal.Models;
using IllustrationGlossaryPackage.Dal.Infrastructure;
using IllustrationGlossaryPackage.Dal.Interfaces;

namespace IllustrationGlossaryPackage.App
{
    public class Program
    {

        public static IList<string> noArchiveArgs = new List<string> { "-n" };
        public static IList<string> helpPageArgs = new List<string> { "-h", "/?", "-?", "--help", "?" };

        /// <summary>
        /// Add illustrations to a test package from a csv of items to add
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            if (helpPageArgs.Any(s => args.Contains(s)))
            {
                ShowHelpPageAndExit();
                return;
            }

            bool noArchive = noArchiveArgs.Any(s => args.Contains(s));
            args = args.Where(s => !noArchiveArgs.Contains(s)).ToArray();

            if (args.Length != 2)
            {
                ExitWithErrorString("Error: Command line arguments are required for test package and illustrations csv");
            }

            string testPackageFilePath = args[0];
            string csvFilePath = args[1];

            Console.WriteLine("Validating test package..." + Environment.NewLine);
            ValidateFiles(testPackageFilePath, csvFilePath);

            if (!noArchive)
            {
                Console.WriteLine("Creating archive of test package..." + Environment.NewLine);
                CreateArchive(testPackageFilePath);
            }
           
            Console.WriteLine("Adding illustrations to test package..." + Environment.NewLine);
            IEnumerable<Error> errors = AddIllustrationToTestPackage(testPackageFilePath, csvFilePath);

            Console.WriteLine("Recording Errors..." + Environment.NewLine);
            RecordErrors(errors, Path.GetDirectoryName(testPackageFilePath));

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

        static IEnumerable<Error> AddIllustrationToTestPackage(string testPackageFilePath, string csvFilePath)
        {
            IGlossaryAugmenter augmenter = new GlossaryAugmenter();
            try
            {
                augmenter.AddItemsToGlossary(testPackageFilePath, csvFilePath);
            }
            catch (IOException e) 
            {
                ExitWithErrorString("Error: IOException: " + e.Message);
            }

            return augmenter.GetErrors();
        }

        static void RecordErrors(IEnumerable<Error> errors, string directory)
        {
            IErrorRecorder errorRecorder = new ErrorRecorder();
            try
            {
                errorRecorder.RecordErrors(errors, directory);
            }
            catch (IOException e)
            {
                ExitWithErrorString("Error: IOException: " + e.Message);
            }
        }


        static void ExitWithErrorString(string errorString)
        {
            Console.WriteLine(errorString);
            Console.Read();
            Environment.Exit(0);
        }

        static void ShowHelpPageAndExit()
        {
            Console.WriteLine("TODO: This is my help page");
            Environment.Exit(0);
        }
    }
}
