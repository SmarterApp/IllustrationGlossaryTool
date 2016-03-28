using IllustrationGlossaryPackage.Dal.Interfaces;
using IllustrationGlossaryPackage.Dal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
                        //TODO: parse illustration location path and find viewbox, find last two for size
                        Illustration illustration = new Illustration
                        {
                            ItemId = lineItems[0],
                            Term = lineItems[1],
                            OriginalFilePath = lineItems[2],
                            FileExists = File.Exists(lineItems[2]),
                            Identifier = Path.GetFileNameWithoutExtension(lineItems[2]),
                            LineNumber = count
                        };
                        try
                        {
                            XElement illustrationFile = XElement.Load(illustration.OriginalFilePath);
                            string size = illustrationFile
                                                       .Element("svg")
                                                       .Attribute("viewBox")
                                                       .Value;
                            string[] sizeValues = size.Split(' ');
                            if (size.Count() == 1)
                            {

                                illustration.Width = Int32.Parse(sizeValues[2]);  //3rd element
                                illustration.Height = Int32.Parse(sizeValues[3]); //4th element
                            }
                            else //no size values available
                            {
                                errors.Add(new Error(Error.Exception.IllustrationSize, illustration.OriginalFilePath + " has no size value", count));
                            }
                        }
                        catch
                        {
                            errors.Add(new Error(Error.Exception.IllustrationSize, illustration.OriginalFilePath + " does not exist", count));
                        }

                        if (illustration.FileExists)
                        {
                            illustrations.Add(illustration);
                        }
                        else
                        {
                            errors.Add(new Error(Error.Exception.FileDNE, illustration.OriginalFilePath + " does not exist", count));

                        }
                    }
                    else
                    {
                        errors.Add(new Error(Error.Exception.InvalidCsvLine, "Illustration must have 3 non-empty columns", count));
                    }

                    count++;
                }
            }

            return illustrations;
        }
    }
}
