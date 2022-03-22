using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Events.Devices
{
    public class QuestBitFilter : BasicEventSender, IEventReceiver
    {
        public void Receive(IEventSender sender, DarkEvent ev)
        {
            // normally should filter something here, we are just passing it along
            SendEvent(ev);
        }
    }
}
