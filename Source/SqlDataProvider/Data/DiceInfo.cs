namespace SqlDataProvider.Data
{
    using System;
    using System.Collections.Generic;

    public class DiceInfo
    {
        private int m_currentPosition = -1;
        private List<DiceItem> m_ItemDice;
        private int m_LuckIntegral;
        private int m_LuckIntegralLevel = -1;
        private int m_UserID;
        private bool userFirstCell;

        public int CurrentPosition
        {
            get
            {
                return this.m_currentPosition;
            }
            set
            {
                this.m_currentPosition = value;
            }
        }

        public List<DiceItem> ItemDice
        {
            get
            {
                if (this.m_ItemDice == null)
                {
                    this.m_ItemDice = new List<DiceItem>();
                }
                return this.m_ItemDice;
            }
            set
            {
                this.m_ItemDice = value;
            }
        }

        public int LuckIntegral
        {
            get
            {
                return this.m_LuckIntegral;
            }
            set
            {
                this.m_LuckIntegral = value;
            }
        }

        public int LuckIntegralLevel
        {
            get
            {
                return this.m_LuckIntegralLevel;
            }
            set
            {
                this.m_LuckIntegralLevel = value;
            }
        }

        public bool UserFirstCell
        {
            get
            {
                return this.userFirstCell;
            }
            set
            {
                this.userFirstCell = value;
            }
        }

        public int UserID
        {
            get
            {
                return this.m_UserID;
            }
            set
            {
                this.m_UserID = value;
            }
        }
    }
}

