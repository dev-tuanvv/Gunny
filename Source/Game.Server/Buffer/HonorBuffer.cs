namespace Game.Server.Buffer
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class HonorBuffer : AbstractBuffer
    {
        public HonorBuffer(BufferInfo info) : base(info)
        {
        }

        public override void Start(GamePlayer player)
        {
            HonorBuffer ofType = player.BufferList.GetOfType(typeof(HonorBuffer)) as HonorBuffer;
            if (ofType != null)
            {
                BufferInfo info = ofType.Info;
                info.ValidDate += base.Info.ValidDate;
                player.BufferList.UpdateBuffer(ofType);
            }
            else
            {
                base.Start(player);
            }
        }

        public override void Stop()
        {
            base.Stop();
        }
    }
}

