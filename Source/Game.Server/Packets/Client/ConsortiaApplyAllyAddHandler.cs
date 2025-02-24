namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0x90, "申请同盟")]
    public class ConsortiaApplyAllyAddHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID != 0)
            {
                int num = packet.ReadInt();
                packet.ReadBoolean();
                bool val = false;
                string msg = "ConsortiaApplyAllyAddHandler.Add_Failed";
                using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                {
                    ConsortiaApplyAllyInfo info = new ConsortiaApplyAllyInfo {
                        Consortia1ID = client.Player.PlayerCharacter.ConsortiaID,
                        Consortia2ID = num,
                        Date = DateTime.Now,
                        State = 0,
                        Remark = "",
                        IsExist = true
                    };
                    if (bussiness.AddConsortiaApplyAlly(info, client.Player.PlayerCharacter.ID, ref msg))
                    {
                        msg = "ConsortiaApplyAllyAddHandler.Add_Success";
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

