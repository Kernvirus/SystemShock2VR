using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Events.Devices
{
    public class Door : MonoBehaviour, IEventReceiver
    {
        public enum DoorStatus
        {
            Closed,
            Open,
            Closing,
            Opening,
            Halted
        }

        [SerializeField]
        float closed = 1;
        [SerializeField]
        float open = 0;
        [SerializeField]
        float speed = 0.5f;
        [SerializeField]
        Vector3 moveAxis = Vector3.right;
        [SerializeField]
        DoorStatus status = DoorStatus.Closed;
        [SerializeField]
        float doorClosingDelay = 3;

        Vector3 baseLocation;
        Coroutine currentDoorAction;

        private void Awake()
        {
            baseLocation = transform.localPosition;
        }

        public void Open()
        {
            if (currentDoorAction != null)
                StopCoroutine(currentDoorAction);
            currentDoorAction = StartCoroutine(PerformOpening());
        }

        public void DelayedClosed()
        {
            if (currentDoorAction != null)
                StopCoroutine(currentDoorAction);
            currentDoorAction = StartCoroutine(PerformClosing());
        }

        private IEnumerator PerformOpening()
        {
            Vector3 targetPos = baseLocation + transform.TransformDirection(moveAxis) * open;
            Vector3 dir = targetPos - transform.localPosition;
            while ((targetPos - transform.localPosition).sqrMagnitude > 0.01f)
            {
                transform.localPosition = transform.localPosition + dir * speed * Time.deltaTime;

                // wait a frame
                yield return null;
            }

            transform.localPosition = targetPos;
            status = DoorStatus.Open;
        }

        private IEnumerator PerformClosing()
        {
            yield return new WaitForSeconds(doorClosingDelay);

            Vector3 targetPos = baseLocation + transform.TransformDirection(moveAxis) * closed;
            Vector3 dir = targetPos - transform.localPosition;
            while ((targetPos - transform.localPosition).sqrMagnitude > 0.01f)
            {
                transform.localPosition = transform.localPosition + dir * speed * Time.deltaTime;

                // wait a frame
                yield return null;
            }

            transform.localPosition = targetPos;
            status = DoorStatus.Closed;
        }

        public void Receive(IEventSender sender, DarkEvent darkEvent)
        {
            if (darkEvent.State && (status == DoorStatus.Closed || status == DoorStatus.Closing))
            {
                Open();
            }
            else if (!darkEvent.State && (status == DoorStatus.Open || status == DoorStatus.Opening))
            {
                DelayedClosed();
            }
        }
    }
}