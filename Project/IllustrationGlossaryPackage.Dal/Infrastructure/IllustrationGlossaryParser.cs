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
    public class IllustrationGlossaryParser : Errorable, IErrorable, IIllustrationGlossaryParser
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
                int count = 2;
                line = file.ReadLine(); //ignore header line of csv
                while ((line = file.ReadLine()) != null)
                {
                    IList<string> lineItems = line.Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                    if (lineItems.Count() == 3)
                    {
                        Illustration illustration = new Illustration
                        {
                            ItemId = lineItems[0],
                            Term = lineItems[1],
                            FileName = lineItems[2],
                            FileExists = File.Exists(lineItems[2]),
                            Identifier = Path.GetFileNameWithoutExtension(lineItems[2]),
                            LineNumber = count
                        };
                        if (illustration.FileExists)
                        {
                            illustrations.Add(illustration);
                        }
                        else
                        {
                            errors.Add(new Error(Error.Type.FileDNE, illustration.FileName + " does not exist", count));
                        }
                    }
                    else
                    {
                        errors.Add(new Error(Error.Type.InvalidCsvLine, "Illustration must have 3 non-empty columns", count));
                    }

                    count++;
                }
            }

            return illustrations;
        }
    }
}
