namespace Assets.Scripts.Editor.DarkEngine.Animation
{
    static class CreatureDefinitions
    {
        public static CreatureDefinition Get(CreatureType type)
        {
            return creatureDefs[(int)type];
        }

        public static CreatureDefinition human = new CreatureDefinition()
        {
            actorType = ActorType.Humanoid,
            nTorsos = 2,
            nLimbs = 5,
            nJoints = 20,
            defLengthsName = "mguard",
            jointParents = new int[] {
            (int)HumanJoints.LAnkle, (int)HumanJoints.RAnkle, (int)HumanJoints.LKnee,
            (int)HumanJoints.RKnee, (int)HumanJoints.LHip, (int)HumanJoints.RHip,
            (int)HumanJoints.Butt, (int)HumanJoints.Butt, -1,
            (int)HumanJoints.Abdomen, (int)HumanJoints.Abdomen, (int)HumanJoints.Abdomen,
            (int)HumanJoints.LShldr, (int)HumanJoints.RShldr, (int)HumanJoints.LElbow,
            (int)HumanJoints.RElbow, (int)HumanJoints.LWrist, (int)HumanJoints.RWrist,
            (int)HumanJoints.Butt, (int)HumanJoints.Neck },
            jointMap = new int[] {
            -1, 19, 9, 18, 8, 10, 11, 12, 13, 14, 15, 16,
            17, 6, 7, 4, 5, 2, 3, 0, 1, -1},
            jointNames = new string[] {
            "LToe", "RToe", "LAnkle", "RAnkle", "LKnee",
            "RKnee", "LHip", "RHip", "Butt", "Neck",
            "LShldr", "RShldr", "LElbow", "RElbow", "LWrist",
            "RWrist", "LFinger", "RFinger", "Abdomen", "Head",
            "LShldrIn", "RShldrIn", "LWeap", "RWeap"},
            nFeet = 2,
            footJoints = new int[] { (int)HumanJoints.LAnkle, (int)HumanJoints.RAnkle },
            canHeadTrack = true
        };

        public static CreatureDefinition wrench = new CreatureDefinition()
        {
            actorType = ActorType.PlayerLimb,
            nTorsos = 1,
            nLimbs = 1,
            nJoints = 5,
            defLengthsName = "wrench_h",
            jointParents = new int[] {
            (int)ArmJoints.Butt, (int)ArmJoints.Butt, (int)ArmJoints.Shldr,
            (int)ArmJoints.Elbow, (int)ArmJoints.Wrist},
            jointMap = new int[] {
               -1, -1,-1,-1,0,-1,1,-1,2,-1,3,-1,4,-1,-1,-1,
               -1,-1,-1,-1,-1,-1,},
            jointNames = new string[] {
            "Butt", "Shldr", "Elbow", "Wrist", "Finger"},
            nFeet = 0,
            footJoints = null,
            canHeadTrack = false
        };

        public static CreatureDefinition avatar = new CreatureDefinition()
        {
            actorType = ActorType.Humanoid,
            nTorsos = 2,
            nLimbs = 5,
            nJoints = 20,
            defLengthsName = "mguard",
            jointParents = new int[] {
            (int)HumanJoints.LAnkle, (int)HumanJoints.RAnkle, (int)HumanJoints.LKnee,
            (int)HumanJoints.RKnee, (int)HumanJoints.LHip, (int)HumanJoints.RHip,
            (int)HumanJoints.Butt, (int)HumanJoints.Butt, -1,
            (int)HumanJoints.Abdomen, (int)HumanJoints.Abdomen, (int)HumanJoints.Abdomen,
            (int)HumanJoints.LShldr, (int)HumanJoints.RShldr, (int)HumanJoints.LElbow,
            (int)HumanJoints.RElbow, (int)HumanJoints.LWrist, (int)HumanJoints.RWrist,
            (int)HumanJoints.Butt, (int)HumanJoints.Neck },
            jointMap = new int[] {
            -1, 19, 9, 18, 8, 10, 11, 12, 13, 14, 15, 16,
            17, 6, 7, 4, 5, 2, 3, 0, 1, -1},
            jointNames = new string[] {
            "LToe", "RToe", "LAnkle", "RAnkle", "LKnee",
            "RKnee", "LHip", "RHip", "Butt", "Neck",
            "LShldr", "RShldr", "LElbow", "RElbow", "LWrist",
            "RWrist", "LFinger", "RFinger", "Abdomen", "Head",
            "LShldrIn", "RShldrIn", "LWeap", "RWeap"},
            nFeet = 2,
            footJoints = new int[] { (int)HumanJoints.LAnkle, (int)HumanJoints.RAnkle },
            canHeadTrack = true
        };

