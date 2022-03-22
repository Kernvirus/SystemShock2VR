using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Events.Devices
{
    public class TriggerDelay : BasicEventSender, IEventReceiver
    {
        [SerializeField]
        float delayTime;

        public void Receive(IEventSender sender, DarkEvent ev)
        {
            StartCoroutine(Delay(ev));
        }

        IEnumerator Delay(DarkEvent ev)
        {
            yield return new WaitForSeconds(delayTime);
            this.SendEvent(ev);
        }
    }
}