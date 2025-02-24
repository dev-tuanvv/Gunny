namespace Game.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    public class MapPoint
    {
        private List<Point> posX = new List<Point>();
        private List<Point> posX1 = new List<Point>();

        public List<Point> PosX
        {
            get
            {
                return this.posX;
            }
            set
            {
                this.posX = value;
            }
        }

        public List<Point> PosX1
        {
            get
            {
                return this.posX1;
            }
            set
            {
                this.posX1 = value;
            }
        }
    }
}

