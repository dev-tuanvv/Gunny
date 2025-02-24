﻿namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0xeb, "获取征婚信息")]
    internal class MarryInfoGetHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.MarryInfoID != 0)
            {
                int iD = packet.ReadInt();
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    MarryInfo marryInfoSingle = bussiness.GetMarryInfoSingle(iD);
                    if (marryInfoSingle != null)
                    {
                        client.Player.Out.SendMarryInfo(client.Player, marryInfoSingle);
                        return 0;
                    }
                }
            }
            return 1;
        }
    }
}

