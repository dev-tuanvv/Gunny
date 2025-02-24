namespace Tank.Request
{
    using System;
    using System.Configuration;
    using System.Security.Cryptography;
    using System.Web;
    using System.Web.SessionState;
    using System.Xml.Linq;

    public class GetSID : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            CspParameters parameters = new CspParameters {
                Flags = CspProviderFlags.UseMachineKeyStore
            };
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider(0x800);
            provider.FromXmlString(ConfigurationSettings.AppSettings["privateKey"]);
            RSAParameters parameters2 = provider.ExportParameters(false);
            object[] content = new object[] { new XAttribute("m1", Convert.ToBase64String(parameters2.Modulus)), new XAttribute("m2", Convert.ToBase64String(parameters2.Exponent)) };
            XElement element = new XElement("result", content);
            context.Response.ContentType = "text/plain";
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

