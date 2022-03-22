using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.Models
{
    class Skeleton
    {
        public Matrix4x4[] boneMat;
        Dictionary<uint, int> boneMap;
        int[] parentMap;
        string[] jointNames;

        public Skeleton(BinaryReader reader, string[] jointNames)
        {
            this.jointNames = jointNames;
            CalHdr header = new CalHdr(reader);

            if (header.version != 1)
                throw new Exception("Cal file has version other than 1 : " + header.version);

            CalTorso[] torsos = new CalTorso[header.num_torsos];
            int jointCount = 1;
            for (int i = 0; i < header.num_torsos; i++)
            {
                torsos[i] = new CalTorso(reader);
                jointCount += torsos[i].fixed_count;
            }

            CalLimb[] limbs = new CalLimb[header.num_limbs];
            for (int i = 0; i < header.num_limbs; i++)
            {
                limbs[i] = new CalLimb(reader);
                jointCount += limbs[i].num_segments;
            }

            if (torsos[0].parent != -1)
                throw new Exception("Cal file expected to have torsos in order of creation");

            parentMap = new int[jointCount];

            boneMat = new Matrix4x4[jointCount];
            boneMap = new Dictionary<uint, int>();

            parentMap[torsos[0].joint] = -1;
            CreateBindPose(torsos[0].joint, Vector3.zero);

            for (int i = 0; i < header.num_torsos; i++)
            {
                for (int f = 0; f < torsos[i].fixed_count; f++)
                {
                    parentMap[torsos[i].fixed_joints[f]] = (int)torsos[i].joint;

                    CreateBindPose(torsos[i].fixed_joints[f], torsos[i].fixed_joint_diff_coord[f]);
                }
            }

            // Torsos are processed. Now limbs
            for (int i = 0; i < header.num_limbs; ++i)
            {
                // get the attachment bone
                int parent = (int)limbs[i].attachment_joint;
                for (int s = 0; s < limbs[i].num_segments; ++s)
                {
                    parentMap[limbs[i].segments[s]] = parent;
                    parent = limbs[i].segments[s];

                    CreateBindPose(limbs[i].segments[s], limbs[i].lengths[s] * limbs[i].segment_diff_coord[s]);
                }
            }

            for (int i = 0; i < boneMat.Length; i++)
                boneMat[i] = ImporterSettings.modelCoorTransl * boneMat[i];
        }

        public Transform[] CreateBones(out int rootTransformIndex)
        {
            Transform[] boneObjs = new Transform[boneMat.Length];
            for (int i = 0; i < boneMat.Length; i++)
            {
                boneObjs[i] = new GameObject(jointNames[i]).transform;

                var m = boneMat[i];
                boneObjs[i].localPosition = m.GetPosition();
            }

            // set parents
            rootTransformIndex = -1;
            for (int i = 0; i < boneMat.Length; i++)
            {
                int parent = GetParent(i);
                if (parent == -1)
                {
                    rootTransformIndex = i;
                }
                else
                {

                    boneObjs[i].SetParent(boneObjs[parent], true);
                }
            }
            return boneObjs;
        }

        public int GetBoneIndex(uint slot)
        {
            return (int)slot;
        }

        private void CreateBindPose(uint slot, Vector3 position)
        {
            Matrix4x4 bp = Matrix4x4.Translate(position);
            int parent = parentMap[slot];
            if (parent != -1)
            {
                bp = bp * boneMat[parent];
            }
            boneMap.Add(slot, boneMap.Count);
            boneMat[slot] = bp;
        }

        private int GetParent(int slot)
        {
            return parentMap[slot];
        }

    }
}
