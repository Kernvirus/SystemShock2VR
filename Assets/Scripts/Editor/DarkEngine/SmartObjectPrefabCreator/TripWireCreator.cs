using Assets.Scripts.Editor.DarkEngine.DarkObjects;
using Assets.Scripts.Editor.DarkEngine.DarkObjects.DarkProps;
using Assets.Scripts.Events.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.SmartObjectPrefabCreator
{
    class TripWireCreator : ISmartObjectPrefabCreator
    {
        public void ApplyFlags(DarkObject darkObject, HashSet<Type> flags)
        {
            if (darkObject.GetParentWithId(-305) != null)
            {
                flags.Add(typeof(TripWireCreator));
                flags.Add(typeof(FallbackCreator));
            }
        }

        public void Initialize(int objCount)
        {

        }

        public void Preprocess(int index, DarkObject darkObject, DarkObjectCollection collection)
        {
            
        }

        public void Process(int index, DarkObject darkObject, DarkObjectCollection collection)
        {
            var g = darkObject.gameObject;
            var tw = g.AddComponent<TripWire>();
            var flags = darkObject.GetProp<TripFlagsProp>();
            if (flags != null)
            {
                var so = new SerializedObject(tw);
                so.FindProperty("tripFlags").intValue = flags.Value;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabCreatorUtil.AdjustPosition(darkObject);
            g.transform.localScale = Vector3.one;
            PrefabCreatorUtil.ApplyCollider(darkObject);
            darkObject.GetComponent<Collider>().isTrigger = true;

            var state = darkObject.GetProp<PhysStateProp>();
            g.transform.localPosition = state.location;
            /*if (state.facing != Vector3.zero)
                g.transform.localRotation = Quaternion.LookRotation(state.facing) * g.transform.localRotation;*/
        }
    }
}
