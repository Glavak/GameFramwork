using GameFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace TestLauncher
{
    class Program
    {
        static void Main(string[] args)
        {
            DirectNetworkMessage m = new DirectNetworkMessage(DhtUtils.GeneratePlayerId(), DhtUtils.GeneratePlayerId(), new byte[0]);

            IFormatter formatter = new BinaryFormatter();
            MemoryStream s = new MemoryStream();
            formatter.Serialize(s, m);
            
        }
    }
}