        public static CreatureDefinition rumbler = new CreatureDefinition()
        {
            actorType = ActorType.Humanoid,
            nTorsos = 2,
            nLimbs = 5,
            nJoints = 20,
            defLengthsName = "mguard",
            jointParents = new int[] {
            (int)HumanJoints.LAnkle, (int)HumanJoints.RAnkle, (int)HumanJoints.LKnee,
            (int)HumanJoints.RKnee, (int)HumanJoints.LHip, (int)HumanJoints.RHip,
            (int)HumanJoints.Butt, (int)HumanJoints.Butt, -1,
            (int)HumanJoints.Abdomen, (int)HumanJoints.Abdomen, (int)HumanJoints.Abdomen,
            (int)HumanJoints.LShldr, (int)HumanJoints.RShldr, (int)HumanJoints.LElbow,
            (int)HumanJoints.RElbow, (int)HumanJoints.LWrist, (int)HumanJoints.RWrist,
            (int)HumanJoints.Butt, (int)HumanJoints.Neck },
            jointMap = new int[] {
            -1, 19, 9, 18, 8, /**/  10, 11, 12, 13, 14,
            15, 16, 17, 6, 7, /**/   4,  5,  2,  3,  0,
            1, -1},
            jointNames = new string[] {
            "LToe", "RToe", "LAnkle",
            "RAnkle", "LKnee", "RKnee",
            "LHip", "RHip", "Butt",
            "Neck", "LShldr", "RShldr",
            "LElbow", "RElbow", "LWrist",
            "RWrist", "LFinger", "RFinger",
            "Abdomen", "Head", "Tail",},
            nFeet = 2,
            footJoints = new int[] { (int)HumanJoints.LAnkle, (int)HumanJoints.RAnkle },
            canHeadTrack = true
        };

        public static CreatureDefinition droid = new CreatureDefinition()
        {
            actorType = ActorType.Droid,
            nTorsos = 2,
            nLimbs = 5,
            nJoints = 18,
            defLengthsName = "mainbot",
            jointParents = new int[] {
            (int)DroidJoints.LAnkle, (int)DroidJoints.RAnkle, (int)DroidJoints.LKnee,
            (int)DroidJoints.RKnee, (int)DroidJoints.LHip, (int)DroidJoints.RHip,
            (int)DroidJoints.Butt, (int)DroidJoints.Butt, -1,
            (int)DroidJoints.Butt, (int)DroidJoints.Abdomen, (int)DroidJoints.Abdomen,
            (int)DroidJoints.Abdomen, (int)DroidJoints.LShldr, (int)DroidJoints.RShldr, (int)DroidJoints.LElbow,
            (int)DroidJoints.RElbow, (int)DroidJoints.Neck },
            jointMap = new int[] {
            -1, 17, 10, 9, 8, 11, 12, 13, 14, 15, 16,
            -1, -1, 6, 7, 4, 5, 2, 3, 0, 1, -1},
            jointNames = new string[] {
            "LToe", "RToe", "LAnkle", "RAnkle", "LKnee",
            "RKnee", "LHip", "RHip", "Butt", "Abdomen", "Neck",
            "LShldr", "RShldr", "LElbow", "RElbow", "LWrist",
            "RWrist", "Head",},
            nFeet = 2,
            footJoints = new int[] { (int)DroidJoints.LAnkle, (int)DroidJoints.RAnkle },
            canHeadTrack = false
        };

