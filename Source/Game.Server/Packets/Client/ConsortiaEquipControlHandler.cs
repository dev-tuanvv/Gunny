namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(170, "财富控制")]
    public class ConsortiaEquipControlHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID != 0)
            {
                bool val = false;
                string msg = "ConsortiaEquipControlHandler.Fail";
                ConsortiaEquipControlInfo info = new ConsortiaEquipControlInfo {
                    ConsortiaID = client.Player.PlayerCharacter.ConsortiaID
                };
                using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                {
                    for (int i = 0; i < 5; i++)
                    {
                        info.Riches = packet.ReadInt();
                        info.Type = 1;
                        info.Level = i + 1;
                        bussiness.AddAndUpdateConsortiaEuqipControl(info, client.Player.PlayerCharacter.ID, ref msg);
                    }
                    info.Riches = packet.ReadInt();
                    info.Type = 2;
                    info.Level = 0;
                    bussiness.AddAndUpdateConsortiaEuqipControl(info, client.Player.PlayerCharacter.ID, ref msg);
                    msg = "ConsortiaEquipControlHandler.Success";
                    val = true;
                }
                packet.WriteBoolean(val);
                packet.WriteString(LanguageMgr.GetTranslation(msg, new object[0]));
                client.Out.SendTCP(packet);
            }
            return 0;
        }
    }
}

