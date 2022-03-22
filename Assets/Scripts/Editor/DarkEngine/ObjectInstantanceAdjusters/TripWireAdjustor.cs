using Assets.Scripts.Editor.DarkEngine.DarkObjects;
using Assets.Scripts.Editor.DarkEngine.SmartObjectPrefabCreator;

namespace Assets.Scripts.Editor.DarkEngine.ObjectInstantanceAdjusters
{
    class TripWireAdjustor : IObjectInstanceAdjustor
    {
        TripWireCreator twc = new TripWireCreator();

        public void Process(int index, DarkObject darkObject, DarkObjectCollection collection)
        {
            if (darkObject.GetParentWithId(-305) != null)
                twc.Process(index, darkObject, collection);
        }
    }
}
