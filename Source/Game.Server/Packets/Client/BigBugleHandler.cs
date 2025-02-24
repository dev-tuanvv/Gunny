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

    [PacketHandler(0x48, "大喇叭")]
    public class BigBugleHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int templateId = packet.ReadInt();
            SqlDataProvider.Data.ItemInfo itemByTemplateID = client.Player.PropBag.GetItemByTemplateID(0, templateId);
            if (DateTime.Compare(client.Player.LastChatTime.AddSeconds(5.0), DateTime.Now) > 0)
            {
                client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Bố cứ bình tĩnh đợi 5 giây rồi nói ! :))", new object[0]));
                return 1;
            }
            GSPacketIn @in = new GSPacketIn(0x48);
            if (itemByTemplateID != null)
            {
                packet.ReadInt();
                packet.ReadString();
                string str = packet.ReadString();
                client.Player.PropBag.RemoveCountFromStack(itemByTemplateID, 1);
                @in.WriteInt(itemByTemplateID.Template.Property2);
                @in.WriteInt(client.Player.PlayerCharacter.ID);
                @in.WriteString(client.Player.PlayerCharacter.NickName);
                @in.WriteString(str);
                GameServer.Instance.LoginServer.SendPacket(@in);
                client.Player.LastChatTime = DateTime.Now;
                foreach (GamePlayer player in WorldMgr.GetAllPlayers())
                {
                    @in.ClientID = player.PlayerCharacter.ID;
                    player.Out.SendTCP(@in);
                }
            }
            else
            {
                packet.ReadString();
                string str2 = packet.ReadString();
                SqlDataProvider.Data.ItemInfo item = client.Player.PropBag.GetItemByCategoryID(0, 11, 4);
                client.Player.PropBag.RemoveCountFromStack(item, 1);
                @in.WriteInt(client.Player.ZoneId);
                @in.WriteInt(client.Player.PlayerCharacter.ID);
                @in.WriteString(client.Player.PlayerCharacter.NickName);
                @in.WriteString(str2);
                @in.WriteString(client.Player.ZoneName);
                GameServer.Instance.LoginServer.SendPacket(@in);
                client.Player.LastChatTime = DateTime.Now;
                foreach (GamePlayer player2 in WorldMgr.GetAllPlayers())
                {
                    @in.ClientID = player2.PlayerCharacter.ID;
                    player2.Out.SendTCP(@in);
                }
            }
            return 0;
        }
    }
}

