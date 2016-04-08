using IllustrationGlossaryPackage.Dal.Interfaces;
using IllustrationGlossaryPackage.Dal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Configuration;

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
                            OriginalFilePath = lineItems[2],
                            FileExists = File.Exists(lineItems[2]),
                            Identifier = Path.GetFileNameWithoutExtension(lineItems[2]),
                            LineNumber = count
                        };

                        SetIllustrationSize(illustration, count);

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

        /// <summary>
        /// Parses the svg item for the natural width and height and calculates the max dimensions for the html tag
        /// image. The image is scaled using the DefaultPixelSize in the project settings.
        /// </summary>
        /// <param name="illustration"></param>
        /// <param name="count"></param>
        private void SetIllustrationSize(Illustration illustration, int count)
        {
            try
            {
                XDocument illustrationFile = XDocument.Load(illustration.OriginalFilePath);
                XElement node = illustrationFile.Root;
                string size = node.Attribute("viewBox").Value;
                string[] sizeValues = size.Split(' ');
                double maxPixels = Properties.Settings.Default.DefaultPixelSize;

                if (sizeValues.Count() == 4)
                {
                    double width = Convert.ToDouble(sizeValues[2]);
                    double height = Convert.ToDouble(sizeValues[3]);
                    double ratio = maxPixels / Math.Max(width, height);
                    width = Math.Round(width * ratio, 2);
                    height = Math.Round(height * ratio, 2);

                    illustration.Width = Convert.ToString(width); 
                    illustration.Height = Convert.ToString(height);
                }
                else
                {
                    errors.Add(new Error(Error.Exception.IllustrationSize, illustration.OriginalFilePath + " has no size value", count));
                }

            }
            catch
            {
                errors.Add(new Error(Error.Exception.IllustrationSize, illustration.OriginalFilePath + " does not exist", count));
            }
        }
    }
}
