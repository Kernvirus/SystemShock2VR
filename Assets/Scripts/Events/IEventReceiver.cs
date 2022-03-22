using UnityEngine;

namespace Assets.Scripts.Events
{
    public interface IEventReceiver
    {
        GameObject gameObject { get; }
        void Receive(IEventSender sender, DarkEvent ev);
    }
}
