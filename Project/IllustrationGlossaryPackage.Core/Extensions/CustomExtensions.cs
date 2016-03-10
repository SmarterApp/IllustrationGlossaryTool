using IllustrationGlossaryPackage.Core.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace IllustrationGlossaryPackage.Core.Infrastructure
{
    public static class CustomExtensions
    {
        public static XElement ElementOrCreate(this XElement parent, XName name)
        {
            AddIfNotExists(parent, name);
            return parent.Element(name);
        }

        public static IEnumerable<XElement> ElementsOrCreate(this XElement parent, XName name)
        {
            AddIfNotExists(parent, name);
            return parent.Elements(name);
        }

        private static void AddIfNotExists(this XElement parent, XName name)
        {
            bool elementExists = parent.Elements(name).Any();
            if (!elementExists)
            {
                XElement element = new XElement(name);
                parent.Add(element);
            }
        }

        public static XElement ElementOrException(this XElement parent, string eltName)
        {
            XElement elt = parent.Element(eltName);
            return ElementOrException(parent, elt, eltName);
        }

        public static XElement ElementOrException(this XElement parent, XName eltName)
        {
            XElement elt = parent.Element(eltName);
            return ElementOrException(parent, elt, eltName.LocalName);
        }

        public static XElement ElementOrException(this XDocument parent, string eltName)
        {
            XElement elt = parent.Element(eltName);
            return ElementOrException(parent, elt, eltName);
        }

        public static XElement ElementOrException(this XDocument parent, XName eltName)
        {
            XElement elt = parent.Element(eltName);
            return ElementOrException(parent, elt, eltName.LocalName);
        }

        public static IEnumerable<XElement> ElementsOrException(this XElement parent, XName eltName)
        {
            if(parent == null)
            {
                throw new NullReferenceException();
            }
            IEnumerable<XElement> elts = parent.Elements(eltName);
            return ElementsOrException(parent, elts, eltName.LocalName);
        }

        private static XElement ElementOrException(this XElement parent, XElement elt, string eltName)
        {
            string msg = "Element <{0}> does not exist as child of <{1}>";
            if (elt == null)
            {
                throw new ElementDoesNotExistException(string.Format(msg, eltName, parent.Name.LocalName));
            }
            return elt;
        }

        private static XElement ElementOrException(this XDocument parent, XElement elt, string eltName)
        {
            string msg = "Element <{0}> does not exist";
            if (elt == null)
            {
                throw new ElementDoesNotExistException(string.Format(msg, eltName));
            }
            return elt;
        }

        private static IEnumerable<XElement> ElementsOrException(this XElement parent, IEnumerable<XElement> elts, string eltName)
        {
            string msg = "Element <{0}> does not exist as child of <{1}>";
            if (elts == null || elts.Count() < 1)
            {
                throw new ElementDoesNotExistException(string.Format(msg, eltName, parent.Name.LocalName));
            }
            return elts;
        }

        /// <summary>
        /// null safe way to get an attributes value from an xml element
        /// </summary>
        /// <param name="e"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static string GetAttribute(this XElement e, string attributeName)
        {
            XAttribute attribute = e.Attribute(attributeName);
            return NullSaveValue(attribute);
        }

        public static string GetAttribute(this XElement e, XName attributeName)
        {
            XAttribute attribute = e.Attribute(attributeName);
            return NullSaveValue(attribute);
        }

        private static string NullSaveValue(this XAttribute attribute)
        {
            return attribute == null ? string.Empty : attribute.Value;
        }
    }
}
