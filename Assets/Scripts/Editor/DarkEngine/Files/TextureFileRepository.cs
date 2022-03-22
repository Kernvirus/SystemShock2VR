using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Editor.DarkEngine.Files
{
    public class TextureFileRepository : SS2FileRepository
    {
        protected static readonly string[] ss2ImageSearchPatterns = new string[] {
            ".mtl",
            ".dds",
            ".png",
            ".gif",
            ".tga",
            ".bmp",
            ".pcx",
            //".CEL" (not supported)
        };

        public bool IsMTL(string name)
        {
            return name.ToLower().EndsWith("mtl");
        }

        public override SS2FileEntry GetPath(string name)
        {
            for (int i = 0; i < ss2ImageSearchPatterns.Length; i++)
            {
                if (nameToPathMap.ContainsKey(name + ss2ImageSearchPatterns[i]))
                    return nameToPathMap[name + ss2ImageSearchPatterns[i]];
            }
            throw new KeyNotFoundException();
        }

        public override bool DoesNameExist(string name)
        {
            for (int i = 0; i < ss2ImageSearchPatterns.Length; i++)
            {
                if (nameToPathMap.ContainsKey(name + ss2ImageSearchPatterns[i]))
                    return true;
            }
            return false;
        }

        public bool DoesTextureNameExist(string name)
        {
            for (int i = 1; i < ss2ImageSearchPatterns.Length; i++)
            {
                if (nameToPathMap.ContainsKey(name + ss2ImageSearchPatterns[i]))
                    return true;
            }
            return false;
        }

        public SS2FileEntry GetTextureOnlyPath(string name)
        {
            for (int i = 1; i < ss2ImageSearchPatterns.Length; i++)
            {
                if (nameToPathMap.ContainsKey(name + ss2ImageSearchPatterns[i]))
                    return nameToPathMap[name + ss2ImageSearchPatterns[i]];
            }
            string searched = name;
            while (!nameToPathMap.Keys.Any(n => n.StartsWith(searched)) && searched.Length >= 0)
            {
                searched = searched.Substring(0, searched.Length - 1);
            }
            string[] similiar = nameToPathMap.Keys.Where(n => n.StartsWith(searched)).ToArray();

            throw new KeyNotFoundException();
        }

        protected override IEnumerable<Tuple<string, SS2FileEntry>> DataFiles(ImporterSettings settings)
        {
            return settings.DataFiles(ss2ImageSearchPatterns)
                .Select(entry =>
                {
                    string key = entry.relativePath.ToLower();
                    return new Tuple<string, SS2FileEntry>(key, entry);
                });
        }
    }
}
