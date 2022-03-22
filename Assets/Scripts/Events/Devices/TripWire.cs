using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Events.Devices
{
    public class TripWire : BasicEventSender
    {
        [Flags]
        public enum TripControlFlags
        {
            Enter = 1,
            Exit = 2,
            Mono = 4,
            Once = 8,
            Invert = 16,
            Player = 32,
            Alarm = 64,
            Shove = 128,
            ZapInside = 256,
            EasterEgg1 = 512
        }

        [SerializeField]
        private TripControlFlags tripFlags;


        private void OnTriggerEnter(Collider other)
        {
            if ((tripFlags & TripControlFlags.Player) != 0 && !other.CompareTag("Player"))
                return;

            if ((tripFlags & TripControlFlags.Enter) != 0)
            {
                if ((tripFlags & TripControlFlags.Invert) != 0)
                {
                    SendEvent(new DarkEvent(false));
                }
                else
                {
                    SendEvent(new DarkEvent(true));
                }

                if ((tripFlags & TripControlFlags.Once) != 0)
                    Destroy(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if ((tripFlags & TripControlFlags.Player) != 0 && !other.CompareTag("Player"))
                return;

            if ((tripFlags & TripControlFlags.Exit) != 0)
            {
                if ((tripFlags & TripControlFlags.Invert) != 0)
                {
                    SendEvent(new DarkEvent(true));
                }
                else
                {
                    SendEvent(new DarkEvent(false));
                }

                if ((tripFlags & TripControlFlags.Once) != 0)
                    Destroy(this);
            }
        }
    }
}