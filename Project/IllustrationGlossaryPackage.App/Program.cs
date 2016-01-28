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
                if (!File.Exists(testPackageFilePath))
                {
                    Console.WriteLine("File does not exist: " + testPackageFilePath);
                }

                else if (!testPackageFilePath.EndsWith(".zip")){
                    Console.WriteLine("Test package must be a zip file");
                }

                else if (!File.Exists(csvFilePath))
                {
                    Console.WriteLine("File does not exist: " + csvFilePath);
                }

                else if (!csvFilePath.EndsWith(".csv"))
                {
                    Console.WriteLine("Illustration Glossary Spreadsheet must be a csv");
                }

                else
                {
                    Console.WriteLine("Augmenting the test package: " + testPackageFilePath);
                    Console.WriteLine("Using glossary illustrations: " + csvFilePath);
                    IArchiver archiver = new Archiver();
                    archiver.CreateArchive(testPackageFilePath);
                    IGlossaryAugmenter augmenter = new GlossaryAugmenter();
                    augmenter.AddItemsToGlossary(testPackageFilePath, csvFilePath);
                }

            }

            Console.WriteLine("Finished!");
            Console.Read();
        }
    }
}
