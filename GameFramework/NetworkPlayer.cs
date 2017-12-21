using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework
{
    public class NetworkPlayer : NetworkFile
    {
        /// <summary>
        /// Player's username. Can change any time, not guaranteed to be unique
        /// </summary>
        public string Username { get; }
    }
}
