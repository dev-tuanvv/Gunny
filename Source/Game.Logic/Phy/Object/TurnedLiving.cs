namespace Game.Logic.Phy.Object
{
    using Game.Logic;
    using System;

    public class TurnedLiving : Living
    {
        public int DefaultDelay;
        private int m_dander;
        protected int m_delay;
        private int m_petMaxMP;
        private int m_petMP;
        private int m_psychic;

        public TurnedLiving(int id, BaseGame game, int team, string name, string modelId, int maxBlood, int immunity, int direction) : base(id, game, team, name, modelId, maxBlood, immunity, direction)
        {
            this.m_psychic = 20;
            this.m_petMaxMP = 100;
            this.m_petMP = 10;
        }

        public void AddDander(int value)
        {
            if ((value > 0) && base.IsLiving)
            {
                this.SetDander(this.m_dander + value);
            }
        }

        public void AddDelay(int value)
        {
            this.m_delay += value;
        }

        public void AddPetMP(int value)
        {
            if (value > 0)
            {
                if (base.IsLiving && (this.PetMP < this.PetMaxMP))
                {
                    this.m_petMP += value;
                    if (this.m_petMP > this.PetMaxMP)
                    {
                        this.m_petMP = this.PetMaxMP;
                    }
                }
                else
                {
                    this.m_petMP = this.PetMaxMP;
                }
            }
        }

        public override void PrepareSelfTurn()
        {
            base.PrepareSelfTurn();
        }

        public void RemovePetMP(int value)
        {
            if (((value > 0) && base.IsLiving) && (this.PetMP > 0))
            {
                this.m_petMP -= value;
                if (this.m_petMP < 0)
                {
                    this.m_petMP = 0;
                }
            }
        }

        public override void Reset()
        {
            base.Reset();
        }

        public void SetDander(int value)
        {
            this.m_dander = Math.Min(value, 200);
            if (base.SyncAtTime)
            {
                base.m_game.SendGameUpdateDander(this);
            }
        }

        public virtual void Skip(int spendTime)
        {
            if (base.IsAttacking)
            {
                this.StopAttacking();
                base.m_game.SendFightStatus(this, 0);
                base.m_game.CheckState(0);
            }
        }

        public virtual void StartGame()
        {
        }

        public int Dander
        {
            get
            {
                return this.m_dander;
            }
            set
            {
                this.m_dander = value;
            }
        }

        public int Delay
        {
            get
            {
                return this.m_delay;
            }
            set
            {
                this.m_delay = value;
            }
        }

        public int PetMaxMP
        {
            get
            {
                return this.m_petMaxMP;
            }
            set
            {
                this.m_petMaxMP = value;
            }
        }

        public int PetMP
        {
            get
            {
                return this.m_petMP;
            }
            set
            {
                this.m_petMP = value;
            }
        }

        public int psychic
        {
            get
            {
                return this.m_psychic;
            }
            set
            {
                this.m_psychic = value;
            }
        }
    }
}

