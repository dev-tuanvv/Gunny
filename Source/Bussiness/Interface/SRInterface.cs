namespace Bussiness.Interface
{
    using Bussiness.WebLogin;
    using System;

    public class SRInterface : BaseInterface
    {
        public override bool GetUserSex(string name)
        {
            try
            {
                PassPortSoapClient client = new PassPortSoapClient();
                return client.Get_UserSex(string.Empty, name).Value;
            }
            catch (Exception exception)
            {
                BaseInterface.log.Error("获取性别失败", exception);
                return true;
            }
        }
    }
}

