namespace Game.Server.Buffer
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class WorldBossAttrack_MoneyBuffBuffer : AbstractBuffer
    {
        public WorldBossAttrack_MoneyBuffBuffer(BufferInfo buffer) : base(buffer)
        {
        }

        public override void Start(GamePlayer player)
        {
            WorldBossAttrack_MoneyBuffBuffer ofType = player.BufferList.GetOfType(typeof(WorldBossAttrack_MoneyBuffBuffer)) as WorldBossAttrack_MoneyBuffBuffer;
            if (ofType != null)
            {
                ofType.Info.ValidDate = base.m_info.ValidDate;
                player.BufferList.UpdateBuffer(ofType);
                for (int i = 0; i < player.FightBuffs.Count; i++)
                {
                    if ((player.FightBuffs[i].Type == base.m_info.Type) && (player.FightBuffs[i].ValidCount < 20))
                    {
                        BufferInfo local1 = player.FightBuffs[i];
                        local1.Value += base.m_info.Value;
                        BufferInfo local2 = player.FightBuffs[i];
                        local2.ValidCount++;
                        break;
                    }
                }
            }
            else
            {
                base.Start(player);
                player.FightBuffs.Add(base.Info);
            }
        }

        public override void Stop()
        {
            base.m_player.FightBuffs.Remove(base.Info);
            base.Stop();
        }
    }
}

