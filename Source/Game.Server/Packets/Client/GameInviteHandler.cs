namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameObjects;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using System;
    using System.Collections.Generic;

    [PacketHandler(70, "邀请")]
    public class GameInviteHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.CurrentRoom != null)
            {
                GamePlayer playerById = WorldMgr.GetPlayerById(packet.ReadInt());
                if (playerById == client.Player)
                {
                    return 0;
                }
                GSPacketIn @in = new GSPacketIn(70, client.Player.PlayerCharacter.ID);
                List<GamePlayer> players = client.Player.CurrentRoom.GetPlayers();
                foreach (GamePlayer player2 in players)
                {
                    if (player2 == playerById)
                    {
                        client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("friendnotinthesameserver.Sameroom", new object[0]));
                        return 0;
                    }
                }
                if ((playerById != null) && (playerById.CurrentRoom == null))
                {
                    @in.WriteInt(client.Player.PlayerCharacter.ID);
                    @in.WriteInt(client.Player.CurrentRoom.RoomId);
                    @in.WriteInt(client.Player.CurrentRoom.MapId);
                    @in.WriteByte(client.Player.CurrentRoom.TimeMode);
                    @in.WriteByte((byte) client.Player.CurrentRoom.RoomType);
                    @in.WriteByte((byte) client.Player.CurrentRoom.HardLevel);
                    @in.WriteByte((byte) client.Player.CurrentRoom.LevelLimits);
                    @in.WriteString(client.Player.PlayerCharacter.NickName);
                    @in.WriteByte(client.Player.PlayerCharacter.typeVIP);
                    @in.WriteInt(client.Player.PlayerCharacter.VIPLevel);
                    @in.WriteString(client.Player.CurrentRoom.Name);
                    @in.WriteString(client.Player.CurrentRoom.Password);
                    @in.WriteInt(client.Player.CurrentRoom.barrierNum);
                    @in.WriteBoolean(client.Player.CurrentRoom.isOpenBoss);
                    playerById.Out.SendTCP(@in);
                }
                else if (((playerById != null) && (playerById.CurrentRoom != null)) && (playerById.CurrentRoom != client.Player.CurrentRoom))
                {
                    client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("friendnotinthesameserver.Room", new object[0]));
                }
                else
                {
                    client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("friendnotinthesameserver.Fail", new object[0]));
                }
            }
            return 0;
        }
    }
}

