using IllustrationGlossaryPackage.Dal.Interfaces;
using IllustrationGlossaryPackage.Dal.Models;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;

namespace IllustrationGlossaryPackage.Dal.Infrastructure
{
    public class ManifestModifier : IManifestModifier
    {
        private IItemsModifier itemsModifier;
        public ManifestModifier() : this(new ItemsModifier()) { }
        public ManifestModifier(ItemsModifier itemsModifier)
        {
            this.itemsModifier = itemsModifier;
        }
        public void SaveManifest(XDocument manifest, ZipArchive testPackageArchive)
        {
            // TODO: Avoid hardcoded file name
            ZipArchiveEntry manifestXml = testPackageArchive.Entries.FirstOrDefault(x => x.Name == "imsmanifest.xml");
            itemsModifier.SaveItem(manifest, manifestXml);
        }

        public XDocument GetManifestXml(string testPackageFilePath)
        {
            ZipArchive testPackageArchive = ZipFile.Open(testPackageFilePath, ZipArchiveMode.Update);
            ZipArchiveEntry manifestEntry = testPackageArchive.Entries.SingleOrDefault(t => t.FullName.ToLower().Contains("manifest"));
            XDocument manifestXml = XDocument.Load(manifestEntry.Open());
            testPackageArchive.Dispose();
            return manifestXml;
        }
    }
}
