using RogueFilter.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RogueFilter
{
    public static class FilterList
    {
        static FilterList()
        {
            BlockedIPs = new List<string>();
            ApprovedIPs = new List<string>();
            Blacklists = new List<DnsblServer>();

            foreach (DnsblElement el in FilterSection.GetConfig().Servers)
            {
                Blacklists.Add(new DnsblServer(el.Hostname));
            }
        }

        public static List<string> BlockedIPs { get; set; }
        public static List<string> ApprovedIPs { get; set; }
        public static List<DnsblServer> Blacklists { get; set; }
    }
}
