namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameObjects;
    using Game.Server.Rooms;
    using System;

    [PacketHandler(0xfb, "当前场景状态")]
    internal class MarryStateHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            switch (packet.ReadInt())
            {
                case 0:
                    if (!client.Player.IsInMarryRoom)
                    {
                        goto Label_0138;
                    }
                    if (client.Player.MarryMap != 1)
                    {
                        if (client.Player.MarryMap == 2)
                        {
                            client.Player.X = 800;
                            client.Player.Y = 0x2fb;
                        }
                        break;
                    }
                    client.Player.X = 0x286;
                    client.Player.Y = 0x4d9;
                    break;

                case 1:
                    RoomMgr.EnterWaitingRoom(client.Player);
                    goto Label_0138;

                default:
                    goto Label_0138;
            }
            foreach (GamePlayer player in client.Player.CurrentMarryRoom.GetAllPlayers())
            {
                if ((player != client.Player) && (player.MarryMap == client.Player.MarryMap))
                {
                    player.Out.SendPlayerEnterMarryRoom(client.Player);
                    client.Player.Out.SendPlayerEnterMarryRoom(player);
                }
            }
        Label_0138:
            return 0;
        }
    }
}

