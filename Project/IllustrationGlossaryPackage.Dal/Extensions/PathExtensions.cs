using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IllustrationGlossaryPackage.Dal.Extensions
{
    public static class PathExtensions
    {
        public static string ToPath(this string s)
        {
            return s.Replace("\\", "/");
        }
    }
}
