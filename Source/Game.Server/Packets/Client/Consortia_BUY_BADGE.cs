namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameObjects;
    using Game.Server.Managers;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0xa4, "添加敌对")]
    public class Consortia_BUY_BADGE : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int id = packet.ReadInt();
            string msg = "BuyBadgeHandler.Fail";
            bool result = false;
            int validDate = 30;
            string badgeBuyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ConsortiaInfo info = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
            using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
            {
                info.BadgeID = id;
                info.ValidDate = validDate;
                info.BadgeBuyTime = badgeBuyTime;
                if (bussiness.BuyBadge(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, info, ref msg))
                {
                    msg = "BuyBadgeHandler.Success";
                    result = true;
                }
            }
            if (result)
            {
                using (PlayerBussiness bussiness2 = new PlayerBussiness())
                {
                    foreach (ConsortiaUserInfo info2 in bussiness2.GetAllMemberByConsortia(client.Player.PlayerCharacter.ConsortiaID))
                    {
                        GamePlayer playerById = WorldMgr.GetPlayerById(info2.UserID);
                        if ((playerById != null) && (playerById.PlayerId != client.Player.PlayerCharacter.ID))
                        {
                            playerById.UpdateBadgeId(id);
                            playerById.SendMessage("Guild của bạn đ\x00e3 thay đổi huy hiệu mới!");
                            playerById.UpdateProperties();
                        }
                    }
                }
            }
            client.Player.SendMessage(msg);
            client.Out.sendBuyBadge(id, validDate, result, badgeBuyTime, client.Player.PlayerCharacter.ID);
            client.Player.UpdateBadgeId(id);
            client.Player.UpdateProperties();
            return 0;
        }
    }
}

