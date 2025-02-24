namespace SqlDataProvider.Data
{
    using System;
    using System.Collections.Generic;

    public class UserAvatarCollectionInfo : DataObject
    {
        private int _AvatarID;
        private ClothPropertyTemplateInfo _clothProperty;
        private string _data;
        private int _ID;
        private bool _isActive;
        private bool _IsExit;
        private List<UserAvatarCollectionDataInfo> _items;
        private int _Sex;
        private DateTime _timeend;
        private DateTime _timeStart;
        private int _userID;

        public UserAvatarCollectionInfo()
        {
        }

        public UserAvatarCollectionInfo(int UserId, int AvatarID, int Sex, bool IsActive, DateTime timeend)
        {
            this._userID = UserId;
            this._AvatarID = AvatarID;
            this._Sex = Sex;
            this._isActive = IsActive;
            this._timeend = timeend;
            this._timeStart = DateTime.Now;
            this._data = "";
            this._IsExit = true;
        }

        public bool ActiveAvatar(int days)
        {
            if (days <= 0)
            {
                return false;
            }
            if (this._isActive && (this._timeend >= DateTime.Now))
            {
                this.TimeEnd = this.TimeEnd.AddDays((double) days);
                return true;
            }
            this.IsActive = true;
            this.TimeEnd = DateTime.Now.AddDays((double) days);
            return true;
        }

        public bool AddItem(UserAvatarCollectionDataInfo item)
        {
            if (this._items == null)
            {
                this._items = this.GetData();
            }
            if (this.GetItemWithTemplateID(item.TemplateID) == null)
            {
                this.Items.Add(item);
                this.SaveData();
                return true;
            }
            return false;
        }

        public List<UserAvatarCollectionDataInfo> GetData()
        {
            List<UserAvatarCollectionDataInfo> list = new List<UserAvatarCollectionDataInfo>();
            if (this._data == null)
            {
                this._data = "";
            }
            if (this._data.Length > 0)
            {
                string[] strArray = this._data.Split(new char[] { '|' });
                if (strArray.Length <= 0)
                {
                    return list;
                }
                foreach (string str in strArray)
                {
                    string[] strArray3 = str.Split(new char[] { ',' });
                    if (strArray3.Length >= 2)
                    {
                        UserAvatarCollectionDataInfo item = new UserAvatarCollectionDataInfo {
                            TemplateID = int.Parse(strArray3[0]),
                            Sex = int.Parse(strArray3[1])
                        };
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        public UserAvatarCollectionDataInfo GetItemWithTemplateID(int ItemID)
        {
            if (this._items == null)
            {
                this._items = this.GetData();
            }
            if (this._items.Count > 0)
            {
                foreach (UserAvatarCollectionDataInfo info in this._items)
                {
                    if (info.TemplateID == ItemID)
                    {
                        return info;
                    }
                }
                return null;
            }
            return null;
        }

        public bool IsAvalible()
        {
            return (this._isActive && (this._timeend > DateTime.Now));
        }

        public bool RemoveItem(UserAvatarCollectionDataInfo item)
        {
            if (this._items == null)
            {
                this._items = this.GetData();
            }
            if (this.GetItemWithTemplateID(item.TemplateID) != null)
            {
                this.Items.Remove(item);
                this.SaveData();
                return true;
            }
            return false;
        }

        public bool SaveData()
        {
            bool flag = false;
            if (this._items == null)
            {
                this._items = this.GetData();
            }
            string[] strArray = new string[2];
            List<string> list = new List<string>();
            if (this._items.Count > 0)
            {
                foreach (UserAvatarCollectionDataInfo info in this._items)
                {
                    strArray[0] = info.TemplateID.ToString();
                    strArray[1] = info.Sex.ToString();
                    string item = string.Join(",", strArray);
                    list.Add(item);
                }
                if (list.Count > 0)
                {
                    string str2 = string.Join("|", list.ToArray());
                    this.Data = str2;
                    flag = true;
                }
            }
            return flag;
        }

        public void UpdateItems()
        {
            if (this._items == null)
            {
                this._items = this.GetData();
            }
        }

        public int AvatarID
        {
            get
            {
                return this._AvatarID;
            }
            set
            {
                this._AvatarID = value;
                base._isDirty = true;
            }
        }

        public ClothPropertyTemplateInfo ClothProperty
        {
            get
            {
                return this._clothProperty;
            }
            set
            {
                this._clothProperty = value;
            }
        }

        public string Data
        {
            get
            {
                return this._data;
            }
            set
            {
                this._data = value;
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

        public bool IsActive
        {
            get
            {
                return this._isActive;
            }
            set
            {
                this._isActive = value;
                base._isDirty = true;
            }
        }

        public bool IsExit
        {
            get
            {
                return this._IsExit;
            }
            set
            {
                this._IsExit = value;
                base._isDirty = true;
            }
        }

        public List<UserAvatarCollectionDataInfo> Items
        {
            get
            {
                if (this._items == null)
                {
                    this._items = this.GetData();
                }
                return this._items;
            }
            set
            {
                this._items = value;
            }
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
                base._isDirty = true;
            }
        }

        public DateTime TimeEnd
        {
            get
            {
                return this._timeend;
            }
            set
            {
                this._timeend = value;
                base._isDirty = true;
            }
        }

        public DateTime TimeStart
        {
            get
            {
                return this._timeStart;
            }
            set
            {
                this._timeStart = value;
                base._isDirty = true;
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

