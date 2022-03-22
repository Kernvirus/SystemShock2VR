using Assets.Scripts.Editor.DarkEngine.Animation;
using Assets.Scripts.Editor.DarkEngine.DarkObjects;
using Assets.Scripts.Editor.DarkEngine.DarkObjects.DarkLinks;
using Assets.Scripts.Editor.DarkEngine.DarkObjects.DarkProps;
using Assets.Scripts.Editor.DarkEngine.Files;
using Assets.Scripts.Editor.DarkEngine.LevelFile;
using Assets.Scripts.Editor.DarkEngine.Materials;
using Assets.Scripts.Editor.DarkEngine.Models;
using Assets.Scripts.Editor.DarkEngine.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.Importer
{
    class ObjectAssetImporter
    {
        UnitySS2AssetRepository unitySS2AssetRepo;
        LevelFileLoader levelFileLoader;
        BinFileLoader modelLoader;
        BinFileRepository binFileRepo;
        ITextureFileLoader textureFileLoader;
        MotionLoader motionLoader;
        UnityMaterialCreator materialCreator;

        int importedObjectCount;
        List<string> importedTexturePaths;
        Dictionary<DarkObject, IBinMesh> binMeshMapping;

        public ObjectAssetImporter(LevelFileRepository levelFileRepo, BinFileRepository binFileRepo, CalFileRepository calFileRepo, TextureFileRepository textureFileRepo, MotionFileRepository motionFileRepo, MTLRepository mtlRepo, UnitySS2AssetRepository unitySS2AssetRepo)
        {
            this.binFileRepo = binFileRepo;
            this.modelLoader = new BinFileLoader(binFileRepo, calFileRepo);
            this.levelFileLoader = new LevelFileLoader(levelFileRepo);
            this.textureFileLoader = new TextureCache(new TextureFileLoader(textureFileRepo, mtlRepo));
            this.motionLoader = new MotionLoader(motionFileRepo);
            this.unitySS2AssetRepo = unitySS2AssetRepo;
            this.materialCreator = new UnityMaterialCreator(textureFileRepo, mtlRepo, unitySS2AssetRepo, textureFileLoader);
        }

        public void Import(IEnumerable<string> selectedLevels)
        {
            importedObjectCount = 0;

            foreach (var levelName in selectedLevels)
            {
                var levelFiles = levelFileLoader.Load(levelName);
                ImportLevelObjects(levelFiles);
            }
        }

        private void ImportLevelObjects(IList<LevelFileGroup> levelFiles)
        {
            // read required properties and links
            var objectCollection = new DarkObjectCollection();
            foreach (var levelFile in levelFiles)
            {
                objectCollection.LoadPropertyChunk<ModelNameProp>(levelFile);
                objectCollection.LoadPropertyChunk<SymNameProp>(levelFile);
                objectCollection.LoadPropertyChunk<CreatureProp>(levelFile);
                objectCollection.LoadPropertyChunk<MotActorTagsProp>(levelFile);
                objectCollection.LoadPropertyChunk<RenderAlphaProp>(levelFile);
                objectCollection.LoadLinkAndDataChunk<MetaPropLink>(levelFile);
            }

            importedTexturePaths = new List<string>(100);
            binMeshMapping = new Dictionary<DarkObject, IBinMesh>(100);

            var objs = objectCollection.Where(d => d.IsInstance && d.HasProp<ModelNameProp>()).
                    Select(d => new Tuple<DarkObject, string>(d, d.GetProp<ModelNameProp>().Value))
                    .GroupBy(t => t.Item2)
                    .Select(group => group.First().Item1);

            AssetDatabase.StartAssetEditing();
            try
            {
                foreach (var darkObject in objs)
                {
                    ImportObjectPhase1(darkObject);
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }


            AssetDatabase.StartAssetEditing();
            try
            {
                MarkObjectTextures();
                foreach (var darkObject in objs)
                {
                    ImportObjectPhase2(darkObject);
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.StartAssetEditing();
            try
            {
                foreach (var darkObject in objs)
                {
                    ImportObjectPhase3(darkObject);
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }

        private void ImportObjectPhase1(DarkObject darkObject)
        {
            // mesh
            // texture
            // -animations
            IBinMesh binMesh = GetBinMesh(darkObject);
            if (binMesh == null)
                return;

            binMeshMapping.Add(darkObject, binMesh);
            ImportObjectTextures(binMesh);
            foreach (var mesh in binMesh.Meshs())
            {
                var up = new UnwrapParam();
                UnwrapParam.SetDefaults(out up);
                up.packMargin = 0.03f;
                Unwrapping.GenerateSecondaryUVSet(mesh, up);
                unitySS2AssetRepo.CreateMeshAsset(mesh, binMesh.IsObj);
            }
            //ImportObjectAnimations(darkObject, binMesh.IsObj);
        }

        private void ImportObjectPhase2(DarkObject darkObject)
        {
            IBinMesh binMesh;
            if (!binMeshMapping.TryGetValue(darkObject, out binMesh))
                return;

            CreateObjectMaterials(binMesh, darkObject);
        }

        private void ImportObjectPhase3(DarkObject binKey)
        {
            IBinMesh binMesh;
            if (!binMeshMapping.TryGetValue(binKey, out binMesh))
                return;

            GameObject gameObj = binMesh.Instantiate(unitySS2AssetRepo, materialCreator);
            unitySS2AssetRepo.CreateObjPrefabAsset(gameObj, binMesh.IsObj);
            GameObject.DestroyImmediate(gameObj);
        }

        private void ImportObjectTextures(IBinMesh binMesh)
        {
            string pathPrefix = binMesh.IsObj ? "obj/txt16/" : "mesh/txt16/";
            foreach (var material in binMesh.Materials())
            {
                string texName = pathPrefix + PathUtility.FilePathWithoutExtension(material.Name).ToLower().Replace('\\', '/');
                if (!unitySS2AssetRepo.WouldWriteTexture(texName))
                    continue;

                foreach (var textureWithPath in textureFileLoader.Load(texName))
                {
                    unitySS2AssetRepo.CreateTextureAsset(textureWithPath.Item1, textureWithPath.Item2);
                    if (textureWithPath.Item1 is Texture2D)
                        importedTexturePaths.Add(textureWithPath.Item2);
                }
            }
        }

        private IBinMesh GetBinMesh(DarkObject darkObject)
        {
            string modelName = darkObject.GetProp<ModelNameProp>().Value.ToLower();

            if (!binFileRepo.DoesNameExist(modelName))
            {
                if (modelName.StartsWith("fx_"))
                    return null;

                if (modelName.StartsWith("spark_"))
                    return null;

                throw new KeyNotFoundException($"{modelName} not found");
            }

            IBinMesh binMesh;
            if (modelLoader.IsObjectMesh(modelName))
            {
                binMesh = modelLoader.LoadObjectMesh(modelName);
            }
            else
            {
                var creType = darkObject.GetProp<CreatureProp>().Value;
                binMesh = modelLoader.LoadAIMesh(modelName, creType);
            }
            return binMesh;
        }

        private void MarkObjectTextures()
        {
            foreach (var importedTexture in importedTexturePaths)
            {
                unitySS2AssetRepo.MarkTextureTransparent(importedTexture);
            }
        }

        private void ImportObjectAnimations(DarkObject darkObject, bool isObj)
        {
            if (!darkObject.HasProp<MotActorTagsProp>() || !darkObject.HasProp<CreatureProp>())
                return;

            foreach (var clip in motionLoader.LoadAllClipsForObject(darkObject))
            {
                unitySS2AssetRepo.CreateAnimationAsset(clip, isObj);
            }
        }

        private void CreateObjectMaterials(IBinMesh binMesh, DarkObject darkObject)
        {
            float renderAlpha = darkObject.GetProp<RenderAlphaProp>()?.Value ?? 1;
            string pathPrefix = binMesh.IsObj ? "obj/txt16/" : "mesh/txt16/";
            foreach (var material in binMesh.Materials())
            {
                string texName = pathPrefix + PathUtility.FilePathWithoutExtension(material.Name).ToLower().Replace('\\', '/');

                Color matColor = material.Color;
                matColor.a = renderAlpha;

                bool emissive = material.Illum == 1;

                var mat = materialCreator.CreateMaterial(texName, matColor, emissive);
                if (mat.Item2 == null)
                    continue;
                unitySS2AssetRepo.CreateMaterialAsset(mat.Item1, mat.Item2);
            }
        }
    }
}
