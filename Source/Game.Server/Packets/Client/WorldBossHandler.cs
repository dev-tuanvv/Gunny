namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Buffer;
    using Game.Server.Packets;
    using Game.Server.Rooms;
    using System;

    [PacketHandler(0x66, "场景用户离开")]
    public class WorldBossHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            byte num = packet.ReadByte();
            if (!RoomMgr.WorldBossRoom.WorldbossOpen)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Boss thế giới đ\x00e3 kết th\x00fac", new object[0]));
                return 0;
            }
            GSPacketIn @in = new GSPacketIn(0x66, client.Player.PlayerCharacter.ID);
            switch (num)
            {
                case 0x20:
                    @in.WriteByte(2);
                    @in.WriteBoolean(true);
                    @in.WriteBoolean(false);
                    @in.WriteInt(0);
                    @in.WriteInt(0);
                    client.Out.SendTCP(@in);
                    return 0;

                case 0x21:
                    RoomMgr.WorldBossRoom.RemovePlayer(client.Player);
                    client.Player.IsInWorldBossRoom = false;
                    break;

                case 0x22:
                {
                    int num2 = packet.ReadInt();
                    int num3 = packet.ReadInt();
                    client.Player.X = num2;
                    client.Player.Y = num3;
                    if (client.Player.CurrentRoom != null)
                    {
                        client.Player.CurrentRoom.RemovePlayerUnsafe(client.Player);
                    }
                    BaseWorldBossRoom worldBossRoom = RoomMgr.WorldBossRoom;
                    if (client.Player.IsInWorldBossRoom)
                    {
                        @in.WriteByte(4);
                        @in.WriteInt(client.Player.PlayerId);
                        worldBossRoom.SendToALL(@in);
                        worldBossRoom.RemovePlayer(client.Player);
                        client.Player.IsInWorldBossRoom = false;
                    }
                    else if (worldBossRoom.AddPlayer(client.Player))
                    {
                        worldBossRoom.ViewOtherPlayerRoom(client.Player);
                    }
                    break;
                }
                case 0x23:
                {
                    int val = packet.ReadInt();
                    int num5 = packet.ReadInt();
                    string str = packet.ReadString();
                    @in.WriteByte(6);
                    @in.WriteInt(client.Player.PlayerId);
                    @in.WriteInt(val);
                    @in.WriteInt(num5);
                    @in.WriteString(str);
                    client.Player.SendTCP(@in);
                    RoomMgr.WorldBossRoom.SendToALL(@in, client.Player);
                    client.Player.X = val;
                    client.Player.Y = num5;
                    break;
                }
                case 0x24:
                {
                    byte num6 = packet.ReadByte();
                    if ((num6 != 3) || (client.Player.States != 3))
                    {
                        @in.WriteByte(7);
                        @in.WriteInt(client.Player.PlayerId);
                        @in.WriteByte(num6);
                        @in.WriteInt(client.Player.X);
                        @in.WriteInt(client.Player.Y);
                        RoomMgr.WorldBossRoom.SendToALL(@in);
                        if ((num6 == 3) && (client.Player.CurrentRoom.Game != null))
                        {
                            client.Player.CurrentRoom.RemovePlayerUnsafe(client.Player);
                        }
                        string nickName = client.Player.PlayerCharacter.NickName;
                        RoomMgr.WorldBossRoom.SendPrivateInfo(nickName);
                    }
                    client.Player.States = num6;
                    break;
                }
                case 0x25:
                {
                    int num7 = packet.ReadInt();
                    packet.ReadBoolean();
                    int reviveMoney = RoomMgr.WorldBossRoom.reviveMoney;
                    if (num7 == 2)
                    {
                        reviveMoney = RoomMgr.WorldBossRoom.reFightMoney;
                    }
                    if (client.Player.MoneyDirect(reviveMoney))
                    {
                        @in.WriteByte(11);
                        @in.WriteInt(client.Player.PlayerId);
                        RoomMgr.WorldBossRoom.SendToALL(@in);
                    }
                    break;
                }
                case 0x26:
                {
                    int addInjureBuffMoney = RoomMgr.WorldBossRoom.addInjureBuffMoney;
                    int addInjureValue = RoomMgr.WorldBossRoom.addInjureValue;
                    if (client.Player.MoneyDirect(addInjureBuffMoney))
                    {
                        client.Player.RemoveMoney(addInjureBuffMoney);
                        AbstractBuffer buffer = BufferList.CreatePayBuffer(0x193, addInjureValue, 1);
                        if (buffer != null)
                        {
                            buffer.Start(client.Player);
                        }
                    }
                    break;
                }
                default:
                    Console.WriteLine("WorldBossPackageType." + ((WorldBossPackageType) num));
                    break;
            }
            return 0;
        }
    }
}

