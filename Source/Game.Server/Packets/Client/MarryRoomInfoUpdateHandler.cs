namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Packets;
    using Game.Server.SceneMarryRooms;
    using System;

    [PacketHandler(0xfd, "更新礼堂信息")]
    internal class MarryRoomInfoUpdateHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if ((client.Player.CurrentMarryRoom != null) && (client.Player.PlayerCharacter.ID == client.Player.CurrentMarryRoom.Info.PlayerID))
            {
                string str = packet.ReadString();
                bool flag = packet.ReadBoolean();
                string str2 = packet.ReadString();
                string str3 = packet.ReadString();
                MarryRoom currentMarryRoom = client.Player.CurrentMarryRoom;
                currentMarryRoom.Info.RoomIntroduction = str3;
                currentMarryRoom.Info.Name = str;
                if (flag)
                {
                    currentMarryRoom.Info.Pwd = str2;
                }
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    bussiness.UpdateMarryRoomInfo(currentMarryRoom.Info);
                }
                currentMarryRoom.SendMarryRoomInfoUpdateToScenePlayers(currentMarryRoom);
                client.Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("MarryRoomInfoUpdateHandler.Successed", new object[0]));
                return 0;
            }
            return 1;
        }
    }
}

