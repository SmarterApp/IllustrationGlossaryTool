using IllustrationGlossaryPackage.Dal.Interfaces;
using IllustrationGlossaryPackage.Dal.Models;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Linq;

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
            ZipArchive testPackageArchive = ZipFile.Open(testPackageFilePath, ZipArchiveMode.Update);
            ZipArchiveEntry manifestEntry = testPackageArchive.Entries.SingleOrDefault(t => t.FullName.ToLower().Contains("manifest"));
            XDocument manifestXml = XDocument.Load(manifestEntry.Open());
            return manifestXml;
        }
    }
}
