namespace Game.Server.Quests
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Server.GameObjects;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class QuestInventoryOld
    {
        private Dictionary<int, QuestDataInfo> _currentQuest;
        private object _lock;
        private GamePlayer _player;

        public QuestInventoryOld(GamePlayer player)
        {
            this._player = player;
            this._lock = new object();
            this._currentQuest = new Dictionary<int, QuestDataInfo>();
        }

        public bool AddQuest(int questID, out string msg)
        {
            QuestInfo singleQuest = QuestMgr.GetSingleQuest(questID);
            msg = "未开始";
            return false;
        }

        public bool ClearConsortiaQuest()
        {
            if (this._player.PlayerCharacter.ConsortiaID != 0)
            {
                return false;
            }
            lock (this._lock)
            {
                foreach (QuestDataInfo info in this._currentQuest.Values)
                {
                }
            }
            return true;
        }

        public bool ClearMarryQuest()
        {
            return true;
        }

        public void CheckClient(int questID, int count)
        {
        }

        public void CheckCompose(int itemID)
        {
        }

        public void CheckKillPlayer(int map, int fightMode, int timeMode, bool captain, int killLevel, int selfCount, int rivalCount, int relation, int roomType)
        {
        }

        public void CheckStrengthen(int strengthenLevel, int categoryID)
        {
        }

        public void CheckUseItem(int itemID)
        {
        }

        public void CheckWin(int map, int fightMode, int timeMode, bool captain, int selfCount, int rivalCount, bool isWin, bool isFightConsortia, int roomType, bool isMarry)
        {
        }

        public bool FinishQuest(int questID)
        {
            return true;
        }

        public QuestDataInfo[] GetALlQuest()
        {
            QuestDataInfo[] array = null;
            lock (this._lock)
            {
                this._currentQuest.Values.CopyTo(array, 0);
            }
            return ((array == null) ? new QuestDataInfo[0] : array);
        }

        public QuestDataInfo GetCurrentQuest(int questID)
        {
            lock (this._lock)
            {
                if (this._currentQuest.ContainsKey(questID))
                {
                    return this._currentQuest[questID];
                }
            }
            return null;
        }

        public QuestDataInfo GetCurrentQuest(int questID, bool isExist)
        {
            lock (this._lock)
            {
                if (this._currentQuest.ContainsKey(questID) && (this._currentQuest[questID].IsExist == isExist))
                {
                    return this._currentQuest[questID];
                }
            }
            return null;
        }

        public int GetQuestCount()
        {
            int num = 0;
            lock (this._lock)
            {
                foreach (QuestDataInfo info in this._currentQuest.Values)
                {
                    if (!info.IsComplete && info.IsExist)
                    {
                        num++;
                    }
                }
            }
            return num;
        }

        public Dictionary<int, int> GetRequestItems()
        {
            return new Dictionary<int, int>();
        }

        public void LoadFromDatabase(int playerId)
        {
            lock (this._lock)
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    QuestDataInfo[] userQuest = bussiness.GetUserQuest(playerId);
                    foreach (QuestDataInfo info in userQuest)
                    {
                        if (!this._currentQuest.ContainsKey(info.QuestID))
                        {
                            this._currentQuest.Add(info.QuestID, info);
                        }
                    }
                }
            }
            this.ClearConsortiaQuest();
            this.ClearMarryQuest();
        }

        public bool RemoveQuest(int questID)
        {
            QuestDataInfo currentQuest = this.GetCurrentQuest(questID, true);
            if (!((currentQuest == null) || currentQuest.IsComplete))
            {
                currentQuest.IsExist = false;
                return true;
            }
            return false;
        }

        public void SaveToDatabase()
        {
            lock (this._lock)
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    foreach (QuestDataInfo info in this._currentQuest.Values)
                    {
                        if (info.IsDirty)
                        {
                            bussiness.UpdateDbQuestDataInfo(info);
                        }
                    }
                }
            }
        }
    }
}

