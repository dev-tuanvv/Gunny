namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0x77, "物品比较")]
    public class GetLinkGoodsHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int val = packet.ReadInt();
            string str = packet.ReadString();
            string str2 = packet.ReadString();
            GSPacketIn @in = new GSPacketIn(0x77, client.Player.PlayerCharacter.ID);
            string nickName = client.Player.PlayerCharacter.NickName;
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                @in.WriteInt(val);
                switch (val)
                {
                    case 4:
                        @in.WriteString(nickName);
                        @in.WriteInt(0);
                        @in.WriteInt(0);
                        @in.WriteInt(0);
                        @in.WriteInt(0);
                        @in.WriteInt(0);
                        client.Out.SendTCP(@in);
                        return 0;

                    case 5:
                        str = packet.ReadString();
                        @in.WriteString(nickName);
                        @in.WriteInt(0);
                        @in.WriteInt(0);
                        @in.WriteInt(0);
                        @in.WriteInt(0);
                        @in.WriteInt(0);
                        @in.WriteInt(0);
                        @in.WriteInt(0);
                        @in.WriteInt(0);
                        client.Out.SendTCP(@in);
                        return 0;
                }
                if (str2 == "")
                {
                    str2 = str;
                }
                SqlDataProvider.Data.ItemInfo userItemSingle = bussiness.GetUserItemSingle(Convert.ToInt32(str2));
                if (userItemSingle != null)
                {
                    @in.WriteString(nickName);
                    @in.WriteInt(userItemSingle.TemplateID);
                    @in.WriteInt(userItemSingle.ItemID);
                    @in.WriteInt(userItemSingle.StrengthenLevel);
                    @in.WriteInt(userItemSingle.AttackCompose);
                    @in.WriteInt(userItemSingle.AgilityCompose);
                    @in.WriteInt(userItemSingle.LuckCompose);
                    @in.WriteInt(userItemSingle.DefendCompose);
                    @in.WriteInt(userItemSingle.ValidDate);
                    @in.WriteBoolean(userItemSingle.IsBinds);
                    @in.WriteBoolean(userItemSingle.IsJudge);
                    @in.WriteBoolean(userItemSingle.IsUsed);
                    if (userItemSingle.IsUsed)
                    {
                        @in.WriteString(userItemSingle.BeginDate.ToString());
                    }
                    @in.WriteInt(userItemSingle.Hole1);
                    @in.WriteInt(userItemSingle.Hole2);
                    @in.WriteInt(userItemSingle.Hole3);
                    @in.WriteInt(userItemSingle.Hole4);
                    @in.WriteInt(userItemSingle.Hole5);
                    @in.WriteInt(userItemSingle.Hole6);
                    @in.WriteString(userItemSingle.Template.Hole);
                    @in.WriteString(userItemSingle.Template.Pic);
                    @in.WriteInt(userItemSingle.RefineryLevel);
                    @in.WriteDateTime(DateTime.Now);
                    @in.WriteByte((byte) userItemSingle.Hole5Level);
                    @in.WriteInt(userItemSingle.Hole5Exp);
                    @in.WriteByte((byte) userItemSingle.Hole6Level);
                    @in.WriteInt(userItemSingle.Hole6Exp);
                    if (userItemSingle.IsGold)
                    {
                        @in.WriteBoolean(userItemSingle.IsGold);
                        @in.WriteInt(userItemSingle.goldValidDate);
                        @in.WriteDateTime(userItemSingle.goldBeginTime);
                    }
                    else
                    {
                        @in.WriteBoolean(false);
                    }
                    client.Out.SendTCP(@in);
                }
                return 1;
            }
        }
    }
}

