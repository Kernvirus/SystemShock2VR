using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Editor.DarkEngine.Exceptions
{
    class DarkException : Exception
    {
        public DarkException()
        {
        }

        public DarkException(string message) : base(message)
        {
        }

        public DarkException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
