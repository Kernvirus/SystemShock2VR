using UnityEngine;
using UnityEditor;
using Assets.Scripts.Editor.DarkEngine.Files;

namespace Assets.Scripts.Editor.DarkEngine.Materials
{
    interface IMTLConverter
    {
        bool Match(MTL mtl);

        Material Create(MTL mtl, Color tint, UnitySS2AssetRepository unitySS2AssetRepo);
    }
}