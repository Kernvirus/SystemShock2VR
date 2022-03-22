using Assets.Scripts.Editor.DarkEngine.DarkObjects;
using Assets.Scripts.Editor.DarkEngine.DarkObjects.DarkProps;
using Assets.Scripts.Events.Devices;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.SmartObjectPrefabCreator
{
    class DoorCreator : ISmartObjectPrefabCreator
    {
        public void ApplyFlags(DarkObject darkObject, HashSet<Type> flags)
        {
            if (flags.Contains(typeof(DecorationCreator)) && darkObject.HasProp<TransDoorProp>())
            {
                flags.Add(typeof(DoorCreator));
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
            TransDoorProp door = darkObject.GetProp<TransDoorProp>();
            GameObject g = darkObject.gameObject;

            PrefabCreatorUtil.AddKinematicRigidbody(darkObject);
            GameObjectUtility.SetStaticEditorFlags(g, 0);
            var doorComp = g.AddComponent<Door>();

            SetDoorVars(doorComp, door);
        }

        public static void SetDoorVars(Door door, TransDoorProp doorProp)
        {
            SerializedObject so = new SerializedObject(door);

            so.FindProperty("closed").floatValue = doorProp.closed;
            so.FindProperty("open").floatValue = doorProp.open;
            so.FindProperty("speed").floatValue = doorProp.baseSpeed;

            switch (doorProp.axis)
            {
                case 0:
                    so.FindProperty("moveAxis").vector3Value = Vector3.left;
                    break;
                case 1:
                    so.FindProperty("moveAxis").vector3Value = Vector3.forward;
                    break;
                case 2:
                    so.FindProperty("moveAxis").vector3Value = Vector3.up;
                    break;
            }
            so.FindProperty("status").enumValueIndex = (int)doorProp.state;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
