namespace Assets.Scripts.Editor.DarkEngine.LevelFile
{
    struct LevelFileChunk
    {
        public uint offsetPos;
        public uint size;

        public LevelFileChunk(uint offsetPos, uint size)
        {
            this.offsetPos = offsetPos;
            this.size = size;
        }
    }
}
