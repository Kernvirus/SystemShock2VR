using Assets.Scripts.Editor.DarkEngine.DarkObjects;
using Assets.Scripts.Editor.DarkEngine.DarkObjects.DarkProps;
using Assets.Scripts.Events.Devices;
using UnityEditor;

namespace Assets.Scripts.Editor.DarkEngine.ObjectInstantanceAdjusters
{
    class MultiLevelAdjustor : IObjectInstanceAdjustor
    {
        public void Process(int index, DarkObject darkObject, DarkObjectCollection collection)
        {
            var destLoc = darkObject.GetProp<DestLocProp>();
            var destLevel = darkObject.GetProp<DestLevelProp>();

            if (destLoc == null || destLevel == null)
                return;

            var lw = darkObject.gameObject.AddComponent<LevelSwitcher>();

            var so = new SerializedObject(lw);

            so.FindProperty("destLoc").intValue = destLoc.Value;

            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
