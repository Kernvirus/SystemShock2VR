using System.IO;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.DarkObjects.DarkProps
{
    class DoorProp : Prop
    {
        public enum DoorType
        {
            Rotating,
            Translating
        }
        public enum DoorStatus
        {
            Closed,
            Open,
            Closing,
            Opening,
            Halted
        }

        public DoorType type;
        public float closed;
        public float open;

        public float baseSpeed;

        public int axis;
        public DoorStatus state;

        public bool hardLimits;

        public float soundBlocking;
        public bool visionBlocking;

        public float pushMass;

        public Vector3 baseClosedLocation;
        public Vector3 baseOpenLocation;

        public Vector3 baseLocation;
        public Vector3 baseAngle;

        public float base_;

        public int room1;
        public int room2;

        public override void Load(BinaryReader reader, int propLen)
        {
            type = (DoorType)reader.ReadInt32();
            closed = reader.ReadSingle() * ImporterSettings.globalScale;
            open = reader.ReadSingle() * ImporterSettings.globalScale;
            baseSpeed = reader.ReadSingle() * ImporterSettings.globalScale;
            axis = reader.ReadInt32();
            state = (DoorStatus)reader.ReadInt32();
            hardLimits = reader.ReadInt32() != 0;
            soundBlocking = reader.ReadSingle();
            visionBlocking = reader.ReadInt32() != 0;
            pushMass = reader.ReadSingle();
            baseClosedLocation = reader.ReadVector3();
            baseOpenLocation = reader.ReadVector3();
            baseLocation = reader.ReadVector3();
            baseAngle = reader.ReadAngVec();
            base_ = reader.ReadSingle();
            room1 = reader.ReadInt32();
            room2 = reader.ReadInt32();
        }
    }
}
