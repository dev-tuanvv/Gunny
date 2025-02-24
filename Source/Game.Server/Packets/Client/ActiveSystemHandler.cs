namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Server;
    using Game.Server.Buffer;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using Game.Server.Rooms;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [PacketHandler(0x91, "场景用户离开")]
    public class ActiveSystemHandler : IPacketHandler
    {
        public static ThreadSafeRandom random = new ThreadSafeRandom();

        private bool CanGetGift(int damageNum, int id, string[] YearMonsterBoxInfo)
        {
            if ((id > 4) || (id < 0))
            {
                return false;
            }
            int num = int.Parse(YearMonsterBoxInfo[id].Split(new char[] { ',' })[1]) * 0x2710;
            return (num <= damageNum);
        }

        private string[] GetLayerItems(string[] lists, int layer)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < lists.Length; i++)
            {
                string item = lists[i];
                string str2 = item.Split(new char[] { '-' })[0];
                if (str2 == layer.ToString())
                {
                    list.Add(item);
                }
            }
            return list.ToArray();
        }

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            byte num = packet.ReadByte();
            GSPacketIn pkg = new GSPacketIn(0x91, client.Player.PlayerCharacter.ID);
            BaseChristmasRoom christmasRoom = RoomMgr.ChristmasRoom;
            UserChristmasInfo christmas = client.Player.Actives.Christmas;
            bool flag = false;
            if (num <= 10)
            {
                flag = client.Player.Actives.LoadPyramid();
            }
            PyramidInfo pyramid = client.Player.Actives.Pyramid;
            UserBoguAdventureInfo boguAdventure = client.Player.Actives.BoguAdventure;
            string title = "Event Noel";
            byte num2 = num;
            if (num2 > 0x31)
            {
                switch (num2)
                {
                    case 0x4b:
                        this.SendDDPlayInfo(client);
                        return 0;

                    case 0x4c:
                        pkg = new GSPacketIn(0x91);
                        pkg.WriteByte(0x4c);
                        if (client.Player.PropBag.GetItemCount(0x3125e) > 0)
                        {
                            client.Player.PropBag.RemoveTemplate(0x3125e, 1);
                            PlayerInfo playerCharacter = client.Player.PlayerCharacter;
                            playerCharacter.DDPlayPoint += 10;
                            int num54 = random.Next(1, 50);
                            int[] source = new int[] { 2, 3, 5, 10 };
                            if (!source.Contains<int>(num54))
                            {
                                num54 = 0;
                            }
                            pkg.WriteInt(num54);
                            pkg.WriteInt(client.Player.PlayerCharacter.DDPlayPoint);
                            client.Player.SendTCP(pkg);
                            if (num54 > 0)
                            {
                                int money = GameProperties.DDPlayActivityMoney * num54;
                                using (PlayerBussiness bussiness = new PlayerBussiness())
                                {
                                    client.Player.SendMoneyMailToUser(bussiness, "Ch\x00fac mừng bạn được x" + num54 + " Xu từ sự kiện T\x00ecm H\x00ecnh.", "Xu tr\x00fang thưởng event T\x00ecm h\x00ecnh", money, eMailType.BuyItem);
                                }
                                GSPacketIn in2 = WorldMgr.SendSysNotice(string.Format("Người chơi [{0}] trong sự kiện \"T\x00ecm h\x00ecnh\" đ\x00e3 may mắn nhận được X{1} Xu!", client.Player.PlayerCharacter.NickName, num54));
                                GameServer.Instance.LoginServer.SendPacket(in2);
                                return 0;
                            }
                            return 0;
                        }
                        client.Player.SendMessage("Bạn kh\x00f4ng đủ Xu Vui Vẻ để quay!");
                        return 0;

                    case 0x4d:
                    {
                        int count = packet.ReadInt();
                        if ((count > 0) && (count <= 0x3e7))
                        {
                            int num57 = count * 100;
                            if (client.Player.PlayerCharacter.DDPlayPoint >= num57)
                            {
                                client.Player.RemoveDDPlayPoint(num57);
                                using (new PlayerBussiness())
                                {
                                    SqlDataProvider.Data.ItemInfo info11 = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(0x3125e), count, 0x69);
                                    info11.IsBinds = true;
                                    List<SqlDataProvider.Data.ItemInfo> items = new List<SqlDataProvider.Data.ItemInfo> {
                                        info11
                                    };
                                    client.Player.SendItemsToMail(items, "Bạn nhận được " + count + " Xu Vui Vẻ từ đổi điểm.", "Đổi Xu Vui vẻ", eMailType.BuyItem);
                                }
                                client.Player.SendMessage("Đổi th\x00e0nh c\x00f4ng! Vật phẩm gửi về thư.");
                                this.SendDDPlayInfo(client);
                                return 0;
                            }
                            client.Player.SendMessage("Bạn kh\x00f4ng đủ điểm để đổi");
                            return 0;
                        }
                        return 0;
                    }
                    case 90:
                        this.SendEnterBoguAdventure(client, packet);
                        return 0;

                    case 0x5b:
                    {
                        int val = packet.ReadInt();
                        int index = 0;
                        if (val != 3)
                        {
                            index = packet.ReadInt();
                        }
                        BoguCeilInfo ceilInfo = client.Player.Actives.FindCeilBoguMap(index);
                        int num60 = -2;
                        switch (val)
                        {
                            case 1:
                                if ((ceilInfo != null) && (ceilInfo.State == 3))
                                {
                                    ceilInfo.State = 1;
                                    client.Player.Actives.UpdateCeilBoguMap(ceilInfo);
                                }
                                break;

                            case 2:
                                if ((ceilInfo != null) && (ceilInfo.State == 1))
                                {
                                    ceilInfo.State = 3;
                                    client.Player.Actives.UpdateCeilBoguMap(ceilInfo);
                                }
                                break;

                            case 3:
                                if (((boguAdventure.HP > 0) && (boguAdventure.CurrentPostion > 0)) && (client.Player.PlayerCharacter.Money >= client.Player.Actives.BoguAdventureMoney[0]))
                                {
                                    BoguCeilInfo[] totalMineAroundNotOpen = client.Player.Actives.GetTotalMineAroundNotOpen(boguAdventure.CurrentPostion);
                                    if (totalMineAroundNotOpen.Length > 0)
                                    {
                                        int num61 = random.Next(0, totalMineAroundNotOpen.Length - 1);
                                        ceilInfo = totalMineAroundNotOpen[num61];
                                        if (ceilInfo != null)
                                        {
                                            client.Player.RemoveMoney(client.Player.Actives.BoguAdventureMoney[0]);
                                            ceilInfo.State = 2;
                                            client.Player.Actives.UpdateCeilBoguMap(ceilInfo);
                                            index = ceilInfo.Index;
                                            num60 = -1;
                                        }
                                    }
                                    else
                                    {
                                        client.Player.SendMessage("Bạn đ\x00e3 mở hết bom xung quanh bạn!");
                                    }
                                }
                                else
                                {
                                    client.Player.SendMessage("Hiện kh\x00f4ng c\x00f3 bom xung quanh chưa mở!");
                                }
                                break;

                            case 4:
                                if (ceilInfo != null)
                                {
                                    if ((ceilInfo.State == 3) && (boguAdventure.HP > 0))
                                    {
                                        if (ceilInfo.Result != -1)
                                        {
                                            boguAdventure.OpenCount++;
                                        }
                                        else
                                        {
                                            boguAdventure.HP--;
                                            num60 = -1;
                                        }
                                        ceilInfo.State = 2;
                                        client.Player.Actives.UpdateCeilBoguMap(ceilInfo);
                                    }
                                    boguAdventure.CurrentPostion = ceilInfo.Index;
                                }
                                break;
                        }
                        pkg = new GSPacketIn(0x91);
                        pkg.WriteByte(0x5b);
                        pkg.WriteInt(val);
                        pkg.WriteInt(index);
                        pkg.WriteInt(num60);
                        pkg.WriteInt(client.Player.Actives.BoguAdventureMoney[0]);
                        if (val == 4)
                        {
                            pkg.WriteInt(boguAdventure.HP);
                            pkg.WriteInt(boguAdventure.OpenCount);
                        }
                        client.Player.SendTCP(pkg);
                        return 0;
                    }
                    case 0x5c:
                    {
                        int num62 = packet.ReadInt();
                        pkg = new GSPacketIn(0x91);
                        if (num62 != 1)
                        {
                            if (((boguAdventure.CurrentPostion > 0) && (client.Player.PlayerCharacter.Money >= client.Player.Actives.BoguAdventureMoney[2])) && (boguAdventure.ResetCount > 0))
                            {
                                if (((boguAdventure.GetAward()[0] == "1") && (boguAdventure.GetAward()[1] == "1")) && (boguAdventure.GetAward()[2] == "1"))
                                {
                                    client.Player.SendMessage("Bạn đ\x00e3 ho\x00e0n tất cuộc h\x00e0nh tr\x00ecnh v\x00e0o ng\x00e0y h\x00f4m nay.");
                                }
                                else
                                {
                                    client.Player.RemoveMoney(client.Player.Actives.BoguAdventureMoney[1]);
                                    boguAdventure.ResetCount--;
                                    client.Player.Actives.ResetBoguAdventureInfo();
                                }
                            }
                            else
                            {
                                client.Player.SendMessage("Xu của bạn kh\x00f4ng đủ");
                            }
                            this.SendEnterBoguAdventure(client, packet);
                            return 0;
                        }
                        if ((boguAdventure.HP > 0) || (client.Player.PlayerCharacter.Money < client.Player.Actives.BoguAdventureMoney[1]))
                        {
                            client.Player.SendMessage("Xu của bạn kh\x00f4ng đủ");
                        }
                        else
                        {
                            client.Player.RemoveMoney(client.Player.Actives.BoguAdventureMoney[1]);
                            boguAdventure.HP = 2;
                        }
                        pkg.WriteByte(0x5c);
                        pkg.WriteInt(boguAdventure.HP);
                        client.Player.SendTCP(pkg);
                        return 0;
                    }
                    case 0x5d:
                    {
                        int num63 = packet.ReadInt();
                        if (!(boguAdventure.GetAward()[num63] == "0"))
                        {
                            client.Player.SendMessage("Bạn đ\x00e3 nhận hộp qu\x00e0 n\x00e0y.");
                        }
                        else
                        {
                            boguAdventure.SetAward(num63, 1);
                            List<EventAwardInfo> boGuBoxAward = EventAwardMgr.GetBoGuBoxAward(num63);
                            List<SqlDataProvider.Data.ItemInfo> list6 = new List<SqlDataProvider.Data.ItemInfo>();
                            List<string> list7 = new List<string>();
                            foreach (EventAwardInfo info13 in boGuBoxAward)
                            {
                                SqlDataProvider.Data.ItemInfo cloneItem = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(info13.TemplateID), info13.Count, 0x69);
                                cloneItem.AttackCompose = info13.AttackCompose;
                                cloneItem.DefendCompose = info13.DefendCompose;
                                cloneItem.AgilityCompose = info13.AgilityCompose;
                                cloneItem.LuckCompose = info13.LuckCompose;
                                cloneItem.IsBinds = info13.IsBinds;
                                cloneItem.ValidDate = info13.ValidDate;
                                if (!client.Player.AddTemplate(cloneItem))
                                {
                                    list6.Add(cloneItem);
                                }
                                list7.Add(cloneItem.Template.Name + "x" + cloneItem.Count);
                            }
                            if (list6.Count > 0)
                            {
                                client.Player.SendItemsToMail(list6, "Qu\x00e0 từ event g\x00e0 con mạo hiểm gửi về thư do t\x00fai bạn đầy", "Qu\x00e0 event G\x00e0 Con Mạo Hiểm", eMailType.BuyItem);
                            }
                            if (list7.Count > 0)
                            {
                                client.Player.SendMessage("Bạn nhận được " + string.Join(", ", list7.ToArray()));
                            }
                            if (((boguAdventure.GetAward()[0] == "1") && (boguAdventure.GetAward()[1] == "1")) && (boguAdventure.GetAward()[2] == "1"))
                            {
                                GameServer.Instance.LoginServer.SendPacket(WorldMgr.SendSysNotice(string.Format("Người chơi [{0}] trong sự kiện \"G\x00e0 con mạo hiểm\" đ\x00e3 ho\x00e0n th\x00e0nh xuất sắc cuộc h\x00e0nh tr\x00ecnh đầy nguy hiểm của ng\x00e0y h\x00f4m nay!", client.Player.PlayerCharacter.NickName)));
                            }
                        }
                        pkg = new GSPacketIn(0x91);
                        pkg.WriteByte(0x5d);
                        pkg.WriteInt(int.Parse(boguAdventure.GetAward()[0]));
                        pkg.WriteInt(int.Parse(boguAdventure.GetAward()[1]));
                        pkg.WriteInt(int.Parse(boguAdventure.GetAward()[2]));
                        client.Player.SendTCP(pkg);
                        return 0;
                    }
                    case 0x5e:
                        client.Player.Actives.SaveToDatabase();
                        return 0;
                }
            }
            else
            {
                switch (num2)
                {
                    case 1:
                        if (!flag || (pyramid == null))
                        {
                            client.Player.SendMessage("Tải dữ liệu thất bại. Vui l\x00f2ng thử lại sau.");
                            return 0;
                        }
                        pkg.WriteByte(1);
                        pkg.WriteInt(pyramid.currentLayer);
                        pkg.WriteInt(pyramid.maxLayer);
                        pkg.WriteInt(pyramid.totalPoint);
                        pkg.WriteInt(pyramid.turnPoint);
                        pkg.WriteInt(pyramid.pointRatio);
                        pkg.WriteInt(pyramid.currentFreeCount);
                        pkg.WriteInt(pyramid.currentReviveCount);
                        pkg.WriteBoolean(pyramid.isPyramidStart);
                        if (pyramid.isPyramidStart)
                        {
                            string[] lists = pyramid.LayerItems.Split(new char[] { '|' });
                            int[] numArray = new int[] { 8, 7, 6, 5, 4, 3, 2 };
                            pkg.WriteInt(numArray.Length);
                            for (int i = 1; i <= numArray.Length; i++)
                            {
                                string[] layerItems = this.GetLayerItems(lists, i);
                                pkg.WriteInt(i);
                                pkg.WriteInt(layerItems.Length);
                                for (int j = 0; j < layerItems.Length; j++)
                                {
                                    int num5 = int.Parse(layerItems[j].Split(new char[] { '-' })[1]);
                                    int num6 = int.Parse(layerItems[j].Split(new char[] { '-' })[2]);
                                    pkg.WriteInt(num5);
                                    pkg.WriteInt(num6);
                                }
                            }
                        }
                        break;

                    case 2:
                    {
                        bool flag2 = packet.ReadBoolean();
                        pyramid.isPyramidStart = flag2;
                        pkg.WriteByte(2);
                        pkg.WriteBoolean(flag2);
                        if (!flag2)
                        {
                            pyramid.totalPoint += (pyramid.totalPoint * pyramid.pointRatio) / 100;
                            pyramid.totalPoint += pyramid.turnPoint;
                            pyramid.turnPoint = 0;
                            pyramid.pointRatio = 0;
                            pyramid.currentLayer = 1;
                            pyramid.currentReviveCount = 0;
                            pyramid.LayerItems = "";
                            pkg.WriteInt(pyramid.totalPoint);
                            pkg.WriteInt(pyramid.turnPoint);
                            pkg.WriteInt(pyramid.pointRatio);
                            pkg.WriteInt(pyramid.currentLayer);
                        }
                        client.Player.SendTCP(pkg);
                        return 0;
                    }
                    case 3:
                    {
                        string str2;
                        int layer = packet.ReadInt();
                        int num8 = packet.ReadInt();
                        if (pyramid.currentFreeCount >= client.Player.Actives.PyramidConfig.freeCount)
                        {
                            int turnCardPrice = client.Player.Actives.PyramidConfig.turnCardPrice;
                            if (!client.Player.MoneyDirect(turnCardPrice))
                            {
                                return 1;
                            }
                        }
                        else
                        {
                            pyramid.currentFreeCount++;
                        }
                        bool flag3 = true;
                        if (layer >= 8)
                        {
                            int num12 = random.Next(0x31, 0x1f5);
                            pyramid.turnPoint += num12;
                            str2 = string.Format("Thật may bạn nhận th\x00eam {0} điểm t\x00edch lũy.", num12);
                            pyramid.isPyramidStart = false;
                            pyramid.currentLayer = 1;
                            pyramid.currentReviveCount = 0;
                            pyramid.totalPoint += (pyramid.totalPoint * pyramid.pointRatio) / 100;
                            pyramid.totalPoint += pyramid.turnPoint;
                            pyramid.turnPoint = 0;
                            pyramid.pointRatio = 0;
                            pkg.WriteByte(2);
                            pkg.WriteBoolean(pyramid.isPyramidStart);
                            pkg.WriteInt(pyramid.totalPoint);
                            pkg.WriteInt(pyramid.turnPoint);
                            pkg.WriteInt(pyramid.pointRatio);
                            pkg.WriteInt(pyramid.currentLayer);
                            client.Player.SendTCP(pkg);
                        }
                        else
                        {
                            List<SqlDataProvider.Data.ItemInfo> pyramidAward = ActiveSystemMgr.GetPyramidAward(layer);
                            int num10 = random.Next(pyramidAward.Count);
                            SqlDataProvider.Data.ItemInfo info4 = pyramidAward[num10];
                            int templateID = info4.TemplateID;
                            bool flag4 = templateID == 0x3117b;
                            bool flag5 = templateID == 0x3117a;
                            str2 = string.Format("Bạn nhận được {0} x{1}.", info4.Template.Name, info4.Count);
                            if (flag5)
                            {
                                str2 = "Thật may bạn được l\x00ean tầng tiếp theo.";
                                pyramid.currentLayer++;
                                if (pyramid.currentLayer > pyramid.maxLayer)
                                {
                                    pyramid.maxLayer++;
                                }
                                flag3 = false;
                            }
                            pyramid.totalPoint += 10;
                            switch (templateID)
                            {
                                case 0x31175:
                                    pyramid.pointRatio += 5;
                                    str2 = "Thật may bạn nhận th\x00eam 5% điểm t\x00edch lũy.";
                                    flag3 = false;
                                    break;

                                case 0x31176:
                                    pyramid.pointRatio += 10;
                                    str2 = "Thật may bạn nhận th\x00eam 10% điểm t\x00edch lũy.";
                                    flag3 = false;
                                    break;

                                case 0x31177:
                                    pyramid.turnPoint += 10;
                                    str2 = "Thật may bạn nhận th\x00eam 10 điểm t\x00edch lũy.";
                                    flag3 = false;
                                    break;

                                case 0x31178:
                                    pyramid.turnPoint += 30;
                                    str2 = "Thật may bạn nhận th\x00eam 30 điểm t\x00edch lũy.";
                                    flag3 = false;
                                    break;

                                case 0x31179:
                                    pyramid.turnPoint += 50;
                                    str2 = "Thật may bạn nhận th\x00eam 40 điểm t\x00edch lũy.";
                                    flag3 = false;
                                    break;
                            }
                            if (flag3)
                            {
                                client.Player.AddTemplate(info4, "Kim tự th\x00e1p");
                            }
                            string str3 = string.Format("{0}-{1}-{2}", layer, templateID, num8);
                            if (pyramid.LayerItems == "")
                            {
                                pyramid.LayerItems = str3;
                            }
                            else
                            {
                                PyramidInfo info5 = pyramid;
                                info5.LayerItems = info5.LayerItems + "|" + str3;
                            }
                            pkg.WriteByte(3);
                            pkg.WriteInt(templateID);
                            pkg.WriteInt(num8);
                            pkg.WriteBoolean(flag4);
                            pkg.WriteBoolean(flag5);
                            pkg.WriteInt(pyramid.currentLayer);
                            pkg.WriteInt(pyramid.maxLayer);
                            pkg.WriteInt(pyramid.totalPoint);
                            pkg.WriteInt(pyramid.turnPoint);
                            pkg.WriteInt(pyramid.pointRatio);
                            pkg.WriteInt(pyramid.currentFreeCount);
                            client.Player.SendTCP(pkg);
                        }
                        client.Player.SendMessage(str2);
                        return 0;
                    }
                    case 4:
                        if (packet.ReadBoolean())
                        {
                            int num13 = client.Player.Actives.PyramidConfig.revivePrice[pyramid.currentReviveCount];
                            if (!client.Player.MoneyDirect(num13))
                            {
                                return 1;
                            }
                            pyramid.currentReviveCount++;
                            pkg.WriteByte(4);
                            pkg.WriteBoolean(pyramid.isPyramidStart);
                            pkg.WriteInt(pyramid.currentLayer);
                            pkg.WriteInt(pyramid.totalPoint);
                            pkg.WriteInt(pyramid.turnPoint);
                            pkg.WriteInt(pyramid.pointRatio);
                            pkg.WriteInt(pyramid.currentReviveCount);
                            client.Player.SendTCP(pkg);
                            return 0;
                        }
                        pyramid.isPyramidStart = false;
                        pyramid.currentLayer = 1;
                        pyramid.currentReviveCount = 0;
                        pyramid.totalPoint += (pyramid.totalPoint * pyramid.pointRatio) / 100;
                        pyramid.totalPoint += pyramid.turnPoint;
                        pyramid.turnPoint = 0;
                        pyramid.pointRatio = 0;
                        pyramid.LayerItems = "";
                        pkg.WriteByte(4);
                        pkg.WriteBoolean(pyramid.isPyramidStart);
                        pkg.WriteInt(pyramid.currentLayer);
                        pkg.WriteInt(pyramid.totalPoint);
                        pkg.WriteInt(pyramid.turnPoint);
                        pkg.WriteInt(pyramid.pointRatio);
                        pkg.WriteInt(pyramid.currentReviveCount);
                        client.Player.SendTCP(pkg);
                        return 0;

                    case 5:
                    case 6:
                    case 7:
                        goto Label_2768;

                    case 8:
                        pkg.WriteByte(8);
                        pkg.WriteInt(1);
                        return 0;

                    default:
                        switch (num2)
                        {
                            case 0x11:
                            {
                                byte num14 = packet.ReadByte();
                                if (num14 != 2)
                                {
                                    switch (num14)
                                    {
                                        case 0:
                                        {
                                            client.Player.X = christmasRoom.DefaultPosX;
                                            client.Player.Y = christmasRoom.DefaultPosY;
                                            christmasRoom.AddPlayer(client.Player);
                                            int christmasMinute = GameProperties.ChristmasMinute;
                                            if (!christmas.isEnter)
                                            {
                                                christmas.gameBeginTime = DateTime.Now;
                                                christmas.gameEndTime = DateTime.Now.AddMinutes((double) christmasMinute);
                                                christmas.isEnter = true;
                                                christmas.AvailTime = christmasMinute;
                                            }
                                            else
                                            {
                                                christmasMinute = christmas.AvailTime;
                                                christmas.gameBeginTime = DateTime.Now;
                                                christmas.gameEndTime = DateTime.Now.AddMinutes((double) christmasMinute);
                                            }
                                            bool flag7 = client.Player.Actives.AvailTime();
                                            pkg.WriteByte(0x11);
                                            pkg.WriteBoolean(flag7);
                                            pkg.WriteDateTime(christmas.gameBeginTime);
                                            pkg.WriteDateTime(christmas.gameEndTime);
                                            pkg.WriteInt(christmas.count);
                                            client.Out.SendTCP(pkg);
                                            return 0;
                                        }
                                        case 1:
                                            christmasRoom.RemovePlayer(client.Player);
                                            return 0;
                                    }
                                    return 0;
                                }
                                int num15 = packet.ReadInt();
                                int num16 = packet.ReadInt();
                                client.Player.X = num15;
                                client.Player.Y = num16;
                                if (client.Player.CurrentRoom != null)
                                {
                                    client.Player.CurrentRoom.RemovePlayerUnsafe(client.Player);
                                }
                                christmasRoom.AddMoreMonters();
                                christmasRoom.SetMonterDie(client.Player.PlayerCharacter.ID);
                                if (!client.Player.Actives.AvailTime())
                                {
                                    client.Player.SendMessage("Hết thời gian, bạn phải trả ph\x00ed để tiếp tục.");
                                    return 0;
                                }
                                pkg.WriteByte(0x16);
                                pkg.WriteByte(0);
                                pkg.WriteInt(christmasRoom.Monters.Count);
                                foreach (MonterInfo info6 in christmasRoom.Monters.Values)
                                {
                                    pkg.WriteInt(info6.ID);
                                    pkg.WriteInt(info6.type);
                                    pkg.WriteInt(info6.state);
                                    pkg.WriteInt(info6.MonsterPos.X);
                                    pkg.WriteInt(info6.MonsterPos.Y);
                                }
                                client.Out.SendTCP(pkg);
                                christmasRoom.ViewOtherPlayerRoom(client.Player);
                                return 0;
                            }
                            case 0x15:
                            {
                                int num18 = packet.ReadInt();
                                int num19 = packet.ReadInt();
                                string str = packet.ReadString();
                                client.Player.X = num18;
                                client.Player.Y = num19;
                                pkg.WriteByte(0x15);
                                pkg.WriteInt(client.Player.PlayerId);
                                pkg.WriteInt(num18);
                                pkg.WriteInt(num19);
                                pkg.WriteString(str);
                                christmasRoom.SendToALL(pkg);
                                return 0;
                            }
                            case 0x16:
                            {
                                int id = packet.ReadInt();
                                if (client.Player.MainWeapon != null)
                                {
                                    if (!client.Player.Actives.AvailTime())
                                    {
                                        client.Player.SendMessage("Hết thời gian, bạn phải trả ph\x00ed để tiếp tục.");
                                        return 0;
                                    }
                                    if (christmasRoom.SetFightMonter(id, client.Player.PlayerCharacter.ID) && christmasRoom.Monters.ContainsKey(id))
                                    {
                                        MonterInfo info7 = christmasRoom.Monters[id];
                                        pkg.WriteByte(0x16);
                                        pkg.WriteByte(3);
                                        pkg.WriteInt(id);
                                        pkg.WriteInt(info7.state);
                                        christmasRoom.SendToALL(pkg);
                                        RoomMgr.CreateRoom(client.Player, "Christmas", "Christmas", eRoomType.Christmas, 3);
                                        return 0;
                                    }
                                    return 0;
                                }
                                client.Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip", new object[0]));
                                return 0;
                            }
                            case 0x18:
                                pkg.WriteByte(0x18);
                                pkg.WriteInt(christmas.count);
                                pkg.WriteInt(christmas.exp);
                                pkg.WriteInt(christmas.awardState);
                                pkg.WriteInt(christmas.packsNumber);
                                client.Out.SendTCP(pkg);
                                return 0;

                            case 0x19:
                            {
                                int templateId = 0x311b8;
                                int num22 = packet.ReadInt();
                                bool flag9 = packet.ReadBoolean();
                                int itemCount = client.Player.GetItemCount(templateId);
                                if (num22 <= itemCount)
                                {
                                    if (num22 > 5)
                                    {
                                        num22 = 5;
                                    }
                                    bool flag10 = false;
                                    int num24 = num22;
                                    int num25 = 0;
                                    int num26 = 10;
                                    if (flag9 && client.Player.MoneyDirect(GameProperties.ChristmasBuildSnowmanDoubleMoney))
                                    {
                                        num24 = num22 * 2;
                                    }
                                    christmas.exp += num24;
                                    if (christmas.exp >= num26)
                                    {
                                        christmas.exp -= num26;
                                        flag10 = true;
                                        christmas.count++;
                                        num25 = 1;
                                    }
                                    client.Player.RemoveTemplate(templateId, num22);
                                    pkg.WriteByte(0x19);
                                    pkg.WriteBoolean(flag10);
                                    pkg.WriteInt(christmas.count);
                                    pkg.WriteInt(christmas.exp);
                                    pkg.WriteInt(num24);
                                    pkg.WriteInt(num25);
                                    client.Out.SendTCP(pkg);
                                    return 0;
                                }
                                client.Player.SendMessage("Số lượng ma, thao t\x00e1c thất bại.");
                                return 0;
                            }
                            case 0x1a:
                            {
                                int num27 = packet.ReadInt();
                                if (DateTime.Compare(client.Player.LastOpenChristmasPackage.AddSeconds(1.0), DateTime.Now) <= 0)
                                {
                                    if (christmas.packsNumber >= (GameProperties.ChristmasGiftsMaxNum - 1))
                                    {
                                        client.Player.SendMessage("Số lần nhận thưởng đ\x00e3 hết.");
                                        return 0;
                                    }
                                    string[] strArray3 = GameProperties.ChristmasGifts.Split(new char[] { '|' });
                                    string str5 = "";
                                    int length = strArray3.Length;
                                    foreach (string str6 in strArray3)
                                    {
                                        if (str6.Split(new char[] { ',' })[0] == num27.ToString())
                                        {
                                            str5 = str6;
                                            break;
                                        }
                                    }
                                    if (!(str5 != ""))
                                    {
                                        client.Player.SendMessage("Kh\x00f4ng đủ điều kiện, thao t\x00e1c thất bại.");
                                        return 0;
                                    }
                                    int num30 = int.Parse(str5.Split(new char[] { ',' })[1]);
                                    if (christmas.packsNumber >= (length - 2))
                                    {
                                        num30 = int.Parse(strArray3[length - 2].Split(new char[] { ',' })[1]) + (num30 * (christmas.packsNumber + 1));
                                    }
                                    if (num30 <= christmas.count)
                                    {
                                        christmas.packsNumber++;
                                        christmas.awardState |= ((int) 1) << christmas.packsNumber;
                                        client.Player.SendMessage("Nhận thưởng th\x00e0nh c\x00f4ng.");
                                        client.Player.SendItemToMail(num27, "", title);
                                        if ((christmas.packsNumber == (length - 2)) && (christmas.count < christmas.lastPacks))
                                        {
                                            christmas.count += int.Parse(strArray3[length - 1].Split(new char[] { ',' })[1]) * christmas.packsNumber;
                                        }
                                        pkg.WriteByte(0x1a);
                                        pkg.WriteInt(christmas.awardState);
                                        pkg.WriteInt(christmas.packsNumber);
                                        pkg.WriteInt(num27);
                                        client.Out.SendTCP(pkg);
                                        client.Player.LastOpenChristmasPackage = DateTime.Now;
                                        return 0;
                                    }
                                    client.Player.SendMessage("Kh\x00f4ng đủ người tuyết, thao t\x00e1c thất bại.");
                                    return 0;
                                }
                                return 0;
                            }
                            case 0x1b:
                            {
                                byte num32 = packet.ReadByte();
                                int[] numArray2 = new int[] { 0x311ba, 0x311bb };
                                int num33 = random.Next(numArray2.Length);
                                if (DateTime.Compare(client.Player.LastOpenChristmasPackage.AddSeconds(1.0), DateTime.Now) <= 0)
                                {
                                    if ((num32 == 1) && (christmas.dayPacks < 2))
                                    {
                                        christmas.dayPacks++;
                                        client.Player.SendMessage("Nhận thưởng th\x00e0nh c\x00f4ng.");
                                        client.Player.SendItemToMail(numArray2[num33], "", title);
                                    }
                                    else if (christmas.count < 3)
                                    {
                                        client.Player.SendMessage("T\x00edch lũy 3 người tuyết để nhận qu\x00e0");
                                    }
                                    else
                                    {
                                        pkg.WriteByte(0x1b);
                                        pkg.WriteBoolean(true);
                                        pkg.WriteInt(christmas.dayPacks);
                                        pkg.WriteInt(0);
                                        pkg.WriteInt(0);
                                        client.Out.SendTCP(pkg);
                                    }
                                    client.Player.LastOpenChristmasPackage = DateTime.Now;
                                    return 0;
                                }
                                return 0;
                            }
                            case 0x1d:
                            {
                                int christmasBuyTimeMoney = GameProperties.ChristmasBuyTimeMoney;
                                if (client.Player.MoneyDirect(christmasBuyTimeMoney))
                                {
                                    int christmasBuyMinute = GameProperties.ChristmasBuyMinute;
                                    client.Player.Actives.AddTime(christmasBuyMinute);
                                    client.Player.SendMessage("Thao t\x00e1c th\x00e0nh c\x00f4ng!");
                                    return 0;
                                }
                                return 1;
                            }
                            case 0x21:
                            {
                                client.Player.Actives.YearMonterValidate();
                                pkg.WriteByte(0x21);
                                pkg.WriteInt(client.Player.Actives.Info.ChallengeNum);
                                pkg.WriteInt(client.Player.Actives.Info.BuyBuffNum);
                                pkg.WriteInt(GameProperties.YearMonsterBuffMoney);
                                pkg.WriteInt(client.Player.Actives.Info.DamageNum);
                                string[] strArray5 = GameProperties.YearMonsterBoxInfo.Split(new char[] { '|' });
                                pkg.WriteInt(strArray5.Length);
                                for (int k = 0; k < strArray5.Length; k++)
                                {
                                    string[] strArray6 = strArray5[k].Split(new char[] { ',' });
                                    string[] strArray7 = client.Player.Actives.Info.BoxState.Split(new char[] { '-' });
                                    pkg.WriteInt(int.Parse(strArray6[0]));
                                    pkg.WriteInt(int.Parse(strArray6[1]) * 0x2710);
                                    pkg.WriteInt(int.Parse(strArray7[k]));
                                }
                                client.Out.SendTCP(pkg);
                                return 0;
                            }
                            case 0x22:
                                if (client.Player.MainWeapon != null)
                                {
                                    if (client.Player.Actives.Info.ChallengeNum > 0)
                                    {
                                        ActiveSystemInfo info = client.Player.Actives.Info;
                                        info.ChallengeNum--;
                                        RoomMgr.CreateCatchBeastRoom(client.Player);
                                        return 0;
                                    }
                                    return 0;
                                }
                                client.Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip", new object[0]));
                                return 0;

                            case 0x23:
                                packet.ReadBoolean();
                                if (client.Player.MoneyDirect(GameProperties.YearMonsterBuffMoney))
                                {
                                    if (client.Player.Actives.Info.BuyBuffNum > 0)
                                    {
                                        ActiveSystemInfo info15 = client.Player.Actives.Info;
                                        info15.BuyBuffNum--;
                                    }
                                    pkg.WriteByte(0x23);
                                    pkg.WriteInt(client.Player.Actives.Info.BuyBuffNum);
                                    client.Out.SendTCP(pkg);
                                    client.Player.SendMessage("Thao t\x00e1c th\x00e0nh c\x00f4ng.");
                                    AbstractBuffer buffer = BufferList.CreatePayBuffer(400, 0x7530, 1);
                                    if (buffer != null)
                                    {
                                        buffer.Start(client.Player);
                                    }
                                    buffer = BufferList.CreatePayBuffer(0x196, 0x7530, 1);
                                    if (buffer != null)
                                    {
                                        buffer.Start(client.Player);
                                        return 0;
                                    }
                                    return 0;
                                }
                                return 0;

                            case 0x24:
                            {
                                int num37 = packet.ReadInt();
                                DateTime.Compare(client.Player.LastOpenYearMonterPackage.AddSeconds(1.5), DateTime.Now);
                                string[] yearMonsterBoxInfo = GameProperties.YearMonsterBoxInfo.Split(new char[] { '|' });
                                bool flag11 = this.CanGetGift(client.Player.Actives.Info.DamageNum, num37, yearMonsterBoxInfo);
                                if (flag11)
                                {
                                    int dateId = int.Parse(yearMonsterBoxInfo[num37].Split(new char[] { ',' })[0]);
                                    pkg.WriteByte(0x24);
                                    pkg.WriteBoolean(flag11);
                                    pkg.WriteInt(num37);
                                    client.Out.SendTCP(pkg);
                                    client.Player.Actives.SetYearMonterBoxState(num37);
                                    List<SqlDataProvider.Data.ItemInfo> itemInfos = new List<SqlDataProvider.Data.ItemInfo>();
                                    int point = 0;
                                    int gold = 0;
                                    int giftToken = 0;
                                    int medal = 0;
                                    int exp = 0;
                                    int honor = 0;
                                    int hardCurrency = 0;
                                    int leagueMoney = 0;
                                    int useableScore = 0;
                                    int prestge = 0;
                                    int magicStonePoint = 0;
                                    ItemBoxMgr.CreateItemBox(dateId, itemInfos, ref gold, ref point, ref giftToken, ref medal, ref exp, ref honor, ref hardCurrency, ref leagueMoney, ref useableScore, ref prestge, ref magicStonePoint);
                                    StringBuilder builder = new StringBuilder();
                                    foreach (SqlDataProvider.Data.ItemInfo info8 in itemInfos)
                                    {
                                        builder.Append(info8.Template.Name + " x" + info8.Count.ToString() + ", ");
                                    }
                                    client.Out.SendMessage(eMessageType.Normal, builder.ToString());
                                    client.Player.AddTemplate(itemInfos);
                                }
                                client.Player.LastOpenYearMonterPackage = DateTime.Now;
                                return 0;
                            }
                            case 0x26:
                            {
                                client.Player.Actives.StopLightriddleTimer();
                                LanternriddlesInfo lanternriddles = ActiveSystemMgr.EnterLanternriddles(client.Player.PlayerCharacter.ID);
                                client.Player.Actives.SendLightriddleQuestion(lanternriddles);
                                if (lanternriddles.CanNextQuest)
                                {
                                    client.Player.Actives.BeginLightriddleTimer();
                                }
                                return 0;
                            }
                            case 40:
                            {
                                packet.ReadInt();
                                packet.ReadInt();
                                int option = packet.ReadInt();
                                ActiveSystemMgr.LanternriddlesAnswer(client.Player.PlayerCharacter.ID, option);
                                return 0;
                            }
                            case 0x29:
                            {
                                packet.ReadInt();
                                packet.ReadInt();
                                int num51 = packet.ReadInt();
                                packet.ReadBoolean();
                                LanternriddlesInfo info10 = ActiveSystemMgr.GetLanternriddles(client.Player.PlayerCharacter.ID);
                                if (info10 != null)
                                {
                                    int hitPrice;
                                    bool flag12 = false;
                                    if (num51 == 0)
                                    {
                                        if (info10.HitFreeCount > 0)
                                        {
                                            info10.HitFreeCount--;
                                            info10.IsHint = true;
                                            flag12 = true;
                                        }
                                        else
                                        {
                                            hitPrice = info10.HitPrice;
                                            if (client.Player.ActiveMoneyEnable(hitPrice))
                                            {
                                                info10.IsHint = true;
                                                flag12 = true;
                                            }
                                        }
                                    }
                                    else if (info10.DoubleFreeCount > 0)
                                    {
                                        info10.DoubleFreeCount--;
                                        info10.IsDouble = true;
                                        flag12 = true;
                                    }
                                    else
                                    {
                                        hitPrice = info10.DoublePrice;
                                        if (client.Player.ActiveMoneyEnable(hitPrice))
                                        {
                                            info10.IsDouble = true;
                                            flag12 = true;
                                        }
                                    }
                                    if (flag12)
                                    {
                                        pkg.WriteByte(0x29);
                                        pkg.WriteBoolean(flag12);
                                        client.Out.SendTCP(pkg);
                                        client.Player.SendMessage("Thao t\x00e1c th\x00e0nh c\x00f4ng.");
                                        return 0;
                                    }
                                    return 0;
                                }
                                client.Player.SendMessage("Dữ liệu server lổi.");
                                return 0;
                            }
                            case 0x2a:
                                return 0;

                            case 0x31:
                                pkg.WriteByte(0x31);
                                pkg.WriteByte(1);
                                pkg.WriteInt(1);
                                pkg.WriteString(client.Player.PlayerCharacter.NickName);
                                pkg.WriteBoolean(client.Player.PlayerCharacter.typeVIP == 1);
                                pkg.WriteBoolean(client.Player.PlayerCharacter.Sex);
                                pkg.WriteBoolean(true);
                                pkg.WriteByte((byte) client.Player.PlayerCharacter.Grade);
                                pkg.WriteByte(0x20);
                                pkg.WriteByte(0x10);
                                pkg.WriteByte(8);
                                pkg.WriteByte(4);
                                pkg.WriteByte(2);
                                pkg.WriteByte(1);
                                pkg.WriteByte(0);
                                pkg.WriteByte(0);
                                pkg.WriteByte(0);
                                pkg.WriteByte(0);
                                pkg.WriteByte(0);
                                pkg.WriteByte(0);
                                pkg.WriteInt(0);
                                pkg.WriteDateTime(DateTime.Now.AddDays(7.0));
                                pkg.WriteInt(1);
                                client.Out.SendTCP(pkg);
                                return 0;
                        }
                        goto Label_2768;
                }
                client.Player.SendTCP(pkg);
                return 0;
            }
        Label_2768:
            Console.WriteLine("activeSystem_cmd: " + num);
            return 0;
        }

        private void SendDDPlayInfo(GameClient client)
        {
            GSPacketIn pkg = new GSPacketIn(0x91);
            pkg.WriteByte(0x4b);
            pkg.WriteInt(client.Player.PlayerCharacter.DDPlayPoint);
            client.Player.SendTCP(pkg);
        }

        private void SendEnterBoguAdventure(GameClient client, GSPacketIn packet)
        {
            GSPacketIn pkg = new GSPacketIn(0x91);
            pkg.WriteByte(90);
            pkg.WriteInt(client.Player.Actives.BoguAdventure.CurrentPostion);
            pkg.WriteInt(client.Player.Actives.BoguAdventure.HP);
            pkg.WriteInt(int.Parse(client.Player.Actives.BoguAdventure.GetAward()[0]));
            pkg.WriteInt(int.Parse(client.Player.Actives.BoguAdventure.GetAward()[1]));
            pkg.WriteInt(int.Parse(client.Player.Actives.BoguAdventure.GetAward()[2]));
            pkg.WriteInt(client.Player.Actives.BoguAdventure.OpenCount);
            pkg.WriteInt(client.Player.Actives.BoguAdventureMoney[0]);
            pkg.WriteInt(client.Player.Actives.BoguAdventureMoney[1]);
            pkg.WriteInt(client.Player.Actives.BoguAdventureMoney[2]);
            pkg.WriteBoolean(false);
            pkg.WriteInt(client.Player.Actives.BoguAdventure.ResetCount);
            pkg.WriteInt(70);
            foreach (BoguCeilInfo info in client.Player.Actives.BoguAdventure.MapData)
            {
                pkg.WriteInt(info.Index);
                pkg.WriteInt(info.State);
                pkg.WriteInt((info.State == 2) ? info.Result : -2);
                pkg.WriteInt(client.Player.Actives.GetTotalMineAround(info.Index).Length);
            }
            for (int i = 0; i < 3; i++)
            {
                pkg.WriteInt(client.Player.Actives.CountOpenCanTakeBoxGoguAdventure(i));
                List<EventAwardInfo> boGuBoxAward = EventAwardMgr.GetBoGuBoxAward(i);
                pkg.WriteInt(boGuBoxAward.Count);
                foreach (EventAwardInfo info2 in boGuBoxAward)
                {
                    pkg.WriteInt(info2.TemplateID);
                    pkg.WriteInt(info2.Count);
                }
            }
            client.Player.SendTCP(pkg);
        }
    }
}

