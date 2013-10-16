using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueFilter.Config
{
    public class FilterSection : ConfigurationSection
    {
        [ConfigurationProperty("servers", IsRequired = true)]
        public DnsblCollection Servers
        {
            get
            {
                return this["servers"] as DnsblCollection;
            }
        }

        public static FilterSection GetConfig()
        {
            return ConfigurationManager.GetSection("rogueFilter") as FilterSection;
        }
    }

    public class DnsblCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new DnsblElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as DnsblElement).Hostname;
        }
    }

    public class DnsblElement : ConfigurationElement
    {
        [ConfigurationProperty("hostname", IsRequired = false)]
        public string Hostname
        {
            get
            {
                return this["hostname"] as string;
            }
        }
    }
}
