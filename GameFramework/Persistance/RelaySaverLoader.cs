using System;
using System.Collections.Generic;
using System.IO;

namespace GameFramework.Persistance
{
    public class FileRelaySaverLoader<TNetworkAddress> : IRelaySaverLoader<TNetworkAddress>
    {
        private readonly string filename;

        public FileRelaySaverLoader(string filename)
        {
            this.filename = filename;
        }

        public bool Load(out IEnumerable<TNetworkAddress> addresses, out IEnumerable<NetworkFile> files)
        {
            try
            {
                using (Stream stream = File.Open(filename, FileMode.Open))
                {
                    var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    addresses = (IEnumerable<TNetworkAddress>)binaryFormatter.Deserialize(stream);
                    files = (IEnumerable<NetworkFile>)binaryFormatter.Deserialize(stream);
                    return true;
                }
            }
            catch (Exception)
            {
                addresses = null;
                files = null;
                return false;
            }
        }

        public void Save(IEnumerable<TNetworkAddress> addresses, IEnumerable<NetworkFile> files)
        {
            using (Stream stream = File.Open(filename, FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, addresses);
                binaryFormatter.Serialize(stream, files);
            }
        }
    }
}
