using System.Linq;
using System.Xml.Linq;

namespace IllustrationGlossaryPackage.Dal.Infrastructure
{
    public static class XDocumentExtensions
    {
        /// <summary>
        /// Checks if the given XDocument is a Smarter Balance keyword list Xml
        /// </summary>
        /// <param name="itemXml"></param>
        /// <returns>True if the XDocument is a keyword list, otherwise false</returns>
        public static bool IsKeywordListItem(this XDocument itemXml)
        {
            try
            {
                return itemXml
                    .Element("itemrelease")
                        .Element("item")
                            .Attribute("type").Value == "wordList";
            }
            catch
            {
                return false;
            }
        }
    }
}
