using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Events
{
    public interface IEventSender
    {
        void AddReceiver(IEventReceiver receiver);
        void RemoveReceiver(IEventReceiver receiver);
    }
}
