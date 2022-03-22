using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Rooms
{
    public class RoomManager : MonoBehaviour, ISerializationCallbackReceiver
    {
        public static RoomManager Instance;

        [SerializeField]
        Room[] rooms;

        void Awake()
        {
            Debug.Assert(Instance == null);
            Instance = this;
        }

        public Room FindRoomContaining(Vector3 pos, Room prevRoom = null)
        {
            if (prevRoom != null && prevRoom.Contains(pos))
                return prevRoom;

                // TODO: use BSP tree for faster search
                foreach (var room in rooms)
                    if (room.Contains(pos))
                        return room;
            return null;
        }

        public void OnAfterDeserialize()
        {
            foreach (var room in rooms)
                room.SetupPortalRoomRefs(rooms);
        }

        public void OnBeforeSerialize()
        {

        }
    }
}
