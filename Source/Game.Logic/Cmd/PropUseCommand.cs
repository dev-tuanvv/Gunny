namespace Game.Logic.Cmd
{
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [GameCommand(0x20, "使用道具")]
    public class PropUseCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if ((game.GameState == eGameState.Playing) && !player.GetSealState())
            {
                int bag = packet.ReadByte();
                int place = packet.ReadInt();
                int templateId = packet.ReadInt();
                ItemTemplateInfo item = ItemMgr.FindItemTemplate(templateId);
                int[] propBag = PropItemMgr.PropBag;
                if (((bag != 2) || new List<string> { "10009", "10012", "10017", "10018", "10010", "10011", "10016", "10015" }.Contains(item.TemplateID.ToString())) && (((item != null) && player.CheckCanUseItem(item)) && player.CanUseItem(item)))
                {
                    if (player.PlayerDetail.UsePropItem(game, bag, place, templateId, player.IsLiving))
                    {
                        if (!player.UseItem(item))
                        {
                            BaseGame.log.Error("Using prop error");
                        }
                    }
                    else if ((bag != 2) || player.PlayerDetail.UsePropItem(game, bag, place, templateId, player.IsLiving))
                    {
                        if (propBag.Contains<int>(item.TemplateID))
                        {
                            player.UseItem(item);
                            int num4 = templateId;
                            if (num4 == 0x2711)
                            {
                                if (player.Prop < (templateId * 2))
                                {
                                    player.Prop += templateId;
                                    return;
                                }
                            }
                            else
                            {
                                if (num4 != 0x2714)
                                {
                                    return;
                                }
                                if (player.Prop < (templateId * 2))
                                {
                                    player.Prop += templateId;
                                    return;
                                }
                            }
                        }
                        else
                        {
                            player.PlayerDetail.SendMessage("Vật phẩm lỗi hoặc kh\x00f4ng tồn tại");
                        }
                        if ((templateId == 0x271f) || (item.CategoryID == 0x11))
                        {
                            player.ShootCount = 1;
                        }
                    }
                }
            }
        }
    }
}

