using Assets.Scripts.Editor.DarkEngine.Files;
using Assets.Scripts.Editor.DarkEngine.Materials;
using Pfim;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.Textures
{
    class TextureFileLoader : ITextureFileLoader
    {


        TextureFileRepository textureFileRepo;
        MTLRepository mtlRepo;

        public TextureFileLoader(TextureFileRepository textureFileRepo, MTLRepository mtlRepo)
        {
            this.textureFileRepo = textureFileRepo;
            this.mtlRepo = mtlRepo;
        }

        public IEnumerable<Tuple<Texture, string>> Load(string name)
        {
            if (!textureFileRepo.DoesNameExist(name))
            {
                Debug.LogError($"Couldn't find texture {name}");
                return Enumerable.Empty<Tuple<Texture, string>>();
            }

            var texturePath = textureFileRepo.GetPath(name);
            return Load(texturePath, name);
        }

        public IEnumerable<Tuple<Texture, string>> Load(SS2FileEntry texturePath, string texKey)
        {
            if (textureFileRepo.IsMTL(texturePath.relativePath))
            {
                // load mtl
                var mtl = mtlRepo.Load(texturePath);
                IEnumerable<Tuple<Texture, string>> mainTexTupleEnum;
                if (!mtl.renderMaterialOnly)
                {
                    var mainTex2dTuple = LoadTextureOnly(mtl.GetMainTexturePath());
                    var mainTexTuple = new Tuple<Texture, string>(mainTex2dTuple.Item1, mainTex2dTuple.Item2);

                    mainTexTupleEnum = new[] { mainTexTuple };
                }
                else
                {
                    mainTexTupleEnum = Enumerable.Empty<Tuple<Texture, string>>();
                }

                if (mtl.renderPasses.Any(rp => rp.texturePaths.Any(tp => tp.Contains("gfx_"))))
                {
                    // load as specular
                    return LoadSpecularTexture(mtl);
                }

                return mainTexTupleEnum.Concat(mtl.renderPasses
                    .Where(rp => rp.texturePaths.Count > 0)
                    .Select(rp =>
                    {
                        if (rp.texturePaths.Count == 1)
                        {
                            var texTuple = LoadTextureOnly(rp.texturePaths[0]);
                            if (texTuple == null)
                                return null;
                            return new Tuple<Texture, string>(texTuple.Item1, texTuple.Item2);
                        }
                        else
                        {
                            var texTuple = LoadTextureArray(rp.texturePaths);
                            if (texTuple == null)
                                return null;
                            return new Tuple<Texture, string>(texTuple.Item1, texTuple.Item2);
                        }
                    })
                    .Where(t => t != null));
            }
            else
            {
                if (texKey.EndsWith("_"))
                {
                    var paths = GatherNumericTexturePaths(texKey).ToArray();
                    if (paths.Length > 1)
                    {
                        var texArrTuple = LoadTextureArray(paths);
                        return new[] { new Tuple<Texture, string>(texArrTuple.Item1, texArrTuple.Item2) };
                    }
                }

                // normal texture file
                var texTuple = LoadTextureOnly(texturePath);
                return new Tuple<Texture, string>[] {
                    new Tuple<Texture, string>(texTuple.Item1, texTuple.Item2)
                };
            }
        }

        public Vector2Int GetTextureDimension(string name)
        {
            if (!textureFileRepo.DoesNameExist(name))
                return Vector2Int.one;

            var texturePath = textureFileRepo.GetPath(name);
            return GetTextureDimension(texturePath, name);
        }

        public Vector2Int GetTextureDimension(SS2FileEntry entry, string texKey)
        {
            if (textureFileRepo.IsMTL(entry.relativePath))
            {
                var mtl = mtlRepo.Load(entry);
                if (mtl.terrainScale.HasValue)
                    return mtl.terrainScale.Value;
            }

            var texTuple = Load(entry, texKey).First();
            if (texTuple == null)
            {
                return Vector2Int.one;
            }
            var tex = texTuple.Item1;
            return new Vector2Int(tex.width, tex.height);
        }

        private Tuple<Texture2D, string> LoadTextureOnly(string name)
        {
            var pathName = PathUtility.FilePathWithoutExtension(name).ToLower().Replace('\\', '/');

            if (!textureFileRepo.DoesTextureNameExist(pathName))
            {
                Debug.LogError($"Couldn't find texture {pathName}");
                return null;
            }
            var texturePath = textureFileRepo.GetTextureOnlyPath(pathName);
            return LoadTextureOnly(texturePath);
        }

        private Tuple<Texture2D, string> LoadTextureOnly(SS2FileEntry texturePath)
        {
            // normal texture file
            string textureExtension = Path.GetExtension(texturePath.absolutePath).ToLower();

            Texture2D texture;
            if (textureExtension == ".pcx")
            {
                texture = PcxReader.Load(texturePath.absolutePath).Item1;
            }
            else if (textureExtension == ".gif")
            {
                byte[] texData = File.ReadAllBytes(texturePath.absolutePath);
                var texList = UniGif.GetTextureList(texData, FilterMode.Point);
                texture = texList.First().m_texture2d;
            }
            else if (textureExtension == ".dds" || textureExtension == ".tga")
            {
                var image = Pfim.Pfim.FromFile(texturePath.absolutePath);
                texture = TextureUtil.ConvertPfimImageToTexture(image);
            }
            else
            {
                byte[] texData = File.ReadAllBytes(texturePath.absolutePath);
                texture = new Texture2D(1, 1);
                texture.LoadImage(texData);
            }
            texture.name = Path.GetFileNameWithoutExtension(texturePath.relativePath);
            return new Tuple<Texture2D, string>(texture, PathUtility.FilePathWithoutExtension(texturePath.relativePath));
        }

        private Tuple<Texture2DArray, string> LoadTextureArray(IList<string> texNames)
        {
            var textureTuples = texNames.Select(texName =>
            {
                return LoadTextureOnly(texName);
            }).ToArray();

            var textures = textureTuples.Select(t => t.Item1).ToArray();

            Texture2DArray tArr = new Texture2DArray(textures[0].width, textures[0].height, textures.Length, TextureFormat.RGB24, true, false);

            if (textures.Any(t => t.width != tArr.width || t.height != tArr.height))
            {
                throw new ArgumentException($"Texture {texNames[0]} have differing sizes");
            }

            for (int i = 0; i < textures.Length; i++)
            {
                tArr.SetPixels32(textures[i].GetPixels32(), i);
            }
            tArr.Apply();

            return new Tuple<Texture2DArray, string>(tArr, textureTuples[0].Item2);
        }

        private IEnumerable<Tuple<Texture, string>> LoadSpecularTexture(MTL mtl)
        {
            Debug.Assert(mtl.renderPasses.Count == 2);

            var albedoTexPath = mtl.renderPasses.Where(rp => !rp.texturePaths[0].ToLower().Contains("gfx_")).First();

            Debug.Assert(albedoTexPath.texturePaths.Count == 1);
            var texTuple = LoadTextureOnly(albedoTexPath.texturePaths[0]);

            // create specular texture
            var specularTex = TextureUtil.TextureAlphaToSpecularMap(texTuple.Item1);
            var specularTuple = new Tuple<Texture, string>(specularTex, texTuple.Item2 + "_S");

            return new[] {
                new Tuple<Texture, string>(texTuple.Item1, texTuple.Item2),
                specularTuple
            };
        }

        private IEnumerable<string> GatherNumericTexturePaths(string basePath)
        {
            int i = 0;
            string path = basePath;
            while (textureFileRepo.DoesNameExist(path))
            {
                yield return path;
                i++;
                path = basePath + i.ToString();
            }
        }
    }
}
