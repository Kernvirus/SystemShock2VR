using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Assets.Scripts.Editor.DarkEngine.LevelFile;
using Assets.Scripts.Editor.DarkEngine.Exceptions;
using Assets.Scripts.Rooms;
using UnityEditor;
using System.Linq;

namespace Assets.Scripts.DarkEngine.Editor.Rooms
{
    class RoomDB
    {
        List<Room> rooms = new List<Room>();

        public void Load(LevelFileGroup db)
        {
            if (!db.IsMisFile)
                return;

            BinaryReader reader = db.GetReaderAt(db.GetFile("ROOM_DB"));

            bool roomsOk = reader.ReadUInt32() != 0;
            if (!roomsOk)
                throw new DarkException("Rooms are not ok");

            uint count = reader.ReadUInt32();
            rooms.Capacity = rooms.Count + (int)count;

            for (int i = 0; i < count; i++)
            {
                rooms.Add(new Room(reader));
            }
        }

        public void Instantiate()
        {
            GameObject gameObject = new GameObject("RoomDB");
            var roomManager = gameObject.AddComponent<RoomManager>();
            SerializedObject so = new SerializedObject(roomManager);

            
        }
    }
}
