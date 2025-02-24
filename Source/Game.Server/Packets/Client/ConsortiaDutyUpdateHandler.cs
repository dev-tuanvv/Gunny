namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;
    using System.Text;

    [PacketHandler(0x8b, "更新职务")]
    public class ConsortiaDutyUpdateHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID != 0)
            {
                int val = packet.ReadInt();
                int updateType = packet.ReadByte();
                bool flag = false;
                string msg = "ConsortiaDutyUpdateHandler.Failed";
                using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                {
                    ConsortiaDutyInfo info = new ConsortiaDutyInfo {
                        ConsortiaID = client.Player.PlayerCharacter.ConsortiaID,
                        DutyID = val,
                        IsExist = true,
                        DutyName = ""
                    };
                    switch (updateType)
                    {
                        case 1:
                            return 1;

                        case 2:
                            info.DutyName = packet.ReadString();
                            if (string.IsNullOrEmpty(info.DutyName) || (Encoding.Default.GetByteCount(info.DutyName) > 10))
                            {
                                break;
                            }
                            goto Label_0101;

                        default:
                            goto Label_010F;
                    }
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ConsortiaDutyUpdateHandler.Long", new object[0]));
                    return 1;
                Label_0101:
                    info.Right = packet.ReadInt();
                Label_010F:
                    if (bussiness.UpdateConsortiaDuty(info, client.Player.PlayerCharacter.ID, updateType, ref msg))
                    {
                        val = info.DutyID;
                        msg = "ConsortiaDutyUpdateHandler.Success";
                        flag = true;
                        GameServer.Instance.LoginServer.SendConsortiaDuty(info, updateType, client.Player.PlayerCharacter.ConsortiaID);
                    }
                }
                packet.WriteBoolean(flag);
                packet.WriteInt(val);
                packet.WriteString(LanguageMgr.GetTranslation(msg, new object[0]));
                client.Out.SendTCP(packet);
            }
            return 0;
        }
    }
}

