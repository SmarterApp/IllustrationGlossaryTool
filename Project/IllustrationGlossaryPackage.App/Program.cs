using IllustrationGlossaryPackage.Core;
using IllustrationGlossaryPackage.Core.Infrastructure;
using IllustrationGlossaryPackage.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                else if (!File.Exists(csvFilePath))
                {
                    Console.WriteLine("File does not exist: " + csvFilePath);
                }

                else
                {
                    Console.WriteLine("Augmenting the test package: " + args[0]);
                    Console.WriteLine("Using glossary illustrations: " + args[1]);
                    IGlossaryAugmenter augmenter = new GlossaryAugmenter();
                    augmenter.AddItemsToGlossary(testPackageFilePath, csvFilePath);
                }
            }

            Console.Read();
        }
    }
}
