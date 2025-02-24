namespace Game.Server.Managers
{
    using Bussiness;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Reflection;

    public class CardMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static CardUpdateInfo[] m_cardBox;
        private static Dictionary<int, List<CardUpdateInfo>> m_cardBoxs;
        public static CardUpdateConditionInfo[] m_CardUpdateConditionInfo;
        private static CardGrooveUpdateInfo[] m_grooveUpdate;
        private static Dictionary<int, List<CardGrooveUpdateInfo>> m_grooveUpdates;
        private static ThreadSafeRandom random = new ThreadSafeRandom();

        public static int CardCount()
        {
            return m_cardBox.Count<CardUpdateInfo>();
        }

        public static List<CardUpdateInfo> FindCardBox(int cardId)
        {
            if (m_cardBoxs == null)
            {
                Init();
            }
            if (m_cardBoxs.ContainsKey(cardId))
            {
                return m_cardBoxs[cardId];
            }
            return null;
        }

        public static List<CardGrooveUpdateInfo> FindCardGrooveUpdate(int type)
        {
            if (m_grooveUpdates == null)
            {
                Init();
            }
            if (m_grooveUpdates.ContainsKey(type))
            {
                return m_grooveUpdates[type];
            }
            return null;
        }

        public static CardUpdateInfo FindCardTemplate(int cardId)
        {
            if (m_cardBoxs == null)
            {
                Init();
            }
            if (m_cardBoxs.ContainsKey(cardId))
            {
                List<CardUpdateInfo> list = m_cardBoxs[cardId];
                using (List<CardUpdateInfo>.Enumerator enumerator = list.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        return enumerator.Current;
                    }
                }
            }
            return null;
        }

        public static int FindLv(int CardGP)
        {
            foreach (CardUpdateConditionInfo info in m_CardUpdateConditionInfo)
            {
                if (info.Exp > CardGP)
                {
                    return (info.Level - 1);
                }
            }
            return 0;
        }

        public static List<CardUpdateInfo> GetAllCard()
        {
            if (m_cardBox == null)
            {
                Init();
            }
            List<CardUpdateInfo> list = new List<CardUpdateInfo>();
            Dictionary<int, CardUpdateInfo> dictionary = new Dictionary<int, CardUpdateInfo>();
            foreach (CardUpdateInfo info in m_cardBox)
            {
                if (!dictionary.Keys.Contains<int>(info.Id))
                {
                    if (info.Id != 0x4cb26)
                    {
                        list.Add(info);
                    }
                    dictionary.Add(info.Id, info);
                }
            }
            return list;
        }

        public static CardUpdateInfo GetCard(int cardId)
        {
            CardUpdateInfo info = new CardUpdateInfo();
            List<CardUpdateInfo> list = FindCardBox(cardId);
            if (list == null)
            {
                return null;
            }
            int num2 = 0;
            while (num2 < list.Count)
            {
                return list[num2];
            }
            return info;
        }

        public static int GetGP(int level, int type)
        {
            for (int i = 1; i <= MaxLv(type); i++)
            {
                if (level == FindCardGrooveUpdate(type)[i].Level)
                {
                    return FindCardGrooveUpdate(type)[i].Exp;
                }
            }
            return 0;
        }

        public static int GetGrooveSlot(int lv, int typeProp)
        {
            foreach (CardUpdateInfo info in m_cardBox)
            {
                if (info.Level == lv)
                {
                    switch (typeProp)
                    {
                        case 0:
                            return info.Attack;

                        case 1:
                            return info.Defend;

                        case 2:
                            return info.Agility;

                        case 3:
                            return info.Lucky;

                        case 4:
                            return info.Damage;

                        case 5:
                            return info.Guard;
                    }
                }
            }
            return 0;
        }

        public static int GetLevel(int GP, int type)
        {
            if (GP >= FindCardGrooveUpdate(type)[MaxLv(type)].Exp)
            {
                return FindCardGrooveUpdate(type)[MaxLv(type)].Level;
            }
            for (int i = 1; i <= MaxLv(type); i++)
            {
                if (GP < FindCardGrooveUpdate(type)[i].Exp)
                {
                    int num2 = ((i - 1) == -1) ? 0 : (i - 1);
                    return FindCardGrooveUpdate(type)[num2].Level;
                }
            }
            return 0;
        }

        public static int GetProp(UsersCardInfo slot, int type)
        {
            int num = 0;
            for (int i = 0; i <= slot.Level; i++)
            {
                num += GetGrooveSlot(i, type);
            }
            if (slot.CardID != 0)
            {
                num += GetPropCard(slot.CardID, type);
            }
            return num;
        }

        public static int GetPropCard(int cardID, int type)
        {
            foreach (CardUpdateInfo info in m_cardBox)
            {
                if (info.Id == cardID)
                {
                    switch (type)
                    {
                        case 0:
                            return info.Attack;

                        case 1:
                            return info.Defend;

                        case 2:
                            return info.Agility;

                        case 3:
                            return info.Lucky;

                        case 4:
                            return info.Damage;

                        case 5:
                            return info.Guard;
                    }
                }
            }
            return 0;
        }

        public static int[] GetRandomUnrepeatArray(int minValue, int maxValue, int count)
        {
            int[] numArray = new int[count];
            for (int i = 0; i < count; i++)
            {
                int num2 = random.Next(minValue, maxValue + 1);
                int num3 = 0;
                for (int j = 0; j < i; j++)
                {
                    if (numArray[j] == num2)
                    {
                        num3++;
                    }
                }
                if (num3 == 0)
                {
                    numArray[i] = num2;
                }
                else
                {
                    i--;
                }
            }
            return numArray;
        }

        public static CardUpdateInfo GetSingleCard(int id)
        {
            List<CardUpdateInfo> allCard = GetAllCard();
            foreach (CardUpdateInfo info in allCard)
            {
                if (info.Id == id)
                {
                    return info;
                }
            }
            return null;
        }

        public static CardUpdateInfo GetSingleCardByTemplae(int TemplateID)
        {
            List<CardUpdateInfo> allCard = GetAllCard();
            foreach (CardUpdateInfo info in allCard)
            {
                if (info.Id == TemplateID)
                {
                    return info;
                }
            }
            return null;
        }

        public static bool Init()
        {
            return ReLoad();
        }

        public static CardUpdateInfo[] LoadCardBoxDb()
        {
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                return bussiness.GetAllCardUpdateInfo();
            }
        }

        public static Dictionary<int, List<CardUpdateInfo>> LoadCardBoxs(CardUpdateInfo[] CardBoxs)
        {
            Dictionary<int, List<CardUpdateInfo>> dictionary = new Dictionary<int, List<CardUpdateInfo>>();
            for (int i = 0; i < CardBoxs.Length; i++)
            {
                Func<CardUpdateInfo, bool> predicate = null;
                CardUpdateInfo info = CardBoxs[i];
                if (!dictionary.Keys.Contains<int>(info.Id))
                {
                    if (predicate == null)
                    {
                        predicate = s => s.Id == info.Id;
                    }
                    IEnumerable<CardUpdateInfo> source = CardBoxs.Where<CardUpdateInfo>(predicate);
                    dictionary.Add(info.Id, source.ToList<CardUpdateInfo>());
                }
            }
            return dictionary;
        }

        public static CardGrooveUpdateInfo[] LoadGrooveUpdateDb()
        {
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                return bussiness.GetAllCardGrooveUpdate();
            }
        }

        public static Dictionary<int, List<CardGrooveUpdateInfo>> LoadGrooveUpdates(CardGrooveUpdateInfo[] GrooveUpdates)
        {
            Dictionary<int, List<CardGrooveUpdateInfo>> dictionary = new Dictionary<int, List<CardGrooveUpdateInfo>>();
            for (int i = 0; i < GrooveUpdates.Length; i++)
            {
                Func<CardGrooveUpdateInfo, bool> predicate = null;
                CardGrooveUpdateInfo info = GrooveUpdates[i];
                if (!dictionary.Keys.Contains<int>(info.Type))
                {
                    if (predicate == null)
                    {
                        predicate = s => s.Type == info.Type;
                    }
                    IEnumerable<CardGrooveUpdateInfo> source = GrooveUpdates.Where<CardGrooveUpdateInfo>(predicate);
                    dictionary.Add(info.Type, source.ToList<CardGrooveUpdateInfo>());
                }
            }
            return dictionary;
        }

        public static int MaxLv(int type)
        {
            return (FindCardGrooveUpdate(type).Count - 1);
        }

        public static bool ReLoad()
        {
            try
            {
                CardGrooveUpdateInfo[] grooveUpdates = LoadGrooveUpdateDb();
                Dictionary<int, List<CardGrooveUpdateInfo>> dictionary = LoadGrooveUpdates(grooveUpdates);
                m_CardUpdateConditionInfo = new PlayerBussiness().GetAllCardUpdateCondition();
                if (grooveUpdates != null)
                {
                    Interlocked.Exchange<CardGrooveUpdateInfo[]>(ref m_grooveUpdate, grooveUpdates);
                    Interlocked.Exchange<Dictionary<int, List<CardGrooveUpdateInfo>>>(ref m_grooveUpdates, dictionary);
                }
                CardUpdateInfo[] cardBoxs = LoadCardBoxDb();
                Dictionary<int, List<CardUpdateInfo>> dictionary2 = LoadCardBoxs(cardBoxs);
                if (cardBoxs != null)
                {
                    Interlocked.Exchange<CardUpdateInfo[]>(ref m_cardBox, cardBoxs);
                    Interlocked.Exchange<Dictionary<int, List<CardUpdateInfo>>>(ref m_cardBoxs, dictionary2);
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ReLoad", exception);
                }
                return false;
            }
            return true;
        }
    }
}

