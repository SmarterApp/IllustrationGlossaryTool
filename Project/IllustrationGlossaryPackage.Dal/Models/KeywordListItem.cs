using System;
using System.Collections.Generic;
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
        public IList<AssessmentItem> AssessmentItems;
    }
}
