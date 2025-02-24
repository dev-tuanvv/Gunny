namespace Game.Server.Packets.Client
{
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Server;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [PacketHandler(0x98, "场景用户离开")]
    public class FightFootballTimeTakeoutHandler : IPacketHandler
    {
        private Dictionary<int, CardInfo> CreateFightFootballTimeAward()
        {
            Dictionary<int, CardInfo> dictionary = new Dictionary<int, CardInfo>();
            Dictionary<int, CardInfo> dictionary2 = new Dictionary<int, CardInfo>();
            int key = 0;
            for (int i = 0; dictionary.Count < 9; i++)
            {
                List<CardInfo> fightFootballTimeAward = EventAwardMgr.GetFightFootballTimeAward(eEventType.FIGHT_FOOTBALL_TIME);
                if (fightFootballTimeAward.Count > 0)
                {
                    CardInfo info = fightFootballTimeAward[0];
                    if (!dictionary2.Keys.Contains<int>(info.templateID))
                    {
                        dictionary2.Add(info.templateID, info);
                        info.place = key;
                        info.count = info.count;
                        dictionary.Add(key, info);
                        key++;
                    }
                }
            }
            return dictionary;
        }

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int val = packet.ReadByte();
            client.Player.Card = this.CreateFightFootballTimeAward();
            GSPacketIn @in = new GSPacketIn(0x98, client.Player.PlayerCharacter.ID);
            if (((val < 9) && (val >= 0)) && (client.Player.takeoutCount > 0))
            {
                @in.WriteInt(1);
                @in.WriteInt(client.Player.Card[val].templateID);
                @in.WriteInt(val);
                @in.WriteInt(client.Player.Card[val].count);
                client.Player.TakeFootballCard(client.Player.Card[val]);
            }
            else
            {
                client.Player.ShowAllFootballCard();
                @in.WriteInt(2);
                @in.WriteInt(client.Player.canTakeOut);
                foreach (CardInfo info in client.Player.CardsTakeOut)
                {
                    if (info.IsTake)
                    {
                        @in.WriteInt(info.templateID);
                        @in.WriteInt(info.place);
                        @in.WriteInt(info.count);
                    }
                }
                @in.WriteInt(client.Player.Card.Count - client.Player.canTakeOut);
                foreach (CardInfo info2 in client.Player.CardsTakeOut)
                {
                    if (!info2.IsTake)
                    {
                        @in.WriteInt(info2.templateID);
                        @in.WriteInt(info2.place);
                        @in.WriteInt(info2.count);
                    }
                }
                client.Player.RemoveFightFootballStyle();
            }
            client.Out.SendTCP(@in);
            return 0;
        }
    }
}

