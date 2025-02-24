namespace Game.Logic.Phy.Maps
{
    using System;
    using System.Drawing;
    using System.IO;

    public class Tile
    {
        private int _bh;
        private int _bw;
        private byte[] _data;
        private bool _digable;
        private int _height;
        private Rectangle _rect;
        private int _width;

        public Tile(Bitmap bitmap, bool digable)
        {
            this._width = bitmap.Width;
            this._height = bitmap.Height;
            this._bw = (this._width / 8) + 1;
            this._bh = this._height;
            this._data = new byte[this._bw * this._bh];
            this._digable = digable;
            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int j = 0; j < bitmap.Width; j++)
                {
                    byte num3 = (bitmap.GetPixel(j, i).A <= 100) ? ((byte) 0) : ((byte) 1);
                    byte[] buffer = this._data;
                    int index = (i * this._bw) + (j / 8);
                    buffer[index] = (byte) (buffer[index] | ((byte) (num3 << (7 - (j % 8)))));
                }
            }
            this._rect = new Rectangle(0, 0, this._width, this._height);
            GC.AddMemoryPressure((long) this._data.Length);
        }

        public Tile(string file, bool digable)
        {
            BinaryReader reader = new BinaryReader(File.Open(file, FileMode.Open));
            this._width = reader.ReadInt32();
            this._height = reader.ReadInt32();
            this._bw = (this._width / 8) + 1;
            this._bh = this._height;
            this._data = reader.ReadBytes(this._bw * this._bh);
            this._digable = digable;
            this._rect = new Rectangle(0, 0, this._width, this._height);
            reader.Close();
            GC.AddMemoryPressure((long) this._data.Length);
        }

        public Tile(byte[] data, int width, int height, bool digable)
        {
            this._data = data;
            this._width = width;
            this._height = height;
            this._digable = digable;
            this._bw = (this._width / 8) + 1;
            this._bh = this._height;
            this._rect = new Rectangle(0, 0, this._width, this._height);
            GC.AddMemoryPressure((long) data.Length);
        }

        protected void Add(int x, int y, Tile tile)
        {
        }

        public Tile Clone()
        {
            return new Tile(this._data.Clone() as byte[], this._width, this._height, this._digable);
        }

        public void Dig(int cx, int cy, Tile surface, Tile border)
        {
            if (this._digable && (surface != null))
            {
                int x = cx - (surface.Width / 2);
                int y = cy - (surface.Height / 2);
                this.Remove(x, y, surface);
                if (border != null)
                {
                    x = cx - (border.Width / 2);
                    y = cy - (border.Height / 2);
                    this.Add(x, y, surface);
                }
            }
        }

        public Point FindNotEmptyPoint(int x, int y, int h)
        {
            if ((x >= 0) && (x < this._width))
            {
                y = (y < 0) ? 0 : y;
                h = ((y + h) > this._height) ? (this._height - y) : h;
                for (int i = 0; i < h; i++)
                {
                    if (!this.IsEmpty(x, y + i))
                    {
                        return new Point(x, y + i);
                    }
                }
                return new Point(-1, -1);
            }
            return new Point(-1, -1);
        }

        public bool IsEmpty(int x, int y)
        {
            if ((((x >= 0) && (x < this._width)) && (y >= 0)) && (y < this._height))
            {
                byte num = (byte) (((int) 1) << (7 - (x % 8)));
                return ((this._data[(y * this._bw) + (x / 8)] & num) == 0);
            }
            return true;
        }

        public bool IsRectangleEmptyQuick(Rectangle rect)
        {
            rect.Intersect(this._rect);
            return (((this.IsEmpty(rect.Right, rect.Bottom) && this.IsEmpty(rect.Left, rect.Bottom)) && this.IsEmpty(rect.Right, rect.Top)) && this.IsEmpty(rect.Left, rect.Top));
        }

        public bool IsYLineEmtpy(int x, int y, int h)
        {
            if ((x >= 0) && (x < this._width))
            {
                y = (y < 0) ? 0 : y;
                h = ((y + h) > this._height) ? (this._height - y) : h;
                for (int i = 0; i < h; i++)
                {
                    if (!this.IsEmpty(x, y + i))
                    {
                        return false;
                    }
                }
                return true;
            }
            return true;
        }

        protected void Remove(int x, int y, Tile tile)
        {
            byte[] buffer = tile._data;
            Rectangle bound = tile.Bound;
            bound.Offset(x, y);
            bound.Intersect(this._rect);
            if ((bound.Width != 0) && (bound.Height != 0))
            {
                bound.Offset(-x, -y);
                int num = bound.X / 8;
                int num2 = (bound.X + x) / 8;
                int num3 = bound.Y;
                int num4 = (bound.Width / 8) + 1;
                int height = bound.Height;
                if (bound.X == 0)
                {
                    if ((num4 + num2) < this._bw)
                    {
                        num4++;
                        num4 = (num4 > tile._bw) ? tile._bw : num4;
                    }
                    int num6 = (bound.X + x) % 8;
                    for (int i = 0; i < height; i++)
                    {
                        int num8 = 0;
                        for (int j = 0; j < num4; j++)
                        {
                            int index = ((((i + y) + num3) * this._bw) + j) + num2;
                            int num11 = (((i + num3) * tile._bw) + j) + num;
                            int num12 = buffer[num11];
                            int num13 = num12 >> num6;
                            int num14 = this._data[index];
                            num14 &= ~(num14 & num13);
                            if (num8 != 0)
                            {
                                num14 &= ~(num14 & num8);
                            }
                            this._data[index] = (byte) num14;
                            num8 = num12 << (8 - num6);
                        }
                    }
                }
                else
                {
                    int num15 = bound.X % 8;
                    for (int k = 0; k < height; k++)
                    {
                        for (int m = 0; m < num4; m++)
                        {
                            int num22;
                            int num18 = ((((k + y) + num3) * this._bw) + m) + num2;
                            int num19 = (((k + num3) * tile._bw) + m) + num;
                            int num20 = buffer[num19];
                            int num21 = num20 << num15;
                            if (m < (num4 - 1))
                            {
                                num20 = buffer[num19 + 1];
                                num22 = num20 >> (8 - num15);
                            }
                            else
                            {
                                num22 = 0;
                            }
                            int num23 = this._data[num18];
                            num23 &= ~(num23 & num21);
                            if (num22 != 0)
                            {
                                num23 &= ~(num23 & num22);
                            }
                            this._data[num18] = (byte) num23;
                        }
                    }
                }
            }
        }

        public Bitmap ToBitmap()
        {
            Bitmap bitmap = new Bitmap(this._width, this._height);
            for (int i = 0; i < this._height; i++)
            {
                for (int j = 0; j < this._width; j++)
                {
                    if (this.IsEmpty(j, i))
                    {
                        bitmap.SetPixel(j, i, Color.FromArgb(0, 0, 0, 0));
                    }
                    else
                    {
                        bitmap.SetPixel(j, i, Color.FromArgb(0xff, 0, 0, 0));
                    }
                }
            }
            return bitmap;
        }

        public Rectangle Bound
        {
            get
            {
                return this._rect;
            }
        }

        public byte[] Data
        {
            get
            {
                return this._data;
            }
        }

        public int Height
        {
            get
            {
                return this._height;
            }
        }

        public int Width
        {
            get
            {
                return this._width;
            }
        }
    }
}

