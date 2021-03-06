﻿using IllustrationGlossaryPackage.Dal.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IllustrationGlossaryPackage.Dal.Models
{
    public class Illustration
    {
        public string ItemId { get; set; }
        public string Term { get; set; }
        public string OriginalFilePath { get; set; }
        public bool FileExists { get; set; }
        public string CopiedToPath { get; set; }
        public string Identifier { get; set; }
        public int LineNumber { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public int Index { get; set; }

        public bool KeywordAdded = false;
        public string FileName
        {
            get
            {
                return Path.GetFileName(OriginalFilePath);
            }
        }

        public ZipArchiveEntry GetZipArchiveEntry(ZipArchive z)
        {
            return z.Entries.FirstOrDefault(x => x.FullName == this.CopiedToPath);
        }

        public string CopiedToPathForCreate
        {
            get
            {
                return CopiedToPath.ToPath();
            }
        }
    }
}
