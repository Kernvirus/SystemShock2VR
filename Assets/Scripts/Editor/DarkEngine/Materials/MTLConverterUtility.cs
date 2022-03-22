using Assets.Scripts.Editor.DarkEngine.Files;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.Materials
{
    static class MTLConverterUtility
    {
        public static Texture2D AssignMainTexture(Material material, MTL mtl, UnitySS2AssetRepository unitySS2AssetRepo)
        {
            return AssignTexture(material, "_MainTex", mtl.GetMainTexturePath(), unitySS2AssetRepo);
        }

        public static Texture2DArray AssignTextureArray(Material material, string propName, string textureName, UnitySS2AssetRepository unitySS2AssetRepo)
        {
            var mainTexPath = PathUtility.FilePathWithoutExtension(textureName).ToLower().Replace('\\', '/');
            var tex = unitySS2AssetRepo.LoadTextureArrayAsset(mainTexPath);
            
            material.SetTexture(propName, tex);
            return tex;
        }

        public static Texture2D AssignTexture(Material material, string propName, string textureName, UnitySS2AssetRepository unitySS2AssetRepo)
        {
            var texPath = PathUtility.FilePathWithoutExtension(textureName).ToLower().Replace('\\', '/');
            var tex = unitySS2AssetRepo.LoadTextureAsset(texPath);
            material.SetTexture(propName, tex);
            return tex;
        }
    }
}