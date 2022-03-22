using Assets.Scripts.Utility;
using System;
using UnityEngine;

namespace Assets.Scripts.Rooms
{
    [System.Serializable]
    public class Room : ISerializationCallbackReceiver
    {
        // Bounding box as described by 6 enclosing planes
        [NonSerialized]
        Plane[] planes;
        // Center point of the room. Should not be in solid space or overlapping another room
        [SerializeField]
        Vector3 center;

        [SerializeField]
        RoomPortal[] portals;

        [SerializeField]
        int index;

        [SerializeField]
        SerializablePlane[] sPlanes;

        public int Index => index;

        public Room(Plane[] planes, Vector3 center, RoomPortal[] portals, int index)
        {
            this.planes = planes;
            this.center = center;
            this.portals = portals;
            this.index = index;
        }

        public bool Contains(Vector3 pos)
        {
            return planes[0].GetSide(pos) &&
                planes[1].GetSide(pos) &&
                planes[2].GetSide(pos) &&
                planes[3].GetSide(pos) &&
                planes[4].GetSide(pos) &&
                planes[5].GetSide(pos);
        }

        public void OnAfterDeserialize()
        {
            planes = new Plane[6];
            for (int i = 0; i < 6; i++)
            {
                planes[i] = sPlanes[i].ToPlane();
            }
            sPlanes = null;
        }

        public void OnBeforeSerialize()
        {
            sPlanes = new SerializablePlane[6];
            for (int i = 0; i < 6; i++)
            {
                sPlanes[i] = new SerializablePlane(planes[i]);
            }
        }

        public void SetupPortalRoomRefs(Room[] rooms)
        {
            foreach (var portal in portals)
                portal.SetupDestRoomRef(rooms);
        }
    }
}
