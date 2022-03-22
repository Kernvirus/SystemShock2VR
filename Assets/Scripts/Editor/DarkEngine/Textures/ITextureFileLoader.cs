using UnityEngine;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Editor.DarkEngine.Textures
{
    public interface ITextureFileLoader
    {
        IEnumerable<Tuple<Texture, string>> Load(string name);

        Vector2Int GetTextureDimension(string name);
    }
}
