namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameObjects;
    using System;

    [PacketHandler(0xe9, "结婚场景切换")]
    internal class MarrySceneChangeHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if ((client.Player.CurrentMarryRoom == null) || (client.Player.MarryMap == 0))
            {
                return 1;
            }
            int num = packet.ReadInt();
            if (num == client.Player.MarryMap)
            {
                return 1;
            }
            GSPacketIn @in = new GSPacketIn(0xf4, client.Player.PlayerCharacter.ID);
            client.Player.CurrentMarryRoom.SendToPlayerExceptSelfForScene(@in, client.Player);
            client.Player.MarryMap = num;
            switch (num)
            {
                case 1:
                    client.Player.X = 0x202;
                    client.Player.Y = 0x27d;
                    break;

                case 2:
                    client.Player.X = 800;
                    client.Player.Y = 0x2fb;
                    break;
            }
            foreach (GamePlayer player in client.Player.CurrentMarryRoom.GetAllPlayers())
            {
                if ((player != client.Player) && (player.MarryMap == client.Player.MarryMap))
                {
                    player.Out.SendPlayerEnterMarryRoom(client.Player);
                    client.Player.Out.SendPlayerEnterMarryRoom(player);
                }
            }
            return 0;
        }
    }
}

