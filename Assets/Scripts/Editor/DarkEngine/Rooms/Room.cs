using Assets.Scripts.Editor.DarkEngine;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.DarkEngine.Editor.Rooms
{
    class Room
    {
        public int objId;
        public int roomId;

        /// Center point of the room. Should not be in solid space or overlapping
        /// another room
        public Vector3 center;
        /// Bounding box as described by 6 enclosing planes
        public Plane[] planes;
        /// Portal count
        public uint portalCount;
        /// Portal list
        public RoomPortal[] portals;
        /// Portal to portal distances (a 2d array, single index here for simplicity
        /// of allocations)
        public float[] portalDistances;

        public Room(BinaryReader reader)
        {
            objId = reader.ReadInt32();
            roomId = reader.ReadInt16();

            center = reader.ReadPosition();

            planes = new Plane[6];
            for (int i = 0; i < 6; i++)
            {
                planes[i] = reader.ReadPlane();
                planes[i].distance *= -1;
            }

            portalCount = reader.ReadUInt32();
            portals = new RoomPortal[portalCount];
            for (int i = 0; i < portalCount; i++)
            {
                portals[i] = new RoomPortal(reader);
            }

            portalDistances = new float[portalCount * portalCount];
            for (int i = 0; i < portalDistances.Length; i++)
            {
                portalDistances[i] = reader.ReadSingle();
            }

            // read ids in this room
            // there are usually two lists of ID's in the room database
            uint numLists = reader.ReadUInt32();
            for (int i = 0; i < numLists; i++)
            {
                uint count = reader.ReadUInt32();
                for (int k = 0; k < count; k++)
                {
                    int id = reader.ReadInt32();
                }
            }
        }
    }
}
