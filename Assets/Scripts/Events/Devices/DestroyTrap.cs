using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Events.Devices
{
    public class DestroyTrap : BasicEventSender, IEventReceiver
    {
        public void Receive(IEventSender sender, DarkEvent ev)
        {
            for (int i = 0; i < receivers.Count; i++)
                Destroy(receivers[i]);
        }
    }
}