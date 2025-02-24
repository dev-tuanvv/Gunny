namespace Game.Server.Buffer
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class ConsortionAddDamageBuffer : AbstractBuffer
    {
        public ConsortionAddDamageBuffer(BufferInfo buffer) : base(buffer)
        {
        }

        public override void Start(GamePlayer player)
        {
            ConsortionAddDamageBuffer ofType = player.BufferList.GetOfType(typeof(ConsortionAddDamageBuffer)) as ConsortionAddDamageBuffer;
            if (ofType != null)
            {
                BufferInfo info = ofType.Info;
                info.ValidDate += base.m_info.ValidDate;
                ofType.Info.TemplateID = base.m_info.TemplateID;
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

