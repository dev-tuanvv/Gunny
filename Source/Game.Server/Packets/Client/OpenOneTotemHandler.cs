namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using System;

    [PacketHandler(0x88, "场景用户离开")]
    public class OpenOneTotemHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.Grade < 20)
            {
                client.Out.SendMessage(eMessageType.Normal, "Thao t\x00e1c thất bại, thử lại sau!");
                return 0;
            }
            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
                return 1;
            }
            int totemId = client.Player.PlayerCharacter.totemId;
            int consumeExp = TotemMgr.FindTotemInfo(totemId).ConsumeExp;
            int consumeHonor = TotemMgr.FindTotemInfo(totemId).ConsumeHonor;
            if (ActiveSystemMgr.ReduceToemUpGrace > 0)
            {
                consumeExp -= (consumeExp * ActiveSystemMgr.ReduceToemUpGrace) / 100;
            }
            if (client.Player.PlayerCharacter.myHonor >= consumeHonor)
            {
                if (client.Player.MoneyDirect(consumeExp))
                {
                    if (totemId == 0)
                    {
                        client.Player.AddTotem(0x2711);
                    }
                    else
                    {
                        client.Player.AddTotem(1);
                    }
                    client.Player.AddExpVip(consumeExp);
                    client.Player.RemovemyHonor(consumeHonor);
                    client.Player.Out.SendPlayerRefreshTotem(client.Player.PlayerCharacter);
                    client.Player.EquipBag.UpdatePlayerProperties();
                    client.Player.OnUserToemGemstoneEvent();
                }
            }
            else
            {
                client.Out.SendMessage(eMessageType.Normal, "Danh vọng kh\x00f4ng đủ.");
            }
            return 0;
        }
    }
}

