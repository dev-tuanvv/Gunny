﻿namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Packets;
    using System;

    [PacketHandler(0x2a, "删除物品")]
    public class UserDeleteItemHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
                return 0;
            }
            packet.ReadByte();
            packet.ReadInt();
            return 0;
        }
    }
}

