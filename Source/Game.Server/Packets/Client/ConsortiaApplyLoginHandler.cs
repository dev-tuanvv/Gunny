namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0x81, "申请进入")]
    public class ConsortiaApplyLoginHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID == 0)
            {
                int num = packet.ReadInt();
                bool val = false;
                string msg = "ConsortiaApplyLoginHandler.ADD_Failed";
                using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                {
                    ConsortiaApplyUserInfo info = new ConsortiaApplyUserInfo {
                        ApplyDate = DateTime.Now,
                        ConsortiaID = num,
                        ConsortiaName = "",
                        IsExist = true,
                        Remark = "",
                        UserID = client.Player.PlayerCharacter.ID,
                        UserName = client.Player.PlayerCharacter.NickName
                    };
                    if (bussiness.AddConsortiaApplyUsers(info, ref msg))
                    {
                        msg = (num != 0) ? "ConsortiaApplyLoginHandler.ADD_Success" : "ConsortiaApplyLoginHandler.DELETE_Success";
                        val = true;
                    }
                }
                packet.WriteBoolean(val);
                packet.WriteString(LanguageMgr.GetTranslation(msg, new object[0]));
                client.Out.SendTCP(packet);
            }
            return 0;
        }
    }
}

