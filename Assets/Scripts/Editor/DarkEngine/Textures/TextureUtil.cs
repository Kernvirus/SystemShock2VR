using Pfim;
using System;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.Textures
{
    static class TextureUtil
    {
        public static Texture2D ConvertPfimImageToTexture(IImage image)
        {
            TextureFormat texFormat;
            switch (image.Format)
            {
                case ImageFormat.Rgba32:
                    texFormat = TextureFormat.BGRA32;
                    break;
                case ImageFormat.Rgb24:
                    texFormat = TextureFormat.RGB24;
                    break;
                default:
                    throw new ArgumentException($"Can't handle image format {image.Format}");
            }
            var tex = new Texture2D(image.Width, image.Height, texFormat, false);
            tex.LoadRawTextureData(image.Data);
            return FlipTexture(tex);
        }

        private static Texture2D FlipTexture(Texture2D original)
        {
            int width = original.width;
            int height = original.height;
            Texture2D snap = new Texture2D(width, height);
            Color[] pixels = original.GetPixels();
            Color[] pixelsFlipped = new Color[pixels.Length];

            for (int i = 0; i < height; i++)
            {
                Array.Copy(pixels, i * width, pixelsFlipped, (height - i - 1) * width, width);
            }

            snap.SetPixels(pixelsFlipped);
            snap.Apply();
            return snap;
        }

        public static Texture2D TextureAlphaToSpecularMap(Texture2D original)
        {
            Color32[] colors = original.GetPixels32();
            for (int i = 0; i < colors.Length; i++)
            {
                byte a = (byte)(byte.MaxValue - colors[i].a);
                colors[i] = new Color32(a, a, a, byte.MaxValue);
            }
            var specularTex = new Texture2D(original.width, original.height);
            specularTex.SetPixels32(colors);
            specularTex.Apply();

            return specularTex;
        }
    }
}
