using Assets.Scripts.Editor.DarkEngine.Files;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.Materials
{
    class AnimTexArrayCreator : IMTLConverter
    {
        Shader shader = Shader.Find("SS2/AnimTexArray");

        public Material Create(MTL mtl, Color tint, UnitySS2AssetRepository unitySS2AssetRepo)
        { 
            Material mat = new Material(shader);
            var texArray = MTLConverterUtility.AssignTextureArray(mat, "_MainTexArr", mtl.renderPasses[0].texturePaths[0], unitySS2AssetRepo);
            int frameCount = texArray.depth;

            mat.SetFloat("_TexCount", frameCount);
            mat.SetFloat("_FPS", 1000.0f / mtl.renderPasses[0].aniRate);
            mat.SetFloat("_EmissionIntensity", 1);
            return mat;
        }

        public bool Match(MTL mtl)
        {
            return mtl.renderPasses.Count >= 1 &&
                mtl.renderMaterialOnly &&
                mtl.renderPasses[0].shaded &&
                mtl.renderPasses[0].texturePaths.Count > 1;
        }
    }
}