using UnityEngine;

namespace Assets.Scripts.Utility
{
    [System.Serializable]
    class SerializablePlane
    {
        [SerializeField]
        Vector3 normal;

        [SerializeField]
        float distance;

        public SerializablePlane(Plane plane)
        {
            this.normal = plane.normal;
            this.distance = plane.distance;
        }

        public Plane ToPlane()
        {
            return new Plane(normal, distance);
        }
    }
}
