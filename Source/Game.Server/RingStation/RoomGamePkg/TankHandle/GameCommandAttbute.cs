namespace Game.Server.RingStation.RoomGamePkg.TankHandle
{
    using System;
    using System.Runtime.CompilerServices;

    public class GameCommandAttbute : Attribute
    {
        public GameCommandAttbute(byte code)
        {
            this.Code = code;
        }

        public byte Code { get; private set; }
    }
}

