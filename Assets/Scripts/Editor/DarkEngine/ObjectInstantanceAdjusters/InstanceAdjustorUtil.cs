using Assets.Scripts.Editor.DarkEngine.DarkObjects;
using Assets.Scripts.Editor.DarkEngine.DarkObjects.DarkProps;
using Assets.Scripts.Editor.DarkEngine.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.ObjectInstantanceAdjusters
{
    static class InstanceAdjustorUtil
    {
        public static void ChangeModelAt(DarkObject darkObject, UnitySS2AssetRepository unitySS2AssetRepo, BinFileRepository binFileRepo)
        {
            string modelName = darkObject.GetProp<ModelNameProp>().Value.ToLower();
            if (modelName == "fx_particle")
                return;

            bool isObj = binFileRepo.IsObjectMesh(modelName);
            var modelPrefab = unitySS2AssetRepo.LoadObjPrefabAsset(modelName, isObj);

            var g = darkObject.gameObject;

            CopyComponent(modelPrefab.GetComponent<MeshFilter>(), GetOrCreateComponent<MeshFilter>(g));
            if (isObj)
                CopyComponent(modelPrefab.GetComponent<MeshRenderer>(), GetOrCreateComponent<MeshRenderer>(g));
            else
            {
                var orgInst = GameObject.Instantiate<GameObject>(modelPrefab);
                CopyComponent(orgInst.GetComponent<SkinnedMeshRenderer>(), GetOrCreateComponent<SkinnedMeshRenderer>(g));

                var boneRoot = orgInst.transform.GetChild(0);
                boneRoot.transform.SetParent(g.transform, false);

                GameObject.DestroyImmediate(orgInst);
            }
        }

        private static T GetOrCreateComponent<T>(GameObject g) where T : Component
        {
            var comp = g.GetComponent<T>();
            if (comp != null)
                return comp;

            return g.AddComponent<T>();
        }

        private static void CopyComponent<T>(T source, T target) where T : Component
        {
            EditorUtility.CopySerialized(source, target);
        }
    }
}
