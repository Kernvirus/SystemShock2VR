using System;
using System.Collections.Generic;
using Assets.Scripts.Editor.DarkEngine.DarkObjects;
using Assets.Scripts.Events.Devices;

namespace Assets.Scripts.Editor.DarkEngine.SmartObjectPrefabCreator
{
    class PlayerTeleportTrapCreator : ISmartObjectPrefabCreator
    {
        public void ApplyFlags(DarkObject darkObject, HashSet<Type> flags)
        {
            if (darkObject.id == -3518 || darkObject.GetParentWithId(-3518) != null)
            {
                flags.Add(typeof(PlayerTeleportTrapCreator));
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
            darkObject.gameObject.AddComponent<PlayerTeleportPoint>();
        }
    }
}
