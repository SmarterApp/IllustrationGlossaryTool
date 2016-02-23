using IllustrationGlossaryPackage.Dal.Interfaces;
using IllustrationGlossaryPackage.Dal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IllustrationGlossaryPackage.Dal.Infrastructure
{
    public class IllustrationGlossaryParser : IIllustrationGlossaryParser
    {
        /// <summary>
        /// Parses the csv of illustrations to be added to the test package
        /// </summary>
        /// <param name="csvFilePath">path to csv file</param>
        /// <returns>
        /// Returns the list of illustrations that were in the csv
        /// </returns>
        public IEnumerable<Illustration> GetIllustrationsFromSpreadsheet(string csvFilePath)
        {
            IList<Illustration> illustrations = new List<Illustration>();
            using (StreamReader file = new StreamReader(csvFilePath))
            {
                string line;
                line = file.ReadLine(); //ignore header line of csv
                while ((line = file.ReadLine()) != null)
                {
                    IList<string> lineItems = line.Split(',');
                    if (lineItems.Count() == 3)
                    {
                        Illustration illustration = new Illustration
                        {
                            ItemId = lineItems[0],
                            Term = lineItems[1],
                            FileName = lineItems[2],
                            FileExists = File.Exists(lineItems[2]),
                            Identifier = Path.GetFileNameWithoutExtension(lineItems[2])
                        };
                        if (illustration.FileExists)
                        {
                            illustrations.Add(illustration);
                        }
                        else
                        {
                            // TODO: Errors
                        }
                    }
                    else
                    {
                        // TODO: Errors
                    }
                }
            }

            return illustrations;
        }

    }
}
