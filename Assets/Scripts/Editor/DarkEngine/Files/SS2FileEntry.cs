using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Editor.DarkEngine
{
    public class SS2FileEntry
    {
        public string relativePath;
        public string absolutePath;

        public SS2FileEntry(string relativePath, string absolutePath)
        {
            this.relativePath = relativePath.Replace('\\', '/');
            this.absolutePath = absolutePath.Replace('\\', '/');
        }
    }
}
