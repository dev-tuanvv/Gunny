namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Packets;
    using Game.Server.Rooms;
    using System;

    [PacketHandler(0x99, "场景用户离开")]
    public class ConsortiaBattleHander : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            byte num = packet.ReadByte();
            BaseConsBatRoom consBatRoom = RoomMgr.ConsBatRoom;
            int iD = client.Player.PlayerCharacter.ID;
            byte num3 = num;
            switch (num3)
            {
                case 3:
                    consBatRoom.SendUpdateRoom(client.Player);
                    break;

                case 4:
                {
                    int val = packet.ReadInt();
                    int num5 = packet.ReadInt();
                    string str = packet.ReadString();
                    GSPacketIn @in = new GSPacketIn(0x99, iD);
                    @in.WriteByte(4);
                    @in.WriteInt(iD);
                    @in.WriteInt(val);
                    @in.WriteInt(num5);
                    @in.WriteString(str);
                    consBatRoom.PlayerMove(val, num5, iD);
                    consBatRoom.SendToALL(@in);
                    break;
                }
                case 5:
                    consBatRoom.RemovePlayer(client.Player);
                    break;

                case 6:
                {
                    int challengeId = packet.ReadInt();
                    if (client.Player.MainWeapon != null)
                    {
                        consBatRoom.Challenge(iD, challengeId);
                        break;
                    }
                    client.Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip", new object[0]));
                    return 0;
                }
                default:
                    switch (num3)
                    {
                        case 0x10:
                            Console.WriteLine("UPDATE_SCORE. " + packet.ReadByte());
                            goto Label_01C4;

                        case 0x11:
                        {
                            int num8 = packet.ReadInt();
                            packet.ReadBoolean();
                            Console.WriteLine("CONSUME. " + num8);
                            goto Label_01C4;
                        }
                    }
                    if (num3 != 0x15)
                    {
                        Console.WriteLine("ConsortiaBattleType." + ((ConsBatPackageType) num));
                    }
                    else
                    {
                        consBatRoom.SendConfirmEnterRoom(client.Player);
                    }
                    break;
            }
        Label_01C4:
            return 0;
        }
    }
}

