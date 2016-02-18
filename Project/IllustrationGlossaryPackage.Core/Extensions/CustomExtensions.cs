using System.Linq;
using System.Xml.Linq;

namespace IllustrationGlossaryPackage.Core.Infrastructure
{
    public static class CustomExtensions
    {
        public static XElement ElementOrCreate(this XElement parent, XName name)
        {
            bool elementExists = parent.Elements(name).Any();
            if (!elementExists)
            {
                XElement element = new XElement(name);
                parent.Add(element);
            }
            return parent.Element(name);
        }
    }
}
