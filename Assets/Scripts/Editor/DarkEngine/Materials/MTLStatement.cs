using UnityEngine;
using System.Collections;
using System;

namespace Assets.Scripts.Editor.DarkEngine.Materials
{
    class MTLStatement
    {
        public string Current
        {
            get
            {
                if (currentPart >= parts.Length)
                    return null;
                return parts[currentPart];
            }
        }

        public string[] Parts => parts;

        string[] parts;
        int currentPart = 0;

        public MTLStatement(string line)
        {
            parts = line.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public bool NextPartMatches(string match)
        {
            if (Current == match)
            {
                currentPart++;
                return true;
            }
            return false;
        }
    }
}
