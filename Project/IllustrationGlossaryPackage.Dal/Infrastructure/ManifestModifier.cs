using IllustrationGlossaryPackage.Dal.Interfaces;
using IllustrationGlossaryPackage.Dal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
