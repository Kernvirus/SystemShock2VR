using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.Animation
{
    struct MotionMI
    {
        public int type; // virtual or motion cap

        // MotionSig is a bitfield defining which joints the motion affects.
        // If more than 32 joints are needed, this will get more complicated.
        public uint sig;
        public float frameCount;
        public int frameRate;
        public int mot_num;
        public string name;
        public byte app_type;
        public byte[] app_data;

        public MotionMI(BinaryReader reader)
        {
            type = reader.ReadInt32();
            sig = reader.ReadUInt32();
            frameCount = reader.ReadSingle();
            frameRate = reader.ReadInt32();
            mot_num = reader.ReadInt32();
            name = reader.ReadCString(12);
            app_type = reader.ReadByte();
            app_data = reader.ReadBytes(63);
        }
    }

    struct MotionClip
    {
        uint numJoints;
        Vector3[] framesPos;
        Quaternion[,] framesRot;
        int[] parentJoints;

        public MotionClip(BinaryReader reader, int numFrames)
        {
            parentJoints = null;

            numJoints = reader.ReadUInt32();
            uint[] jointOffsets = new uint[numJoints];
            for (int j = 0; j < numJoints; j++)
            {
                jointOffsets[j] = reader.ReadUInt32();
            }

            framesPos = new Vector3[numFrames];
            framesRot = new Quaternion[numJoints - 1, numFrames];

            if (numJoints == 0)
                return;

            reader.BaseStream.Seek(jointOffsets[0], SeekOrigin.Begin);

            for (int f = 0; f < numFrames; f++)
            {
                framesPos[f] = ImporterSettings.modelCoorTransl * reader.ReadVector3();
            }

            for (int j = 1; j < numJoints; j++)
            {
                reader.BaseStream.Seek(jointOffsets[j], SeekOrigin.Begin);
                for (int f = 0; f < numFrames; f++)
                {
                    float w = reader.ReadSingle();
                    float x = reader.ReadSingle();
                    float y = reader.ReadSingle();
                    float z = reader.ReadSingle();
                    var q = new Quaternion(x, y, z, w);
                    //var euler = q.eulerAngles;
                    //var euler = QuaternationHelper.Quaternion2Euler(q, QuaternationHelper.RotSeq.xyz) * Mathf.Rad2Deg;
                    // var euler = QuaternationHelper.MotEditQuaternation2Euler(q) * Mathf.Rad2Deg;
                    // q = Quaternion.AngleAxis(euler.y, Vector3.up) * Quaternion.AngleAxis(euler.x, Vector3.right) * Quaternion.AngleAxis(euler.z, Vector3.forward);
                    // euler.y *= -1;
                    // euler.z *= -1;

                    //euler.y += 180;
                    //euler.z += 180;

                    //float swap = euler.z;
                    //euler.z = euler.x;
                    //euler.x = swap;

                    //q = Quaternion.Euler(euler);
                    //if (j == 1 && f == 0)
                    //    Debug.Log(euler);
                    //q = Quaternion.AngleAxis(euler.z, Vector3.forward) * Quaternion.AngleAxis(euler.y, Vector3.up) * Quaternion.AngleAxis(euler.x, Vector3.right);
                    //q = Quaternion.Euler(euler);
                    //q = Quaternion.AngleAxis(180, Vector3.forward) * Quaternion.AngleAxis(180, Vector3.right) * q;
                    switch (f)
                    {
                        case 0:
                            q = Quaternion.AngleAxis(180, Vector3.forward) * Quaternion.AngleAxis(180, Vector3.up) * q;
                            break;
                        case 1:
                            q = Quaternion.AngleAxis(180, Vector3.up) * Quaternion.AngleAxis(180, Vector3.forward) * q;
                            break;
                        case 2:
                            q = Quaternion.AngleAxis(180, Vector3.forward) * Quaternion.AngleAxis(180, Vector3.right) * q;
                            break;
                        case 3:
                            q = Quaternion.AngleAxis(180, Vector3.right) * Quaternion.AngleAxis(180, Vector3.forward) * q;
                            break;
                        case 4:
                            q = Quaternion.AngleAxis(180, Vector3.up) * Quaternion.AngleAxis(180, Vector3.right) * q;
                            break;
                        case 5:
                            q = Quaternion.AngleAxis(180, Vector3.right) * Quaternion.AngleAxis(180, Vector3.up) * q;
                            break;
                        case 6:
                            q = Quaternion.AngleAxis(180, Vector3.right) * q;
                            break;
                        case 7:
                            q = Quaternion.AngleAxis(180, Vector3.forward) * q;
                            break;
                        case 8:
                            q = Quaternion.AngleAxis(180, Vector3.up) * q;
                            break;
                        case 9:
                            q = Quaternion.Inverse(q);
                            break;
                    }
                    //q = Quaternion.Inverse(q);
                    //q = Quaternion.AngleAxis(180, Vector3.up) * q;
                    //q = Quaternion.AngleAxis(180, Vector3.forward) * q;
                    //if (j == 7)
                    //    framesRot[j - 1, f] = Quaternion.identity;
                    //else
                    framesRot[j - 1, f] = ImporterSettings.globalRotation * q;
                }
            }
        }

        public void AddPositionCurves(AnimationClip clip, float framesPerSecond, CreatureDefinition creature, MpsMotion motion)
        {
            string jointPath = creature.GetJointPath(motion.GetJointId(0));
            Keyframe[] keyframes = new Keyframe[framesPos.Length];
            for (int f = 0; f < framesPos.Length; f++)
            {
                keyframes[f] = new Keyframe(f / framesPerSecond, framesPos[f].x);
            }
            clip.SetCurve(jointPath, typeof(Transform), "localPosition.x", new AnimationCurve(keyframes));

            keyframes = new Keyframe[framesPos.Length];
            for (int f = 0; f < framesPos.Length; f++)
            {
                keyframes[f] = new Keyframe(f / framesPerSecond, framesPos[f].y);
            }
            clip.SetCurve(jointPath, typeof(Transform), "localPosition.y", new AnimationCurve(keyframes));

            keyframes = new Keyframe[framesPos.Length];
            for (int f = 0; f < framesPos.Length; f++)
            {
                keyframes[f] = new Keyframe(f / framesPerSecond, framesPos[f].z);
            }
            clip.SetCurve(jointPath, typeof(Transform), "localPosition.z", new AnimationCurve(keyframes));
        }

        public void AddRotationCurves(AnimationClip clip, float framesPerSecond, CreatureDefinition creature, MpsMotion motion)
        {
            for (int j = 1; j < numJoints; j++)
            {
                Keyframe[] keyframesX = new Keyframe[framesPos.Length];
                Keyframe[] keyframesY = new Keyframe[framesPos.Length];
                Keyframe[] keyframesZ = new Keyframe[framesPos.Length];
                Keyframe[] keyframesW = new Keyframe[framesPos.Length];
                for (int f = 0; f < framesPos.Length; f++)
                {
                    var quat = framesRot[j - 1, f];

                    keyframesX[f] = new Keyframe(f / framesPerSecond, quat.x);
                    keyframesY[f] = new Keyframe(f / framesPerSecond, quat.y);
                    keyframesZ[f] = new Keyframe(f / framesPerSecond, quat.z);
                    keyframesW[f] = new Keyframe(f / framesPerSecond, quat.w);
                }
                string jointPath = creature.GetJointPath(motion.GetJointId(j));

                clip.SetCurve(jointPath, typeof(Transform), "localRotation.x", new AnimationCurve(keyframesX));
                clip.SetCurve(jointPath, typeof(Transform), "localRotation.y", new AnimationCurve(keyframesY));
                clip.SetCurve(jointPath, typeof(Transform), "localRotation.z", new AnimationCurve(keyframesZ));
                clip.SetCurve(jointPath, typeof(Transform), "localRotation.w", new AnimationCurve(keyframesW));
            }
        }

        public void BuildJointTree(CreatureDefinition creature, MpsMotion motion)
        {
            parentJoints = new int[numJoints];
            Dictionary<int, int> jointIdToIndex = new Dictionary<int, int>();
            for (int j = 1; j < numJoints; j++)
            {
                jointIdToIndex.Add(motion.GetJointId(j), j);
            }
            jointIdToIndex.Add(-1, -1);

            for (int j = 1; j < numJoints; j++)
            {
                int parent = creature.GetJointParent(motion.GetJointId(j));

                parentJoints[j] = jointIdToIndex[parent];
            }
            parentJoints[0] = -1;
        }

        private Quaternion CalcLocalToWorldRotation(int joint, int frame)
        {
            if (joint == -1)
                return Quaternion.identity;

            var quat = framesRot[joint - 1, frame];
            return quat * CalcLocalToWorldRotation(parentJoints[joint], frame);
        }

        private string VExactStr(Vector3 q)
        {
            return string.Format("({0}, {1}, {2})", q.x, q.y, q.z);
        }
    }
}
