using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGOStat
{
    public class GameStateInitFailedException : Exception
    {
        public GameStateInitFailedException()
        {

        }
        public GameStateInitFailedException(string message)
            : base(message)
        {

        }
        public GameStateInitFailedException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
