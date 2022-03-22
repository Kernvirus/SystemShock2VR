using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Events.Devices
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundTrap : MonoBehaviour, IEventReceiver
    {
        AudioSource audioSource;

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void Receive(IEventSender sender, DarkEvent ev)
        {
            if (ev.State)
            {
                //audioSource.Play();
            }
            else
            {
                //audioSource.Stop();
            }
        }
    }
}
