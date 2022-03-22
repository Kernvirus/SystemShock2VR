using Assets.Scripts.DebugHelper;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Events
{
    public abstract class BasicEventSender : MonoBehaviour, IEventSender
    {
        [SerializeField]
        protected List<MonoBehaviour> receivers = new List<MonoBehaviour>();

        public void AddReceiver(IEventReceiver receiver)
        {
            receivers.Add(receiver as MonoBehaviour);
        }

        public void RemoveReceiver(IEventReceiver receiver)
        {
            receivers.Remove(receiver as MonoBehaviour);
        }

        protected void SendEvent(DarkEvent ev)
        {
            foreach (var r in receivers)
                ((IEventReceiver)r)?.Receive(this, ev);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(transform.position, Vector3.one * 0.1f);
            foreach (var r in receivers)
            {
                if (r == null)
                    continue;

                DebugDrawingExtensions.DrawArrow(transform.position, r.gameObject.transform.position);
            }
        }
    }
}
