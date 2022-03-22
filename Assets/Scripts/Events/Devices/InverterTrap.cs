using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Events.Devices
{
    public class InverterTrap : BasicEventSender, IEventReceiver
    {
        public void Receive(IEventSender sender, DarkEvent ev)
        {
            SendEvent(new DarkEvent(!ev.State));
        }
    }
}