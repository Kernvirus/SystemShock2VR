using Assets.Scripts.Utility;
using System;
using UnityEngine;

namespace Assets.Scripts.Rooms
{
    [Serializable]
    public class RoomPortal : ISerializationCallbackReceiver
    {
        // Plane this portal lies on
        [NonSerialized]
        Plane plane;
        // center point of the portal. (should not be in solid space)
        [SerializeField]
        Vector3 center;
        [NonSerialized]
        Room destRoom;

        [SerializeField]
        int destRoomIndex;
        [SerializeField]
        SerializablePlane sPlane;

        // actual room props
        [SerializeField]
        float gravity;

        public void OnAfterDeserialize()
        {
            plane = sPlane.ToPlane();
            sPlane = null;
        }

        public void OnBeforeSerialize()
        {
            destRoomIndex = destRoom.Index;
            sPlane = new SerializablePlane(plane);
        }

        public void SetupDestRoomRef(Room[] rooms)
        {
            destRoom = rooms[destRoomIndex];
        }
    }
}
