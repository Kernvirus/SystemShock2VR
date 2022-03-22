using Assets.Scripts.Editor.DarkEngine.DarkObjects;
using Assets.Scripts.Editor.DarkEngine.DarkObjects.DarkProps;
using UnityEditor;

namespace Assets.Scripts.Editor.DarkEngine.ObjectInstantanceAdjusters
{
    class SpawnMarkerAdjustor : IObjectInstanceAdjustor
    {
        public void Process(int index, DarkObject darkObject, DarkObjectCollection collection)
        {
            if (!darkObject.HasProp<StartLocProp>())
                return;

            //var darkObj = darkObject.gameObject.AddComponent<SpawnMarker>();

            //var so = new SerializedObject(darkObj);
            //so.FindProperty("locationId").intValue = darkObject.GetProp<StartLocProp>().Value;
            //so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
