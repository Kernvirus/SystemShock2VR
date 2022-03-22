using System;
using System.Collections.Generic;
using Assets.Scripts.Editor.DarkEngine.DarkObjects;
using Assets.Scripts.Events.Devices;

namespace Assets.Scripts.Editor.DarkEngine.SmartObjectPrefabCreator
{
    class QBitFilterCreator : ISmartObjectPrefabCreator
    {
        public void ApplyFlags(DarkObject darkObject, HashSet<Type> flags)
        {
            if (darkObject.id == -1245 || darkObject.GetParentWithId(-1245) != null)
            {
                flags.Add(typeof(QBitFilterCreator));
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
            darkObject.gameObject.AddComponent<QuestBitFilter>();
        }
    }
}
