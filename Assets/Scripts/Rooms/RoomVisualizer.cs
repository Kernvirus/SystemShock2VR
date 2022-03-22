using UnityEngine;

namespace Assets.Scripts.Rooms
{
    public class RoomVisualizer : MonoBehaviour
    {
        [SerializeField]
        public Vector3 center;

        [SerializeField]
        public Vector3[] planesA;
        public float[] planesB;

        public float planeSize = 10;

        void OnDrawGizmosSelected()
        {
            for (int i = 0; i < planesA.Length; i++)
            {
                var pos = new Plane(planesA[i], planesB[i]).ClosestPointOnPlane(transform.position);
                DrawPlane(planesA[i], pos);
            }
        }

        void DrawPlane(Vector3 normal, Vector3 pos) {
            Quaternion rotation = Quaternion.LookRotation(normal);
            Matrix4x4 trs = Matrix4x4.TRS(pos, rotation, Vector3.one);
            Gizmos.matrix = trs;
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(Vector3.zero, new Vector3(planeSize, planeSize, 0.0001f));
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.white;


        }
    }
}
