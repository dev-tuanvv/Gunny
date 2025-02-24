namespace Game.Server.Buffer
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class PropsBuffer : AbstractBuffer
    {
        public PropsBuffer(BufferInfo buffer) : base(buffer)
        {
        }

        public override void Start(GamePlayer player)
        {
            PropsBuffer ofType = player.BufferList.GetOfType(typeof(PropsBuffer)) as PropsBuffer;
            if (ofType != null)
            {
                BufferInfo info = ofType.Info;
                info.ValidDate += base.Info.ValidDate;
                player.BufferList.UpdateBuffer(ofType);
            }
            else
            {
                base.Start(player);
                player.CanUseProp = true;
            }
        }

        public override void Stop()
        {
            base.m_player.CanUseProp = false;
            base.Stop();
        }
    }
}

