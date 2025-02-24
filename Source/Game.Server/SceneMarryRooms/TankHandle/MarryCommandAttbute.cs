namespace Game.Server.SceneMarryRooms.TankHandle
{
    using System;
    using System.Runtime.CompilerServices;

    public class MarryCommandAttbute : Attribute
    {
        public MarryCommandAttbute(byte code)
        {
            this.Code = code;
        }

        public byte Code { get; private set; }
    }
}

