using System.Collections.Generic;
using System.Net;

namespace DNS.Server
{
    public class MasterFile
    {
        private readonly Dictionary<string, IPAddress> records =
            new Dictionary<string, IPAddress>(System.StringComparer.OrdinalIgnoreCase);

        public void AddIPAddressResourceRecord(string name, string ip)
        {
            records[name] = IPAddress.Parse(ip);
        }

        public bool TryResolve(string name, out IPAddress address) =>
            records.TryGetValue(name, out address);
    }
}