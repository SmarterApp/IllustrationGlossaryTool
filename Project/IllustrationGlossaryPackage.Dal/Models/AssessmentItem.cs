using IllustrationGlossaryPackage.Dal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
        public string Bankkey;
        public string ItemVersion;
        public XDocument Document;
        public IEnumerable<Illustration> Illustrations;

        public AssessmentItem(string ItemId, string KeywordListItemId, string bankKey, string itemVersion, IEnumerable<Illustration> illustrations, ItemDocument document)
        {
            if (document != null)
            {
                this.ItemId = ItemId;
                this.KeywordListItemId = KeywordListItemId;
                this.Illustrations = illustrations;
                this.Document = document.Document;
                this.FullPath = document.FullPath;
                this.Name = document.Name;
                this.Bankkey = bankKey;
                this.ItemVersion = itemVersion;
                this.Identifier = Path.GetFileNameWithoutExtension(document.FullPath);
            }   
        }

        public AssessmentItem(string ItemId, IEnumerable<Illustration> illustrations)
        {
            this.ItemId = ItemId;
            this.Illustrations = illustrations.ToList();
        }

        public string KeywordListFullPath
        {
            get
            {
                return string.Format("Items/Item-{0}-{1}/item-{0}-{1}.xml", Bankkey, KeywordListItemId);
            }
        }

        public string KeywordListMetadataFullPath
        {
            get
            {
                return string.Format("Items/Item-{0}-{1}/metadata.xml", Bankkey, KeywordListItemId);
            }
        }

        public ZipArchiveEntry GetZipArchiveEntry(ZipArchive z)
        {
            return z.Entries.FirstOrDefault(x => x.FullName == this.FullPath);
        }
    }

}
