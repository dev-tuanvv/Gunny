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

    [PacketHandler(0x47, "小喇叭")]
    public class SmallBugleHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            SqlDataProvider.Data.ItemInfo item = client.Player.PropBag.GetItemByCategoryID(0, 11, 4);
            if (item != null)
            {
                client.Player.PropBag.RemoveCountFromStack(item, 1);
                packet.ReadInt();
                packet.ReadString();
                string str = packet.ReadString();
                if (DateTime.Compare(client.Player.LastChatTime.AddSeconds(2.0), DateTime.Now) > 0)
                {
                    client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Qu\x00e1 nhiều thao t\x00e1c!", new object[0]));
                    return 1;
                }
                GSPacketIn @in = new GSPacketIn(0x47);
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
            return 0;
        }
    }
}

