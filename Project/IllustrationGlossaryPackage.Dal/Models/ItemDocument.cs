﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IllustrationGlossaryPackage.Dal.Models
{
    public class ItemDocument
    {
        public string FullPath;
        public string Name;
        public XDocument Document;
    }
}
