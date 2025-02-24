namespace Game.Server.RingStation
{
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Server.RingStation.Action;
    using Game.Server.RingStation.RoomGamePkg;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using System.Reflection;

    public class RingStationGamePlayer
    {
        private bool _canUserProp = true;
        private int _ID;
        public BaseRoomRingStation CurRoom;
        public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private ArrayList m_actions = new ArrayList();
        private long m_passTick;
        private RoomGame seflRoom = new RoomGame();

        public void AddAction(IAction action)
        {
            lock (this.m_actions)
            {
                this.m_actions.Add(action);
            }
        }

        public void AddAction(ArrayList action)
        {
            lock (this.m_actions)
            {
                this.m_actions.AddRange(action);
            }
        }

        public void AddTurn(GSPacketIn pkg)
        {
            if (pkg.Parameter1 == this.GamePlayerId)
            {
                this.m_actions.Add(new PlayerShotAction(this.LastX, this.LastY - 0x19, this.LastForce, this.LastAngle, 0));
            }
        }

        public static double ComputeVx(double dx, float m, float af, float f, float t)
        {
            return (((dx - ((((f / m) * t) * t) / 2f)) / ((double) t)) + (((af / m) * dx) * 0.8));
        }

        public static double ComputeVy(double dx, float m, float af, float f, float t)
        {
            return (((dx - ((((f / m) * t) * t) / 2f)) / ((double) t)) + (((af / m) * dx) * 1.3));
        }

        public void FindTarget()
        {
            GSPacketIn pkg = new GSPacketIn(0x5b) {
                Parameter1 = this.GamePlayerId
            };
            pkg.WriteByte(0x8f);
            this.SendTCP(pkg);
        }

        public void NextTurn(GSPacketIn pkg)
        {
            this.SendSelfTurn(pkg.Parameter1 == this.GamePlayerId, true);
        }

        public void Pause(int time)
        {
            this.m_passTick = Math.Max(this.m_passTick, TickHelper.GetTickCount() + time);
        }

        internal void ProcessPacket(GSPacketIn pkg)
        {
            if (this.seflRoom != null)
            {
                this.seflRoom.ProcessData(this, pkg);
            }
        }

        public void Resume()
        {
            this.m_passTick = 0L;
        }

        public void SendCreateGame(GSPacketIn pkg)
        {
            this.ShootCount = 100;
            this.FirtDirection = true;
            this.Direction = -1;
            pkg.ReadInt();
            pkg.ReadInt();
            pkg.ReadInt();
            int num = pkg.ReadInt();
            int num2 = 0;
            for (int i = 0; i < num; i = num2 + 1)
            {
                pkg.ReadInt();
                pkg.ReadString();
                pkg.ReadInt();
                pkg.ReadString();
                pkg.ReadBoolean();
                pkg.ReadByte();
                pkg.ReadInt();
                pkg.ReadBoolean();
                pkg.ReadInt();
                pkg.ReadString();
                pkg.ReadString();
                pkg.ReadString();
                pkg.ReadInt();
                pkg.ReadInt();
                if (pkg.ReadInt() != 0)
                {
                    pkg.ReadInt();
                    pkg.ReadString();
                    pkg.ReadDateTime();
                }
                pkg.ReadInt();
                pkg.ReadInt();
                pkg.ReadInt();
                pkg.ReadBoolean();
                pkg.ReadInt();
                pkg.ReadString();
                pkg.ReadInt();
                pkg.ReadInt();
                pkg.ReadInt();
                pkg.ReadInt();
                pkg.ReadInt();
                pkg.ReadInt();
                pkg.ReadInt();
                pkg.ReadInt();
                pkg.ReadString();
                pkg.ReadInt();
                pkg.ReadString();
                pkg.ReadInt();
                pkg.ReadBoolean();
                pkg.ReadInt();
                if (pkg.ReadBoolean())
                {
                    pkg.ReadInt();
                    pkg.ReadString();
                }
                pkg.ReadInt();
                pkg.ReadInt();
                pkg.ReadInt();
                pkg.ReadInt();
                pkg.ReadInt();
                pkg.ReadInt();
                int num4 = pkg.ReadInt();
                int num5 = pkg.ReadInt();
                pkg.ReadInt();
                if (this.GamePlayerId == num5)
                {
                    this.Team = num4;
                }
                int num6 = pkg.ReadInt();
                for (int j = 0; j < num6; j = num2 + 1)
                {
                    pkg.ReadInt();
                    pkg.ReadInt();
                    pkg.ReadInt();
                    pkg.ReadString();
                    pkg.ReadInt();
                    pkg.ReadInt();
                    int num8 = pkg.ReadInt();
                    for (int m = 0; m < num8; m = num2 + 1)
                    {
                        pkg.ReadInt();
                        pkg.ReadInt();
                        num2 = m;
                    }
                    num2 = j;
                }
                int num10 = pkg.ReadInt();
                for (int k = 0; k < num10; k = num2 + 1)
                {
                    pkg.ReadInt();
                    num2 = k;
                }
                num2 = i;
            }
        }

        private void SendDirection(int Direction)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b) {
                Parameter1 = this.GamePlayerId
            };
            pkg.WriteByte(7);
            pkg.WriteInt(Direction);
            this.SendTCP(pkg);
        }

        public void SendGameCMDShoot(int x, int y, int force, int angle)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b) {
                Parameter1 = this.GamePlayerId
            };
            pkg.WriteByte(2);
            pkg.WriteInt(x);
            pkg.WriteInt(y);
            pkg.WriteInt(force);
            pkg.WriteInt(angle);
            this.SendTCP(pkg);
        }

        public void sendGameCMDStunt(int type)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b) {
                Parameter1 = this.GamePlayerId
            };
            pkg.WriteByte(15);
            pkg.WriteInt(type);
            this.SendTCP(pkg);
        }

        public void SendLoadingComplete(int state)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b) {
                Parameter1 = this.GamePlayerId
            };
            pkg.WriteByte(0x10);
            pkg.WriteInt(state);
            pkg.WriteInt(0x68);
            pkg.WriteInt(this.GamePlayerId);
            this.SendTCP(pkg);
        }

        private void SendSelfTurn(bool fire)
        {
            this.SendSelfTurn(fire, false);
        }

        private void SendSelfTurn(bool fire, bool useBuff)
        {
            if (fire)
            {
                this.FindTarget();
            }
        }

        public void SendShootTag(bool b, int time)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b) {
                Parameter1 = this.GamePlayerId
            };
            pkg.WriteByte(0x60);
            pkg.WriteBoolean(b);
            pkg.WriteByte((byte) time);
            this.SendTCP(pkg);
        }

        private void SendSkipNext()
        {
            GSPacketIn pkg = new GSPacketIn(0x5b) {
                Parameter1 = this.GamePlayerId
            };
            pkg.WriteByte(12);
            pkg.WriteByte(100);
            this.SendTCP(pkg);
        }

        internal void SendTCP(GSPacketIn pkg)
        {
            this.CurRoom.SendTCP(pkg);
        }

        public void SendUseProp(int templateId)
        {
            this.SendUseProp(templateId, 0, 0, 0, 0);
        }

        public void SendUseProp(int templateId, int x, int y, int force, int angle)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b) {
                Parameter1 = this.GamePlayerId
            };
            pkg.WriteByte(0x20);
            pkg.WriteByte(3);
            pkg.WriteInt(-1);
            pkg.WriteInt(templateId);
            this.SendTCP(pkg);
            if ((templateId == 0x2711) || (templateId == 0x2712))
            {
                ItemTemplateInfo info = ItemMgr.FindItemTemplate(templateId);
                for (int i = 0; i < info.Property2; i++)
                {
                    this.SendGameCMDShoot(x, y, force, angle);
                }
            }
        }

        public void Update(long tick)
        {
            if (this.m_passTick < tick)
            {
                ArrayList list;
                lock (this.m_actions)
                {
                    list = (ArrayList) this.m_actions.Clone();
                    this.m_actions.Clear();
                }
                if (((list != null) && (this.seflRoom != null)) && (list.Count > 0))
                {
                    ArrayList list2 = new ArrayList();
                    foreach (IAction action in list)
                    {
                        try
                        {
                            action.Execute(this, tick);
                            if (!action.IsFinished(tick))
                            {
                                list2.Add(action);
                            }
                        }
                        catch (Exception exception)
                        {
                            log.Error("Bot action error:", exception);
                        }
                    }
                    this.AddAction(list2);
                }
            }
        }

        public int Agility { get; set; }

        public double AntiAddictionRate { get; set; }

        public int Attack { get; set; }

        public float AuncherExperienceRate { get; set; }

        public float AuncherOfferRate { get; set; }

        public float AuncherRichesRate { get; set; }

        public int badgeID { get; set; }

        public double BaseAgility { get; set; }

        public double BaseAttack { get; set; }

        public double BaseBlood { get; set; }

        public double BaseDefence { get; set; }

        public bool CanUserProp
        {
            get
            {
                return this._canUserProp;
            }
            set
            {
                this._canUserProp = value;
            }
        }

        public string Colors { get; set; }

        public int ConsortiaID { get; set; }

        public int ConsortiaLevel { get; set; }

        public string ConsortiaName { get; set; }

        public int ConsortiaRepute { get; set; }

        public int Dander { get; set; }

        public int Defence { get; set; }

        public int Direction { get; set; }

        public int FightPower { get; set; }

        public bool FirtDirection { get; set; }

        public int GamePlayerId
        {
            get
            {
                return this._ID;
            }
            set
            {
                this._ID = value;
            }
        }

        public float GMExperienceRate { get; set; }

        public float GMOfferRate { get; set; }

        public float GMRichesRate { get; set; }

        public int GP { get; set; }

        public double GPAddPlus { get; set; }

        public int Grade { get; set; }

        public int Healstone { get; set; }

        public int HealstoneCount { get; set; }

        public int Hide { get; set; }

        public string Honor { get; set; }

        public int hp { get; set; }

        public int ID
        {
            get
            {
                return this._ID;
            }
            set
            {
                this._ID = value;
            }
        }

        public int LastAngle { get; set; }

        public int LastForce { get; set; }

        public int LastX { get; set; }

        public int LastY { get; set; }

        public int Luck { get; set; }

        public int LX { get; set; }

        public int LY { get; set; }

        public string NickName { get; set; }

        public int Nimbus { get; set; }

        public int Offer { get; set; }

        public double OfferAddPlus { get; set; }

        public int Repute { get; set; }

        public int SecondWeapon { get; set; }

        public bool Sex { get; set; }

        public int ShootCount { get; set; }

        public string Skin { get; set; }

        public string Style { get; set; }

        public int StrengthLevel { get; set; }

        public int Team { get; set; }

        public int TemplateID { get; set; }

        public int Total { get; set; }

        public byte typeVIP { get; set; }

        public int VIPLevel { get; set; }

        public string WeaklessGuildProgressStr { get; set; }

        public int Win { get; set; }

        public int X { get; set; }

        public int Y { get; set; }
    }
}

