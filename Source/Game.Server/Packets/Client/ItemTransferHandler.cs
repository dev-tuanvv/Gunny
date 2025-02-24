namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;
    using System.Text;

    [PacketHandler(0x3d, "物品转移")]
    public class ItemTransferHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            GSPacketIn @in = packet.Clone();
            @in.ClearContext();
            new StringBuilder();
            int num = 0x1388;
            packet.ReadInt();
            packet.ReadInt();
            SqlDataProvider.Data.ItemInfo itemAt = client.Player.StoreBag.GetItemAt(0);
            SqlDataProvider.Data.ItemInfo item = client.Player.StoreBag.GetItemAt(1);
            client.Player.StoreBag.ClearBag();
            client.Player.EquipBag.AddItem(itemAt);
            client.Player.EquipBag.AddItem(item);
            if ((((itemAt != null) && (item != null)) && ((itemAt.Template.CategoryID == item.Template.CategoryID) && (item.Count == 1))) && (itemAt.Count == 1))
            {
                if (client.Player.PlayerCharacter.Gold < num)
                {
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("itemtransferhandler.nogold", new object[0]));
                    return 1;
                }
                client.Player.BeginChanges();
                client.Player.EquipBag.BeginChanges();
                try
                {
                    client.Player.RemoveGold(num);
                    this.method_0(ref itemAt, ref item);
                    itemAt.IsBinds = true;
                    item.IsBinds = true;
                    if ((itemAt.Template.CategoryID == 7) && (item.Template.CategoryID == 7))
                    {
                        ItemTemplateInfo realWeaponTemplate = StrengthenMgr.GetRealWeaponTemplate(itemAt);
                        ItemTemplateInfo goods = StrengthenMgr.GetRealWeaponTemplate(item);
                        if (realWeaponTemplate != null)
                        {
                            SqlDataProvider.Data.ItemInfo info5 = SqlDataProvider.Data.ItemInfo.CloneFromTemplate(realWeaponTemplate, itemAt);
                            info5.LianGrade = itemAt.LianGrade;
                            info5.LianExp = itemAt.LianExp;
                            client.Player.EquipBag.RemoveItem(itemAt);
                            client.Player.EquipBag.AddItem(info5);
                        }
                        if (goods != null)
                        {
                            SqlDataProvider.Data.ItemInfo info6 = SqlDataProvider.Data.ItemInfo.CloneFromTemplate(goods, item);
                            info6.LianGrade = item.LianGrade;
                            info6.LianExp = item.LianExp;
                            client.Player.EquipBag.RemoveItem(item);
                            client.Player.EquipBag.AddItem(info6);
                        }
                    }
                    else
                    {
                        client.Player.EquipBag.UpdateItem(itemAt);
                        client.Player.EquipBag.UpdateItem(item);
                    }
                    @in.WriteByte(0);
                    client.Out.SendTCP(@in);
                }
                finally
                {
                    client.Player.CommitChanges();
                    client.Player.EquipBag.CommitChanges();
                }
            }
            else
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("itemtransferhandler.nocondition", new object[0]));
            }
            return 0;
        }

        private void method_0(ref SqlDataProvider.Data.ItemInfo itemInfo_0, ref SqlDataProvider.Data.ItemInfo itemInfo_1)
        {
            int strengthenLevel = itemInfo_0.StrengthenLevel;
            int attackCompose = itemInfo_0.AttackCompose;
            int defendCompose = itemInfo_0.DefendCompose;
            int agilityCompose = itemInfo_0.AgilityCompose;
            int luckCompose = itemInfo_0.LuckCompose;
            int lianGrade = itemInfo_0.LianGrade;
            int lianExp = itemInfo_0.LianExp;
            int num8 = itemInfo_1.StrengthenLevel;
            int num9 = itemInfo_1.AttackCompose;
            int num10 = itemInfo_1.DefendCompose;
            int num11 = itemInfo_1.AgilityCompose;
            int num12 = itemInfo_1.LuckCompose;
            int num13 = itemInfo_1.LianGrade;
            int num14 = itemInfo_1.LianExp;
            itemInfo_0.StrengthenLevel = num8;
            itemInfo_0.AttackCompose = num9;
            itemInfo_0.DefendCompose = num10;
            itemInfo_0.AgilityCompose = num11;
            itemInfo_0.LuckCompose = num12;
            itemInfo_0.LianGrade = num13;
            itemInfo_0.LianExp = num14;
            itemInfo_1.StrengthenLevel = strengthenLevel;
            itemInfo_1.AttackCompose = attackCompose;
            itemInfo_1.AgilityCompose = agilityCompose;
            itemInfo_1.LuckCompose = luckCompose;
            itemInfo_1.DefendCompose = defendCompose;
            itemInfo_1.LianGrade = lianGrade;
            itemInfo_1.LianExp = lianExp;
        }
    }
}

