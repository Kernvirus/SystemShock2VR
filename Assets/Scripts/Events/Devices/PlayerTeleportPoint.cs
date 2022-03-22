using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Events.Devices
{
    public class PlayerTeleportPoint : MonoBehaviour, IEventReceiver
    {
        public void Receive(IEventSender sender, DarkEvent darkEvent)
        {
            if (darkEvent.State)
            {
                // IOU: teleportation logic

            }
        }
    }
}