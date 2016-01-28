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
            if (args.Length == 2)
            {
                string testPackageFilePath = args[0];
                string csvFilePath = args[1];
     
                IFileValidator fileValidator = new FileValidator();
                Console.WriteLine("Validating test package...");
                fileValidator.ValidateTestPackage(testPackageFilePath);
                Console.WriteLine("Validating illustration list...");
                fileValidator.ValidateIllustrationSpreadsheet(csvFilePath);
                Console.WriteLine(); Console.WriteLine();

                IArchiver archiver = new Archiver();
                Console.WriteLine("Augmenting the test package: " + testPackageFilePath);
                Console.WriteLine("Using glossary illustrations: " + csvFilePath);   
                archiver.CreateArchive(testPackageFilePath);
                Console.WriteLine(); Console.WriteLine();

                IGlossaryAugmenter augmenter = new GlossaryAugmenter();
                augmenter.AddItemsToGlossary(testPackageFilePath, csvFilePath);

            }

            Console.WriteLine("Finished!");
            Console.Read();
        }
    }
}
