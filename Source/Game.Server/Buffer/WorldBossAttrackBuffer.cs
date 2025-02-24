namespace Game.Server.Buffer
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class WorldBossAttrackBuffer : AbstractBuffer
    {
        public WorldBossAttrackBuffer(BufferInfo buffer) : base(buffer)
        {
        }

        public override void Start(GamePlayer player)
        {
            WorldBossAttrackBuffer ofType = player.BufferList.GetOfType(typeof(WorldBossAttrackBuffer)) as WorldBossAttrackBuffer;
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

