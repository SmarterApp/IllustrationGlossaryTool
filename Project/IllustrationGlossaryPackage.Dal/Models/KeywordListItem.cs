using IllustrationGlossaryPackage.Dal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IllustrationGlossaryPackage.Core.Infrastructure
{
    public class KeywordListItem
    {
        public string ItemId;
        public string FullPath;
        public XDocument Document;
        public string Identifier;
        public IList<AssessmentItem> AssessmentItems;

        public KeywordListItem(string ItemId, ItemDocument itDoc)
        {
            this.ItemId = ItemId;
            this.AssessmentItems = new List<AssessmentItem>();
            this.Document = itDoc.Document;
            this.FullPath = itDoc.FullPath;
            this.Identifier = Path.GetFileNameWithoutExtension(itDoc.FullPath);
        }

        public string MetadataIdentifier
        {
            get
            {
                return Identifier + "_metadata";
            }
        }
    }
}
