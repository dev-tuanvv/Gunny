namespace Game.Server.Managers
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server.GameObjects;
    using Game.Server.HotSpringRooms;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Reflection;

    public class HotSpringMgr
    {
        protected static TankHotSpringLogicProcessor _processor = new TankHotSpringLogicProcessor();
        private static Dictionary<int, HotSpringRoom> dictionary_0;
        public static string[] HotSpringEnterPriRoom;
        private static readonly ILog ilog_0 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected static ReaderWriterLock m_lock;
        protected static ThreadSafeRandom m_rand;
        private static string[] string_0;

        public static HotSpringRoom CreateHotSpringRoomFromDB(HotSpringRoomInfo roomInfo)
        {
            HotSpringRoom room = null;
            m_lock.AcquireWriterLock(-1);
            try
            {
                room = new HotSpringRoom(roomInfo, _processor);
                if (room != null)
                {
                    dictionary_0.Add(room.Info.roomID, room);
                    return room;
                }
            }
            finally
            {
                m_lock.ReleaseWriterLock();
            }
            return null;
        }

        public static HotSpringRoom[] GetAllHotSpringRoom()
        {
            HotSpringRoom[] array = null;
            m_lock.AcquireReaderLock(-1);
            try
            {
                array = new HotSpringRoom[dictionary_0.Count];
                dictionary_0.Values.CopyTo(array, 0);
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            if (array != null)
            {
                return array;
            }
            return new HotSpringRoom[0];
        }

        public static int GetExpWithLevel(int grade)
        {
            try
            {
                if (grade <= string_0.Length)
                {
                    return int.Parse(string_0[grade - 1]);
                }
            }
            catch (Exception exception)
            {
                ilog_0.Error("GetExpWithLevel Error: " + exception.ToString());
            }
            return 0;
        }

        public static HotSpringRoom GetHotSpringRoombyID(int id)
        {
            HotSpringRoom room = null;
            m_lock.AcquireReaderLock(-1);
            try
            {
                if ((id > 0) && dictionary_0.Keys.Contains<int>(id))
                {
                    room = dictionary_0[id];
                }
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return room;
        }

        public static HotSpringRoom GetHotSpringRoombyID(int id, string pwd, ref string msg)
        {
            HotSpringRoom room = null;
            m_lock.AcquireReaderLock(-1);
            try
            {
                if (!((id > 0) && dictionary_0.Keys.Contains<int>(id)))
                {
                    return room;
                }
                if (dictionary_0[id].Info.roomPassword != pwd)
                {
                    msg = "Mật khẩu kh\x00f4ng ch\x00ednh x\x00e1c";
                    return room;
                }
                room = dictionary_0[id];
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return room;
        }

        public static HotSpringRoom GetRandomRoom()
        {
            HotSpringRoom room = null;
            m_lock.AcquireReaderLock(-1);
            try
            {
                List<HotSpringRoom> list = new List<HotSpringRoom>();
                foreach (HotSpringRoom room2 in dictionary_0.Values)
                {
                    if (room2.Count < room2.Info.maxCount)
                    {
                        list.Add(room2);
                    }
                }
                if (list.Count > 0)
                {
                    int num = m_rand.Next(0, list.Count);
                    room = list[num];
                }
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return room;
        }

        public static bool Init()
        {
            try
            {
                m_lock = new ReaderWriterLock();
                m_rand = new ThreadSafeRandom();
                dictionary_0 = new Dictionary<int, HotSpringRoom>();
                char[] separator = new char[] { ',' };
                string_0 = GameProperties.HotSpringExp.Split(separator);
                char[] chArray2 = new char[] { ',' };
                HotSpringEnterPriRoom = GameProperties.SpaPubRoomLoginPay.Split(chArray2);
                smethod_0();
                return true;
            }
            catch (Exception exception)
            {
                if (ilog_0.IsErrorEnabled)
                {
                    ilog_0.Error("HotSpringMgr", exception);
                }
                return false;
            }
        }

        public static void SendUpdateAllRoom(GamePlayer p, HotSpringRoom[] rooms)
        {
            GSPacketIn pkg = new GSPacketIn(0xc5);
            pkg.WriteInt(rooms.Length);
            foreach (HotSpringRoom room in rooms)
            {
                pkg.WriteInt(room.Info.roomNumber);
                pkg.WriteInt(room.Info.roomID);
                pkg.WriteString(room.Info.roomName);
                pkg.WriteString((room.Info.roomPassword != null) ? "password" : "");
                pkg.WriteInt(room.Info.effectiveTime);
                pkg.WriteInt(room.Count);
                pkg.WriteInt(room.Info.playerID);
                pkg.WriteString(room.Info.playerName);
                pkg.WriteDateTime(room.Info.startTime);
                pkg.WriteString(room.Info.roomIntroduction);
                pkg.WriteInt(room.Info.roomType);
                pkg.WriteInt(room.Info.maxCount);
            }
            if (p != null)
            {
                p.SendTCP(pkg);
            }
            else
            {
                WorldMgr.HotSpringScene.SendToALL(pkg);
            }
        }

        private static void smethod_0()
        {
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                HotSpringRoomInfo[] allHotSpringRooms = bussiness.GetAllHotSpringRooms();
                for (int i = 0; i < allHotSpringRooms.Length; i++)
                {
                    CreateHotSpringRoomFromDB(allHotSpringRooms[i]);
                }
            }
        }
    }
}

