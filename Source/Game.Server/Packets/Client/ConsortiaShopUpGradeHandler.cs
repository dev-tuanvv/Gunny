namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameObjects;
    using Game.Server.Managers;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0x9e, "公会商城升级")]
    public class ConsortiaShopUpGradeHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID == 0)
            {
                return 0;
            }
            bool val = false;
            string msg = "ConsortiaShopUpGradeHandler.Failed";
            ConsortiaInfo info = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
            if (info == null)
            {
                msg = "ConsortiaShopUpGradeHandler.NoConsortia";
            }
            else
            {
                using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                {
                    if (bussiness.UpGradeShopConsortia(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, ref msg))
                    {
                        info.ShopLevel++;
                        GameServer.Instance.LoginServer.SendConsortiaShopUpGrade(info);
                        msg = "ConsortiaShopUpGradeHandler.Success";
                        val = true;
                    }
                }
            }
            if (info.ShopLevel >= 2)
            {
                string translation = LanguageMgr.GetTranslation("ConsortiaShopUpGradeHandler.Notice", new object[] { client.Player.PlayerCharacter.ConsortiaName, info.ShopLevel });
                GSPacketIn @in = new GSPacketIn(10);
                @in.WriteInt(2);
                GameServer.Instance.LoginServer.SendPacket(@in);
                foreach (GamePlayer player in WorldMgr.GetAllPlayers())
                {
                    if (player != client.Player)
                    {
                        player.Out.SendTCP(@in);
                    }
                }
            }
            packet.WriteBoolean(val);
            packet.WriteString(LanguageMgr.GetTranslation(msg, new object[0]));
            client.Out.SendTCP(packet);
            return 1;
        }
    }
}

