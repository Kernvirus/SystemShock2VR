using Assets.Scripts.Editor.DarkEngine.Animation;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Editor.DarkEngine.DarkObjects.DarkProps
{
    class ImmobileProp : BoolProp { }
    class ScaleProp : Vector3Prop { }
    class ModelNameProp : CStringProp { }
    class SymNameProp : VariableLengthStringProp { }
    class ObjNameProp : VariableLengthStringProp { }
    class ObjShortNameProp : VariableLengthStringProp { }
    class NameTypeProp : IntProp { }
    class CreatureProp : EnumProp<CreatureType> { }
    class RenderAlphaProp : FloatProp { }
    class TransDoorProp : DoorProp { }
    class TripFlagsProp : IntProp { }
    class InvDimsProp : Vector2IntProp { }
    class ObjStateProp : EnumProp<ObjState> { }
    class StackCountProp : IntProp { }
    class RenderTypeProp : EnumProp<RenderType> { }
    class ShockWeaponTypeProp : EnumProp<WeaponType> { }
    class MotActorTagsProp : CStringProp { }
    class CollisionTypeProp : EnumProp<CollisionFlags> { }
    class WeaponTypeProp : EnumProp<WeaponType> { }
    class StartLocProp : IntProp { }
    class DestLocProp : IntProp { }
    class DestLevelProp : CStringProp { }
    class HasRefsProp : BoolProp { }
    class DelayTimeProp : FloatProp { }
    class ObjSoundNameProp : CStringProp { }

    class PositionProp : ComplexProp {
        public Vector3 position;
        public short cell;
        public Vector3 facing;

        public override void Load(BinaryReader reader, int propLen)
        {
            position = reader.ReadVector3();
            cell = reader.ReadInt16();
            reader.ReadInt16();
            facing = reader.ReadAngVec();
        }
    }

    class PhysTypeProp : ComplexProp
    {
        public ModelType type;
        public int numSubmodels;
        public bool removeOnSleep;
        public bool special;

        public override void Load(BinaryReader reader, int propLen)
        {
            type = (ModelType)reader.ReadInt32();
            numSubmodels = reader.ReadInt32();
            removeOnSleep = reader.ReadInt32() != 0;
            special = reader.ReadInt32() != 0;
        }
    }

    class PhysStateProp : ComplexProp
    {
        public Vector3 location;
        public Vector3 facing;
        public Vector3 velocity;
        public Vector3 rotVelocity;

        public override void Load(BinaryReader reader, int propLen)
        {
            location = reader.ReadPosition();
            facing = reader.ReadPosition();
            velocity = reader.ReadPosition();
            rotVelocity = reader.ReadVector3();
        }
    }

    class PhysDimsProp : ComplexProp
    {
        public float[] radius;
        public Vector3[] offset;
        public Vector3 size;
        public bool pt_vs_terrain;
        public bool pt_vs_not_special;

        public override void Load(BinaryReader reader, int propLen)
        {
            radius = new float[] {
                reader.ReadScale(),
                reader.ReadScale(),
            };

            offset = new Vector3[] {
                reader.ReadPosition(),
                reader.ReadPosition(),
            };

            size = reader.ReadPosition();
            pt_vs_terrain = reader.ReadInt32() != 0;
            pt_vs_not_special = reader.ReadInt32() != 0;
        }
    }

    class RotDoorProp : DoorProp {
        public bool clockwise;
        public Vector3 baseClosedFacing;
        public Vector3 baseOpenFacing;

        public override void Load(BinaryReader reader, int propLen)
        {
            base.Load(reader, propLen);

            clockwise = reader.ReadInt32() != 0;
            baseClosedFacing = reader.ReadAngVec();
            baseOpenFacing = reader.ReadAngVec();
        }
    }

    class GunStateProp : ComplexProp
    {
        public int ammoCount;
        public float condition; // 0 -> 100 as percent (100 is perfect)
        public int setting;
        public int modification;

        public override void Load(BinaryReader reader, int propLen)
        {
            ammoCount = reader.ReadInt32();
            condition = reader.ReadSingle();
            setting = reader.ReadInt32();
            modification = reader.ReadInt32();
            reader.ReadSingle();
        }
    }

    class GunReliabilityProp : ComplexProp
    {
        public float minBreak; // break chance for condition == threshBreak
        public float maxBreak; // break chance for condition == 0   
        public float degradeRate; // degrade amt per use
        public float threshBreak; // no break at condition above this

        public override void Load(BinaryReader reader, int propLen)
        {
            minBreak = reader.ReadSingle();
            maxBreak = reader.ReadSingle();
            degradeRate = reader.ReadSingle();
            threshBreak = reader.ReadSingle();
            reader.ReadSingle();
        }
    }

    class BaseGunDescriptionProp : ComplexProp
    {
        public int burst; // number of projectiles/trigger pull (over time)
        public int clip;
        public int spray; // number of projectiles/trigger pull (instantaneous)
        public float stimModifier; // multiplier on stimulus intensity of projectile
        public float burstInterval; // interval between bursts
        public float shotInterval; // interval between shots
        public int ammoUsage; // number of ammo units used per shot
        public float speedModifier; // multiplier on projectile speed
        public float reloadTime; // time to reload

        public override void Load(BinaryReader reader, int propLen)
        {
            burst = reader.ReadInt32();
            clip = reader.ReadInt32();
            spray = reader.ReadInt32();
            stimModifier = reader.ReadSingle();
            burstInterval = reader.ReadTime();
            shotInterval = reader.ReadTime();
            ammoUsage = reader.ReadInt32();
            speedModifier = reader.ReadSingle();
            reloadTime = reader.ReadTime();
            reader.ReadSingle();
        }

    }

    class BaseGunDescriptionArrayProp : ComplexProp
    {
        public BaseGunDescriptionProp[] gunDescrs;
        public float[] zooms;

        public override void Load(BinaryReader reader, int propLen)
        {
            int count = (propLen - 1) / 40;
            gunDescrs = new BaseGunDescriptionProp[count];
            zooms = new float[count];
            for (int i = 0; i < count; i++)
            {
                gunDescrs[i] = new BaseGunDescriptionProp();
                gunDescrs[i].Load(reader, 36);
            }
            reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                zooms[i] = reader.ReadSingle();
            }
        }
    }
}
