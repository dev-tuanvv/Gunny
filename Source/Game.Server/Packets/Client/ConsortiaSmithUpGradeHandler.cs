namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameObjects;
    using Game.Server.Managers;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0x9d, "公会铁匠铺升级")]
    public class ConsortiaSmithUpGradeHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID == 0)
            {
                return 0;
            }
            bool val = false;
            string msg = "ConsortiaSmithUpGradeHandler.Failed";
            ConsortiaInfo info = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
            if (info == null)
            {
                msg = "ConsortiaSmithUpGradeHandler.NoConsortia";
            }
            else
            {
                using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                {
                    if (bussiness.UpGradeSmithConsortia(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, ref msg))
                    {
                        info.SmithLevel++;
                        GameServer.Instance.LoginServer.SendConsortiaSmithUpGrade(info);
                        msg = "ConsortiaSmithUpGradeHandler.Success";
                        val = true;
                    }
                }
            }
            if (info.SmithLevel >= 3)
            {
                string translation = LanguageMgr.GetTranslation("ConsortiaSmithUpGradeHandler.Notice", new object[] { client.Player.PlayerCharacter.ConsortiaName, info.SmithLevel });
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

