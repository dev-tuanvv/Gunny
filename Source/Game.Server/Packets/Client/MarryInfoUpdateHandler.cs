namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0xd5, "更新征婚信息")]
    public class MarryInfoUpdateHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.MarryInfoID == 0)
            {
                return 1;
            }
            bool flag = packet.ReadBoolean();
            string str = packet.ReadString();
            int marryInfoID = client.Player.PlayerCharacter.MarryInfoID;
            string translateId = "MarryInfoUpdateHandler.Fail";
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                MarryInfo marryInfoSingle = bussiness.GetMarryInfoSingle(marryInfoID);
                if (marryInfoSingle == null)
                {
                    translateId = "MarryInfoUpdateHandler.Msg1";
                }
                else
                {
                    marryInfoSingle.IsPublishEquip = flag;
                    marryInfoSingle.Introduction = str;
                    marryInfoSingle.RegistTime = DateTime.Now;
                    if (bussiness.UpdateMarryInfo(marryInfoSingle))
                    {
                        translateId = "MarryInfoUpdateHandler.Succeed";
                    }
                }
                client.Out.SendMarryInfoRefresh(marryInfoSingle, marryInfoID, marryInfoSingle != null);
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(translateId, new object[0]));
            }
            return 0;
        }
    }
}

