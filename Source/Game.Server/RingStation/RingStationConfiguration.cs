namespace Game.Server.RingStation
{
    using System;
    using System.Threading;

    public class RingStationConfiguration
    {
        public static int PlayerID = 0x3e8;
        public static string[] RandomName = new string[] { "Auto Bot01", "Auto Bot02", "Auto Bot03", "Auto Bot04", "Auto Bot05", "Auto Bot06", "Auto Bot07", "Auto Bot08", "Auto Bot09", "Auto Bot10", "Auto Bot11", "Auto Bot12", "Auto Bot12", "Auto Bot13" };
        public static int roomID = 0x3e8;
        public static int ServerID = 0x68;
        public static string ServerName = "AutoBot";

        public static int NextPlayerID()
        {
            return Interlocked.Increment(ref PlayerID);
        }

        public static int NextRoomId()
        {
            return Interlocked.Increment(ref roomID);
        }
    }
}

