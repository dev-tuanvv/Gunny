namespace SqlDataProvider.Data
{
    using System;
    using System.Collections.Generic;

    public class UsersPetinfo : DataObject
    {
        private int _agility;
        private int _agilityGrow;
        private int _attack;
        private int _attackGrow;
        private int _blood;
        private int _bloodGrow;
        private int _currentStarExp;
        private int _damage;
        private int _damageGrow;
        private int _defence;
        private int _defenceGrow;
        private int _gp;
        private int _guard;
        private int _guardGrow;
        private int _hunger;
        private int _ID;
        private bool _isEquip;
        private bool _isExit;
        private int _level;
        private int _luck;
        private int _luckGrow;
        private int _maxGP;
        private int _mp;
        private string _name;
        private int _petID;
        private int _place;
        private string _skill;
        private string _skillEquip;
        private int _templateID;
        private int _userID;
        private List<PetEquipDataInfo> m_peEquip;

        public List<PetEquipDataInfo> GetEquip()
        {
            List<PetEquipDataInfo> list = new List<PetEquipDataInfo>();
            if (this.m_peEquip != null)
            {
                int num = 0;
                for (int i = 0; i < this.m_peEquip.Count; i++)
                {
                    if (this.m_peEquip[i].IsValidate())
                    {
                        list.Add(this.m_peEquip[i]);
                        num++;
                    }
                    else
                    {
                        this.m_peEquip[i].eqTemplateID = -1;
                        this.m_peEquip[i].ValidDate = 7;
                    }
                }
            }
            return list;
        }

        public static PetType GetPetType(int Id)
        {
            if (Id <= 0x3b)
            {
                switch (Id)
                {
                    case 1:
                        return PetType.FORZEN;

                    case 2:
                    case 4:
                        return PetType.Normal;

                    case 3:
                        return PetType.TRANFORM;

                    case 5:
                        goto Label_0074;
                }
                if (Id != 0x3b)
                {
                    return PetType.Normal;
                }
            }
            else
            {
                switch (Id)
                {
                    case 0x61:
                    case 0x62:
                    case 0x40:
                        goto Label_0074;
                }
                return PetType.Normal;
            }
        Label_0074:
            return PetType.CURE;
        }

        public List<string> GetSkill()
        {
            List<string> list = new List<string>();
            string[] strArray = this._skill.Split(new char[] { '|' });
            for (int i = 0; i < strArray.Length; i++)
            {
                list.Add(strArray[i]);
            }
            return list;
        }

        public List<string> GetSkillEquip()
        {
            List<string> list = new List<string>();
            string[] strArray = this._skillEquip.Split(new char[] { '|' });
            for (int i = 0; i < strArray.Length; i++)
            {
                list.Add(strArray[i]);
            }
            return list;
        }

        private int happyPercent()
        {
            double num = (((double) this._hunger) / 10000.0) * 100.0;
            int num2 = 0;
            if (num >= 80.0)
            {
                num2 = 3;
            }
            if ((num < 80.0) && (num >= 60.0))
            {
                num2 = 2;
            }
            if ((num < 60.0) && (num > 0.0))
            {
                num2 = 1;
            }
            return num2;
        }

        private int ReduceProp(int val)
        {
            if (this.happyPercent() == 2)
            {
                return ((val * 20) / 100);
            }
            if (this.happyPercent() == 1)
            {
                return ((val * 40) / 100);
            }
            return 0;
        }

        public int Agility
        {
            get
            {
                return this._agility;
            }
            set
            {
                this._agility = value;
                base._isDirty = true;
            }
        }

        public int AgilityGrow
        {
            get
            {
                return this._agilityGrow;
            }
            set
            {
                this._agilityGrow = value;
                base._isDirty = true;
            }
        }

        public int Attack
        {
            get
            {
                return this._attack;
            }
            set
            {
                this._attack = value;
                base._isDirty = true;
            }
        }

        public int AttackGrow
        {
            get
            {
                return this._attackGrow;
            }
            set
            {
                this._attackGrow = value;
                base._isDirty = true;
            }
        }

        public int Blood
        {
            get
            {
                return this._blood;
            }
            set
            {
                this._blood = value;
                base._isDirty = true;
            }
        }

        public int BloodGrow
        {
            get
            {
                return this._bloodGrow;
            }
            set
            {
                this._bloodGrow = value;
                base._isDirty = true;
            }
        }

        public int currentStarExp
        {
            get
            {
                return this._currentStarExp;
            }
            set
            {
                this._currentStarExp = value;
                base._isDirty = true;
            }
        }

        public int Damage
        {
            get
            {
                return this._damage;
            }
            set
            {
                this._damage = value;
                base._isDirty = true;
            }
        }

        public int DamageGrow
        {
            get
            {
                return this._damageGrow;
            }
            set
            {
                this._damageGrow = value;
                base._isDirty = true;
            }
        }

        public int Defence
        {
            get
            {
                return this._defence;
            }
            set
            {
                this._defence = value;
                base._isDirty = true;
            }
        }

        public int DefenceGrow
        {
            get
            {
                return this._defenceGrow;
            }
            set
            {
                this._defenceGrow = value;
                base._isDirty = true;
            }
        }

        public List<PetEquipDataInfo> EquipList
        {
            get
            {
                return this.m_peEquip;
            }
            set
            {
                this.m_peEquip = value;
            }
        }

        public int GP
        {
            get
            {
                return this._gp;
            }
            set
            {
                this._gp = value;
                base._isDirty = true;
            }
        }

        public int Guard
        {
            get
            {
                return this._guard;
            }
            set
            {
                this._guard = value;
                base._isDirty = true;
            }
        }

        public int GuardGrow
        {
            get
            {
                return this._guardGrow;
            }
            set
            {
                this._guardGrow = value;
                base._isDirty = true;
            }
        }

        public int Hunger
        {
            get
            {
                return this._hunger;
            }
            set
            {
                this._hunger = value;
                base._isDirty = true;
            }
        }

        public int ID
        {
            get
            {
                return this._ID;
            }
            set
            {
                this._ID = value;
                base._isDirty = true;
            }
        }

        public bool IsEquip
        {
            get
            {
                return this._isEquip;
            }
            set
            {
                this._isEquip = value;
                base._isDirty = true;
            }
        }

        public bool IsExit
        {
            get
            {
                return this._isExit;
            }
            set
            {
                this._isExit = value;
                base._isDirty = true;
            }
        }

        public int Level
        {
            get
            {
                return this._level;
            }
            set
            {
                this._level = value;
                base._isDirty = true;
            }
        }

        public int Luck
        {
            get
            {
                return this._luck;
            }
            set
            {
                this._luck = value;
                base._isDirty = true;
            }
        }

        public int LuckGrow
        {
            get
            {
                return this._luckGrow;
            }
            set
            {
                this._luckGrow = value;
                base._isDirty = true;
            }
        }

        public int MaxGP
        {
            get
            {
                return this._maxGP;
            }
            set
            {
                this._maxGP = value;
                base._isDirty = true;
            }
        }

        public int MP
        {
            get
            {
                return this._mp;
            }
            set
            {
                this._mp = value;
                base._isDirty = true;
            }
        }

        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
                base._isDirty = true;
            }
        }

        public int PetHappyStar
        {
            get
            {
                return this.happyPercent();
            }
        }

        public int PetID
        {
            get
            {
                return this._petID;
            }
            set
            {
                this._petID = value;
                base._isDirty = true;
            }
        }

        public int Place
        {
            get
            {
                return this._place;
            }
            set
            {
                this._place = value;
                base._isDirty = true;
            }
        }

        public string Skill
        {
            get
            {
                return this._skill;
            }
            set
            {
                this._skill = value;
                base._isDirty = true;
            }
        }

        public string SkillEquip
        {
            get
            {
                return this._skillEquip;
            }
            set
            {
                this._skillEquip = value;
                base._isDirty = true;
            }
        }

        public int TemplateID
        {
            get
            {
                return this._templateID;
            }
            set
            {
                this._templateID = value;
                base._isDirty = true;
            }
        }

        public int TotalAgility
        {
            get
            {
                int num = 0;
                int num2 = 0;
                if (this.m_peEquip != null)
                {
                    while (num2 < this.m_peEquip.Count)
                    {
                        ItemTemplateInfo template = this.m_peEquip[num2].Template;
                        if (template != null)
                        {
                            num += template.Agility;
                        }
                        num2++;
                    }
                }
                return ((this._agility - this.ReduceProp(this._agility)) + num);
            }
        }

        public int TotalAttack
        {
            get
            {
                int num = 0;
                int num2 = 0;
                int num3 = 0;
                if (this.m_peEquip != null)
                {
                    while (num2 < this.m_peEquip.Count)
                    {
                        ItemTemplateInfo template = this.m_peEquip[num2].Template;
                        if (template != null)
                        {
                            num += template.Attack;
                        }
                        num2++;
                    }
                }
                return (((this._attack - this.ReduceProp(this._attack)) + num) + num3);
            }
        }

        public int TotalBlood
        {
            get
            {
                return (this._blood - this.ReduceProp(this._blood));
            }
        }

        public int TotalDamage
        {
            get
            {
                return (this._damage - this.ReduceProp(this._damage));
            }
        }

        public int TotalDefence
        {
            get
            {
                int num = 0;
                int num2 = 0;
                if (this.m_peEquip != null)
                {
                    while (num2 < this.m_peEquip.Count)
                    {
                        ItemTemplateInfo template = this.m_peEquip[num2].Template;
                        if (template != null)
                        {
                            num += template.Defence;
                        }
                        num2++;
                    }
                }
                return ((this._defence - this.ReduceProp(this._defence)) + num);
            }
        }

        public int TotalGuard
        {
            get
            {
                return (this._guard - this.ReduceProp(this._guard));
            }
        }

        public int TotalLuck
        {
            get
            {
                int num = 0;
                int num2 = 0;
                if (this.m_peEquip != null)
                {
                    while (num2 < this.m_peEquip.Count)
                    {
                        ItemTemplateInfo template = this.m_peEquip[num2].Template;
                        if (template != null)
                        {
                            num += template.Luck;
                        }
                        num2++;
                    }
                }
                return ((this._luck - this.ReduceProp(this._luck)) + num);
            }
        }

        public int UserID
        {
            get
            {
                return this._userID;
            }
            set
            {
                this._userID = value;
                base._isDirty = true;
            }
        }
    }
}

