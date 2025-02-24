namespace Game.Logic.Phy.Actions
{
    using System;

    public class PetAction
    {
        public int blood;
        public int damage;
        public int dander;
        public int id;
        public float Time;
        public int Type;

        public PetAction(float time, PetActionType type, int _id, int _damage, int _dander, int _blood)
        {
            this.Time = time;
            this.Type = (int) type;
            this.id = _id;
            this.damage = _damage;
            this.blood = _blood;
            this.dander = _dander;
        }

        public int TimeInt
        {
            get
            {
                return (int) Math.Round((double) (this.Time * 1000f));
            }
        }
    }
}

