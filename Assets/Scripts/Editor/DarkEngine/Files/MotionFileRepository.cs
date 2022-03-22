using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assets.Scripts.Editor.DarkEngine.Files
{
    class MotionFileRepository : SS2FileRepository
    {
        public string MotionDBPath { get; private set; }

        public MotionFileRepository(string motionDBPath)
        {
            MotionDBPath = motionDBPath;
        }

        protected override IEnumerable<Tuple<string, SS2FileEntry>> DataFiles(ImporterSettings settings)
        {
            return settings.DataFiles(".mi", ".mc").Select(entry =>
            {
                string key = Path.GetFileName(entry.relativePath).ToLower();
                return new Tuple<string, SS2FileEntry>(key, entry);
            });
        }
    }
}
