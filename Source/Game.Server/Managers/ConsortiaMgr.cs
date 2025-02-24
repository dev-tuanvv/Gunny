namespace Game.Server.Managers
{
    using Bussiness;
    using Game.Logic;
    using Game.Server.GameObjects;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Game.Logic.Phy.Object;
    using System.Reflection;

    public class ConsortiaMgr
    {
        private static Dictionary<string, int> _ally;
        private static Dictionary<int, ConsortiaInfo> _consortia;
        private static Dictionary<int, ConsortiaBossConfigInfo> _consortiaBossConfigInfos;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock;

        public static bool AddConsortia(int consortiaID)
        {
            m_lock.AcquireWriterLock(-1);
            try
            {
                if (!_consortia.ContainsKey(consortiaID))
                {
                    ConsortiaInfo info = new ConsortiaInfo {
                        BuildDate = DateTime.Now,
                        Level = 1,
                        IsExist = true,
                        ConsortiaName = "",
                        ConsortiaID = consortiaID
                    };
                    _consortia.Add(consortiaID, info);
                }
            }
            catch (Exception exception)
            {
                log.Error("ConsortiaUpGrade", exception);
            }
            finally
            {
                m_lock.ReleaseWriterLock();
            }
            return false;
        }

        public static int CanConsortiaFight(int consortiaID1, int consortiaID2)
        {
            if (((consortiaID1 == 0) || (consortiaID2 == 0)) || (consortiaID1 == consortiaID2))
            {
                return -1;
            }
            ConsortiaInfo info = FindConsortiaInfo(consortiaID1);
            ConsortiaInfo info2 = FindConsortiaInfo(consortiaID2);
            if ((((info == null) || (info2 == null)) || (info.Level < 3)) || (info2.Level < 3))
            {
                return -1;
            }
            return FindConsortiaAlly(consortiaID1, consortiaID2);
        }

        public static int ConsortiaFight(int consortiaWin, int consortiaLose, Dictionary<int, Player> players, eRoomType roomType, eGameType gameClass, int totalKillHealth, int playercount)
        {
            if (roomType != eRoomType.Match)
            {
                return 0;
            }
            int playerCount = playercount / 2;
            int riches = 0;
            int state = 2;
            int num4 = 1;
            int num5 = 3;
            if (gameClass == eGameType.Guild)
            {
                num5 = 10;
                num4 = (int) RateMgr.GetRate(eRateType.Offer_Rate);
            }
            float rate = RateMgr.GetRate(eRateType.Riches_Rate);
            using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
            {
                if (gameClass == eGameType.Free)
                {
                    playerCount = 0;
                }
                else
                {
                    bussiness.ConsortiaFight(consortiaWin, consortiaLose, playerCount, out riches, state, totalKillHealth, rate);
                }
                foreach (KeyValuePair<int, Player> pair in players)
                {
                    if (pair.Value != null)
                    {
                        if (pair.Value.PlayerDetail.PlayerCharacter.ConsortiaID == consortiaWin)
                        {
                            pair.Value.PlayerDetail.AddOffer((playerCount + num5) * num4);
                            PlayerInfo playerCharacter = pair.Value.PlayerDetail.PlayerCharacter;
                            playerCharacter.RichesRob += riches;
                        }
                        else if (pair.Value.PlayerDetail.PlayerCharacter.ConsortiaID == consortiaLose)
                        {
                            pair.Value.PlayerDetail.AddOffer(((int) Math.Round((double) (playerCount * 0.5))) * num4);
                            pair.Value.PlayerDetail.RemoveOffer(num5);
                        }
                    }
                }
            }
            return riches;
        }

        public static bool ConsortiaShopUpGrade(int consortiaID, int shopLevel)
        {
            m_lock.AcquireWriterLock(-1);
            try
            {
                if (_consortia.ContainsKey(consortiaID) && _consortia[consortiaID].IsExist)
                {
                    _consortia[consortiaID].ShopLevel = shopLevel;
                }
            }
            catch (Exception exception)
            {
                log.Error("ConsortiaUpGrade", exception);
            }
            finally
            {
                m_lock.ReleaseWriterLock();
            }
            return false;
        }

        public static bool ConsortiaSmithUpGrade(int consortiaID, int smithLevel)
        {
            m_lock.AcquireWriterLock(-1);
            try
            {
                if (_consortia.ContainsKey(consortiaID) && _consortia[consortiaID].IsExist)
                {
                    _consortia[consortiaID].SmithLevel = smithLevel;
                }
            }
            catch (Exception exception)
            {
                log.Error("ConsortiaUpGrade", exception);
            }
            finally
            {
                m_lock.ReleaseWriterLock();
            }
            return false;
        }

        public static bool ConsortiaStoreUpGrade(int consortiaID, int storeLevel)
        {
            m_lock.AcquireWriterLock(-1);
            try
            {
                if (_consortia.ContainsKey(consortiaID) && _consortia[consortiaID].IsExist)
                {
                    _consortia[consortiaID].StoreLevel = storeLevel;
                }
            }
            catch (Exception exception)
            {
                log.Error("ConsortiaUpGrade", exception);
            }
            finally
            {
                m_lock.ReleaseWriterLock();
            }
            return false;
        }

        public static bool ConsortiaUpGrade(int consortiaID, int consortiaLevel)
        {
            bool flag = false;
            m_lock.AcquireWriterLock(-1);
            try
            {
                if (_consortia.ContainsKey(consortiaID) && _consortia[consortiaID].IsExist)
                {
                    _consortia[consortiaID].Level = consortiaLevel;
                    return flag;
                }
                ConsortiaInfo info = new ConsortiaInfo {
                    BuildDate = DateTime.Now,
                    Level = consortiaLevel,
                    IsExist = true
                };
                _consortia.Add(consortiaID, info);
                return flag;
            }
            catch (Exception exception)
            {
                log.Error("ConsortiaUpGrade", exception);
            }
            finally
            {
                m_lock.ReleaseWriterLock();
            }
            return flag;
        }

        public static int FindConsortiaAlly(int cosortiaID1, int consortiaID2)
        {
            string str;
            if (((cosortiaID1 == 0) || (consortiaID2 == 0)) || (cosortiaID1 == consortiaID2))
            {
                return -1;
            }
            if (cosortiaID1 < consortiaID2)
            {
                str = cosortiaID1 + "&" + consortiaID2;
            }
            else
            {
                str = consortiaID2 + "&" + cosortiaID1;
            }
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (_ally.ContainsKey(str))
                {
                    return _ally[str];
                }
            }
            catch
            {
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return 0;
        }

        public static int FindConsortiaBossBossMaxLevel(int param1, ConsortiaInfo info)
        {
            int num;
            if (param1 == 0)
            {
                num = (((info.Level + info.SmithLevel) + info.ShopLevel) + info.StoreLevel) + info.SkillLevel;
            }
            else
            {
                num = param1;
            }
            for (int i = _consortiaBossConfigInfos.Count; i >= 0; i--)
            {
                if (num >= _consortiaBossConfigInfos[i].Level)
                {
                    return i;
                }
            }
            return 1;
        }

        public static ConsortiaBossConfigInfo FindConsortiaBossConfig(int level)
        {
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (_consortiaBossConfigInfos.ContainsKey(level))
                {
                    return _consortiaBossConfigInfos[level];
                }
            }
            catch
            {
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return null;
        }

        public static ConsortiaInfo FindConsortiaInfo(int consortiaID)
        {
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (_consortia.ContainsKey(consortiaID))
                {
                    return _consortia[consortiaID];
                }
            }
            catch
            {
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return null;
        }

        private static int GetOffer(int state, eGameType gameType)
        {
            switch (gameType)
            {
                case eGameType.Free:
                    switch (state)
                    {
                        case 0:
                            return 1;

                        case 1:
                            return 0;

                        case 2:
                            return 3;
                    }
                    break;

                case eGameType.Guild:
                    switch (state)
                    {
                        case 0:
                            return 5;

                        case 1:
                            return 0;

                        case 2:
                            return 10;
                    }
                    break;
            }
            return 0;
        }

        public static int GetOffer(int cosortiaID1, int consortiaID2, eGameType gameType)
        {
            return GetOffer(FindConsortiaAlly(cosortiaID1, consortiaID2), gameType);
        }

        public static bool Init()
        {
            try
            {
                m_lock = new ReaderWriterLock();
                _ally = new Dictionary<string, int>();
                if (!Load(_ally))
                {
                    return false;
                }
                _consortia = new Dictionary<int, ConsortiaInfo>();
                _consortiaBossConfigInfos = new Dictionary<int, ConsortiaBossConfigInfo>();
                if (!LoadConsortia(_consortia, _consortiaBossConfigInfos))
                {
                    return false;
                }
                return true;
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ConsortiaMgr", exception);
                }
                return false;
            }
        }

        public static int KillPlayer(GamePlayer win, GamePlayer lose, Dictionary<GamePlayer, Player> players, eRoomType roomType, eGameType gameClass)
        {
            if (roomType != eRoomType.Match)
            {
                return -1;
            }
            int state = FindConsortiaAlly(win.PlayerCharacter.ConsortiaID, lose.PlayerCharacter.ConsortiaID);
            if (state != -1)
            {
                int offer = GetOffer(state, gameClass);
                if (lose.PlayerCharacter.Offer < offer)
                {
                    offer = lose.PlayerCharacter.Offer;
                }
                if (offer != 0)
                {
                    players[win].GainOffer = offer;
                    players[lose].GainOffer = -offer;
                }
            }
            return state;
        }

        private static bool Load(Dictionary<string, int> ally)
        {
            using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
            {
                foreach (ConsortiaAllyInfo info in bussiness.GetConsortiaAllyAll())
                {
                    if (info.IsExist)
                    {
                        string str;
                        if (info.Consortia1ID < info.Consortia2ID)
                        {
                            str = info.Consortia1ID + "&" + info.Consortia2ID;
                        }
                        else
                        {
                            str = info.Consortia2ID + "&" + info.Consortia1ID;
                        }
                        if (!ally.ContainsKey(str))
                        {
                            ally.Add(str, info.State);
                        }
                    }
                }
            }
            return true;
        }

        private static bool LoadConsortia(Dictionary<int, ConsortiaInfo> consortia, Dictionary<int, ConsortiaBossConfigInfo> consortiaBossConfig)
        {
            using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
            {
                foreach (ConsortiaInfo info in bussiness.GetConsortiaAll())
                {
                    if (!(!info.IsExist || consortia.ContainsKey(info.ConsortiaID)))
                    {
                        consortia.Add(info.ConsortiaID, info);
                    }
                }
                foreach (ConsortiaBossConfigInfo info2 in bussiness.GetConsortiaBossConfigAll())
                {
                    if (!consortiaBossConfig.ContainsKey(info2.BossLevel))
                    {
                        consortiaBossConfig.Add(info2.BossLevel, info2);
                    }
                }
            }
            return true;
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<string, int> ally = new Dictionary<string, int>();
                Dictionary<int, ConsortiaInfo> consortia = new Dictionary<int, ConsortiaInfo>();
                Dictionary<int, ConsortiaBossConfigInfo> consortiaBossConfig = new Dictionary<int, ConsortiaBossConfigInfo>();
                if (Load(ally) && LoadConsortia(consortia, consortiaBossConfig))
                {
                    m_lock.AcquireWriterLock(-1);
                    try
                    {
                        _ally = ally;
                        _consortia = consortia;
                        _consortiaBossConfigInfos = consortiaBossConfig;
                        return true;
                    }
                    catch
                    {
                    }
                    finally
                    {
                        m_lock.ReleaseWriterLock();
                    }
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ConsortiaMgr", exception);
                }
            }
            return false;
        }

        public static int UpdateConsortiaAlly(int cosortiaID1, int consortiaID2, int state)
        {
            string str;
            if (cosortiaID1 < consortiaID2)
            {
                str = cosortiaID1 + "&" + consortiaID2;
            }
            else
            {
                str = consortiaID2 + "&" + cosortiaID1;
            }
            m_lock.AcquireWriterLock(-1);
            try
            {
                if (!_ally.ContainsKey(str))
                {
                    _ally.Add(str, state);
                }
                else
                {
                    _ally[str] = state;
                }
            }
            catch
            {
            }
            finally
            {
                m_lock.ReleaseWriterLock();
            }
            return 0;
        }
    }
}

