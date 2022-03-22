using Assets.Scripts.Editor.DarkEngine.Animation;
using Assets.Scripts.Editor.DarkEngine.DarkObjects;
using Assets.Scripts.Editor.DarkEngine.DarkObjects.DarkLinks;
using Assets.Scripts.Editor.DarkEngine.DarkObjects.DarkProps;
using Assets.Scripts.Editor.DarkEngine.Files;
using Assets.Scripts.Editor.DarkEngine.LevelFile;
using Assets.Scripts.Editor.DarkEngine.Materials;
using Assets.Scripts.Editor.DarkEngine.Models;
using Assets.Scripts.Editor.DarkEngine.Textures;
using Assets.Scripts.Editor.DarkEngine.World;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.Importer
{
    class WorldAssetImporter
    {
        UnitySS2AssetRepository unitySS2AssetRepo;
        LevelFileLoader levelFileLoader;
        ITextureFileLoader textureFileLoader;
        UnityMaterialCreator materialCreator;

        public WorldAssetImporter(LevelFileRepository levelFileRepo, TextureFileRepository textureFileRepo, MTLRepository mtlRepo, UnitySS2AssetRepository unitySS2AssetRepo)
        {
            this.levelFileLoader = new LevelFileLoader(levelFileRepo);
            this.textureFileLoader = new TextureCache(new TextureFileLoader(textureFileRepo, mtlRepo));
            this.unitySS2AssetRepo = unitySS2AssetRepo;
            this.materialCreator = new UnityMaterialCreator(textureFileRepo, mtlRepo, unitySS2AssetRepo, textureFileLoader);
        }

        public void Import(IEnumerable<string> selectedLevels)
        {
            foreach (var levelName in selectedLevels)
            {
                var levelFiles = levelFileLoader.Load(levelName).Where(l => l.IsMisFile);
                string strippedLevelName = Path.GetFileNameWithoutExtension(levelName);
                foreach (var level in levelFiles)
                    ImportLevel(level, strippedLevelName);
            }
        }

        private void ImportLevel(LevelFileGroup level, string levelName)
        {
            WorldRep wr = new WorldRep(level);
            TextureList tl = new TextureList(level);

            AssetDatabase.StartAssetEditing();
            try
            {
                foreach (var cell in wr.cells)
                    ImportCell(cell, tl, levelName);

                // import textures
                for (int i = 0; i < tl.TextureCount; i++)
                {
                    string texName = tl.GetTextureName(i).ToLower().Replace('\\', '/');
                    foreach (var tex in textureFileLoader.Load(texName))
                    {
                        unitySS2AssetRepo.CreateTextureAsset(tex.Item1, tex.Item2);
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
            AssetDatabase.StartAssetEditing();
            try
            {
                for (int i = 0; i < tl.TextureCount; i++)
                {
                    string texName = tl.GetTextureName(i).ToLower().Replace('\\', '/');

                    Color matColor = Color.white;

                    var mat = materialCreator.CreateMaterial(texName, matColor, false);
                    if (mat.Item2 == null)
                        continue;
                    unitySS2AssetRepo.CreateMaterialAsset(mat.Item1, mat.Item2);
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.StartAssetEditing();
            try
            {
                // create prefab
                GameObject rootObj = new GameObject(levelName);
                GameObject cellRoot = new GameObject("Cells");
                cellRoot.transform.parent = rootObj.transform;
                GameObjectUtility.SetStaticEditorFlags(rootObj, ImporterSettings.staticFlags);
                GameObjectUtility.SetStaticEditorFlags(cellRoot, ImporterSettings.staticFlags);
                foreach (var cell in wr.cells)
                {
                    var cellObj = CreateCellGO(cell, tl, levelName);
                    cellObj.transform.parent = cellRoot.transform;
                }
                
                /* lightRoot = new GameObject("Lights");
                foreach (var lightEntry in wr.lightEntries)
                {
                    var g = CreateLightGO(lightEntry);
                    g.transform.SetParent(lightRoot.transform, true);
                }

                GameObjectUtility.SetStaticEditorFlags(lightRoot, ImporterSettings.staticFlags);
                lightRoot.transform.parent = rootObj.transform;
                */
                unitySS2AssetRepo.CreateLevelPrefabAsset(levelName, rootObj);
                GameObject.DestroyImmediate(rootObj);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }

        private void ImportCell(Cell cell, TextureList tl, string levelName)
        {
            Mesh mesh;
            try
            {
                mesh = cell.CreateMesh(tl, textureFileLoader);
            }
            catch (Exception e)
            {
                Debug.LogError($"Creation of mesh for cell {cell.Name} failed, because {e}");
                return;
            }
            if (mesh == null)
                return;

            unitySS2AssetRepo.CreateLevelMeshAsset(levelName, mesh);
            var physmesh = cell.CreatePhysicsMesh(mesh);
            if (physmesh == mesh || physmesh == null)
                return;

            unitySS2AssetRepo.CreateLevelMeshAsset(levelName, physmesh);
        }

        private GameObject CreateLightGO(LightTableEntry lightEntry)
        {
            GameObject g = new GameObject("", typeof(Light));

            var light = g.GetComponent<Light>();
            light.gameObject.name = "Light_" + (lightEntry.dynamic ? 'D' : 'S');
            light.transform.position = lightEntry.pos;
            var bn = lightEntry.brightness.normalized;
            light.color = new Color(bn.x, bn.y, bn.z);
            light.shadows = LightShadows.Soft;
            light.lightmapBakeType = LightmapBakeType.Baked;

            if (lightEntry.cone_inner < 0)
            {
                light.type = LightType.Point;
            }
            else
            {
                light.type = LightType.Spot;
                light.innerSpotAngle = Mathf.Acos(lightEntry.cone_inner) * Mathf.Rad2Deg;
                light.spotAngle = Mathf.Acos(lightEntry.cone_outer) * Mathf.Rad2Deg;
                light.transform.rotation = Quaternion.LookRotation(lightEntry.rot);
            }

            if (lightEntry.radius > 0)
            {
                light.range = lightEntry.radius;
            }
            else
            {
                light.range = lightEntry.brightness.magnitude * ImporterSettings.globalScale * 10;
            }

            GameObjectUtility.SetStaticEditorFlags(g, ImporterSettings.staticFlags);
            return g;
        }

        private GameObject CreateCellGO(Cell cell, TextureList tl, string levelName)
        {
            var mesh = unitySS2AssetRepo.LoadLevelMeshAsset(levelName, cell.Name);

            GameObject cellObj = new GameObject(cell.Name, typeof(MeshRenderer), typeof(MeshFilter));
            cellObj.GetComponent<MeshFilter>().sharedMesh = mesh;

            var materials = cell.MaterialIndecies().Select(iMat =>
            {
                string texName = tl.GetTextureName(iMat).ToLower().Replace('\\', '/');
                string matName = materialCreator.GetMaterialPath(texName);
                return unitySS2AssetRepo.LoadMaterialAsset(matName);
            }).ToArray();

            var mr = cellObj.GetComponent<MeshRenderer>();
            mr.sharedMaterials = materials;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
            
            if (cell.RequiresCollider())
            {
                var physMesh = unitySS2AssetRepo.LoadLevelPhysMeshAsset(levelName, cell.Name);
                cellObj.AddComponent<MeshCollider>().sharedMesh = physMesh;
            }
            GameObjectUtility.SetStaticEditorFlags(cellObj, ImporterSettings.staticFlags);

            return cellObj;
        }
    }
}
