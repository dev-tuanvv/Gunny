namespace Game.Server.GameUtils
{
    using Game.Server.GameObjects;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class PlayerProperty
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected int m_loading;
        private Dictionary<string, Dictionary<string, int>> m_otherPlayerProperty;
        protected GamePlayer m_player;
        private Dictionary<string, Dictionary<string, int>> m_playerProperty;
        protected int m_totalArmor;
        protected int m_totalDamage;

        public PlayerProperty(GamePlayer player)
        {
            this.m_player = player;
            this.m_playerProperty = new Dictionary<string, Dictionary<string, int>>();
            this.m_otherPlayerProperty = new Dictionary<string, Dictionary<string, int>>();
            this.m_loading = 0;
            this.m_totalDamage = 0;
            this.m_totalArmor = 0;
            this.CreateProp(true, "Texp", 0, 0, 0, 0, 0);
            this.CreateProp(true, "Card", 0, 0, 0, 0, 0);
            this.CreateProp(true, "Pet", 0, 0, 0, 0, 0);
            this.CreateProp(true, "Suit", 0, 0, 0, 0, 0);
            this.CreateProp(true, "Gem", 0, 0, 0, 0, 0);
            this.CreateProp(true, "Bead", 0, 0, 0, 0, 0);
            this.CreateProp(true, "Avatar", 0, 0, 0, 0, 0);
            this.CreateProp(true, "MagicStone", 0, 0, 0, 0, 0);
            this.CreateBaseProp(true, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
            this.CreateProp(false, "Texp", 0, 0, 0, 0, 0);
            this.CreateProp(false, "Card", 0, 0, 0, 0, 0);
            this.CreateProp(false, "Pet", 0, 0, 0, 0, 0);
            this.CreateProp(true, "Suit", 0, 0, 0, 0, 0);
            this.CreateProp(false, "Gem", 0, 0, 0, 0, 0);
            this.CreateProp(false, "Bead", 0, 0, 0, 0, 0);
            this.CreateProp(false, "Avatar", 0, 0, 0, 0, 0);
            this.CreateProp(false, "MagicStone", 0, 0, 0, 0, 0);
            this.CreateBaseProp(false, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
        }

        public void AddOtherProp(string key, Dictionary<string, int> propAdd)
        {
            if (!this.m_playerProperty.ContainsKey(key))
            {
                this.m_otherPlayerProperty.Add(key, propAdd);
            }
            else
            {
                this.m_otherPlayerProperty[key] = propAdd;
            }
        }

        public void AddProp(string key, Dictionary<string, int> propAdd)
        {
            if (!this.m_playerProperty.ContainsKey(key))
            {
                this.m_playerProperty.Add(key, propAdd);
            }
            else
            {
                this.m_playerProperty[key] = propAdd;
            }
        }

        public void CreateBaseProp(bool isSelf, double beaddefence, double beadattack, double suitdefence, double suitattack, double avatarattack, double avatardefence, double magicstoneattack, double magicstonedefence)
        {
            Dictionary<string, int> propAdd = new Dictionary<string, int>();
            propAdd.Add("Bead", (int) beadattack);
            propAdd.Add("Suit", (int) suitattack);
            propAdd.Add("Avatar", (int) avatarattack);
            Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
            dictionary2.Add("Bead", (int) beaddefence);
            dictionary2.Add("Suit", (int) suitdefence);
            dictionary2.Add("Avatar", (int) avatardefence);
            Dictionary<string, int> dictionary3 = new Dictionary<string, int>();
            dictionary3.Add("MagicStone", (int) magicstoneattack);
            Dictionary<string, int> dictionary4 = new Dictionary<string, int>();
            dictionary4.Add("MagicStone", (int) magicstonedefence);
            if (isSelf)
            {
                this.AddProp("Damage", propAdd);
                this.AddProp("Armor", dictionary2);
                this.AddProp("MagicAttack", dictionary3);
                this.AddProp("MagicDefence", dictionary4);
            }
            else
            {
                this.AddOtherProp("Damage", propAdd);
                this.AddOtherProp("Armor", dictionary2);
                this.AddOtherProp("MagicAttack", dictionary3);
                this.AddOtherProp("MagicDefence", dictionary4);
            }
        }

        public void CreateProp(bool isSelf, string skey, int attack, int defence, int agility, int lucky, int hp)
        {
            Dictionary<string, int> propAdd = new Dictionary<string, int>();
            propAdd.Add("Attack", attack);
            propAdd.Add("Defence", defence);
            propAdd.Add("Agility", agility);
            propAdd.Add("Luck", lucky);
            propAdd.Add("HP", hp);
            if (isSelf)
            {
                this.AddProp(skey, propAdd);
            }
            else
            {
                this.AddOtherProp(skey, propAdd);
            }
        }

        public void UpadateBaseProp(bool isSelf, string mainKey, string subKey, double value)
        {
            if (isSelf)
            {
                if (this.m_playerProperty.ContainsKey(mainKey) && this.m_playerProperty[mainKey].ContainsKey(subKey))
                {
                    this.m_playerProperty[mainKey][subKey] = (int) value;
                }
            }
            else if (this.m_otherPlayerProperty.ContainsKey(mainKey) && this.m_otherPlayerProperty[mainKey].ContainsKey(subKey))
            {
                this.m_otherPlayerProperty[mainKey][subKey] = (int) value;
            }
        }

        public void ViewCurrent()
        {
            if (this.m_player.ShowPP)
            {
                this.m_player.Out.SendUpdatePlayerProperty(this.m_player.PlayerCharacter, this);
            }
        }

        public void ViewOther(PlayerInfo player)
        {
            this.m_player.Out.SendUpdatePlayerProperty(player, this);
        }

        public Dictionary<string, Dictionary<string, int>> Current
        {
            get
            {
                return this.m_playerProperty;
            }
            set
            {
                this.m_playerProperty = value;
            }
        }

        public int Loading
        {
            get
            {
                return this.m_loading;
            }
            set
            {
                this.m_loading = value;
            }
        }

        public Dictionary<string, Dictionary<string, int>> OtherPlayerProperty
        {
            get
            {
                return this.m_playerProperty;
            }
            set
            {
                this.m_playerProperty = value;
            }
        }

        public GamePlayer Player
        {
            get
            {
                return this.m_player;
            }
        }

        public int totalArmor
        {
            get
            {
                return this.m_totalArmor;
            }
            set
            {
                this.m_totalArmor = value;
            }
        }

        public int totalDamage
        {
            get
            {
                return this.m_totalDamage;
            }
            set
            {
                this.m_totalDamage = value;
            }
        }
    }
}

