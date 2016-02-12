using System.Linq;
using System.Xml.Linq;

namespace IllustrationGlossaryPackage.Dal.Infrastructure
{
    public static class XDocumentExtensions
    {
        /// <summary>
        /// Checks if the given XDocument is a Smarter Balance keyword list item
        /// </summary>
        /// <param name="itemXml"></param>
        /// <returns>True if the XDocument is a keyword list item, otherwise false</returns>
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

        /// <summary>
        /// Checks if the given XDocument is a Smarter Balance content item
        /// </summary>
        /// <param name="itemXml"></param>
        /// <returns>True if the XDocument is a content item, otherwise false</returns>
        public static bool IsContentItem(this XDocument itemXml)
        {
            try
            {
                return itemXml
                    .Element("itemrelease")
                        .Element("item")
                            .Attribute("format") != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
