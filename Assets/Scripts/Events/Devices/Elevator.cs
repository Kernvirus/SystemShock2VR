using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Events.Devices
{
    public class Elevator : MonoBehaviour, IEventReceiver
    {
        [SerializeField]
        public ElevatorPoint[] pathPoints;

        bool isMoving;
        int currentGoal;

        public void Receive(IEventSender sender, DarkEvent ev)
        {
            isMoving = true;
        }

        void Update()
        {
            if (isMoving)
            {
                Vector3 pos = transform.position;
                if (pathPoints[currentGoal].MoveTowards(pos, out pos))
                {
                    isMoving = false;
                    StartCoroutine(WaitAtGoal(pathPoints[currentGoal].PauseTime));
                }
                transform.position = pos;
            }
        }

        IEnumerator WaitAtGoal(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            MoveToNextGoal();
        }

        void MoveToNextGoal()
        {
            currentGoal++;
            if (currentGoal >= pathPoints.Length)
            {
                currentGoal = 0;
            }
        }
    }

    [System.Serializable]
    public class ElevatorPoint {

        public float PauseTime => pauseTime;

        [SerializeField]
        Transform position;
        [SerializeField]
        float speed;
        [SerializeField]
        float pauseTime;
        [SerializeField]
        bool pathLimit;

        public ElevatorPoint(Transform position, float speed, float pauseTime, bool pathLimit)
        {
            this.position = position;
            this.speed = speed;
            this.pauseTime = pauseTime;
            this.pathLimit = pathLimit;
        }

        public bool MoveTowards(Vector3 pos, out Vector3 newPos)
        {
            Vector3 dir = position.position - pos;
            float dist = Time.deltaTime * speed;

            float dirDist = dir.magnitude;

            if (dirDist < dist)
            {
                newPos = position.position;
                return true;
            }
            else
            {
                newPos = pos + dir * (dist / dirDist);
                return false;
            }
        }
    }
}
