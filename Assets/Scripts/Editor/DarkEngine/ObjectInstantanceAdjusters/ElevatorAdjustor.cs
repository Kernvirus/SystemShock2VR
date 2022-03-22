using Assets.Scripts.Editor.DarkEngine.DarkObjects;
using Assets.Scripts.Editor.DarkEngine.DarkObjects.DarkLinks;
using Assets.Scripts.Editor.DarkEngine.SmartObjectPrefabCreator;
using Assets.Scripts.Events.Devices;
using System.Collections.Generic;

namespace Assets.Scripts.Editor.DarkEngine.ObjectInstantanceAdjusters
{
    class ElevatorAdjustor : IObjectInstanceAdjustor
    {
        public void Process(int index, DarkObject darkObject, DarkObjectCollection collection)
        {
            if (darkObject.GetParentWithId(-237) == null)
                return;

            var elevator = darkObject.gameObject.AddComponent<Elevator>();
            PrefabCreatorUtil.AddKinematicRigidbody(darkObject);

            var initLinks = darkObject.GetLinks("TPathInit");
            if (initLinks.Count > 0)
            {
                var pathStart = initLinks[0];

                List<ElevatorPoint> pathPoints = new List<ElevatorPoint>();

                List<Link> pathLinks;
                var currentPathObj = pathStart.dest;
                while ((pathLinks = currentPathObj.GetLinks(typeof(TPathLink))).Count > 0)
                {
                    var pathLink = pathLinks[0];
                    var pathData = (TPathLink)pathLink.data;
                    ElevatorPoint point = new ElevatorPoint(pathLink.dest.gameObject.transform, pathData.speed, pathData.pause, pathData.pathLimit);

                    pathPoints.Add(point);
                    currentPathObj = pathLink.dest;

                    if (currentPathObj == pathStart.dest)
                        break;
                }

                elevator.pathPoints = pathPoints.ToArray();
            }
        }
    }
}
