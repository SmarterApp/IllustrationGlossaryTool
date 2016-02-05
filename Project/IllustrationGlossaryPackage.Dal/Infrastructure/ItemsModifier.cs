using IllustrationGlossaryPackage.Dal.Interfaces;
using IllustrationGlossaryPackage.Dal.Models;
using System.Collections.Generic;

namespace IllustrationGlossaryPackage.Dal.Infrastructure
{
    public class ItemsModifier : IItemsModifier
    {
        /// <summary>
        /// Adds each illustration to the corresponding item in the test package
        /// </summary>
        /// <param name="illustrations"></param>
        /// <param name="testPackageFilePath"></param>
        public void AddIllustrationsToItems(IEnumerable<Illustration> illustrations, string testPackageFilePath)
        {

        }

        private static void MoveMediaFileForIllustrationToPath(Illustration illustration, string path)
        {

        }

        private static void UpdateXmlForIllustration(Illustration illustration)
        {

        }
    }
}
