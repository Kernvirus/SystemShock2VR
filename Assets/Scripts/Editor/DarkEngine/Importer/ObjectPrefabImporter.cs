using Assets.Scripts.Editor.DarkEngine.DarkObjects;
using Assets.Scripts.Editor.DarkEngine.DarkObjects.DarkLinks;
using Assets.Scripts.Editor.DarkEngine.DarkObjects.DarkProps;
using Assets.Scripts.Editor.DarkEngine.Files;
using Assets.Scripts.Editor.DarkEngine.LevelFile;
using Assets.Scripts.Editor.DarkEngine.SmartObjectPrefabCreator;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.Importer
{
    class ObjectPrefabImporter
    {
        UnitySS2AssetRepository unitySS2AssetRepo;
        BinFileRepository binFileRepo;
        LevelFileLoader levelFileLoader;

        public ObjectPrefabImporter(LevelFileRepository levelFileRepo, BinFileRepository binFileRepo, UnitySS2AssetRepository unitySS2AssetRepo)
        {
            this.levelFileLoader = new LevelFileLoader(levelFileRepo);
            this.binFileRepo = binFileRepo;
            this.unitySS2AssetRepo = unitySS2AssetRepo;
        }

        public void Load(IEnumerable<string> selectedLevels)
        {
            foreach (var levelName in selectedLevels)
            {
                var levelFiles = levelFileLoader.Load(levelName);
                LoadLevel(levelFiles);
            }
        }

        private void LoadLevel(IList<LevelFileGroup> levelFiles)
        {
            var objectCollection = new DarkObjectCollection();
            foreach (var levelFile in levelFiles)
            {
                LoadAttributs(objectCollection, levelFile);
            }

            var objs = objectCollection.Where(d => d.IsInstance && d.Parent != null).Select(d =>
            {
                return d.Parent;
            }).Distinct().ToArray();

            var processors = CreateProcessors();

            // Initialize
            foreach (var p in processors)
            {
                p.Initialize(objectCollection.Count);
            }

            // flag
            HashSet<Type>[] flags = new HashSet<Type>[objs.Length];
            for (int i = 0; i < objs.Length; i++)
            {
                flags[i] = new HashSet<Type>();
                foreach (var p in processors)
                {
                    p.ApplyFlags(objs[i], flags[i]);
                }
            }

            // Preprocess
            for (int i = 0; i < objs.Length; i++)
            {
                if (flags[i] == null)
                    continue;

                var flag = flags[i];
                foreach (var p in processors)
                {
                    if (flag.Contains(p.GetType()))
                    {
                        p.Preprocess(i, objs[i], objectCollection);
                    }
                }
            }

            // Process
            for (int i = 0; i < objs.Length; i++)
            {
                if (flags[i] == null)
                    continue;

                var flag = flags[i];
                foreach (var p in processors)
                {
                    if (flag.Contains(p.GetType()))
                        p.Process(i, objs[i], objectCollection);
                }
            }

            // import
            AssetDatabase.StartAssetEditing();
            try
            {
                for (int i = 0; i < objs.Length; i++)
                {
                    if (objs[i].gameObject == null)
                        continue;

                    unitySS2AssetRepo.CreateProcessedObjPrefabAsset(objs[i].gameObject, objs[i].FullPath() + "_" + objs[i].id);
                    GameObject.DestroyImmediate(objs[i].gameObject);
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }

        private ISmartObjectPrefabCreator[] CreateProcessors()
        {
            // order is from special to generic
            return new ISmartObjectPrefabCreator[] {

                // default stuff
                new DecorationCreator(unitySS2AssetRepo, binFileRepo),
                new DoorCreator(),
                new PlayerTeleportTrapCreator(),
                new ButtonCreator(),
                new QBitFilterCreator(),
                new FallbackCreator()
            };
        }

        private static void LoadAttributs(DarkObjectCollection coll, LevelFileGroup db)
        {
            coll.LoadPropertyChunk<SymNameProp>(db);
            coll.LoadPropertyChunk<ObjNameProp>(db);
            coll.LoadPropertyChunk<PositionProp>(db);
            coll.LoadPropertyChunk<ScaleProp>(db);
            coll.LoadPropertyChunk<ModelNameProp>(db);
            coll.LoadPropertyChunk<ImmobileProp>(db);
            coll.LoadPropertyChunk<RenderTypeProp>(db);

            // physics
            coll.LoadPropertyChunk<PhysTypeProp>(db);
            coll.LoadPropertyChunk<PhysStateProp>(db);
            coll.LoadPropertyChunk<PhysDimsProp>(db);
            coll.LoadPropertyChunk<CollisionTypeProp>(db);

            // doors
            coll.LoadPropertyChunk<RotDoorProp>(db);
            coll.LoadPropertyChunk<TransDoorProp>(db);

            // trip wire
            coll.LoadPropertyChunk<TripFlagsProp>(db);

            // inventory
            coll.LoadPropertyChunk<InvDimsProp>(db);
            coll.LoadPropertyChunk<StackCountProp>(db);

            // weapons
            coll.LoadPropertyChunk<GunStateProp>(db);
            coll.LoadPropertyChunk<GunReliabilityProp>(db);
            coll.LoadPropertyChunk<BaseGunDescriptionProp>(db);
            coll.LoadPropertyChunk<WeaponTypeProp>(db);

            coll.LoadLinkAndDataChunk<MetaPropLink>(db);
        }
    }
}
