namespace Game.Server.Buffer
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class GuardBuffer : AbstractBuffer
    {
        public GuardBuffer(BufferInfo buffer) : base(buffer)
        {
        }

        public override void Start(GamePlayer player)
        {
            GuardBuffer ofType = player.BufferList.GetOfType(typeof(GuardBuffer)) as GuardBuffer;
            if (ofType != null)
            {
                BufferInfo info = ofType.Info;
                info.ValidDate += base.Info.ValidDate;
                if (ofType.Info.ValidDate > 30)
                {
                    ofType.Info.ValidDate = 30;
                }
                player.BufferList.UpdateBuffer(ofType);
            }
            else
            {
                base.Start(player);
                PlayerInfo playerCharacter = player.PlayerCharacter;
                playerCharacter.GuardAddPlus += base.Info.Value;
            }
        }

        public override void Stop()
        {
            PlayerInfo playerCharacter = base.m_player.PlayerCharacter;
            playerCharacter.GuardAddPlus -= base.m_info.Value;
            base.Stop();
        }
    }
}

