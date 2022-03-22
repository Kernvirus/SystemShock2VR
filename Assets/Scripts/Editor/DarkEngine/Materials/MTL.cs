using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.Materials
{
    public class MTL
    {
        public enum AniMode
        {
            Wrap = 1,
            Reverse = 2,
            PingPong = 4,
        }

        public enum UMaterial
        {
            Standard_Specular,
            Standard
        }

        public int aniRate = 250;
        public int aniFrames = 0;
        public AniMode aniMode = AniMode.Wrap;
        public bool forceAniSettings = false;

        public Vector2Int? terrainScale;
        public Vector2Int tileFactor;

        public bool forceOpaque = false;
        public bool forceFullAlpha = false;
        public bool forceAlphaKey = false;

        public bool noMipMap = false;
        public bool uvClamp = false;
        public bool edgePadding = true;

        public bool renderMaterialOnly = false;

        public UMaterial? uMaterial;

        public List<DarkRenderPass> renderPasses = new List<DarkRenderPass>();
        string mainTexturePath;

        public MTL(string mainTexturePath)
        {
            this.mainTexturePath = mainTexturePath;
        }

        internal void AddRenderpass(DarkRenderPass pass)
        {
            renderPasses.Add(pass);
        }

        public IEnumerable<string> ReferencedTexturePaths()
        {
            if (!renderMaterialOnly || renderPasses.Count == 0)
                yield return mainTexturePath;


            foreach (var rp in renderPasses)
            {
                foreach (var tp in rp.texturePaths)
                    yield return tp;
            }
        }

        public string GetMainTexturePath()
        {
            if (!renderMaterialOnly || renderPasses.Count == 0)
                return mainTexturePath;

            foreach (var rp in renderPasses)
            {
                if (rp.texturePaths.Count > 0 && rp.uvSource == DarkRenderPass.UVSource.Texture)
                    return rp.texturePaths[0];
            }
            return null;
        }
    }

    public class DarkRenderPass
    {
        public enum AniMode
        {
            Normal = 1,
            Reverse = 2,
            Rand = 4,
            PingPong = 8,
        }
        public enum BlendMode
        {
            Zero,
            One, SrcAlpha,
            InvSrcAlpha, SrcColor,
            InvSrcColor, DstAlpha,
            InvDstAlpha, DstColor,
            InvDstColor
        }
        public enum UVSource
        {
            Texture,
            Lightmap,
            Environment,
            Projection
        }

        public enum UType {
            Specular,
            Normal,
            Metal,
        }

        public string textureParameter;
        public int useLocationEnvTex;
        public int aniRate = 250;
        public AniMode aniMode = AniMode.Normal;

        public BlendMode srcBlend = BlendMode.SrcAlpha;
        public BlendMode dstBlend = BlendMode.InvSrcAlpha;

        public UVSource uvSource = UVSource.Texture;

        public UType? uType = null;

        public bool shaded = true;
        public List<string> texturePaths;

        public GenFunction alphaFunction;
        public GenFunction rgbFunction;
    }

    public class GenFunction
    {

    }

    public class WaveFunction : GenFunction
    {
        public enum Output
        {
            RGB,
            Alpha
        }

        public enum Function
        {
            Sine,
            Abs_Sine,
            Triangle,
            Square,
            Sawtooth,
            Inv_Sawtooth,
            Turb
        }

        public static WaveFunction Parse(string[] lineParts)
        {
            Function function = (Function)Enum.Parse(typeof(Function), lineParts[3], true);
            Output output = (Output)Enum.Parse(typeof(Output), lineParts[0], true);

            int? steps = null;
            if (lineParts.Length > 8)
                steps = int.Parse(lineParts[8]);

            return new WaveFunction()
            {
                output = output,
                function = function,
                bias = float.Parse(lineParts[4], CultureInfo.InvariantCulture),
                amplitude = float.Parse(lineParts[5], CultureInfo.InvariantCulture),
                phase = float.Parse(lineParts[6], CultureInfo.InvariantCulture),
                frequency = int.Parse(lineParts[7], CultureInfo.InvariantCulture),
                steps = steps,
            };
        }

        public float bias;
        public float amplitude;
        public float phase;
        public float frequency;
        public int? steps;

        public Output output;
        public Function function;
    }

    public class IncidenceFunction : GenFunction
    {
        public static IncidenceFunction Parse(string[] lineParts)
        {
            Color color = new Color(float.Parse(lineParts[3]), float.Parse(lineParts[4]), float.Parse(lineParts[5]), 1);
            return new IncidenceFunction()
            {
                color = color,
                maxDist = float.Parse(lineParts[6]),
                lutTexture = lineParts[7]
            };
        }

        public Color color;
        public float maxDist;
        public string lutTexture;
    }
}
