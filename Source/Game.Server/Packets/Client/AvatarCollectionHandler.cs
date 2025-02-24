namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Managers;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0x192, "更新征婚信息")]
    public class AvatarCollectionHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            byte num = packet.ReadByte();
            switch (num)
            {
                case 3:
                {
                    int iD = packet.ReadInt();
                    int templateId = packet.ReadInt();
                    int sex = packet.ReadInt();
                    if (client.Player.EquipBag.GetItemByTemplateID(0, templateId) == null)
                    {
                        client.Player.SendMessage("Bạn kh\x00f4ng sở hữu vật phẩm n\x00e0y.");
                        break;
                    }
                    ClothGroupTemplateInfo info = ClothGroupTemplateInfoMgr.GetClothGroup(iD, templateId, sex);
                    if (info == null)
                    {
                        client.Player.SendMessage("Thời trang n\x00e0y kh\x00f4ng tồn tại.");
                        break;
                    }
                    ClothPropertyTemplateInfo clothPropertyWithID = ClothPropertyTemplateInfoMgr.GetClothPropertyWithID(info.ID);
                    if (clothPropertyWithID == null)
                    {
                        client.Player.SendMessage("Bộ thời trang chứa thời trang n\x00e0y kh\x00f4ng tồn tại.");
                        break;
                    }
                    if (client.Player.PlayerCharacter.Gold < info.Cost)
                    {
                        client.Player.SendMessage("V\x00e0ng kh\x00f4ng đủ.");
                        break;
                    }
                    client.Player.RemoveGold(info.Cost);
                    bool flag = false;
                    bool flag2 = false;
                    UserAvatarCollectionInfo avatarCollectWithAvatarID = client.Player.AvatarCollect.GetAvatarCollectWithAvatarID(info.ID);
                    if (avatarCollectWithAvatarID == null)
                    {
                        avatarCollectWithAvatarID = new UserAvatarCollectionInfo(client.Player.PlayerCharacter.ID, clothPropertyWithID.ID, clothPropertyWithID.Sex, false, DateTime.Now);
                        client.Player.AvatarCollect.AddAvatarCollection(avatarCollectWithAvatarID);
                        flag = true;
                    }
                    UserAvatarCollectionDataInfo item = new UserAvatarCollectionDataInfo(info.TemplateID, info.Sex);
                    if (!avatarCollectWithAvatarID.AddItem(item))
                    {
                        client.Player.AddGold(info.Cost);
                        client.Player.SendMessage("Lỗi khi k\x00edch hoạt bộ thời trang.");
                        break;
                    }
                    int num5 = ClothGroupTemplateInfoMgr.CountClothGroupWithID(avatarCollectWithAvatarID.AvatarID);
                    if (!((avatarCollectWithAvatarID.Items.Count != (num5 / 2)) || avatarCollectWithAvatarID.IsActive))
                    {
                        avatarCollectWithAvatarID.ActiveAvatar(10);
                        flag = true;
                    }
                    if ((avatarCollectWithAvatarID.Items.Count == (num5 / 2)) || (avatarCollectWithAvatarID.Items.Count == num5))
                    {
                        flag2 = true;
                    }
                    GSPacketIn pkg = new GSPacketIn(0x192);
                    pkg.WriteByte(3);
                    pkg.WriteInt(iD);
                    pkg.WriteInt(templateId);
                    pkg.WriteInt(sex);
                    client.Player.SendTCP(pkg);
                    if (flag)
                    {
                        client.Player.Out.SendAvatarCollect(client.Player.AvatarCollect);
                    }
                    if (flag2)
                    {
                        client.Player.EquipBag.UpdatePlayerProperties();
                    }
                    client.Player.SendMessage("K\x00edch hoạt th\x00e0nh c\x00f4ng!");
                    break;
                }
                case 4:
                {
                    int avatarId = packet.ReadInt();
                    int days = packet.ReadInt();
                    if (days > 0)
                    {
                        UserAvatarCollectionInfo info5 = client.Player.AvatarCollect.GetAvatarCollectWithAvatarID(avatarId);
                        if (info5 == null)
                        {
                            client.Player.SendMessage("Bạn chưa k\x00edch hoạt bộ thời trang n\x00e0y.");
                        }
                        else
                        {
                            if (info5.Items == null)
                            {
                                info5.UpdateItems();
                            }
                            int num8 = ClothGroupTemplateInfoMgr.CountClothGroupWithID(info5.AvatarID);
                            if (info5.Items.Count >= (num8 / 2))
                            {
                                ClothPropertyTemplateInfo info6 = ClothPropertyTemplateInfoMgr.GetClothPropertyWithID(avatarId);
                                if (info6 == null)
                                {
                                    client.Player.SendMessage("Bộ thời trang n\x00e0y kh\x00f4ng thể gia hạn.");
                                }
                                else
                                {
                                    int num9 = info6.Cost * days;
                                    if ((client.Player.PlayerCharacter.myHonor < info6.Cost) || (num9 <= 0))
                                    {
                                        client.Player.SendMessage("Vinh dự của bạn kh\x00f4ng đủ.");
                                    }
                                    else
                                    {
                                        client.Player.RemovemyHonor(num9);
                                        info5.ActiveAvatar(days);
                                        client.Player.Out.SendAvatarCollect(client.Player.AvatarCollect);
                                        client.Player.SendMessage("Gia hạn th\x00e0nh c\x00f4ng.");
                                    }
                                }
                            }
                            else
                            {
                                client.Player.SendMessage("Bạn phải k\x00edch hoạt hơn 1 nửa bộ sưu tập mới c\x00f3 quyền gia hạn.");
                            }
                        }
                        break;
                    }
                    return 0;
                }
                default:
                    Console.WriteLine("cmd_avatar_collection: " + num);
                    break;
            }
            return 1;
        }
    }
}

