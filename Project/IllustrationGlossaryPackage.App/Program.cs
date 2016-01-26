using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IllustrationGlossaryPackage.App
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                Console.WriteLine("Augmenting the test package: " + args[0]);
                Console.WriteLine("Using glossary illustrations: " + args[1]);
            }
            Console.Read();
        }
    }
}