        public static CreatureDefinition overlord = new CreatureDefinition()
        {
            actorType = ActorType.Overlord,
            nTorsos = 3,
            nLimbs = 6,
            nJoints = 37,
            defLengthsName = "Overlord",
            jointParents = new int[] {
            -1, (int)OverlordJoints.Base, (int)OverlordJoints.F1Snout,
            (int)OverlordJoints.F2Snout, (int)OverlordJoints.F3Snout, (int)OverlordJoints.Base,
            (int)OverlordJoints.B1Stem, (int)OverlordJoints.B2Stage, (int)OverlordJoints.B3Stage,
            (int)OverlordJoints.B4Stage, (int)OverlordJoints.B5Stage, (int)OverlordJoints.Base,
            (int)OverlordJoints.R1Shldr, (int)OverlordJoints.R1Elbow, (int)OverlordJoints.R1Wrist,
            (int)OverlordJoints.R1Finger, (int)OverlordJoints.R1App, (int)OverlordJoints.Base,
            (int)OverlordJoints.R2Shldr, (int)OverlordJoints.R2Elbow, (int)OverlordJoints.R2Wrist,
            (int)OverlordJoints.R2Finger, (int)OverlordJoints.R2App, (int)OverlordJoints.Base,
            (int)OverlordJoints.L1Shldr, (int)OverlordJoints.L1Elbow, (int)OverlordJoints.L1Wrist,
            (int)OverlordJoints.L1Finger, (int)OverlordJoints.L1App, (int)OverlordJoints.Base,
            (int)OverlordJoints.L2Shldr, (int)OverlordJoints.L2Elbow, (int)OverlordJoints.L2Wrist,
            (int)OverlordJoints.L2Finger, (int)OverlordJoints.L2App
        },
            jointMap = new int[] {
            -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,},
            jointNames = new string[] {
            "Base", "F1Snout", "F2Snout", "F3Snout", "F4Snout",
            "B1Stem", "B2Stage", "B3Stage", "B4Stage", "B5Stage", "B6Stage",
            "R1Shldr", "R1Elbow", "R1Wrist", "R1Finger", "R1App", "R1Tip",
            "R2Shldr", "R2Elbow", "R2Wrist", "R2Finger", "R2App", "R2Tip",
            "L1Shldr", "L1Elbow", "L1Wrist", "L1Finger", "L1App", "L1Tip",
            "L2Shldr", "L2Elbow", "L2Wrist", "L2Finger", "L2App", "L2Tip",
            "Sac", "LSac"
        },
            nFeet = 0,
            footJoints = null,
            canHeadTrack = false
        };

        public static CreatureDefinition arachnid = new CreatureDefinition()
        {
            actorType = ActorType.Arachnid,
            nTorsos = 4,
            nLimbs = 10,
            nJoints = 40,
            defLengthsName = "aracboss",
            jointParents = new int[] {
            -1, (int)ArachnidJoints.Base, (int)ArachnidJoints.LMAnd,
            (int)ArachnidJoints.Base, (int)ArachnidJoints.RMAnd, (int)ArachnidJoints.Base,
            (int)ArachnidJoints.R1Shldr, (int)ArachnidJoints.R1Elbow, (int)ArachnidJoints.Base,
            (int)ArachnidJoints.R2Shldr, (int)ArachnidJoints.R2Elbow, (int)ArachnidJoints.Base,
            (int)ArachnidJoints.R3Shldr, (int)ArachnidJoints.R3Elbow, (int)ArachnidJoints.Base,
            (int)ArachnidJoints.R4Shldr, (int)ArachnidJoints.R4Elbow, (int)ArachnidJoints.Base,
            (int)ArachnidJoints.L1Shldr, (int)ArachnidJoints.L1Elbow, (int)ArachnidJoints.Base,
            (int)ArachnidJoints.L2Shldr, (int)ArachnidJoints.L2Elbow, (int)ArachnidJoints.Base,
            (int)ArachnidJoints.L3Shldr, (int)ArachnidJoints.L3Elbow, (int)ArachnidJoints.Base,
            (int)ArachnidJoints.L4Shldr, (int)ArachnidJoints.L4Elbow, (int)ArachnidJoints.Base,
            (int)ArachnidJoints.R1Wrist, (int)ArachnidJoints.R2Wrist, (int)ArachnidJoints.R3Wrist, (int)ArachnidJoints.R4Wrist,
            (int)ArachnidJoints.L1Wrist, (int)ArachnidJoints.L2Wrist, (int)ArachnidJoints.L3Wrist, (int)ArachnidJoints.L4Wrist,
            (int)ArachnidJoints.LMElbow, (int)ArachnidJoints.RMElbow, (int)ArachnidJoints.Base
        },
            jointMap = new int[] {
            -1, 0,-1,-1, 0,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,},
            jointNames = new string[] {
            "Base", "LMAnd", "LMElbow", "RMAnd", "RMElbow",
            "R1Shldr", "R1Elbow", "R1Wrist",
            "R2Shldr", "R2Elbow", "R2Wrist",
            "R3Shldr", "R3Elbow", "R3Wrist",
            "R4Shldr", "R4Elbow", "R4Wrist",
            "L1Shldr", "L1Elbow", "L1Wrist",
            "L2Shldr", "L2Elbow", "L2Wrist",
            "L3Shldr", "L3Elbow", "L3Wrist",
            "L4Shldr", "L4Elbow", "L4Wrist",
            "R1Finger", "R2Finger", "R3Finger", "R4Finger",
            "L1Finger", "L2Finger", "L3Finger", "L4Finger",
            "LTip", "RTip", "Sac",
        },
            nFeet = 2,
            footJoints = new int[] { (int)ArachnidJoints.R1Finger, (int)ArachnidJoints.L1Finger },
            canHeadTrack = false
        };

