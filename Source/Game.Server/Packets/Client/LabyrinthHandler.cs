namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0x83, "场景用户离开")]
    public class LabyrinthHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int num = packet.ReadInt();
            UserLabyrinthInfo labyrinth = client.Player.Labyrinth;
            if (labyrinth == null)
            {
                labyrinth = client.Player.LoadLabyrinth();
            }
            int iD = client.Player.PlayerCharacter.ID;
            switch (num)
            {
                case 1:
                {
                    bool flag = packet.ReadBoolean();
                    if (client.Player.PropBag.GetItemByTemplateID(0, 0x2e8c) != null)
                    {
                        if ((flag && !labyrinth.isDoubleAward) && client.Player.RemoveTemplate(0x2e8c, 1))
                        {
                            labyrinth.isDoubleAward = flag;
                        }
                        client.Player.Out.SendLabyrinthUpdataInfo(iD, labyrinth);
                        return 0;
                    }
                    return 0;
                }
                case 2:
                    if (labyrinth.isValidDate())
                    {
                        labyrinth.completeChallenge = true;
                        labyrinth.accumulateExp = 0;
                        labyrinth.isInGame = false;
                        labyrinth.currentFloor = 1;
                        labyrinth.tryAgainComplete = true;
                        labyrinth.LastDate = DateTime.Now;
                        labyrinth.ProcessAward = client.Player.InitProcessAward();
                    }
                    client.Player.CalculatorClearnOutLabyrinth();
                    client.Player.Out.SendLabyrinthUpdataInfo(iD, labyrinth);
                    return 0;

                case 3:
                {
                    int warriorFamRaidDDTPrice = GameProperties.WarriorFamRaidDDTPrice;
                    if (client.Player.PlayerCharacter.GiftToken >= warriorFamRaidDDTPrice)
                    {
                        labyrinth.isCleanOut = true;
                        client.Player.RemoveGiftToken(warriorFamRaidDDTPrice);
                        client.Player.Actives.CleantOutLabyrinth();
                        return 0;
                    }
                    client.Player.SendMessage("Xu kh\x00f3a kh\x00f4ng đủ!");
                    client.Player.Actives.StopCleantOutLabyrinth();
                    return 0;
                }
                case 4:
                    if (labyrinth.isCleanOut)
                    {
                        int num4 = Math.Abs((int) (labyrinth.currentRemainTime / 60));
                        int num5 = GameProperties.WarriorFamRaidPricePerMin * num4;
                        if (client.Player.Extra.UseKingBless(7))
                        {
                            client.Player.SendMessage(string.Format("Ch\x00fac ph\x00fac thần g\x00e0 gi\x00fap bạn tiết kiệm {0}xu.", num5));
                            client.Player.Actives.SpeededUpCleantOutLabyrinth();
                            return 0;
                        }
                        if (client.Player.MoneyDirect(num5))
                        {
                            client.Player.Actives.SpeededUpCleantOutLabyrinth();
                            return 0;
                        }
                        return 0;
                    }
                    client.Player.SendMessage("Tự động chưa k\x00edch hoạt!");
                    return 0;

                case 5:
                    client.Player.Actives.StopCleantOutLabyrinth();
                    return 0;

                case 6:
                    if (!labyrinth.tryAgainComplete)
                    {
                        client.Player.SendMessage("Số lần t\x00e1i lập h\x00f4m nay đ\x00e3 hết!");
                        return 0;
                    }
                    labyrinth.currentFloor = 1;
                    labyrinth.accumulateExp = 0;
                    labyrinth.tryAgainComplete = false;
                    labyrinth.ProcessAward = client.Player.InitProcessAward();
                    client.Player.SendMessage("T\x00e1i lập th\x00e0nh c\x00f4ng!");
                    client.Player.Out.SendLabyrinthUpdataInfo(iD, labyrinth);
                    return 0;

                case 9:
                {
                    bool flag2 = packet.ReadBoolean();
                    packet.ReadBoolean();
                    if (flag2)
                    {
                        int num6 = client.Player.LabyrinthTryAgainMoney();
                        if (client.Player.MoneyDirect(num6))
                        {
                            labyrinth.completeChallenge = true;
                            labyrinth.isInGame = true;
                            client.Player.SendMessage("Thao t\x00e1c th\x00e0nh c\x00f4ng!");
                            return 0;
                        }
                        return 0;
                    }
                    client.Player.SendMessage("T\x00e1i lập th\x00e0nh c\x00f4ng!");
                    return 0;
                }
            }
            Console.WriteLine("LabyrinthPackageType: " + ((LabyrinthPackageType) num));
            return 0;
        }
    }
}

