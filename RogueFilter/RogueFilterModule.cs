using RogueFilter.Config;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Web;

namespace RogueFilter
{
    public class RogueFilterModule : IHttpModule
    {
        /// <summary>
        /// You will need to configure this module in the Web.config file of your
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpModule Members

        public void Dispose()
        {
            //clean-up code here.
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += context_BeginRequest;
        }

        #endregion

        void context_BeginRequest(object sender, EventArgs e)
        {
            HttpContext c = HttpContext.Current;

            if (FilterList.ApprovedIPs.Contains(c.Request.UserHostAddress))
            {
                return;
            }
            else if (FilterList.BlockedIPs.Contains(c.Request.UserHostAddress))
            {
                c.Response.StatusCode = 403;
                c.Response.SubStatusCode = 6;
                c.Response.SuppressContent = true;
                c.Response.End();
                return;
            }
            else
            {
                string[] o = c.Request.UserHostAddress.Split(new char[] { '.' });

                if (o.Length != 4)
                {
                    // IPv6
                    return;
                }

                List<IPAddress> addr = new List<IPAddress>();

                foreach (DnsblServer b in FilterList.Blacklists)
                {
                    addr.AddRange(b.GetIPv4BlacklistResults(o));
                }

                if (addr.Count > 0)
                {
                    FilterList.BlockedIPs.Add(c.Request.UserHostAddress);
                }
                else
                {
                    FilterList.ApprovedIPs.Add(c.Request.UserHostAddress);
                }

                return;
            }
        }

        
    }
}
