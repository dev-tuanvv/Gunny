﻿namespace Game.Logic
{
    using Bussiness;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Reflection;

    public class BallConfigMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<int, BallConfigInfo> m_infos;

        public static BallConfigInfo FindBall(int id)
        {
            if (m_infos.ContainsKey(id))
            {
                return m_infos[id];
            }
            return null;
        }

        public static bool Init()
        {
            return ReLoad();
        }

        private static Dictionary<int, BallConfigInfo> LoadFromDatabase()
        {
            Dictionary<int, BallConfigInfo> dictionary = new Dictionary<int, BallConfigInfo>();
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                foreach (BallConfigInfo info in bussiness.GetAllBallConfig())
                {
                    if (!dictionary.ContainsKey(info.TemplateID))
                    {
                        dictionary.Add(info.TemplateID, info);
                    }
                }
            }
            return dictionary;
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, BallConfigInfo> dictionary = LoadFromDatabase();
                if (dictionary.Values.Count > 0)
                {
                    Interlocked.Exchange<Dictionary<int, BallConfigInfo>>(ref m_infos, dictionary);
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

