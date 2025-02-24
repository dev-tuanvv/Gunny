namespace Game.Server.Buffer
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class WorldBossAncientBlessingsBuffer : AbstractBuffer
    {
        public WorldBossAncientBlessingsBuffer(BufferInfo buffer) : base(buffer)
        {
        }

        public override void Start(GamePlayer player)
        {
            WorldBossAncientBlessingsBuffer ofType = player.BufferList.GetOfType(typeof(WorldBossAncientBlessingsBuffer)) as WorldBossAncientBlessingsBuffer;
            if (ofType != null)
            {
                ofType.Info.ValidDate = base.Info.ValidDate;
                player.BufferList.UpdateBuffer(ofType);
                player.UpdateFightBuff(base.Info);
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

