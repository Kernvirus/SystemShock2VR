using Assets.Scripts.Editor.DarkEngine.Files;
using Assets.Scripts.Editor.DarkEngine.Models;
using Assets.Scripts.Editor.DarkEngine.Textures;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.Materials
{
    class SingleTextureCreator : IMTLConverter
    {
        public enum TextureAlpha
        {
            None,
            Soft,
            Cutout
        }

        const string litPath = "Assets/Materials/ss2Template_lit.mat";
        const string litSpecularPath = "Assets/Materials/ss2Template_litSpecular.mat";
        const string unlitPath = "Assets/Materials/ss2Template_unlit.mat";
        const string transparentLitPath = "Assets/Materials/ss2Template_transparentLit.mat";
        const string cutOutLitPath = "Assets/Materials/ss2Template_cutOutLit.mat";
        const string emissiveLitPath = "Assets/Materials/ss2Template_emissiveLit.mat";

        Material litMat;
        Material litSpecularMat;
        Material unlitMat;
        Material transparentLitMat;
        Material cutOutLitMat;
        Material emissiveLitMat;

        Shader animTexArray = Shader.Find("Shader Graphs/AnimTexArray");

        ITextureFileLoader textureFileLoader;

        public SingleTextureCreator(ITextureFileLoader textureFileLoader)
        {
            litMat = AssetDatabase.LoadAssetAtPath<Material>(litPath);
            unlitMat = AssetDatabase.LoadAssetAtPath<Material>(unlitPath);
            transparentLitMat = AssetDatabase.LoadAssetAtPath<Material>(transparentLitPath);
            cutOutLitMat = AssetDatabase.LoadAssetAtPath<Material>(cutOutLitPath);
            emissiveLitMat = AssetDatabase.LoadAssetAtPath<Material>(emissiveLitPath);
            litSpecularMat = AssetDatabase.LoadAssetAtPath<Material>(litSpecularPath);
            this.textureFileLoader = textureFileLoader;
        }

        public Material Create(MTL mtl, Color tint, UnitySS2AssetRepository unitySS2AssetRepo)
        {
            var mainTexPath = PathUtility.FilePathWithoutExtension(mtl.GetMainTexturePath()).ToLower().Replace('\\', '/');
            var tex = unitySS2AssetRepo.LoadAnyTextureAsset(mainTexPath);

            Material mat;
            if (mtl.uMaterial.HasValue)
            {
                mat = CreateSetMaterial(tex, mtl, unitySS2AssetRepo);
            }
            else
            {
                var readableTex = textureFileLoader.Load(mainTexPath).First().Item1;
                bool shiny = mtl.renderPasses.Any(rp => rp.uvSource == DarkRenderPass.UVSource.Environment);
                mat = CreateMaterialFromSingleTexture(tex, readableTex, shiny, tint, false);
            }
            return mat;
        }

        public bool Match(MTL mtl)
        {
            return true;
        }

        public Material CreateMaterialFromSingleTexture(Texture tex, Texture readableTex, bool makeShiny, Color tint, bool emissive)
        {
            bool monoChrome = false;
            Color32[] colors;
            TextureAlpha alphaMode = TextureAlpha.None;
            if (tint.a != 1)
                alphaMode = TextureAlpha.Soft;

            if (readableTex is Texture2D)
            {
                colors = ((Texture2D)readableTex).GetPixels32();
            }
            else if (readableTex is Texture2DArray)
            {
                colors = ((Texture2DArray)readableTex).GetPixels32(0);
            }
            else
            {
                throw new System.ArgumentException($"Unknown texture type {tex.GetType()}");
            }
            if (alphaMode == TextureAlpha.None)
            {
                alphaMode = AnalyseTextureAlpha(colors, out monoChrome);
            }
            else
            {
                monoChrome = IsMonochrome(colors);
            }
            var monochromeColor = colors[0];

            Material mat = null;
            if (tex is Texture2DArray)
            {
                mat = new Material(animTexArray);
                if (emissive)
                {
                    mat.SetFloat("_EmissionIntensity", 1);
                }
                else
                {
                    mat.SetFloat("_EmissionIntensity", 0);
                }
                mat.SetTexture("_MainTexArr", tex);
                mat.SetInt("_FrameCount", ((Texture2DArray)tex).depth);
            }
            else
            {
                switch (alphaMode)
                {
                    case TextureAlpha.None:
                        if (emissive)
                            mat = new Material(emissiveLitMat);
                        else
                            mat = new Material(litMat);
                        break;
                    case TextureAlpha.Cutout:
                        mat = new Material(cutOutLitMat);
                        break;
                    case TextureAlpha.Soft:
                        mat = new Material(transparentLitMat);
                        break;
                }
                if (monoChrome)
                {
                    mat.color = tint * monochromeColor;
                }
                else
                {
                    mat.mainTexture = tex;
                    mat.color = tint;
                }
                if (emissive)
                {
                    mat.EnableKeyword("_EMISSION");
                    if (monoChrome)
                    {
                        mat.SetColor("_EmissionColor", tint * monochromeColor);
                    }
                    else
                    {
                        mat.SetTexture("_EmissionMap", tex);
                        mat.SetColor("_EmissionColor", tint);
                    }
                }
            }

            if (makeShiny)
            {
                mat.SetFloat("_Glossiness", 1);
            }
            return mat;
        }

        public Material CreateSetMaterial(Texture tex, MTL mtl, UnitySS2AssetRepository unitySS2AssetRepo)
        {
            Material mat;
            switch (mtl.uMaterial)
            {
                case MTL.UMaterial.Standard_Specular:
                    /*mat = new Material(litSpecularMat);

                    foreach (var rp in mtl.renderPasses)
                    {
                        if (rp.uType == DarkRenderPass.UType.Specular)
                        {
                            var specTex = LoadTexture(rp.texturePaths[0], unitySS2AssetRepo);
                            mat.SetTexture("_MaskMap", specTex);
                        }
                    }
                    break;*/
                case MTL.UMaterial.Standard:
                    mat = new Material(litMat);
                    break;
                default:
                    throw new System.ArgumentException($"Cant handle uMat = {mtl.uMaterial}");
            }

            mat.mainTexture = tex;
            return mat;
        }

        private TextureAlpha AnalyseTextureAlpha(Color32[] colors, out bool monoChrome)
        {
            int softAlpha = 0;
            int hardAlpha = 0;
            monoChrome = true;
            for (int i = 0; i < colors.Length; i++)
            {
                var c = colors[i];
                softAlpha += 1 * ((c.a > 0 && c.a < byte.MaxValue) ? 1 : 0);
                hardAlpha += 1 * (c.a == 0 ? 1 : 0);
                monoChrome = monoChrome && c.r == colors[0].r && c.g == colors[0].g && c.r == colors[0].r && c.a == colors[0].a;
            }
            if (softAlpha > 5)
                return TextureAlpha.Soft;
            if (hardAlpha > 5)
                return TextureAlpha.Cutout;
            return TextureAlpha.None;
        }

        private bool IsMonochrome(Color32[] colors)
        {
            bool monoChrome = true;
            for (int i = 0; i < colors.Length; i++)
            {
                var c = colors[i];
                monoChrome = monoChrome && c.r == colors[0].r && c.g == colors[0].g && c.r == colors[0].r && c.a == colors[0].a;
            }
            return monoChrome;
        }

        private Texture LoadTexture(string name, UnitySS2AssetRepository unitySS2AssetRepo)
        {
            var mainTexPath = PathUtility.FilePathWithoutExtension(name).ToLower().Replace('\\', '/');
            return unitySS2AssetRepo.LoadTextureAsset(mainTexPath);
        }
    }
}