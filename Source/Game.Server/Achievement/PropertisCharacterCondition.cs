namespace Game.Server.Achievement
{
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;

    public class PropertisCharacterCondition : BaseCondition
    {
        private string string_0;

        public PropertisCharacterCondition(BaseAchievement quest, AchievementCondictionInfo info, int value, string type) : base(quest, info, value)
        {
            this.string_0 = type;
        }

        public override void AddTrigger(GamePlayer player)
        {
            player.PropertisChange += new GamePlayer.PlayerPropertisChange(this.method_0);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return (base.Value >= base.m_info.Condiction_Para2);
        }

        private void method_0(PlayerInfo playerInfo_0)
        {
            string str = this.string_0;
            if (str != null)
            {
                if (!(str == "attack"))
                {
                    if (str == "agility")
                    {
                        base.Value = playerInfo_0.Agility;
                    }
                    else if (str == "luck")
                    {
                        base.Value = playerInfo_0.Luck;
                    }
                    else if (str == "defence")
                    {
                        base.Value = playerInfo_0.Defence;
                    }
                    else if (str == "fightpower")
                    {
                        base.Value = playerInfo_0.FightPower;
                    }
                }
                else
                {
                    base.Value = playerInfo_0.Attack;
                }
            }
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.PropertisChange -= new GamePlayer.PlayerPropertisChange(this.method_0);
        }
    }
}

