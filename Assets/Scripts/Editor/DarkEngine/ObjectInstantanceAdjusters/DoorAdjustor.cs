using Assets.Scripts.Editor.DarkEngine.DarkObjects;
using Assets.Scripts.Editor.DarkEngine.DarkObjects.DarkProps;
using Assets.Scripts.Editor.DarkEngine.SmartObjectPrefabCreator;
using Assets.Scripts.Events.Devices;

namespace Assets.Scripts.Editor.DarkEngine.ObjectInstantanceAdjusters
{
    class DoorAdjustor : IObjectInstanceAdjustor
    {
        DoorCreator dc = new DoorCreator();

        public void Process(int index, DarkObject darkObject, DarkObjectCollection collection)
        {
            if (darkObject.HasProp<TransDoorProp>())
            {
                var door = darkObject.gameObject.GetComponent<Door>();

                if (door == null)
                {
                    dc.Process(index, darkObject, collection);
                }
                else
                {
                    DoorCreator.SetDoorVars(door, darkObject.GetProp<TransDoorProp>());
                }
            }
                
        }
    }
}