        public static CreatureDefinition monkey = new CreatureDefinition()
        {
            actorType = ActorType.Humanoid,
            nTorsos = 2,
            nLimbs = 5,
            nJoints = 20,
            defLengthsName = "mguard",
            jointParents = new int[] {
            (int)MonkeyJoints.LAnkle, (int)MonkeyJoints.RAnkle, (int)MonkeyJoints.LKnee,
            (int)MonkeyJoints.RKnee, (int)MonkeyJoints.LHip, (int)MonkeyJoints.RHip,
            (int)MonkeyJoints.Butt, (int)MonkeyJoints.Butt, -1,
            (int)MonkeyJoints.Abdomen, (int)MonkeyJoints.Abdomen, (int)MonkeyJoints.Abdomen,
            (int)MonkeyJoints.LShldr, (int)MonkeyJoints.RShldr, (int)MonkeyJoints.LElbow,
            (int)MonkeyJoints.RElbow, (int)MonkeyJoints.LWrist, (int)MonkeyJoints.RWrist,
            (int)MonkeyJoints.Butt, (int)MonkeyJoints.Neck },
            jointMap = new int[] {
            -1, 19, 9, 18, 8, 10, 11, 12, 13, 14, 15, 16,
            17, 6, 7, 4, 5, 2, 3, 0, 1, -1},
            jointNames = new string[] {
            "LToe", "RToe", "LAnkle", "RAnkle", "LKnee",
            "RKnee", "LHip", "RHip", "Butt", "Neck",
            "LShldr", "RShldr", "LElbow", "RElbow", "LWrist",
            "RWrist", "LFinger", "RFinger", "Abdomen", "Head",
            "Tail"},
            nFeet = 2,
            footJoints = new int[] { (int)MonkeyJoints.LAnkle, (int)MonkeyJoints.RAnkle },
            canHeadTrack = true
        };

