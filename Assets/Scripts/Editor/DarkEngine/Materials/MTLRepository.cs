using Assets.Scripts.Editor.DarkEngine.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.Materials
{
    public class MTLRepository
    {
        TextureFileRepository textureFileRepo;
        Dictionary<SS2FileEntry, MTL> mtlCache = new Dictionary<SS2FileEntry, MTL>();

        public MTLRepository(TextureFileRepository textureFileRepo)
        {
            this.textureFileRepo = textureFileRepo;
        }

        public MTL Load(SS2FileEntry entry)
        {
            MTL mtl;
            if (!mtlCache.TryGetValue(entry, out mtl))
            {
                string materialPath = entry.absolutePath;
                string mtlContent = File.ReadAllText(materialPath);
                mtl = MTLParser.Parse(mtlContent, materialPath, entry.relativePath, textureFileRepo);
                mtlCache.Add(entry, mtl);
            }
            return mtl;
        }
    }
}
