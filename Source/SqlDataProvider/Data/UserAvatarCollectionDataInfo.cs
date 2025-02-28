﻿namespace SqlDataProvider.Data
{
    using System;

    public class UserAvatarCollectionDataInfo
    {
        private int _Sex;
        private int _TemplateID;

        public UserAvatarCollectionDataInfo()
        {
        }

        public UserAvatarCollectionDataInfo(int templateid, int sex)
        {
            this._TemplateID = templateid;
            this._Sex = sex;
        }

        public int Sex
        {
            get
            {
                return this._Sex;
            }
            set
            {
                this._Sex = value;
            }
        }

        public int TemplateID
        {
            get
            {
                return this._TemplateID;
            }
            set
            {
                this._TemplateID = value;
            }
        }
    }
}

