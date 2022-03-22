using Assets.Scripts.Editor.DarkEngine.DarkObjects;
using Assets.Scripts.Editor.DarkEngine.DarkObjects.DarkProps;
using Assets.Scripts.Editor.DarkEngine.Files;
using Assets.Scripts.Editor.DarkEngine.LevelFile;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.Animation
{
    class MotionLoader
    {
        private MotionDB motionDB;
        private MotionSet motionSet;
        private MotionFileRepository motionFileRepo;

        public MotionLoader(MotionFileRepository motionFileRepo)
        {
            this.motionFileRepo = motionFileRepo;
            var file = File.OpenRead(motionFileRepo.MotionDBPath);

            LevelFileGroup group = new LevelFileGroup(file, 0);
            var reader = group.GetReaderAt(group.GetFile("MotDBase"));

            motionSet = new MotionSet();
            motionSet.Load(reader);

            motionDB = new MotionDB();
            motionDB.Load(reader);

            group.Dispose();
        }

        public List<AnimationClip> LoadAllClipsForObject(DarkObject obj)
        {
            CreatureType creType = obj.GetProp<CreatureProp>().Value;
            CreatureDefinition creDef = CreatureDefinitions.Get(creType);
            var creTags = motionDB.ParseTagList(obj.GetProp<MotActorTagsProp>().Value);

            List<AnimationClip> clips = new List<AnimationClip>();
            foreach (var schema in motionDB.ActorSchemasFiltered(creDef.actorType, creTags))
            {
                foreach (var m in schema.motIndexList)
                {
                    var motion = motionSet.mocapList[m];
                    clips.Add(LoadMotionClip(motion.info.name, creDef, motion));
                }
            }
            return clips;
        }

        private AnimationClip LoadMotionClip(string motionName, CreatureDefinition creature, MpsMotion schema)
        {
            var mcReader = motionFileRepo.OpenBinaryReader(motionName + "_.mc");
            var clip = new MotionClip(mcReader, (int)schema.info.numFrames);

            var animClip = new AnimationClip();
            clip.BuildJointTree(creature, schema);
            //clip.AddPositionCurves(animClip, schema.info.freq, creature, schema);
            clip.AddRotationCurves(animClip, schema.info.freq, creature, schema);
            animClip.name = motionName;
            return animClip;
        }
    }
}