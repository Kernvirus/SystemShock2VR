using Assets.Scripts.Editor.DarkEngine.DarkObjects;
using Assets.Scripts.Editor.DarkEngine.DarkObjects.DarkProps;
using Assets.Scripts.Events.Devices;
using UnityEditor;

namespace Assets.Scripts.Editor.DarkEngine.ObjectInstantanceAdjusters
{
    class TriggerDelayAdjustor : IObjectInstanceAdjustor
    {
        public void Process(int index, DarkObject darkObject, DarkObjectCollection collection)
        {
            if (darkObject.GetParentWithId(-911) == null)
                return;

            var triggerDelay = darkObject.gameObject.AddComponent<TriggerDelay>();
            var so = new SerializedObject(triggerDelay);
            so.FindProperty("delayTime").floatValue = darkObject.GetProp<DelayTimeProp>()?.Value ?? 1;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
