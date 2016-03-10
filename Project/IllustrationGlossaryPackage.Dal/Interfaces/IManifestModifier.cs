using IllustrationGlossaryPackage.Dal.Models;
using System.Collections.Generic;
using System.IO.Compression;
using System.Xml.Linq;

namespace IllustrationGlossaryPackage.Dal.Interfaces
{
    public interface IManifestModifier : IItemsModifier
    {
        XDocument GetManifestXml(string testPackageFilePath);
        XDocument GetManifestXml(ZipArchive testPackageArchive);
        IEnumerable<XElement> GetResourcesList(XDocument manifest);
        XElement GetResources(XDocument manifest);
        void SaveManifest(XDocument manifest, ZipArchive testPackageArchive);
    }
}
