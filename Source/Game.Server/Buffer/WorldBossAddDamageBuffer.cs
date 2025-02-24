namespace Game.Server.Buffer
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class WorldBossAddDamageBuffer : AbstractBuffer
    {
        public WorldBossAddDamageBuffer(BufferInfo buffer) : base(buffer)
        {
        }

        public override void Start(GamePlayer player)
        {
            WorldBossAddDamageBuffer ofType = player.BufferList.GetOfType(typeof(WorldBossAddDamageBuffer)) as WorldBossAddDamageBuffer;
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

