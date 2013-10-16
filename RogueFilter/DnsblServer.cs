using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RogueFilter
{
    public class DnsblServer
    {
        public string Hostname { get; set; }

        public DnsblServer(string hostname)
        {
            Hostname = hostname;
        }

        public IPAddress[] GetIPv4BlacklistResults(string[] octets)
        {
            if (octets.Length != 4)
            {
                throw new Exception("Not a valid IPv4");
            }

            string dnsblLookup = string.Format("{0}.{1}.{2}.{3}.{4}", octets[3], octets[2], octets[1], octets[0], Hostname);

            List<IPAddress> result = new List<IPAddress>();

            try
            {
                result.AddRange(Dns.GetHostAddresses(dnsblLookup));
            }
            catch (SocketException)
            {
            }

            return result.ToArray();
        }
    }
}
