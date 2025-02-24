namespace Game.Server.Buffer
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class SaveLifeBuffer : AbstractBuffer
    {
        public SaveLifeBuffer(BufferInfo info) : base(info)
        {
        }

        public override void Start(GamePlayer player)
        {
            SaveLifeBuffer ofType = player.BufferList.GetOfType(typeof(SaveLifeBuffer)) as SaveLifeBuffer;
            if (ofType != null)
            {
                ofType.Info.ValidCount = base.m_info.ValidCount;
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

