namespace Game.Logic
{
    using Bussiness;
    using Game.Logic.Phy.Maps;
    using Game.Logic.Phy.Object;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Reflection;

    public class BallMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<int, BallInfo> m_infos;
        private static Dictionary<int, Tile> m_tiles;

        public static BallInfo FindBall(int id)
        {
            if (m_infos.ContainsKey(id))
            {
                return m_infos[id];
            }
            return null;
        }

        public static Tile FindTile(int id)
        {
            if (m_tiles.ContainsKey(id))
            {
                return m_tiles[id];
            }
            return null;
        }

        public static BombType GetBallType(int ballId)
        {
            if (ballId <= 0x3b)
            {
                switch (ballId)
                {
                    case 1:
                    case 0x38:
                        goto Label_00C5;

                    case 2:
                    case 4:
                        return BombType.Normal;

                    case 3:
                        return BombType.FLY;

                    case 5:
                        return BombType.CURE;
                }
                if (ballId != 0x3b)
                {
                    return BombType.Normal;
                }
                return BombType.CURE;
            }
            if (ballId <= 0x63)
            {
                switch (ballId)
                {
                    case 0x61:
                    case 0x62:
                        return BombType.CURE;

                    case 0x63:
                        goto Label_00C5;

                    case 0x40:
                        return BombType.CURE;
                }
                return BombType.Normal;
            }
            if ((ballId != 120) && (ballId != 0x2719))
            {
                return BombType.Normal;
            }
            return BombType.CURE;
        Label_00C5:
            return BombType.FORZEN;
        }

        public static bool Init()
        {
            return ReLoad();
        }

        private static Dictionary<int, BallInfo> LoadFromDatabase()
        {
            Dictionary<int, BallInfo> dictionary = new Dictionary<int, BallInfo>();
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                foreach (BallInfo info in bussiness.GetAllBall())
                {
                    if (!dictionary.ContainsKey(info.ID))
                    {
                        dictionary.Add(info.ID, info);
                    }
                }
            }
            return dictionary;
        }

        private static Dictionary<int, Tile> LoadFromFiles(Dictionary<int, BallInfo> list)
        {
            Dictionary<int, Tile> dictionary = new Dictionary<int, Tile>();
            foreach (BallInfo info in list.Values)
            {
                if (info.HasTunnel)
                {
                    string path = string.Format(@"bomb\{0}.bomb", info.ID);
                    Tile tile = null;
                    if (File.Exists(path))
                    {
                        tile = new Tile(path, false);
                    }
                    dictionary.Add(info.ID, tile);
                    if ((((tile == null) && (info.ID != 1)) && (info.ID != 2)) && (info.ID != 3))
                    {
                        log.ErrorFormat("can't find bomb file:{0}", path);
                    }
                }
            }
            return dictionary;
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, BallInfo> list = LoadFromDatabase();
                Dictionary<int, Tile> dictionary2 = LoadFromFiles(list);
                if ((list.Values.Count > 0) && (dictionary2.Values.Count > 0))
                {
                    Interlocked.Exchange<Dictionary<int, BallInfo>>(ref m_infos, list);
                    Interlocked.Exchange<Dictionary<int, Tile>>(ref m_tiles, dictionary2);
                    return true;
                }
            }
            catch (Exception exception)
            {
                log.Error("Ball Mgr init error:", exception);
            }
            return false;
        }
    }
}

