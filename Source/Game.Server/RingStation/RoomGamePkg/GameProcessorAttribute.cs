namespace Game.Server.RingStation.RoomGamePkg
{
    using System;

    public class GameProcessorAttribute : Attribute
    {
        private byte _code;
        private string _descript;

        public GameProcessorAttribute(byte code, string description)
        {
            this._code = code;
            this._descript = description;
        }

        public byte Code
        {
            get
            {
                return this._code;
            }
        }

        public string Description
        {
            get
            {
                return this._descript;
            }
        }
    }
}

