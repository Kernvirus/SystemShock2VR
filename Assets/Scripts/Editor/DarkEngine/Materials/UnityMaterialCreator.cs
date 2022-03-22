using UnityEngine;
using UnityEditor;
using System.Collections;
using Assets.Scripts.Editor.DarkEngine.Textures;
using Assets.Scripts.Editor.DarkEngine.Files;
using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;
using Assets.Scripts.Editor.DarkEngine.Models;

namespace Assets.Scripts.Editor.DarkEngine.Materials
{
    class UnityMaterialCreator
    {
        const string missingTexturePath = "Assets/Materials/ss2Template_missingTexture.mat";

        TextureFileRepository textureFileRepo;
        MTLRepository mtlRepo;
        UnitySS2AssetRepository unitySS2AssetRepo;
        ITextureFileLoader textureFileLoader;

        Material missingTextureMat = AssetDatabase.LoadAssetAtPath<Material>(missingTexturePath);

        IMTLConverter[] materialCreators;
        SingleTextureCreator singleTextureCreator;

        public UnityMaterialCreator(TextureFileRepository textureFileRepo, MTLRepository mtlRepo, UnitySS2AssetRepository unitySS2AssetRepo, ITextureFileLoader textureFileLoader)
        {
            this.textureFileRepo = textureFileRepo;
            this.mtlRepo = mtlRepo;
            this.unitySS2AssetRepo = unitySS2AssetRepo;
            this.textureFileLoader = textureFileLoader;

            this.singleTextureCreator = new SingleTextureCreator(textureFileLoader);
            this.materialCreators = new IMTLConverter[]
            {
                new AnimEmissiveBlendCreator(),
                new AnimTexArrayCreator(),
                new AnimSinAbsSlickDarkUnlit(),
                singleTextureCreator
            };
        }

        public string GetMaterialPath(string name)
        {
            if (!textureFileRepo.DoesNameExist(name))
                return missingTexturePath;

            var texturePath = textureFileRepo.GetPath(name);
            return PathUtility.FilePathWithoutExtension(texturePath.relativePath);
        }

        public Tuple<Material, string> CreateMaterial(string name, Color tint, bool emissive)
        {
            if (!textureFileRepo.DoesNameExist(name))
                return new Tuple<Material, string>(missingTextureMat, null);

            Material mat;
            var texturePath = textureFileRepo.GetPath(name);
            try
            {
                if (textureFileRepo.IsMTL(texturePath.relativePath))
                {
                    var mtl = mtlRepo.Load(texturePath);
                    mat = null;
                    foreach (var creator in materialCreators)
                    {
                        if (creator.Match(mtl))
                        {
                            mat = creator.Create(mtl, tint, unitySS2AssetRepo);
                            break;
                        }
                    }
                }
                else
                {
                    var tex = unitySS2AssetRepo.LoadAnyTextureAsset(PathUtility.FilePathWithoutExtension(texturePath.relativePath));
                    var readableTex = textureFileLoader.Load(name).First().Item1;
                    mat = singleTextureCreator.CreateMaterialFromSingleTexture(tex, readableTex, false, tint, emissive);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to create material for " + name);
                return new Tuple<Material, string>(missingTextureMat, null);
            }

            mat.name = Path.GetFileNameWithoutExtension(texturePath.relativePath);
            return new Tuple<Material, string>(mat, PathUtility.FilePathWithoutExtension(texturePath.relativePath));
        }
    }
}
