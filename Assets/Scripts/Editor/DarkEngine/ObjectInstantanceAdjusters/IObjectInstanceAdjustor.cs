using Assets.Scripts.Editor.DarkEngine.DarkObjects;

namespace Assets.Scripts.Editor.DarkEngine.ObjectInstantanceAdjusters
{
    interface IObjectInstanceAdjustor
    {
        void Process(int index, DarkObject darkObject, DarkObjectCollection collection);
    }
}