        public static CreatureDefinition babyArachnid = new CreatureDefinition()
        {
            actorType = ActorType.Arachnid,
            nTorsos = 4,
            nLimbs = 10,
            nJoints = 40,
            defLengthsName = "aracyoun",
            jointParents = new int[] {
            -1, (int)ArachnidJoints.Base, (int)ArachnidJoints.LMAnd,
            (int)ArachnidJoints.Base, (int)ArachnidJoints.RMAnd, (int)ArachnidJoints.Base,
            (int)ArachnidJoints.R1Shldr, (int)ArachnidJoints.R1Elbow, (int)ArachnidJoints.Base,
            (int)ArachnidJoints.R2Shldr, (int)ArachnidJoints.R2Elbow, (int)ArachnidJoints.Base,
            (int)ArachnidJoints.R3Shldr, (int)ArachnidJoints.R3Elbow, (int)ArachnidJoints.Base,
            (int)ArachnidJoints.R4Shldr, (int)ArachnidJoints.R4Elbow, (int)ArachnidJoints.Base,
            (int)ArachnidJoints.L1Shldr, (int)ArachnidJoints.L1Elbow, (int)ArachnidJoints.Base,
            (int)ArachnidJoints.L2Shldr, (int)ArachnidJoints.L2Elbow, (int)ArachnidJoints.Base,
            (int)ArachnidJoints.L3Shldr, (int)ArachnidJoints.L3Elbow, (int)ArachnidJoints.Base,
            (int)ArachnidJoints.L4Shldr, (int)ArachnidJoints.L4Elbow, (int)ArachnidJoints.Base,
            (int)ArachnidJoints.R1Wrist, (int)ArachnidJoints.R2Wrist, (int)ArachnidJoints.R3Wrist, (int)ArachnidJoints.R4Wrist,
            (int)ArachnidJoints.L1Wrist, (int)ArachnidJoints.L2Wrist, (int)ArachnidJoints.L3Wrist, (int)ArachnidJoints.L4Wrist,
            (int)ArachnidJoints.LMElbow, (int)ArachnidJoints.RMElbow, (int)ArachnidJoints.Base
        },
            jointMap = new int[] {
            -1, 0,-1,-1, 0,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,},
            jointNames = new string[] {
            "Base", "LMAnd", "LMElbow", "RMAnd", "RMElbow",
            "R1Shldr", "R1Elbow", "R1Wrist",
            "R2Shldr", "R2Elbow", "R2Wrist",
            "R3Shldr", "R3Elbow", "R3Wrist",
            "R4Shldr", "R4Elbow", "R4Wrist",
            "L1Shldr", "L1Elbow", "L1Wrist",
            "L2Shldr", "L2Elbow", "L2Wrist",
            "L3Shldr", "L3Elbow", "L3Wrist",
            "L4Shldr", "L4Elbow", "L4Wrist",
            "R1Finger", "R2Finger", "R3Finger", "R4Finger",
            "L1Finger", "L2Finger", "L3Finger", "L4Finger",
            "LTip", "RTip", "Sac",
        },
            nFeet = 2,
            footJoints = new int[] { (int)ArachnidJoints.R1Finger, (int)ArachnidJoints.L1Finger },
            canHeadTrack = false
        };

        public static CreatureDefinition shodan = new CreatureDefinition()
        {
            actorType = ActorType.Humanoid,
            nTorsos = 2,
            nLimbs = 5,
            nJoints = 20,
            defLengthsName = "shodan",
            jointParents = new int[] {
            (int)HumanJoints.LAnkle, (int)HumanJoints.RAnkle, (int)HumanJoints.LKnee,
            (int)HumanJoints.RKnee, (int)HumanJoints.LHip, (int)HumanJoints.RHip,
            (int)HumanJoints.Butt, (int)HumanJoints.Butt, -1,
            (int)HumanJoints.Abdomen, (int)HumanJoints.Abdomen, (int)HumanJoints.Abdomen,
            (int)HumanJoints.LShldr, (int)HumanJoints.RShldr, (int)HumanJoints.LElbow,
            (int)HumanJoints.RElbow, (int)HumanJoints.LWrist, (int)HumanJoints.RWrist,
            (int)HumanJoints.Butt, (int)HumanJoints.Neck },
            jointMap = new int[] {
            -1, 19, 9, 18, 8, 10, 11, 12, 13, 14, 15, 16,
            17, 6, 7, 4, 5, 2, 3, 0, 1, -1},
            jointNames = new string[] {
            "LToe", "RToe", "LAnkle", "RAnkle", "LKnee",
            "RKnee", "LHip", "RHip", "Butt", "Neck",
            "LShldr", "RShldr", "LElbow", "RElbow", "LWrist",
            "RWrist", "LFinger", "RFinger", "Abdomen", "Head",
            "LShldrIn", "RShldrIn", "LWeap", "RWeap"},
            nFeet = 2,
            footJoints = new int[] { (int)HumanJoints.LAnkle, (int)HumanJoints.RAnkle },
            canHeadTrack = true
        };

        public static CreatureDefinition[] creatureDefs = new CreatureDefinition[]{
            human,
            wrench,
            avatar,
            rumbler,
            droid,
            overlord,
            arachnid,
            monkey,
            babyArachnid,
            shodan
        };
    }
}
