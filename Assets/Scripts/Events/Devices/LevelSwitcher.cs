using UnityEngine;

namespace Assets.Scripts.Events.Devices
{
    public class LevelSwitcher : MonoBehaviour, IEventReceiver
    {
        [SerializeField]
        int destLoc;

        public void Receive(IEventSender sender, DarkEvent ev)
        {
            // IOU: Level switching logic
        }
    }
}
