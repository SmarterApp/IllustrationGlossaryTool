using IllustrationGlossaryPackage.Core.Infrastructure;
using IllustrationGlossaryPackage.Dal.Interfaces;
using IllustrationGlossaryPackage.Dal.Models;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;

namespace IllustrationGlossaryPackage.Dal.Infrastructure
{
    public class ManifestModifier : ItemsModifier, IManifestModifier
    {

        public static XNamespace Namespace = "http://www.imsglobal.org/xsd/apip/apipv1p0/imscp_v1p1";

        public IEnumerable<XElement> GetResourcesList(XDocument manifest)
        {
            return GetResources(manifest).Elements(Namespace + "resource");
        }

        public XElement GetResources(XDocument manifest)
        {
            return manifest
                .Element(Namespace + "manifest")
                .Element(Namespace + "resources");
        }


        public void SaveManifest(XDocument manifest, ZipArchive testPackageArchive)
        {
            ZipArchiveEntry manifestXml = GetManifestEntry(testPackageArchive);
            SaveItem(manifest, manifestXml);
        }

        public XDocument GetManifestXml(string testPackageFilePath)
        {
            XDocument manifestXml;
            using (ZipArchive testPackageArchive = ZipFile.Open(testPackageFilePath, ZipArchiveMode.Update))
            {
                ZipArchiveEntry manifestEntry = GetManifestEntry(testPackageArchive);
                manifestXml = XDocument.Load(manifestEntry.Open());
            }

            return manifestXml;
        }

        public XDocument GetManifestXml(ZipArchive testPackageArchive)
        {
            ZipArchiveEntry manifestEntry = GetManifestEntry(testPackageArchive);
            return XDocument.Load(manifestEntry.Open());
        }

        private ZipArchiveEntry GetManifestEntry(ZipArchive testPackageArchive)
        {
            return testPackageArchive.Entries.SingleOrDefault(t => t.FullName.ToLower().Contains("manifest"));
        }

    }
}
