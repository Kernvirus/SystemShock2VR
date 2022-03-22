using Assets.Scripts.Editor.DarkEngine;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.DarkEngine.Editor.Rooms
{
    class RoomPortal
    {
        /// Portal ID
        int id;
        /// The index of this portal in the room's portal list
        uint index;
        /// Plane this portal lies on
        Plane plane;
        /// Number of portal edges
        uint edgeCount;
        /// Plane list - planes that make up the portal
        Plane[] edges;
        // Source and destination rooms
        /// room number this portal goes to
        int destRoom;
        /// the source room number
        int srcRoom;
        /// center point of the portal. (should not be in solid space)
        Vector3 center;
        /// portal ID on the other side of this portal
        int destPortal;

        public RoomPortal(BinaryReader reader)
        {
            id = reader.ReadInt32();
            index = reader.ReadUInt32();
            plane = reader.ReadPlane();
            edgeCount = reader.ReadUInt32();

            edges = new Plane[edgeCount];
            for (int i = 0; i < edgeCount; i++)
            {
                edges[i] = reader.ReadPlane();
            }

            srcRoom = reader.ReadInt32();
            destRoom = reader.ReadInt32();

            center = reader.ReadVector3();
            destPortal = reader.ReadInt32();
        }
    }
}
