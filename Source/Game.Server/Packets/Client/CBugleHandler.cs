namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameObjects;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0x49, "大喇叭")]
    public class CBugleHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int templateId = 0x2b5c;
            int clientId = packet.ReadInt();
            SqlDataProvider.Data.ItemInfo itemByTemplateID = client.Player.PropBag.GetItemByTemplateID(0, templateId);
            if (DateTime.Compare(client.Player.LastChatTime.AddSeconds(15.0), DateTime.Now) > 0)
            {
                client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Qu\x00e1 nhiều thao t\x00e1c!", new object[0]));
                return 1;
            }
            GSPacketIn @in = new GSPacketIn(0x49, clientId);
            if (itemByTemplateID != null)
            {
                packet.ReadString();
                string str = packet.ReadString();
                client.Player.PropBag.RemoveCountFromStack(itemByTemplateID, 1);
                @in.WriteInt(client.Player.ZoneId);
                @in.WriteInt(client.Player.PlayerCharacter.ID);
                @in.WriteString(client.Player.PlayerCharacter.NickName);
                @in.WriteString(str);
                @in.WriteString(client.Player.ZoneName);
                GameServer.Instance.LoginServer.SendPacket(@in);
                client.Player.LastChatTime = DateTime.Now;
                foreach (GamePlayer player in WorldMgr.GetAllPlayers())
                {
                    @in.ClientID = player.PlayerCharacter.ID;
                    player.Out.SendTCP(@in);
                }
            }
            return 0;
        }
    }
}

