namespace Game.Logic.PetEffects
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;
    using System.Collections.Generic;

    public class PetShootedAddGuardForTeamEquip : AbstractPetEffect
    {
        private int m_count;
        private int m_currentId;

        public PetShootedAddGuardForTeamEquip(int count, int skillId, string elementID) : base(ePetEffectType.PetShootedAddGuardForTeamEquip, elementID)
        {
            this.m_count = count;
            this.m_currentId = skillId;
        }

        public override void OnAttached(Living living)
        {
            (living as Player).PlayerShoot += new PlayerEventHandle(this.player_Shoot);
        }

        public override void OnRemoved(Living living)
        {
            (living as Player).PlayerShoot -= new PlayerEventHandle(this.player_Shoot);
        }

        private void player_Shoot(Player living)
        {
            List<Player> allTeamPlayers = living.Game.GetAllTeamPlayers(living);
            foreach (Player player in allTeamPlayers)
            {
                if (player != living)
                {
                    player.AddPetEffect(new PetActiveGuardEquip(2, this.m_currentId, base.Info.ID.ToString()), 0);
                }
            }
            this.m_count--;
            if (this.m_count < 0)
            {
                this.Stop();
            }
        }

        public override bool Start(Living living)
        {
            PetShootedAddGuardForTeamEquip ofType = living.PetEffectList.GetOfType(ePetEffectType.PetShootedAddGuardForTeamEquip) as PetShootedAddGuardForTeamEquip;
            if (ofType != null)
            {
                ofType.m_count = this.m_count;
                return true;
            }
            return base.Start(living);
        }
    }
}

