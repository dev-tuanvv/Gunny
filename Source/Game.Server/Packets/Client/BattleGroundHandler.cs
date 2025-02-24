namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using System;

    [PacketHandler(0x84, "场景用户离开")]
    public class BattleGroundHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            byte num = packet.ReadByte();
            int levelLimit = client.Player.BattleData.LevelLimit;
            GSPacketIn @in = new GSPacketIn(0x84, client.Player.PlayerCharacter.ID);
            switch (num)
            {
                case 3:
                {
                    byte val = packet.ReadByte();
                    @in.WriteByte(3);
                    @in.WriteBoolean(true);
                    @in.WriteByte(val);
                    if (val != 1)
                    {
                        if (val == 2)
                        {
                            @in.WriteInt(client.Player.BattleData.GetRank());
                        }
                        break;
                    }
                    if (client.Player.BattleData.MatchInfo == null)
                    {
                        @in.WriteInt(0);
                        @in.WriteInt(0);
                        @in.WriteInt(client.Player.BattleData.fairBattleDayPrestige);
                        break;
                    }
                    @in.WriteInt(client.Player.BattleData.MatchInfo.addDayPrestge);
                    @in.WriteInt(client.Player.BattleData.MatchInfo.totalPrestige);
                    @in.WriteInt(client.Player.BattleData.fairBattleDayPrestige);
                    break;
                }
                case 5:
                    @in.WriteByte(5);
                    @in.WriteInt(client.Player.BattleData.Attack);
                    @in.WriteInt(client.Player.BattleData.Defend);
                    @in.WriteInt(client.Player.BattleData.Agility);
                    @in.WriteInt(client.Player.BattleData.Lucky);
                    @in.WriteInt(client.Player.BattleData.Damage);
                    @in.WriteInt(client.Player.BattleData.Guard);
                    @in.WriteInt(client.Player.BattleData.Blood);
                    @in.WriteInt(client.Player.BattleData.Energy);
                    client.Player.Out.SendTCP(@in);
                    goto Label_022D;

                default:
                    goto Label_022D;
            }
            client.Player.Out.SendTCP(@in);
        Label_022D:
            return 0;
        }
    }
}

