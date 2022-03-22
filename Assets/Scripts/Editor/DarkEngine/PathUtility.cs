using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Editor.DarkEngine
{
    static class PathUtility
    {
        public static string FilePathWithoutExtension(string path)
        {
            int dotIndex = path.IndexOf('.');
            if (dotIndex != -1)
            {
                path = path.Substring(0, dotIndex);
            }
            return path;
        }
    }
}
