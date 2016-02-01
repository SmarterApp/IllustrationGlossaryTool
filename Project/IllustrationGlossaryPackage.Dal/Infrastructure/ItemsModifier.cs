using IllustrationGlossaryPackage.Dal.Interfaces;
using IllustrationGlossaryPackage.Dal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
