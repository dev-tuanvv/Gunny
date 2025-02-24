namespace Tank.Request
{
    using System;
    using System.Text;
    using System.Web;
    using System.Web.Services;

    [WebService(Namespace="http://tempuri.org/"), WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
    public class CreateAllXml : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            if (csFunction.ValidAdminIP(context.Request.UserHostAddress))
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(ActiveList.Bulid(context));
                builder.Append(BallList.Bulid(context));
                builder.Append(LoadMapsItems.Bulid(context));
                builder.Append(LoadPVEItems.Build(context));
                builder.Append(QuestList.Bulid(context));
                builder.Append(TemplateAllList.Bulid(context));
                builder.Append(ShopItemList.Bulid(context));
                builder.Append(LoadItemsCategory.Bulid(context));
                builder.Append(ItemStrengthenList.Bulid(context));
                builder.Append(MapServerList.Bulid(context));
                builder.Append(ConsortiaLevelList.Bulid(context));
                builder.Append(DailyAwardList.Bulid(context));
                builder.Append(NPCInfoList.Bulid(context));
                context.Response.ContentType = "text/plain";
                context.Response.Write(builder.ToString());
            }
            else
            {
                context.Response.Write("IP is not valid!");
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

