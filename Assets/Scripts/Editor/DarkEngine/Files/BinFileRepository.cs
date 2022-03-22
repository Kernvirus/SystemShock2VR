using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assets.Scripts.Editor.DarkEngine.Files
{
    class BinFileRepository : SS2FileRepository
    {
        public bool IsObjectMesh(string modelName)
        {
            string filePath = nameToPathMap[modelName].relativePath;
            return filePath.ToLower().StartsWith("obj/");
        }

        protected override IEnumerable<Tuple<string, SS2FileEntry>> DataFiles(ImporterSettings settings)
        {
            return settings.DataFiles(".bin").Select(t => {
                string key = Path.GetFileNameWithoutExtension(t.relativePath).ToLower();
                return new Tuple<string, SS2FileEntry>(key, t);
            });
        }
    }
}
