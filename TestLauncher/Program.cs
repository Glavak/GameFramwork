using GameFramework;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace TestLauncher
{
    internal static class Program
    {
        private static void Main()
        {
            DirectNetworkMessage m = new DirectNetworkMessage(DhtUtils.GeneratePlayerId(), DhtUtils.GeneratePlayerId(), DhtUtils.GeneratePlayerId(), new byte[0]);

            IFormatter formatter = new BinaryFormatter();
            MemoryStream s = new MemoryStream();
            formatter.Serialize(s, m);
        }
    }
}
