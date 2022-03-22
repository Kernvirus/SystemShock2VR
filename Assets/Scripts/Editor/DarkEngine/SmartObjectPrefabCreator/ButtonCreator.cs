using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Editor.DarkEngine.DarkObjects;
using Assets.Scripts.Editor.DarkEngine.DarkObjects.DarkProps;
using Assets.Scripts.Editor.DarkEngine.Exceptions;
using Assets.Scripts.Events.Devices;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.SmartObjectPrefabCreator
{
    class ButtonCreator : ISmartObjectPrefabCreator
    {
        public void ApplyFlags(DarkObject darkObject, HashSet<Type> flags)
        {
            if (darkObject.id == -200 || darkObject.GetParentWithId(-200) != null)
            {
                flags.Add(typeof(ButtonCreator));
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
            if (darkObject.gameObject == null)
            {
                PrefabCreatorUtil.CreateGO(darkObject);
            }

            darkObject.gameObject.AddComponent<Button>();
            darkObject.gameObject.layer = 9;

            PrefabCreatorUtil.ApplyCollider(darkObject);
        }
    }
}
