using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Editor.DarkEngine.Exceptions
{
    class InvalidHeaderException : DarkException
    {
        public InvalidHeaderException(string message) : base(message)
        {
        }
    }
}
