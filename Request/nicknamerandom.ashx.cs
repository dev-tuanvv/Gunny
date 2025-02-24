using Bussiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Tank.Request
{
    /// <summary>
    /// Summary description for nicknamerandom
    /// </summary>
    public class nicknamerandom : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string name = "unknown";
                XElement result = new XElement("Result");

                int sex = Convert.ToInt32(context.Request["sex"]);
                // get from db
                using (PlayerBussiness db = new PlayerBussiness())
                {
                    name = db.GetSingleRandomName(sex);
                }
                // add a number after name
                Random rand = new Random();
                name += " " + rand.Next(111,999);
                result.Add(new XAttribute("name", name));
                context.Response.ContentType = "text/plain";
                context.Response.Write(result.ToString());
            }
            catch (Exception ex)
            {
                //
            }
            
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