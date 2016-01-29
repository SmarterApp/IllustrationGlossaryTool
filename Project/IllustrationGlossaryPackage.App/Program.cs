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
                string validatingTest = Console.Properties.Resources.ValidatingTest;
                System.Console.WriteLine(validatingTest);
                fileValidator.ValidateTestPackage(testPackageFilePath);
                string validatingIll = Console.Properties.Resources.ValidatingIll;
                System.Console.WriteLine(validatingIll);
                fileValidator.ValidateIllustrationSpreadsheet(csvFilePath);
                System.Console.WriteLine();

                IArchiver archiver = new Archiver();
                string creatingArchive = Console.Properties.Resources.CreatingArchive;
                System.Console.WriteLine(creatingArchive);
                archiver.CreateArchive(testPackageFilePath);
                System.Console.WriteLine();

                IGlossaryAugmenter augmenter = new GlossaryAugmenter();
                augmenter.AddItemsToGlossary(testPackageFilePath, csvFilePath);

            }

            string finished = Console.Properties.Resources.Finished;
            System.Console.WriteLine(finished);
            System.Console.Read();
        }
    }
}
