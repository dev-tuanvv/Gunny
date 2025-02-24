namespace Game.Server.Buffer
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class OfferMultipleBuffer : AbstractBuffer
    {
        public OfferMultipleBuffer(BufferInfo info) : base(info)
        {
        }

        public override void Start(GamePlayer player)
        {
            OfferMultipleBuffer ofType = player.BufferList.GetOfType(typeof(OfferMultipleBuffer)) as OfferMultipleBuffer;
            if (ofType != null)
            {
                BufferInfo info = ofType.Info;
                info.ValidDate += base.Info.ValidDate;
                player.BufferList.UpdateBuffer(ofType);
            }
            else
            {
                base.Start(player);
                player.OfferAddPlus *= base.Info.Value;
            }
        }

        public override void Stop()
        {
            base.m_player.OfferAddPlus /= (double) base.m_info.Value;
            base.Stop();
        }
    }
}

