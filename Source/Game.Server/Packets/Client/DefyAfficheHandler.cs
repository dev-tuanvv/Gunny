namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameObjects;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using System;

    [PacketHandler(0x7b, "场景用户离开")]
    public class DefyAfficheHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            packet.ReadBoolean();
            string str = packet.ReadString();
            int num = 200;
            if (client.Player.PlayerCharacter.Money >= num)
            {
                client.Player.RemoveMoney(num);
                GSPacketIn @in = new GSPacketIn(0x7b);
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
                client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Xu kh\x00f4ng đủ!", new object[0]));
            }
            return 0;
        }
    }
}

