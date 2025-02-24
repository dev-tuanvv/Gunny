namespace Game.Server.Buffer
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class KickProtectBuffer : AbstractBuffer
    {
        public KickProtectBuffer(BufferInfo info) : base(info)
        {
        }

        public override void Start(GamePlayer player)
        {
            KickProtectBuffer ofType = player.BufferList.GetOfType(typeof(KickProtectBuffer)) as KickProtectBuffer;
            if (ofType != null)
            {
                BufferInfo info = ofType.Info;
                info.ValidDate += base.Info.ValidDate;
                player.BufferList.UpdateBuffer(ofType);
            }
            else
            {
                base.Start(player);
                player.KickProtect = true;
            }
        }

        public override void Stop()
        {
            base.m_player.KickProtect = false;
            base.Stop();
        }
    }
}

