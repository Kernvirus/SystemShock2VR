using Assets.Scripts.Editor.DarkEngine.Files;
using Assets.Scripts.Editor.DarkEngine.Materials;
using Assets.Scripts.Editor.DarkEngine.Models;
using Pfim;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.Textures
{
    class TextureCache : ITextureFileLoader
    {
        Dictionary<string, Tuple<Texture, string>[]> textureCache = new Dictionary<string, Tuple<Texture, string>[]>();
        Dictionary<string, Vector2Int> textureDimensionCache = new Dictionary<string, Vector2Int>();
        TextureFileLoader textureFileLoader;

        public TextureCache(TextureFileLoader textureFileLoader)
        {
            this.textureFileLoader = textureFileLoader;
        }

        public IEnumerable<Tuple<Texture, string>> Load(string name)
        {
            Tuple<Texture, string>[] texture;
            if (!textureCache.TryGetValue(name, out texture))
            {
                texture = textureFileLoader.Load(name).ToArray();
                textureCache.Add(name, texture);
            }
            return texture;
        }

        public Vector2Int GetTextureDimension(string name)
        {
            Vector2Int dim;
            if (!textureDimensionCache.TryGetValue(name, out dim))
            {
                dim = textureFileLoader.GetTextureDimension(name);
                textureDimensionCache.Add(name, dim);
            }
            return dim;
        }
    }
}
