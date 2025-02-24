namespace Game.Server.Achievement
{
    using Game.Logic;
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class GameOverPassCondition : BaseCondition
    {
        private int int_1;

        public GameOverPassCondition(BaseAchievement quest, AchievementCondictionInfo info, int type, int value) : base(quest, info, value)
        {
            this.int_1 = type;
        }

        public override void AddTrigger(GamePlayer player)
        {
            player.MissionFullOver += new GamePlayer.PlayerMissionFullOverEventHandle(this.method_0);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return (base.Value >= base.m_info.Condiction_Para2);
        }

        private void method_0(AbstractGame abstractGame_0, int int_2, bool bool_0, int int_3)
        {
            if (bool_0)
            {
                int num;
                switch (this.int_1)
                {
                    case 1:
                        if (((int_2 == 0x17d8) || (int_2 == 0x183c)) || (int_2 == 0x18a0))
                        {
                            num = base.Value;
                            base.Value = num + 1;
                            break;
                        }
                        break;

                    case 2:
                        if (((int_2 == 0x1b5c) || (int_2 == 0x1bc0)) || (int_2 == 0x1c24))
                        {
                            num = base.Value;
                            base.Value = num + 1;
                            break;
                        }
                        break;

                    case 3:
                        if (((int_2 == 0xc22) || (int_2 == 0xc86)) || (int_2 == 0xcea))
                        {
                            num = base.Value;
                            base.Value = num + 1;
                            break;
                        }
                        break;

                    case 4:
                        if (((int_2 == 0x431) || (int_2 == 0x498)) || ((int_2 == 0x4fd) || (int_2 == 0x562)))
                        {
                            num = base.Value;
                            base.Value = num + 1;
                            break;
                        }
                        break;

                    case 5:
                        if ((int_2 == 0x7d2) || (int_2 == 0x836))
                        {
                            num = base.Value;
                            base.Value = num + 1;
                            break;
                        }
                        break;

                    case 6:
                        if (((int_2 == 0x1007) || (int_2 == 0x106b)) || (int_2 == 0x10cf))
                        {
                            num = base.Value;
                            base.Value = num + 1;
                            break;
                        }
                        break;

                    case 7:
                        if (((int_2 == 0x13f0) || (int_2 == 0x1454)) || (int_2 == 0x14b8))
                        {
                            num = base.Value;
                            base.Value = num + 1;
                        }
                        break;
                }
            }
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.MissionFullOver -= new GamePlayer.PlayerMissionFullOverEventHandle(this.method_0);
        }
    }
}

