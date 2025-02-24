namespace SqlDataProvider.Data
{
    using System;
    using System.Collections.Generic;

    public class PlayerInfoHistory : DataObject
    {
        private DateTime _lastQuestsTime;
        private DateTime _lastTreasureTime;
        private int _userID;
        private Dictionary<int, int> m_composeState;
        private object m_composeStateLocker = new object();
        private string m_composeStateString;
        private static readonly int m_exist = 15;

        private void ClearExpireComposeState()
        {
            int[] array = new int[this.m_composeState.Keys.Count];
            this.m_composeState.Keys.CopyTo(array, 0);
            Array.Sort<int>(array);
            int exist = m_exist;
            for (int i = array.Length - 1; i >= 0; i--)
            {
                exist--;
                if (exist < 0)
                {
                    this.m_composeState.Remove(array[i]);
                }
            }
        }

        public int ComposeStateLockIncrement(int key)
        {
            lock (this.m_composeStateLocker)
            {
                Dictionary<int, int> dictionary;
                base._isDirty = true;
                if (!this.m_composeState.ContainsKey(key))
                {
                    this.m_composeState.Add(key, 0);
                }
                (dictionary = this.m_composeState)[key] = dictionary[key] + 1;
                return this.m_composeState[key];
            }
        }

        private string JointStateString(Dictionary<int, int> stateDict)
        {
            List<string> list = new List<string>();
            Dictionary<int, int>.Enumerator enumerator = this.m_composeState.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<int, int> current = enumerator.Current;
                current = enumerator.Current;
                list.Add(string.Format("{0}-{1}", current.Key, current.Value));
            }
            return string.Join(",", list.ToArray());
        }

        private Dictionary<int, int> SplitStateString(string stateString)
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>();
            foreach (string str in stateString.Split(new char[] { ',' }))
            {
                string[] strArray3 = str.Split(new char[] { '-' });
                if (strArray3.Length == 2)
                {
                    int key = int.Parse(strArray3[0]);
                    if (!dictionary.ContainsKey(key))
                    {
                        dictionary.Add(key, int.Parse(strArray3[1]));
                    }
                }
            }
            return dictionary;
        }

        public string ComposeStateString
        {
            get
            {
                this.m_composeStateString = this.JointStateString(this.m_composeState);
                if (this.m_composeStateString.Length > 200)
                {
                    this.ClearExpireComposeState();
                    this.m_composeStateString = this.JointStateString(this.m_composeState);
                    if (this.m_composeStateString.Length > 200)
                    {
                        throw new ArgumentOutOfRangeException("the compose state string is too long, to fix the error you should clean the column Sys_Users_History.ComposeState in DB");
                    }
                }
                return this.m_composeStateString;
            }
            set
            {
                this.m_composeStateString = value;
                this.m_composeState = this.SplitStateString(this.m_composeStateString);
            }
        }

        public DateTime LastQuestsTime
        {
            get
            {
                return this._lastQuestsTime;
            }
            set
            {
                this._lastQuestsTime = value;
            }
        }

        public DateTime LastTreasureTime
        {
            get
            {
                return this._lastTreasureTime;
            }
            set
            {
                this._lastTreasureTime = value;
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
            }
        }
    }
}

