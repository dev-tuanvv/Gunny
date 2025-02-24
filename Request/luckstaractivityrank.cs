namespace Tank.Request
{
    using Bussiness;
    using Road.Flash;
    using SqlDataProvider.Data;
    using System;
    using System.Web;
    using System.Xml.Linq;

    public class luckstaractivityrank : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            int num = Convert.ToInt32(context.Request["selfid"]);
            string str = context.Request["key"];
            bool flag = false;
            string str2 = "fail!";
            XElement node = new XElement("Ranks");
            LuckstarActivityRankInfo info = new LuckstarActivityRankInfo {
                nickName = ""
            };
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                LuckstarActivityRankInfo[] allLuckstarActivityRank = bussiness.GetAllLuckstarActivityRank();
                foreach (LuckstarActivityRankInfo info2 in allLuckstarActivityRank)
                {
                    node.Add(FlashUtils.LuckstarActivityRank(info2));
                    if (info2.UserID == num)
                    {
                        info = info2;
                    }
                }
            }
            XElement content = new XElement("myRank", new object[] { new XAttribute("rank", info.rank), new XAttribute("useStarNum", info.useStarNum), new XAttribute("nickName", info.nickName) });
            node.Add(content);
            flag = true;
            str2 = "Success!";
            node.Add(new XAttribute("lastUpdateTime", DateTime.Now.ToString("MM-dd hh:mm")));
            node.Add(new XAttribute("value", flag));
            node.Add(new XAttribute("message", str2));
            context.Response.ContentType = "text/plain";
            context.Response.Write(node.ToString(false));
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

