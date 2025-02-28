﻿namespace SqlDataProvider.Data
{
    using System;

    public class DiceItem
    {
        private int m_count;
        private bool m_isBind;
        private int m_position;
        private int m_strengthLevel;
        private int m_TemplateID;
        private int m_validate;

        public int Count
        {
            get
            {
                return this.m_count;
            }
            set
            {
                this.m_count = value;
            }
        }

        public bool IsBind
        {
            get
            {
                return this.m_isBind;
            }
            set
            {
                this.m_isBind = value;
            }
        }

        public int Position
        {
            get
            {
                return this.m_position;
            }
            set
            {
                this.m_position = value;
            }
        }

        public int StrengthLevel
        {
            get
            {
                return this.m_strengthLevel;
            }
            set
            {
                this.m_strengthLevel = value;
            }
        }

        public int TemplateID
        {
            get
            {
                return this.m_TemplateID;
            }
            set
            {
                this.m_TemplateID = value;
            }
        }

        public int Validate
        {
            get
            {
                return this.m_validate;
            }
            set
            {
                this.m_validate = value;
            }
        }
    }
}

