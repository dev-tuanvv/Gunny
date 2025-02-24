namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameObjects;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using System;

    [PacketHandler(0x9b, "公会聊天")]
    public class ConsortiaChatHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID != 0)
            {
                if (client.Player.PlayerCharacter.IsBanChat)
                {
                    client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("ConsortiaChatHandler.IsBanChat", new object[0]));
                    return 1;
                }
                packet.ClientID = client.Player.PlayerCharacter.ID;
                packet.ReadByte();
                packet.ReadString();
                packet.ReadString();
                packet.WriteInt(client.Player.PlayerCharacter.ConsortiaID);
                foreach (GamePlayer player in WorldMgr.GetAllPlayers())
                {
                    if (player.PlayerCharacter.ConsortiaID == client.Player.PlayerCharacter.ConsortiaID)
                    {
                        player.Out.SendTCP(packet);
                    }
                }
                GameServer.Instance.LoginServer.SendPacket(packet);
            }
            return 0;
        }
    }
}

