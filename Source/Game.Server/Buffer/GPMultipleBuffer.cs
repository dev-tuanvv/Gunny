namespace Game.Server.Buffer
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class GPMultipleBuffer : AbstractBuffer
    {
        public GPMultipleBuffer(BufferInfo info) : base(info)
        {
        }

        public override void Start(GamePlayer player)
        {
            GPMultipleBuffer ofType = player.BufferList.GetOfType(typeof(GPMultipleBuffer)) as GPMultipleBuffer;
            if (ofType != null)
            {
                BufferInfo info = ofType.Info;
                info.ValidDate += base.Info.ValidDate;
                player.BufferList.UpdateBuffer(ofType);
            }
            else
            {
                base.Start(player);
                player.GPAddPlus *= base.Info.Value;
            }
        }

        public override void Stop()
        {
            if (base.m_player != null)
            {
                base.m_player.GPAddPlus /= (double) base.Info.Value;
                base.Stop();
            }
        }
    }
}

