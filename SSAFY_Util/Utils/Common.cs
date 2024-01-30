using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSAFY_Util.Utils
{
    internal class Common
    {
        public static string GetPath(string path)
        {
            string? folder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string filename = $@"{folder}\{path}";
            return filename;
        }
    }
}
