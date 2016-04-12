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
            string errorsDirectory = Path.GetDirectoryName(testPackageFilePath);

            Console.WriteLine("Validating test package...");
            ValidateFiles(testPackageFilePath, csvFilePath, errorsDirectory);

            if (!noArchive)
            {
                Console.WriteLine("Creating archive of test package...");
                CreateArchive(testPackageFilePath);
            }

            Console.WriteLine("Adding illustrations to test package...");
            IEnumerable<Error> errors = AddIllustrationToTestPackage(testPackageFilePath, csvFilePath);

            Console.WriteLine("Recording Errors...");
            RecordErrors(errors, errorsDirectory);

            Console.WriteLine("Finished!");
            //Console.WriteLine("Press any key to exit.");
            //Console.ReadKey();
            Environment.Exit(0);
        }

        static void ValidateFiles(string testPackageFilePath, string csvFilePath, string errorsDirectory)
        {
            IFileValidator fileValidator = new FileValidator();
            ErrorRecorder errorRecorder = new ErrorRecorder();

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

            Console.WriteLine("Validating illustration list...");

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

            Console.WriteLine("Removing previous warning and error files...");

            try
            {
                errorRecorder.RemoveExistingErrors(errorsDirectory);
            }
            catch (IOException e)
            {
                ExitWithErrorString("Error: IOException: " + e.Message);
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
            catch (ElementDoesNotExistException e)
            {
                ExitWithErrorString("Error: InvalidXmlFile: " + e.Message);
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
            //Console.WriteLine("Press any key to exit.");
            //Console.ReadKey();
            Environment.Exit(1);
        }

        static void ShowHelpPageAndExit()
        {
            Console.WriteLine(getHelpPage(noArchiveArgs, helpPageArgs));
            Environment.Exit(0);
        }


        public static string getHelpPage(IEnumerable<string> noArchiveArgs, IEnumerable<string> helpPageArgs)
        {
            return @"Help Page for Illustration Glossary Package
Required Arguments:
    1. Test Package
    2. Illustrations csv
Optional Arguments
    1. " + string.Join(" ", noArchiveArgs) + @": Does not create an archive of the current test package
    2. " + string.Join(" ", helpPageArgs) + @": Show the help page
Requirements for Illustrations csv:
    Must have 3 columns: ItemId, Term, IllustrationFilename
    The first row in the csv is a header row
Example:
    IllustrationGlossaryPackage -n MyTestPackes/IrpContentPackage.zip MyIllustrations/Illustrations.csv
";
        }
    }
}
