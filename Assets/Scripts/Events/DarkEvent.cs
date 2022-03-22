using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Events
{
    public class DarkEvent
    {
        public bool State => state;

        bool state;

        public DarkEvent(bool state)
        {
            this.state = state;
        }
    }
}
