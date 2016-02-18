using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IllustrationGlossaryPackage.Core.Infrastructure
{
    public class XRaw : XText
    {
        public XRaw(string text) : base(text) { }
        public XRaw(XText text) : base(text) { }

        public override void WriteTo(System.Xml.XmlWriter writer)
        {
            writer.WriteRaw(this.Value);
        }
    }
}
