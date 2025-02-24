namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using Game.Server.SceneMarryRooms;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0xf2, "进入礼堂")]
    public class MarryRoomLoginHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            MarryRoom room = null;
            string msg = "";
            int id = packet.ReadInt();
            string str2 = packet.ReadString();
            int num2 = packet.ReadInt();
            if (id != 0)
            {
                room = MarryRoomMgr.GetMarryRoombyID(id, (str2 == null) ? "" : str2, ref msg);
            }
            else
            {
                if (client.Player.PlayerCharacter.IsCreatedMarryRoom)
                {
                    foreach (MarryRoom room2 in MarryRoomMgr.GetAllMarryRoom())
                    {
                        if ((room2.Info.GroomID == client.Player.PlayerCharacter.ID) || (room2.Info.BrideID == client.Player.PlayerCharacter.ID))
                        {
                            room = room2;
                            break;
                        }
                    }
                }
                if ((room == null) && (client.Player.PlayerCharacter.SelfMarryRoomID != 0))
                {
                    client.Player.Out.SendMarryRoomLogin(client.Player, false);
                    MarryRoomInfo marryRoomInfoSingle = null;
                    using (PlayerBussiness bussiness = new PlayerBussiness())
                    {
                        marryRoomInfoSingle = bussiness.GetMarryRoomInfoSingle(client.Player.PlayerCharacter.SelfMarryRoomID);
                    }
                    if (marryRoomInfoSingle != null)
                    {
                        client.Player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryRoomLoginHandler.RoomExist", new object[] { marryRoomInfoSingle.ServerID, client.Player.PlayerCharacter.SelfMarryRoomID }));
                        return 0;
                    }
                }
            }
            if (room != null)
            {
                if (room.CheckUserForbid(client.Player.PlayerCharacter.ID))
                {
                    client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("MarryRoomLoginHandler.Forbid", new object[0]));
                    client.Player.Out.SendMarryRoomLogin(client.Player, false);
                    return 1;
                }
                if (room.RoomState == eRoomState.FREE)
                {
                    if (room.AddPlayer(client.Player))
                    {
                        client.Player.MarryMap = num2;
                        client.Player.Out.SendMarryRoomLogin(client.Player, true);
                        room.SendMarryRoomInfoUpdateToScenePlayers(room);
                        return 0;
                    }
                }
                else
                {
                    client.Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("MarryRoomLoginHandler.AlreadyBegin", new object[0]));
                }
                client.Player.Out.SendMarryRoomLogin(client.Player, false);
            }
            else
            {
                client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation(string.IsNullOrEmpty(msg) ? "MarryRoomLoginHandler.Failed" : msg, new object[0]));
                client.Player.Out.SendMarryRoomLogin(client.Player, false);
            }
            return 1;
        }
    }
}

