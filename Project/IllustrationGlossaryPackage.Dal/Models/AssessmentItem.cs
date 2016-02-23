using IllustrationGlossaryPackage.Dal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IllustrationGlossaryPackage.Core.Infrastructure
{
    public class AssessmentItem
    {
        public string ItemId;
        public string KeywordListItemId;
        public string FullPath;
        public string Name;
        public string Identifier;
        public XDocument Document;
        public IEnumerable<Illustration> Illustrations;
    }
}
