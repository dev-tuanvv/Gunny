namespace Game.Logic
{
    using Bussiness;
    using Game.Logic.Phy.Maps;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Reflection;

    public class MapMgr
    {
        private static Dictionary<int, Map> _mapInfos;
        private static Dictionary<int, MapPoint> _maps;
        private static Dictionary<int, List<int>> _serverMap;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock;
        private static ThreadSafeRandom random;

        public static Map CloneMap(int index)
        {
            if (_mapInfos.ContainsKey(index))
            {
                return _mapInfos[index].Clone();
            }
            return null;
        }

        public static MapInfo FindMapInfo(int index)
        {
            if (_mapInfos.ContainsKey(index))
            {
                return _mapInfos[index].Info;
            }
            return null;
        }

        public static int GetMapIndex(int index, byte type, int serverId)
        {
            if (!((index == 0) || _maps.Keys.Contains<int>(index)))
            {
                index = 0;
            }
            if (index != 0)
            {
                return index;
            }
            List<int> list = new List<int>();
            foreach (int num in _serverMap[serverId])
            {
                MapInfo info = FindMapInfo(num);
                if ((type & info.Type) != 0)
                {
                    list.Add(num);
                }
            }
            if (list.Count == 0)
            {
                int maxValue = _serverMap[serverId].Count;
                return _serverMap[serverId][random.Next(maxValue)];
            }
            int count = list.Count;
            return list[random.Next(count)];
        }

        public static MapPoint GetMapRandomPos(int index)
        {
            MapPoint point2;
            MapPoint point = new MapPoint();
            if (!((index == 0) || _maps.Keys.Contains<int>(index)))
            {
                index = 0;
            }
            if (index == 0)
            {
                int[] numArray = _maps.Keys.ToArray<int>();
                point2 = _maps[numArray[random.Next(numArray.Length)]];
            }
            else
            {
                point2 = _maps[index];
            }
            if (random.Next(2) == 1)
            {
                point.PosX.AddRange(point2.PosX);
                point.PosX1.AddRange(point2.PosX1);
                return point;
            }
            point.PosX.AddRange(point2.PosX1);
            point.PosX1.AddRange(point2.PosX);
            return point;
        }

        public static MapPoint GetPVEMapRandomPos(int index)
        {
            MapPoint point2;
            MapPoint point = new MapPoint();
            if (!((index == 0) || _maps.Keys.Contains<int>(index)))
            {
                index = 0;
            }
            if (index == 0)
            {
                int[] numArray = _maps.Keys.ToArray<int>();
                point2 = _maps[numArray[random.Next(numArray.Length)]];
            }
            else
            {
                point2 = _maps[index];
            }
            point.PosX.AddRange(point2.PosX);
            point.PosX1.AddRange(point2.PosX1);
            return point;
        }

        public static bool Init()
        {
            try
            {
                random = new ThreadSafeRandom();
                m_lock = new ReaderWriterLock();
                _maps = new Dictionary<int, MapPoint>();
                _mapInfos = new Dictionary<int, Map>();
                if (!LoadMap(_maps, _mapInfos))
                {
                    return false;
                }
                _serverMap = new Dictionary<int, List<int>>();
                if (!InitServerMap(_serverMap))
                {
                    return false;
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("MapMgr", exception);
                }
                return false;
            }
            return true;
        }

        public static bool InitServerMap(Dictionary<int, List<int>> servermap)
        {
            ServerMapInfo[] allServerMap = new MapBussiness().GetAllServerMap();
            try
            {
                int result = 0;
                foreach (ServerMapInfo info in allServerMap)
                {
                    if (!servermap.Keys.Contains<int>(info.ServerID))
                    {
                        string[] strArray = info.OpenMap.Split(new char[] { ',' });
                        List<int> list = new List<int>();
                        foreach (string str in strArray)
                        {
                            if (!(string.IsNullOrEmpty(str) || !int.TryParse(str, out result)))
                            {
                                list.Add(result);
                            }
                        }
                        servermap.Add(info.ServerID, list);
                    }
                }
            }
            catch (Exception exception)
            {
                log.Error(exception.ToString());
            }
            return true;
        }

        public static bool LoadMap(Dictionary<int, MapPoint> maps, Dictionary<int, Map> mapInfos)
        {
            try
            {
                MapBussiness bussiness = new MapBussiness();
                foreach (MapInfo info in bussiness.GetAllMap())
                {
                    if (!string.IsNullOrEmpty(info.PosX))
                    {
                        if (!maps.Keys.Contains<int>(info.ID))
                        {
                            string[] strArray = info.PosX.Split(new char[] { '|' });
                            string[] strArray2 = info.PosX1.Split(new char[] { '|' });
                            MapPoint point = new MapPoint();
                            foreach (string str in strArray)
                            {
                                if (!string.IsNullOrEmpty(str.Trim()))
                                {
                                    string[] strArray4 = str.Split(new char[] { ',' });
                                    point.PosX.Add(new Point(int.Parse(strArray4[0]), int.Parse(strArray4[1])));
                                }
                            }
                            foreach (string str2 in strArray2)
                            {
                                if (!string.IsNullOrEmpty(str2.Trim()))
                                {
                                    string[] strArray6 = str2.Split(new char[] { ',' });
                                    point.PosX1.Add(new Point(int.Parse(strArray6[0]), int.Parse(strArray6[1])));
                                }
                            }
                            maps.Add(info.ID, point);
                        }
                        if (!mapInfos.ContainsKey(info.ID))
                        {
                            Tile tile = null;
                            string path = string.Format(@"map\{0}\fore.map", info.ID);
                            if (File.Exists(path))
                            {
                                tile = new Tile(path, true);
                            }
                            Tile tile2 = null;
                            path = string.Format(@"map\{0}\dead.map", info.ID);
                            if (File.Exists(path))
                            {
                                tile2 = new Tile(path, false);
                            }
                            if ((tile != null) || (tile2 != null))
                            {
                                mapInfos.Add(info.ID, new Map(info, tile, tile2));
                            }
                            else if (log.IsErrorEnabled)
                            {
                                log.Error("Map's file" + info.ID + " is not exist!");
                            }
                        }
                    }
                }
                if ((maps.Count == 0) || (mapInfos.Count == 0))
                {
                    if (log.IsErrorEnabled)
                    {
                        log.Error(string.Concat(new object[] { "maps:", maps.Count, ",mapInfos:", mapInfos.Count }));
                    }
                    return false;
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("MapMgr", exception);
                }
                return false;
            }
            return true;
        }

        public static bool ReLoadMap()
        {
            try
            {
                Dictionary<int, MapPoint> maps = new Dictionary<int, MapPoint>();
                Dictionary<int, Map> mapInfos = new Dictionary<int, Map>();
                if (LoadMap(maps, mapInfos))
                {
                    m_lock.AcquireWriterLock(-1);
                    try
                    {
                        _maps = maps;
                        _mapInfos = mapInfos;
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
                    log.Error("ReLoadMap", exception);
                }
            }
            return false;
        }

        public static bool ReLoadMapServer()
        {
            try
            {
                Dictionary<int, List<int>> servermap = new Dictionary<int, List<int>>();
                if (InitServerMap(servermap))
                {
                    m_lock.AcquireWriterLock(-1);
                    try
                    {
                        _serverMap = servermap;
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
                    log.Error("ReLoadMapWeek", exception);
                }
            }
            return false;
        }

        public static int GetWeekDay
        {
            get
            {
                int num = Convert.ToInt32(DateTime.Now.DayOfWeek);
                if (num != 0)
                {
                    return num;
                }
                return 7;
            }
        }
    }
}

