namespace Tank.Request
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using System.Web.Services;
    using System.Xml.Linq;

    [WebService(Namespace="http://tempuri.org/"), WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
    public class KeyGenerator : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            CspParameters parameters = new CspParameters {
                Flags = CspProviderFlags.UseMachineKeyStore
            };
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider(0x800);
            RSAParameters parameters2 = provider.ExportParameters(true);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < parameters2.Modulus.Length; i++)
            {
                builder.Append(parameters2.Modulus[i].ToString("X2"));
            }
            StringBuilder builder2 = new StringBuilder();
            for (int j = 0; j < parameters2.Exponent.Length; j++)
            {
                builder2.Append(parameters2.Exponent[j].ToString("X2"));
            }
            XElement element = new XElement("list");
            XElement content = new XElement("private", new XAttribute("key", provider.ToXmlString(true)));
            object[] objArray1 = new object[] { new XAttribute("model", builder.ToString()), new XAttribute("exponent", builder2.ToString()) };
            XElement element3 = new XElement("public", objArray1);
            element.Add(content);
            element.Add(element3);
            context.Response.Write(element.ToString());
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

