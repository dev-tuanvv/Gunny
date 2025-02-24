namespace SqlDataProvider.Data
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class SubActiveConditionInfo
    {
        public int GetValue(string index)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(this.Value))
            {
                string[] strArray = this.Value.Split(new char[] { '-' });
                for (int i = 1; i < strArray.Length; i += 2)
                {
                    string key = strArray[i - 1];
                    if (!dictionary.ContainsKey(key))
                    {
                        dictionary.Add(key, strArray[i]);
                    }
                    else
                    {
                        dictionary[key] = strArray[i];
                    }
                }
                if (dictionary.ContainsKey(index))
                {
                    return int.Parse(dictionary[index]);
                }
            }
            return 0;
        }

        public int ActiveID { get; set; }

        public int AwardType { get; set; }

        public string AwardValue { get; set; }

        public int ConditionID { get; set; }

        public int ID { get; set; }

        public bool IsValid { get; set; }

        public int SubID { get; set; }

        public int Type { get; set; }

        public string Value { get; set; }
    }
}

