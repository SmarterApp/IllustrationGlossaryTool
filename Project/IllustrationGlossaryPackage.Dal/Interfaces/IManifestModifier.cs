using IllustrationGlossaryPackage.Dal.Models;
using System.Collections.Generic;
using System.Xml.Linq;

namespace IllustrationGlossaryPackage.Dal.Interfaces
{
    public interface IManifestModifier
    {
        void AddIllustrationsToManifest(IEnumerable<Illustration> illustrations, string testPackageFilePath);
        XDocument GetManifestXml(string testPackageFilePath);
    }
}
