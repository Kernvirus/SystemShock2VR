using Assets.Scripts.Editor.DarkEngine.DarkObjects;
using Assets.Scripts.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Editor.DarkEngine.ObjectInstantanceAdjusters
{
    class SwitchLinkAdjustor : IObjectInstanceAdjustor
    {
        public void Process(int index, DarkObject darkObject, DarkObjectCollection collection)
        {
            IEventSender sender;
            foreach (var swl in darkObject.GetLinks("SwitchLink"))
            {
                sender = swl.src.gameObject.GetComponent<IEventSender>();
                var receiver = swl.dest.gameObject.GetComponent<IEventReceiver>();

                if (sender != null && receiver != null)
                    sender.AddReceiver(receiver);
            }

            sender = darkObject.gameObject.GetComponent<IEventSender>();
            var receivers = darkObject.gameObject.GetComponents<IEventReceiver>();

            if (sender != null)
            {
                foreach (var receiver in receivers)
                    if (sender != receiver)
                        sender.AddReceiver(receiver);
            }
        }
    }
}
