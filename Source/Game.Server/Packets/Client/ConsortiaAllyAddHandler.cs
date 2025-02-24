namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0x93, "添加敌对")]
    public class ConsortiaAllyAddHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID != 0)
            {
                int num = packet.ReadInt();
                bool flag = packet.ReadBoolean();
                bool val = false;
                string msg = "ConsortiaAllyAddHandler.Add_Failed";
                using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                {
                    ConsortiaAllyInfo info = new ConsortiaAllyInfo {
                        Consortia1ID = client.Player.PlayerCharacter.ConsortiaID,
                        Consortia2ID = num,
                        Date = DateTime.Now,
                        IsExist = true,
                        State = 2,
                        ValidDate = 0
                    };
                    if (bussiness.AddConsortiaAlly(info, client.Player.PlayerCharacter.ID, ref msg))
                    {
                        msg = flag ? "ConsortiaAllyAddHandler.Add_Success2" : "ConsortiaAllyAddHandler.Add_Success1";
                        val = true;
                        GameServer.Instance.LoginServer.SendConsortiaAlly(info.Consortia1ID, info.Consortia2ID, info.State);
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

