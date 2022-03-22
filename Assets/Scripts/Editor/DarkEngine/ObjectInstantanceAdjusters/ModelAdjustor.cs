using Assets.Scripts.Editor.DarkEngine.DarkObjects;
using Assets.Scripts.Editor.DarkEngine.DarkObjects.DarkProps;
using Assets.Scripts.Editor.DarkEngine.Files;
using Assets.Scripts.Editor.DarkEngine.SmartObjectPrefabCreator;
using Assets.Scripts.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.ObjectInstantanceAdjusters
{
    class ModelAdjustor : IObjectInstanceAdjustor
    {
        UnitySS2AssetRepository unitySS2AssetRepo;
        BinFileRepository binFileRepo;

        public ModelAdjustor(UnitySS2AssetRepository unitySS2AssetRepo, BinFileRepository binFileRepo)
        {
            this.unitySS2AssetRepo = unitySS2AssetRepo;
            this.binFileRepo = binFileRepo;
        }

        public void Process(int index, DarkObject darkObject, DarkObjectCollection collection)
        {
            if (darkObject.HasPropDirectly<ModelNameProp>())
                InstanceAdjustorUtil.ChangeModelAt(darkObject, unitySS2AssetRepo, binFileRepo);

            var renderType = darkObject.GetProp<RenderTypeProp>();
            var hasRefs = darkObject.GetProp<HasRefsProp>();

            bool disableRenders = darkObject.GetParentWithId(-4581) != null ||
                (hasRefs != null && !hasRefs.Value) ||
                (renderType != null && (renderType.Value == RenderType.NotRendered || renderType.Value == RenderType.EditorOnly));
            if (disableRenders)
                DisableRenderes(darkObject.gameObject);

            PrefabCreatorUtil.ApplyCollider(darkObject);

            if (GameObjectUtility.GetStaticEditorFlags(darkObject.gameObject) != 0 && (!darkObject.HasPropDirectly<ImmobileProp>() || darkObject.GetProp<ImmobileProp>().Value))
            {
                GameObjectUtility.SetStaticEditorFlags(darkObject.gameObject, ImporterSettings.staticFlags);
            }
        }

        private void DisableRenderes(GameObject g)
        {
            var mrs = g.GetComponentsInChildren<MeshRenderer>();
            foreach (var mr in mrs)
                mr.enabled = false;
        }
    }
}
