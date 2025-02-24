namespace Game.Logic.Phy.Maps
{
    using Game.Logic.Phy.Object;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    public class Map
    {
        protected Rectangle _bound;
        private MapInfo _info;
        protected Tile _layer1;
        protected Tile _layer2;
        private HashSet<Physics> _objects;
        private float _wind;

        public Map(MapInfo info, Tile layer1, Tile layer2)
        {
            this._info = info;
            this._objects = new HashSet<Physics>();
            this._layer1 = layer1;
            this._layer2 = layer2;
            if (this._layer1 != null)
            {
                this._bound = new Rectangle(0, 0, this._layer1.Width, this._layer1.Height);
            }
            else
            {
                this._bound = new Rectangle(0, 0, this._layer2.Width, this._layer2.Height);
            }
        }

        public void AddPhysical(Physics phy)
        {
            phy.SetMap(this);
            lock (this._objects)
            {
                this._objects.Add(phy);
            }
        }

        public bool canMove(int x, int y)
        {
            return (this.IsEmpty(x, y) && !this.IsOutMap(x, y));
        }

        public Map Clone()
        {
            Tile tile = (this._layer1 != null) ? this._layer1.Clone() : null;
            return new Map(this._info, tile, (this._layer2 != null) ? this._layer2.Clone() : null);
        }

        public void Dig(int cx, int cy, Tile surface, Tile border)
        {
            if (this._layer1 != null)
            {
                this._layer1.Dig(cx, cy, surface, border);
            }
            if (this._layer2 != null)
            {
                this._layer2.Dig(cx, cy, surface, border);
            }
        }

        public void Dispose()
        {
            foreach (Physics physics in this._objects)
            {
                physics.Dispose();
            }
        }

        public List<Living> FindAllNearestEnemy(int x, int y, double maxdistance, Living except)
        {
            List<Living> list = new List<Living>();
            lock (this._objects)
            {
                foreach (Physics physics in this._objects)
                {
                    if ((((physics is Living) && (physics != except)) && physics.IsLiving) && (((Living) physics).Team != except.Team))
                    {
                        double num = physics.Distance(x, y);
                        if (num < maxdistance)
                        {
                            list.Add(physics as Living);
                            maxdistance = num;
                        }
                    }
                }
            }
            return list;
        }

        public List<Living> FindAllNearestSameTeam(int x, int y, double maxdistance, Living except)
        {
            List<Living> list = new List<Living>();
            lock (this._objects)
            {
                foreach (Physics physics in this._objects)
                {
                    if ((((physics is Living) && (physics != except)) && physics.IsLiving) && (((Living) physics).Team == except.Team))
                    {
                        double num = physics.Distance(x, y);
                        if (num < maxdistance)
                        {
                            list.Add(physics as Living);
                            maxdistance = num;
                        }
                    }
                }
            }
            return list;
        }

        public List<Living> FindHitByHitPiont()
        {
            List<Living> list = new List<Living>();
            lock (this._objects)
            {
                foreach (Physics physics in this._objects)
                {
                    if ((physics is Living) && physics.IsLiving)
                    {
                        list.Add(physics as Living);
                    }
                }
            }
            return list;
        }

        public List<Living> FindHitByHitPiont(Point p, int radius)
        {
            List<Living> list = new List<Living>();
            lock (this._objects)
            {
                foreach (Physics physics in this._objects)
                {
                    if (((physics is Living) && physics.IsLiving) && ((physics as Living).BoundDistance(p) < radius))
                    {
                        list.Add(physics as Living);
                    }
                }
            }
            return list;
        }

        public List<Living> FindLivings(int x, int y, int radius)
        {
            List<Living> list = new List<Living>();
            lock (this._objects)
            {
                foreach (Physics physics in this._objects)
                {
                    if (((physics is Living) && physics.IsLiving) && (physics.Distance(x, y) < radius))
                    {
                        list.Add(physics as Living);
                    }
                }
            }
            return list;
        }

        public Living FindNearestEnemy(int x, int y, double maxdistance, Living except)
        {
            Living living = null;
            lock (this._objects)
            {
                foreach (Physics physics in this._objects)
                {
                    if ((((physics is Living) && (physics != except)) && physics.IsLiving) && (((Living) physics).Team != except.Team))
                    {
                        double num = physics.Distance(x, y);
                        if (num < maxdistance)
                        {
                            living = physics as Living;
                            maxdistance = num;
                        }
                    }
                }
            }
            return living;
        }

        public Point FindNextWalkPoint(int x, int y, int direction, int stepX, int stepY)
        {
            if ((direction != 1) && (direction != -1))
            {
                return Point.Empty;
            }
            int num = x + (direction * stepX);
            if ((num < 0) || (num > this._bound.Width))
            {
                return Point.Empty;
            }
            Point empty = this.FindYLineNotEmptyPoint(num, (y - stepY) - 1, this._bound.Height);
            if ((empty != Point.Empty) && (Math.Abs((int) (empty.Y - y)) > stepY))
            {
                empty = Point.Empty;
            }
            return empty;
        }

        public Point FindNextWalkPointDown(int x, int y, int direction, int stepX, int stepY)
        {
            if ((direction != 1) && (direction != -1))
            {
                return Point.Empty;
            }
            int num = x + (direction * stepX);
            if ((num < 0) || (num > this._bound.Width))
            {
                return Point.Empty;
            }
            Point empty = this.FindYLineNotEmptyPointDown(num, (y - stepY) - 1);
            if ((empty != Point.Empty) && (Math.Abs((int) (empty.Y - y)) > stepY))
            {
                empty = Point.Empty;
            }
            return empty;
        }

        public List<NormalNpc> FindNpcIds(int fx, int tx, List<NormalNpc> exceptPlayers)
        {
            List<NormalNpc> list = new List<NormalNpc>();
            lock (this._objects)
            {
                foreach (Physics physics in this._objects)
                {
                    if ((((physics is NormalNpc) && physics.IsLiving) && (physics.X > fx)) && (physics.X < tx))
                    {
                        if (exceptPlayers != null)
                        {
                            foreach (NormalNpc npc in exceptPlayers)
                            {
                                list.Add(physics as NormalNpc);
                            }
                        }
                        else
                        {
                            list.Add(physics as NormalNpc);
                        }
                    }
                }
            }
            return list;
        }

        public bool FindPlayers(Point p, int radius)
        {
            int num = 0;
            lock (this._objects)
            {
                foreach (Physics physics in this._objects)
                {
                    if (((physics is Player) && physics.IsLiving) && ((physics as Player).BoundDistance(p) < radius))
                    {
                        num++;
                    }
                    if (num >= 2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public List<Player> FindPlayers(int x, int y, int radius)
        {
            List<Player> list = new List<Player>();
            lock (this._objects)
            {
                foreach (Physics physics in this._objects)
                {
                    if (((physics is Player) && physics.IsLiving) && (physics.Distance(x, y) < radius))
                    {
                        list.Add(physics as Player);
                    }
                }
            }
            return list;
        }

        public List<Living> FindPlayers(int fx, int tx, List<Player> exceptPlayers)
        {
            List<Living> list = new List<Living>();
            HashSet<Physics> set = this._objects;
            lock (set)
            {
                foreach (Physics physics in this._objects)
                {
                    if (((physics is Player) || ((physics is Living) && (physics as Living).Config.IsHelper)) && (((physics.IsLiving && (physics.X > fx)) && (physics.X < tx)) && (!(physics is Player) || (physics as Player).IsActive)))
                    {
                        if (exceptPlayers != null)
                        {
                            foreach (Player player in exceptPlayers)
                            {
                                if ((physics is Player) && (((Player) physics).DefaultDelay != player.DefaultDelay))
                                {
                                    list.Add(physics as Living);
                                }
                            }
                        }
                        else
                        {
                            list.Add(physics as Living);
                        }
                    }
                }
            }
            return list;
        }

        public Physics[] FindPhysicalObjects(Rectangle rect, Physics except)
        {
            List<Physics> list = new List<Physics>();
            lock (this._objects)
            {
                foreach (Physics physics in this._objects)
                {
                    if (physics.IsLiving && (physics != except))
                    {
                        Rectangle bound = physics.Bound;
                        Rectangle rectangle2 = physics.Bound1;
                        bound.Offset(physics.X, physics.Y);
                        rectangle2.Offset(physics.X, physics.Y);
                        if (bound.IntersectsWith(rect) || rectangle2.IntersectsWith(rect))
                        {
                            list.Add(physics);
                        }
                    }
                }
            }
            return list.ToArray();
        }

        public Point FindYLineNotEmptyPoint(int x, int y)
        {
            return this.FindYLineNotEmptyPoint(x, y, this._bound.Height);
        }

        public Point FindYLineNotEmptyPoint(int x, int y, int h)
        {
            x = (x < 0) ? 0 : ((x >= this._bound.Width) ? (this._bound.Width - 1) : x);
            y = (y < 0) ? 0 : y;
            h = ((y + h) >= this._bound.Height) ? ((this._bound.Height - y) - 1) : h;
            for (int i = 0; i < h; i++)
            {
                if (!(this.IsEmpty(x - 1, y) && this.IsEmpty(x + 1, y)))
                {
                    return new Point(x, y);
                }
                y++;
            }
            return Point.Empty;
        }

        public Point FindYLineNotEmptyPointDown(int x, int y)
        {
            return this.FindYLineNotEmptyPointDown(x, y, this._bound.Height);
        }

        public Point FindYLineNotEmptyPointDown(int x, int y, int h)
        {
            x = (x < 0) ? 0 : ((x >= this._bound.Width) ? (this._bound.Width - 1) : x);
            y = (y < 0) ? 0 : y;
            h = ((y + h) >= this._bound.Height) ? ((this._bound.Height - y) - 1) : h;
            for (int i = 0; i < h; i++)
            {
                if (!(this.IsEmpty(x - 1, y) && this.IsEmpty(x + 1, y)))
                {
                    return new Point(x, y);
                }
                y++;
            }
            return Point.Empty;
        }

        public List<PhysicalObj> GetAllPhysicalObjSafe()
        {
            List<PhysicalObj> list = new List<PhysicalObj>();
            lock (this._objects)
            {
                foreach (Physics physics in this._objects)
                {
                    if (physics is PhysicalObj)
                    {
                        list.Add(physics as PhysicalObj);
                    }
                }
            }
            return list;
        }

        public List<Physics> GetAllPhysicalSafe()
        {
            List<Physics> list = new List<Physics>();
            lock (this._objects)
            {
                foreach (Physics physics in this._objects)
                {
                    list.Add(physics);
                }
            }
            return list;
        }

        public bool IsEmpty(int x, int y)
        {
            return (((this._layer1 == null) || this._layer1.IsEmpty(x, y)) && ((this._layer2 == null) || this._layer2.IsEmpty(x, y)));
        }

        public bool IsOutMap(int x, int y)
        {
            return (((x < 0) || (x >= this._bound.Width)) || (y >= this._bound.Height));
        }

        public bool IsRectangleEmpty(Rectangle rect)
        {
            return (((this._layer1 == null) || this._layer1.IsRectangleEmptyQuick(rect)) && ((this._layer2 == null) || this._layer2.IsRectangleEmptyQuick(rect)));
        }

        public bool IsSpecialMap()
        {
            return (this.Info.ID == 0x517);
        }

        public void RemovePhysical(Physics phy)
        {
            phy.SetMap(null);
            lock (this._objects)
            {
                this._objects.Remove(phy);
            }
        }

        public float airResistance
        {
            get
            {
                return (float) this._info.DragIndex;
            }
        }

        public Rectangle Bound
        {
            get
            {
                return this._bound;
            }
        }

        public float gravity
        {
            get
            {
                return (float) this._info.Weight;
            }
        }

        public Tile Ground
        {
            get
            {
                return this._layer1;
            }
        }

        public MapInfo Info
        {
            get
            {
                return this._info;
            }
        }

        public float wind
        {
            get
            {
                return this._wind;
            }
            set
            {
                this._wind = value;
            }
        }
    }
}

