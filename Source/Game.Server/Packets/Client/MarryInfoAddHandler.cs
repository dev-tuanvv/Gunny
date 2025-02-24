namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0xec, "添加征婚信息")]
    public class MarryInfoAddHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.MarryInfoID != 0)
            {
                return 1;
            }
            bool flag = packet.ReadBoolean();
            string str = packet.ReadString();
            int iD = client.Player.PlayerCharacter.ID;
            eMessageType normal = eMessageType.Normal;
            string translateId = "MarryInfoAddHandler.Fail";
            int num2 = 0x2710;
            if (num2 > client.Player.PlayerCharacter.Gold)
            {
                normal = eMessageType.ERROR;
                translateId = "MarryInfoAddHandler.Msg1";
            }
            else
            {
                MarryInfo info = new MarryInfo {
                    UserID = iD,
                    IsPublishEquip = flag,
                    Introduction = str,
                    RegistTime = DateTime.Now
                };
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    if (bussiness.AddMarryInfo(info))
                    {
                        client.Player.RemoveGold(num2);
                        translateId = "MarryInfoAddHandler.Msg2";
                        client.Player.PlayerCharacter.MarryInfoID = info.ID;
                        client.Out.SendMarryInfoRefresh(info, info.ID, true);
                    }
                }
            }
            client.Out.SendMessage(normal, LanguageMgr.GetTranslation(translateId, new object[0]));
            return 0;
        }
    }
}

