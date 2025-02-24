namespace Game.Server.SceneMarryRooms.TankHandle
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Server.GameObjects;
    using Game.Server.Packets;
    using Game.Server.SceneMarryRooms;
    using SqlDataProvider.Data;
    using System;
    using System.Linq;

    [MarryCommandAttbute(6)]
    public class UserFirecrackersCommand : IMarryCommandHandler
    {
        public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            if (player.CurrentMarryRoom != null)
            {
                packet.ReadInt();
                ShopItemInfo info = ShopMgr.FindShopbyTemplatID(packet.ReadInt()).FirstOrDefault<ShopItemInfo>();
                if (info != null)
                {
                    if (info.APrice1 == -2)
                    {
                        if (player.PlayerCharacter.Gold >= info.AValue1)
                        {
                            player.RemoveGold(info.AValue1);
                            player.CurrentMarryRoom.ReturnPacketForScene(player, packet);
                            player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("UserFirecrackersCommand.Successed1", new object[] { info.AValue1 }));
                            return true;
                        }
                        player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserFirecrackersCommand.GoldNotEnough", new object[0]));
                    }
                    if (info.APrice1 == -1)
                    {
                        if (player.PlayerCharacter.Money >= info.AValue1)
                        {
                            player.RemoveMoney(info.AValue1);
                            player.CurrentMarryRoom.ReturnPacketForScene(player, packet);
                            player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("UserFirecrackersCommand.Successed2", new object[] { info.AValue1 }));
                            return true;
                        }
                        player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserFirecrackersCommand.MoneyNotEnough", new object[0]));
                    }
                }
            }
            return false;
        }
    }
}

