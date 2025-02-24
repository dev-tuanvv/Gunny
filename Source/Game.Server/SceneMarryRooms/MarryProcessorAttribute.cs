namespace Game.Server.SceneMarryRooms
{
    using System;

    public class MarryProcessorAttribute : Attribute
    {
        private byte _code;
        private string _descript;

        public MarryProcessorAttribute(byte code, string description)
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

