namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Packets;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0x63, "场景用户离开")]
    public class TexpHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int num = packet.ReadInt();
            int templateId = packet.ReadInt();
            int slot = packet.ReadInt();
            SqlDataProvider.Data.ItemInfo itemAt = client.Player.StoreBag.GetItemAt(slot);
            TexpInfo texp = client.Player.PlayerCharacter.Texp;
            if (((itemAt == null) || (texp == null)) || (itemAt.TemplateID != templateId))
            {
                client.Out.SendMessage(eMessageType.Normal, "Xảy ra lỗi, chuyển k\x00eanh v\x00e0 thử lại.");
                return 0;
            }
            if (!itemAt.isTexp())
            {
                client.Out.SendMessage(eMessageType.Normal, "Xảy ra lỗi, chuyển k\x00eanh v\x00e0 thử lại.");
                return 0;
            }
            if (texp.texpCount > client.Player.PlayerCharacter.Grade)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("texpSystem.texpCountToplimit", new object[0]));
            }
            else
            {
                switch (num)
                {
                    case 0:
                        texp.hpTexpExp += itemAt.Template.Property2;
                        break;

                    case 1:
                        texp.attTexpExp += itemAt.Template.Property2;
                        break;

                    case 2:
                        texp.defTexpExp += itemAt.Template.Property2;
                        break;

                    case 3:
                        texp.spdTexpExp += itemAt.Template.Property2;
                        break;

                    case 4:
                        texp.lukTexpExp += itemAt.Template.Property2;
                        break;
                }
                texp.texpTaskCount++;
                texp.texpTaskDate = DateTime.Now;
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    bussiness.UpdateUserTexpInfo(texp);
                }
                client.Player.PlayerCharacter.Texp = texp;
                client.Player.StoreBag.RemoveTemplate(templateId, 1);
                client.Player.EquipBag.UpdatePlayerProperties();
            }
            return 0;
        }
    }
}

