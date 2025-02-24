namespace Game.Logic
{
    using System;

    public class LoadingFileInfo
    {
        public string ClassName;
        public string Path;
        public int Type;

        public LoadingFileInfo(int type, string path, string className)
        {
            this.Type = type;
            this.Path = path;
            this.ClassName = className;
        }
    }
}

