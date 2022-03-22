using Assets.Scripts.Editor.DarkEngine.DarkObjects;
using Assets.Scripts.Editor.DarkEngine.DarkObjects.DarkProps;
using Assets.Scripts.Editor.DarkEngine.Exceptions;
using Assets.Scripts.Editor.DarkEngine.Files;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.SmartObjectPrefabCreator
{
    class DecorationCreator : ISmartObjectPrefabCreator
    {
        UnitySS2AssetRepository unitySS2AssetRepo;
        BinFileRepository binFileRepo;

        public DecorationCreator(UnitySS2AssetRepository unitySS2AssetRepo, BinFileRepository binFileRepo)
        {
            this.unitySS2AssetRepo = unitySS2AssetRepo;
            this.binFileRepo = binFileRepo;
        }

        public void ApplyFlags(DarkObject darkObject, HashSet<Type> flags)
        {
            if (darkObject.HasProp<ModelNameProp>() && darkObject.HasProp<SymNameProp>())
            {
                string modelName = darkObject.GetProp<ModelNameProp>().Value.ToLower();
                if (!binFileRepo.DoesNameExist(modelName))
                    return;

                bool isObj = binFileRepo.IsObjectMesh(modelName);
                if (!unitySS2AssetRepo.DoesAssetExist((isObj ? "obj/prefabs/" : "mesh/prefabs/") + modelName + ".prefab"))
                    return;

                flags.Add(typeof(DecorationCreator));
            }
        }

        public void Initialize(int objCount)
        {
        }

        public void Preprocess(int index, DarkObject darkObject, DarkObjectCollection collection)
        {
            PrefabCreatorUtil.CreateModelGOAt(darkObject, unitySS2AssetRepo, binFileRepo);
            PrefabCreatorUtil.ApplyCollider(darkObject);
        }

        public void Process(int index, DarkObject darkObject, DarkObjectCollection collection)
        {

        }
    }
}
