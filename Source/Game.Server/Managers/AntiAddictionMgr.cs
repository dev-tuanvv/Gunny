﻿namespace Game.Server.Managers
{
    using Bussiness;
    using Game.Server.GameObjects;
    using System;

    internal class AntiAddictionMgr
    {
        private static bool _isASSon;
        public static int count = 0;

        public static int AASStateGet(GamePlayer player)
        {
            int iD = player.PlayerCharacter.ID;
            bool result = true;
            player.IsAASInfo = false;
            player.IsMinor = true;
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                string aSSInfoSingle = bussiness.GetASSInfoSingle(iD);
                if (aSSInfoSingle != "")
                {
                    player.IsAASInfo = true;
                    result = false;
                    int num2 = Convert.ToInt32(aSSInfoSingle.Substring(6, 4));
                    int num3 = Convert.ToInt32(aSSInfoSingle.Substring(10, 2));
                    if ((DateTime.Now.Year.CompareTo((int) (num2 + 0x12)) > 0) || ((DateTime.Now.Year.CompareTo((int) (num2 + 0x12)) == 0) && (DateTime.Now.Month.CompareTo(num3) >= 0)))
                    {
                        player.IsMinor = false;
                    }
                }
            }
            if (((result && (player.PlayerCharacter.IsFirst != 0)) && (player.PlayerCharacter.DayLoginCount < 1)) && ISASSon)
            {
                player.Out.SendAASState(result);
            }
            if (player.IsMinor || (!player.IsAASInfo && ISASSon))
            {
                player.Out.SendAASControl(ISASSon, player.IsAASInfo, player.IsMinor);
            }
            return 0;
        }

        public static double GetAntiAddictionCoefficient(int onlineTime)
        {
            if (!_isASSon)
            {
                return 1.0;
            }
            if ((0 <= onlineTime) && (onlineTime <= 240))
            {
                return 1.0;
            }
            if ((240 < onlineTime) && (onlineTime <= 300))
            {
                return 0.5;
            }
            return 0.0;
        }

        public static void SetASSState(bool ASSState)
        {
            _isASSon = ASSState;
        }

        public static bool ISASSon
        {
            get
            {
                return _isASSon;
            }
        }
    }
}

