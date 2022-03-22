using System.Collections.Generic;

namespace Assets.Scripts.Editor.DarkEngine.Animation
{
    public enum CreatureType
    {
        Humanoid = 0,
        PlayerLimb = 1,
        Avatar = 2,
        Rumbler = 3,
        Droid = 4,
        Overlord = 5,
        Arachnid = 6,
        Monkey = 7,
        BabyArachnid = 8,
        Shodan = 9,
    }

    public enum ActorType
    {
        Humanoid,
        PlayerLimb,
        Droid,
        Overlord,
        Arachnid
    }

    public enum HumanJoints
    {
        LToe,
        RToe,
        LAnkle,
        RAnkle,
        LKnee,
        RKnee,
        LHip,
        RHip,
        Butt,
        Neck,
        LShldr,
        RShldr,
        LElbow,
        RElbow,
        LWrist,
        RWrist,
        LFinger,
        RFinger,
        Abdomen,
        Head,
        LShldrIn,
        RShldrIn,
        LWeap,
        RWeap
    }

    public enum DroidJoints
    {
        LToe,
        RToe,
        LAnkle,
        RAnkle,
        LKnee,
        RKnee,
        LHip,
        RHip,
        Butt,
        Abdomen,
        Neck,
        LShldr,
        RShldr,
        LElbow,
        RElbow,
        LWrist,
        RWrist,
        Head
    }

    public enum ArmJoints
    {
        Butt,
        Shldr,
        Elbow,
        Wrist,
        Finger
    }

    public enum OverlordJoints
    {
        Base,
        F1Snout,
        F2Snout,
        F3Snout,
        F4Snout,
        B1Stem,
        B2Stage,
        B3Stage,
        B4Stage,
        B5Stage,
        B6Stage,
        R1Shldr,
        R1Elbow,
        R1Wrist,
        R1Finger,
        R1App,
        R1Tip,
        R2Shldr,
        R2Elbow,
        R2Wrist,
        R2Finger,
        R2App,
        R2Tip,
        L1Shldr,
        L1Elbow,
        L1Wrist,
        L1Finger,
        L1App,
        L1Tip,
        L2Shldr,
        L2Elbow,
        L2Wrist,
        L2Finger,
        L2App,
        L2Tip,
        Sac,
        LSac
    }

    public enum ArachnidJoints
    {
        Base,
        LMAnd,
        LMElbow,
        RMAnd,
        RMElbow,
        R1Shldr,
        R1Elbow,
        R1Wrist,
        R2Shldr,
        R2Elbow,
        R2Wrist,
        R3Shldr,
        R3Elbow,
        R3Wrist,
        R4Shldr,
        R4Elbow,
        R4Wrist,
        L1Shldr,
        L1Elbow,
        L1Wrist,
        L2Shldr,
        L2Elbow,
        L2Wrist,
        L3Shldr,
        L3Elbow,
        L3Wrist,
        L4Shldr,
        L4Elbow,
        L4Wrist,
        R1Finger,
        R2Finger,
        R3Finger,
        R4Finger,
        L1Finger,
        L2Finger,
        L3Finger,
        L4Finger,
        LTip,
        RTip,
        Sac
    }

    public enum MonkeyJoints
    {
        LToe,
        RToe,
        LAnkle,
        RAnkle,
        LKnee,
        RKnee,
        LHip,
        RHip,
        Butt,
        Neck,
        LShldr,
        RShldr,
        LElbow,
        RElbow,
        LWrist,
        RWrist,
        LFinger,
        RFinger,
        Abdomen,
        Head,
        Tail
    }

    class CreatureDefinition
    {
        public static string GetCreatureName(CreatureType type)
        {
            return creatureNames[(int)type];
        }

        public static string GetActorName(ActorType type)
        {
            return actorNames[(int)type];
        }

        private static string[] creatureNames = new string[]
        {
            "Humanoid",
            "Wrench",
            "Avatar",
            "Rumbler",
            "Droid",
            "Overlord",
            "Arachnid",
            "Monkey",
            "BabyArach",
            "Shodan",
        };

        private static string[] actorNames = new string[]
        {
            "Humanoid",
            "PlayerLimb",
            "Droid",
            "Overlord",
            "Arachnid"
        };

        public ActorType actorType;
        public int nTorsos;
        public int nLimbs;
        public int nJoints;
        public string defLengthsName;
        public int[] jointParents;
        public int[] jointMap;
        public string[] jointNames;
        public int nFeet;
        public int[] footJoints;
        public bool canHeadTrack;

        public string GetJointName(uint jointId)
        {
            return jointNames[jointId];
        }

        public string GetJointPath(int jointId)
        {
            Stack<int> joints = new Stack<int>();
            int j = jointId;
            while (j != -1)
            {
                joints.Push(j);
                j = jointParents[j];
            }

            string path = "";
            while (joints.Count > 0)
            {
                j = joints.Pop();
                path += jointNames[j] + "/";
            }
            return path.Substring(0, path.Length - 1);
        }

        public int GetJointParent(int jointId)
        {
            return jointParents[jointId];
        }
    }
}
