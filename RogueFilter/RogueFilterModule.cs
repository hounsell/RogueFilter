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

                string torblLookup = string.Format("{0}.{1}.{2}.{3}.tor.dan.me.uk", o[3], o[2], o[1], o[0]);
                string prxblLookup = string.Format("{0}.{1}.{2}.{3}.proxies.dnsbl.sorbs.net", o[3], o[2], o[1], o[0]);

                List<IPAddress> addr = new List<IPAddress>();

                try
                {
                    addr.AddRange(Dns.GetHostAddresses(torblLookup));
                }
                catch (SocketException)
                {
                }

                try
                {
                    addr.AddRange(Dns.GetHostAddresses(prxblLookup));
                }
                catch (SocketException)
                {
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
