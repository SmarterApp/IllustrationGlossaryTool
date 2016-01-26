using IllustrationGlossaryPackage.Core.Infrastructure;
using IllustrationGlossaryPackage.Core.Interfaces;
using IllustrationGlossaryPackage.Dal.Interfaces;
using IllustrationGlossaryPackage.Dal.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IllustrationGlossaryPackage.Dal.Models;

namespace IllustrationGlossaryPackage.Core.Infrastructure
{
    public class GlossaryAugmenter : IGlossaryAugmenter
    {
        private IIllustrationGlossaryParser parser;
        public GlossaryAugmenter() : this(new IllustrationGlossaryParser()) { }
        public GlossaryAugmenter(IllustrationGlossaryParser parser)
        {
            this.parser = parser;
        }

        /// <summary>
        /// Add items in csv file to glossary
        /// </summary>
        /// <param name="testPackageFilePath"></param>
        /// <param name="itemsFilePath"></param>
        public void AddItemsToGlossary(string testPackageFilePath, string itemsFilePath)
        {
            IEnumerable<Illustration> illustrations = parser.GetIllustrationsFromSpreadsheet(itemsFilePath);
        }
    }
}
