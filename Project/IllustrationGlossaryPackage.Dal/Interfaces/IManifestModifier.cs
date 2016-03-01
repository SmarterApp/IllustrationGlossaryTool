using IllustrationGlossaryPackage.Dal.Models;
using System.Collections.Generic;
using System.IO.Compression;
using System.Xml.Linq;

namespace IllustrationGlossaryPackage.Dal.Interfaces
{
    public interface IManifestModifier
    {
        XDocument GetManifestXml(string testPackageFilePath);
        void SaveManifest(XDocument manifest, ZipArchive testPackageArchive);
    }
}
