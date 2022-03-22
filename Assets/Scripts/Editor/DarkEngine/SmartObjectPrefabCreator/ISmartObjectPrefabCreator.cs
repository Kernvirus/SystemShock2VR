using Assets.Scripts.Editor.DarkEngine.DarkObjects;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Editor.DarkEngine.SmartObjectPrefabCreator
{
    interface ISmartObjectPrefabCreator
    {
        void Initialize(int objCount);
        void ApplyFlags(DarkObject darkObject, HashSet<Type> flags);

        void Preprocess(int index, DarkObject darkObject, DarkObjectCollection collection);
        void Process(int index, DarkObject darkObject, DarkObjectCollection collection);
    }
}
