using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Tank.Request
{
    /// <summary>
    /// Summary description for SavePlayerAction
    /// </summary>
    public class SavePlayerAction : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int selfid = Convert.ToInt32(context.Request["selfid"]);
            string key = context.Request["key"];
            bool value = false;
            string message = "fail!";
            XElement ranks = new XElement("Result");
            
            value = true;
            message = "Success!";
            ranks.Add(new XAttribute("value", value));
            ranks.Add(new XAttribute("message", message));
            context.Response.ContentType = "text/plain";
            context.Response.Write(ranks.ToString());
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}