using IllustrationGlossaryPackage.Dal.Interfaces;
using IllustrationGlossaryPackage.Dal.Models;
using System.Collections.Generic;
using System.Xml.Linq;

namespace IllustrationGlossaryPackage.Dal.Infrastructure
{
    public class ManifestModifier : IManifestModifier
    {
        /// <summary>
        /// Add illustrations to the imsmanifest.xml file
        /// </summary>
        /// <param name="illustrations"></param>
        /// <param name="testPackageFilePath"></param>
        public void AddIllustrationsToManifest(IEnumerable<Illustration> illustrations, string testPackageFilePath)
        {

        }

        public XDocument GetManifestXml(string testPackageFilePath)
        {
            XDocument manifestXml = new XDocument();
            return manifestXml;
        }
    }
}
