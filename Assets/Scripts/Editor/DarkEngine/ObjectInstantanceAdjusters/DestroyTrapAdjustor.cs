using Assets.Scripts.Editor.DarkEngine.DarkObjects;
using Assets.Scripts.Editor.DarkEngine.SmartObjectPrefabCreator;
using Assets.Scripts.Events.Devices;

namespace Assets.Scripts.Editor.DarkEngine.ObjectInstantanceAdjusters
{
    class DestroyTrapAdjustor : IObjectInstanceAdjustor
    {
        public void Process(int index, DarkObject darkObject, DarkObjectCollection collection)
        {
            if (darkObject.GetParentWithId(-1242) != null)
                darkObject.gameObject.AddComponent<DestroyTrap>();
        }
    }
}
