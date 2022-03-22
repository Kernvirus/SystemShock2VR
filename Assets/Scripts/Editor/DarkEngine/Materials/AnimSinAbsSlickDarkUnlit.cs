using Assets.Scripts.Editor.DarkEngine.Files;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.Materials
{
    class AnimSinAbsSlickDarkUnlit : IMTLConverter
    {
        Shader shader = Shader.Find("SS2/AnimSinAbsSlickDarkUnlit");

        public Material Create(MTL mtl, Color tint, UnitySS2AssetRepository unitySS2AssetRepo)
        { 
            Material mat = new Material(shader);
            MTLConverterUtility.AssignMainTexture(mat, mtl, unitySS2AssetRepo);

            WaveFunction wave = mtl.renderPasses[1].alphaFunction as WaveFunction;

            mat.SetFloat("_Bias", wave.bias);
            mat.SetFloat("_Amplitude", wave.amplitude);
            mat.SetFloat("_Frequency", wave.frequency / 1000);
            mat.SetColor("_Color", tint);
            return mat;
        }

        public bool Match(MTL mtl)
        {
            return mtl.renderPasses.Count == 2 &&
                mtl.renderMaterialOnly &&
                mtl.renderPasses[0].texturePaths.Count == 1 &&
                mtl.renderPasses[0].texturePaths[0].Contains("gfx_slickdark") &&
                mtl.renderPasses[1].alphaFunction != null &&
                mtl.renderPasses[1].alphaFunction as WaveFunction != null &&
                mtl.renderPasses[1].texturePaths.Count == 1;
        }
    }
}