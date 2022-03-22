using Assets.Scripts.Editor.DarkEngine.DarkObjects;
using Assets.Scripts.Events.Devices;

namespace Assets.Scripts.Editor.DarkEngine.ObjectInstantanceAdjusters
{
    class InverterTrapAdjustor : IObjectInstanceAdjustor
    {
        public void Process(int index, DarkObject darkObject, DarkObjectCollection collection)
        {
            if (darkObject.GetParentWithId(-306) == null)
                return;

            darkObject.gameObject.AddComponent<InverterTrap>();
        }
    }
}
