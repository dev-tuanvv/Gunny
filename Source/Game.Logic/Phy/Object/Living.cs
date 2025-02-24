namespace Game.Logic.Phy.Object
{
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Logic.Actions;
    using Game.Logic.Effects;
    using Game.Logic.PetEffects;
    using Game.Logic.Phy.Actions;
    using Game.Logic.Phy.Maps;
    using Game.Logic.Phy.Maths;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    public class Living : Physics
    {
        public bool AddArmor;
        public double Agility;
        public double Attack;
        public int AttackGemLimit;
        public double BaseDamage;
        public double BaseGuard;
        private bool bikhoa;
        private bool bool_7;
        public bool ControlBall;
        public int countBoom;
        public float CurrentDamagePlus;
        public bool CurrentIsHitTarget;
        public float CurrentShootMinus;
        public bool DamageNone;
        public double Defence;
        public int DefendActiveGem;
        public int DefenFisrtGem;
        public int DefenSecondGem;
        public int EffectsCount;
        public bool EffectTrigger;
        public int Experience;
        public int FlyingPartical;
        protected static int GHOST_MOVE_SPEED = 8;
        public int Grade;
        public bool IgnoreArmor;
        public int LastLifeTimeShoot;
        public double Lucky;
        private string m_action;
        private bool m_autoBoot;
        protected int m_blood;
        private LivingConfig m_config;
        private Rectangle m_demageRect;
        public int m_direction;
        private int m_doAction;
        private Game.Logic.Effects.EffectList m_effectList;
        private int m_FallCount;
        private FightBufferInfo m_fightBufferInfo;
        private int m_FindCount;
        protected BaseGame m_game;
        private bool m_isAttacking;
        private bool m_isFrost;
        private bool m_isHide;
        private bool m_isNoHole;
        private bool m_isPet;
        private bool m_isSeal;
        protected int m_maxBlood;
        private string m_modelId;
        private string m_name;
        private bool m_nguyhiem;
        private Game.Logic.PetEffects.PetEffectList m_petEffectList;
        private PetEffectInfo m_petEffects;
        private int m_pictureTurn;
        private int m_specialSkillDelay;
        private int m_state;
        protected bool m_syncAtTime;
        private int m_team;
        private eLivingType m_type;
        private bool m_vaneOpen;
        public int mau;
        public int MaxBeatDis;
        protected static int MOVE_SPEED = 2;
        public bool NoHoleTurn;
        public bool PetEffectTrigger;
        private Random rand;
        public int ReduceCritFisrtGem;
        public int ReduceCritSecondGem;
        public List<int> ScoreArr;
        public int ShootMovieDelay;
        public int solanlon;
        protected static int STEP_X = 3;
        protected static int STEP_Y = 7;
        public int TotalCure;
        public int TotalDameLiving;
        public int TotalHitTargetCount;
        public int TotalHurt;
        public int TotalKill;
        public int TotalShootCount;
        public int TurnNum;

        public event KillLivingEventHanlde AfterKilledByLiving;

        public event KillLivingEventHanlde AfterKillingLiving;

        public event LivingTakedDamageEventHandle BeforeTakeDamage;

        public event LivingEventHandle BeginAttacked;

        public event LivingEventHandle BeginAttacking;

        public event LivingEventHandle BeginNextTurn;

        public event LivingEventHandle BeginSelfTurn;

        public event LivingEventHandle Died;

        public event LivingEventHandle EndAttacking;

        public event LivingTakedDamageEventHandle TakePlayerDamage;

        public Living(int id, BaseGame game, int team, string name, string modelId, int maxBlood, int immunity, int direction) : base(id)
        {
            this.BaseDamage = 10.0;
            this.BaseGuard = 10.0;
            this.Defence = 10.0;
            this.Attack = 10.0;
            this.Agility = 10.0;
            this.Lucky = 10.0;
            this.Grade = 1;
            this.Experience = 10;
            this.bikhoa = false;
            this.DamageNone = false;
            this.solanlon = 0;
            this.m_vaneOpen = false;
            this.m_isPet = false;
            this.m_action = "";
            this.m_game = game;
            this.m_team = team;
            this.m_name = name;
            this.m_modelId = modelId;
            this.m_maxBlood = maxBlood;
            this.m_direction = direction;
            this.m_state = 0;
            this.m_doAction = -1;
            this.MaxBeatDis = 100;
            this.AddArmor = false;
            this.ReduceCritFisrtGem = 0;
            this.ReduceCritSecondGem = 0;
            this.DefenFisrtGem = 0;
            this.DefenSecondGem = 0;
            this.DefendActiveGem = 0;
            this.AttackGemLimit = 0;
            this.m_effectList = new Game.Logic.Effects.EffectList(this, immunity);
            this.m_petEffectList = new Game.Logic.PetEffects.PetEffectList(this, immunity);
            this.m_fightBufferInfo = new FightBufferInfo();
            this.SetupPetEffect();
            this.m_config = new LivingConfig();
            this.m_syncAtTime = true;
            this.m_type = eLivingType.Living;
            this.rand = new Random();
            this.ScoreArr = new List<int>();
            this.m_autoBoot = false;
            this.m_nguyhiem = false;
            this.m_pictureTurn = 0;
            this.TotalCure = 0;
        }

        public virtual int AddBlood(int value)
        {
            return this.AddBlood(value, 0);
        }

        public virtual int AddBlood(int value, int type)
        {
            this.m_blood += value;
            if (this.m_blood > this.m_maxBlood)
            {
                this.m_blood = this.m_maxBlood;
            }
            if (this.m_syncAtTime)
            {
                this.m_game.SendGameUpdateHealth(this, type, value);
            }
            return value;
        }

        public void AddEffect(AbstractEffect effect, int delay)
        {
            this.m_game.AddAction(new LivingDelayEffectAction(this, effect, delay));
        }

        public void AddPetEffect(AbstractPetEffect effect, int delay)
        {
            this.m_game.AddAction(new LivingDelayPetEffectAction(this, effect, delay));
        }

        public void AddRemoveEnergy(int value)
        {
            if (this.m_syncAtTime)
            {
                this.m_game.SendGamePlayerProperty(this, "energy", value.ToString());
            }
        }

        public bool Bay(int x, int y, string action, int delay, string sAction, int speed)
        {
            return this.Bay(x, y, action, delay, sAction, speed, null);
        }

        public bool Bay(int x, int y, string action, int delay, string sAction, int speed, LivingCallBack callback)
        {
            if ((base.m_x != x) || (base.m_y != y))
            {
                if ((x < 0) || (x > base.m_map.Bound.Width))
                {
                    return false;
                }
                List<Point> path = new List<Point>();
                int num = base.m_x;
                int num2 = base.m_y;
                int direction = (x > num) ? 1 : -1;
                if (action != "fly")
                {
                    while (((x - num) * direction) > 0)
                    {
                        Point item = base.m_map.FindNextWalkPoint(num, num2, direction, STEP_X, STEP_Y);
                        if (!(item != Point.Empty))
                        {
                            break;
                        }
                        path.Add(item);
                        num = item.X;
                        num2 = item.Y;
                    }
                }
                else
                {
                    Point point2 = new Point(x, y);
                    Point point3 = new Point(num - point2.X, num2 - point2.Y);
                    point2 = new Point(point2.X + point3.X, y + point3.X);
                    point3 = new Point(x - point2.X, x - point2.Y);
                    bool flag = point2 != Point.Empty;
                    point2 = new Point(x, y);
                    path.Add(point2);
                }
                if (path.Count > 0)
                {
                    this.m_game.AddAction(new LivingMoveToAction(this, path, action, delay, speed, callback));
                    return true;
                }
            }
            return false;
        }

        public bool Beat(Living target, string action, int demageAmount, int criticalAmount, int delay)
        {
            return this.Beat(target, action, demageAmount, criticalAmount, delay, 1, 1);
        }

        public bool Beat(Living target, string action, int demageAmount, int criticalAmount, int delay, int livingCount, int attackEffect)
        {
            if ((target != null) && target.IsLiving)
            {
                demageAmount = this.MakeDamage(target);
                this.OnBeforeTakedDamage(target, ref demageAmount, ref criticalAmount);
                this.StartAttacked();
                int num = (int) target.Distance(this.X, this.Y);
                if (num <= this.MaxBeatDis)
                {
                    if ((this.X - target.X) > 0)
                    {
                        this.Direction = -1;
                    }
                    else
                    {
                        this.Direction = 1;
                    }
                    this.m_game.AddAction(new LivingBeatAction(this, target, demageAmount, criticalAmount, action, delay, livingCount, attackEffect));
                    return true;
                }
            }
            return false;
        }

        public void BeatDirect(Living target, string action, int delay, int livingCount, int attackEffect)
        {
            this.m_game.AddAction(new LivingBeatDirectAction(this, target, action, delay, livingCount, attackEffect));
        }

        public bool BeatNpc(int fx, int tx, string action, int delay, List<NormalNpc> players)
        {
            this.m_game.AddAction(new NpcRangeAttackingAction(this, fx, tx, action, delay, players));
            return true;
        }

        public void BoltMove(int x, int y, int delay)
        {
            this.m_game.AddAction(new LivingBoltMoveAction(this, x, y, delay));
        }

        public double BoundDistance(Point p)
        {
            List<double> list = new List<double>();
            foreach (Rectangle rectangle in this.GetDirectBoudRect())
            {
                for (int i = rectangle.X; i <= (rectangle.X + rectangle.Width); i += 10)
                {
                    list.Add(Math.Sqrt((double) (((i - p.X) * (i - p.X)) + ((rectangle.Y - p.Y) * (rectangle.Y - p.Y)))));
                    list.Add(Math.Sqrt((double) (((i - p.X) * (i - p.X)) + (((rectangle.Y + rectangle.Height) - p.Y) * ((rectangle.Y + rectangle.Height) - p.Y)))));
                }
                for (int j = rectangle.Y; j <= (rectangle.Y + rectangle.Height); j += 10)
                {
                    list.Add(Math.Sqrt((double) (((rectangle.X - p.X) * (rectangle.X - p.X)) + ((j - p.Y) * (j - p.Y)))));
                    list.Add(Math.Sqrt((double) ((((rectangle.X + rectangle.Width) - p.X) * ((rectangle.X + rectangle.Width) - p.X)) + ((j - p.Y) * (j - p.Y)))));
                }
            }
            return ((IEnumerable<double>) list).Min();
        }

        public void CallFuction(LivingCallBack func, int delay)
        {
            if (this.m_game != null)
            {
                this.m_game.AddAction(new LivingCallFunctionAction(this, func, delay));
            }
        }

        public override void CollidedByObject(Physics phy)
        {
            if (phy is SimpleBomb)
            {
                ((SimpleBomb) phy).Bomb();
            }
        }

        public static double ComputDX(double v, float m, float af, float f, float dt)
        {
            return ((v * dt) + ((((f - (af * v)) / ((double) m)) * dt) * dt));
        }

        public static double ComputeVx(double dx, float m, float af, float f, float t)
        {
            return (((dx - ((((f / m) * t) * t) / 2f)) / ((double) t)) + (((af / m) * dx) * 0.7));
        }

        public static double ComputeVy(double dx, float m, float af, float f, float t)
        {
            return (((dx - ((((f / m) * t) * t) / 2f)) / ((double) t)) + (((af / m) * dx) * 1.3));
        }

        public void ChangeDirection(Living obj, int delay)
        {
            int direction = this.FindDirection(obj);
            if (delay > 0)
            {
                this.m_game.AddAction(new LivingChangeDirectionAction(this, direction, delay));
            }
            else
            {
                this.Direction = direction;
            }
        }

        public void ChangeDirection(int direction, int delay)
        {
            if (delay > 0)
            {
                this.m_game.AddAction(new LivingChangeDirectionAction(this, direction, delay));
            }
            else
            {
                this.Direction = direction;
            }
        }

        public void DemagemRect(int x, int y, int width, int height)
        {
            this.m_demageRect.Width = width;
            this.m_demageRect.Height = height;
        }

        public override void Die()
        {
            if (this.m_blood > 0)
            {
                this.m_blood = 0;
                this.m_doAction = -1;
                if (this.m_syncAtTime)
                {
                    this.m_game.SendGameUpdateHealth(this, 6, 0);
                }
            }
            if (base.IsLiving)
            {
                if (this.IsAttacking)
                {
                    this.StopAttacking();
                }
                base.Die();
                this.OnDied();
                this.m_game.CheckState(0);
            }
        }

        public virtual void Die(int delay)
        {
            if (base.IsLiving && (this.m_game != null))
            {
                this.m_game.AddAction(new LivingDieAction(this, delay));
            }
        }

        public double Distance(Point p)
        {
            List<double> list = new List<double>();
            Rectangle directDemageRect = this.GetDirectDemageRect();
            for (int i = directDemageRect.X; i <= (directDemageRect.X + directDemageRect.Width); i += 10)
            {
                list.Add(Math.Sqrt((double) (((i - p.X) * (i - p.X)) + ((directDemageRect.Y - p.Y) * (directDemageRect.Y - p.Y)))));
                list.Add(Math.Sqrt((double) (((i - p.X) * (i - p.X)) + (((directDemageRect.Y + directDemageRect.Height) - p.Y) * ((directDemageRect.Y + directDemageRect.Height) - p.Y)))));
            }
            for (int j = directDemageRect.Y; j <= (directDemageRect.Y + directDemageRect.Height); j += 10)
            {
                list.Add(Math.Sqrt((double) (((directDemageRect.X - p.X) * (directDemageRect.X - p.X)) + ((j - p.Y) * (j - p.Y)))));
                list.Add(Math.Sqrt((double) ((((directDemageRect.X + directDemageRect.Width) - p.X) * ((directDemageRect.X + directDemageRect.Width) - p.X)) + ((j - p.Y) * (j - p.Y)))));
            }
            return ((IEnumerable<double>) list).Min();
        }

        public bool FallFrom(int x, int y, string action, int delay, int type, int speed)
        {
            return this.FallFrom(x, y, action, delay, type, speed, null);
        }

        public bool FallFrom(int x, int y, string action, int delay, int type, int speed, LivingCallBack callback)
        {
            Point point = base.m_map.FindYLineNotEmptyPoint(x, y);
            if (point == Point.Empty)
            {
                point = new Point(x, this.m_game.Map.Bound.Height + 1);
            }
            if (this.Y < point.Y)
            {
                this.m_game.AddAction(new LivingFallingAction(this, point.X, point.Y, speed, action, delay, type, callback));
                return true;
            }
            return false;
        }

        public bool FallFromTo(int x, int y, string action, int delay, int type, int speed, LivingCallBack callback)
        {
            this.m_game.AddAction(new LivingFallingAction(this, x, y, speed, action, delay, type, callback));
            return true;
        }

        public int FindDirection(Living obj)
        {
            if (obj.X > this.X)
            {
                return 1;
            }
            return -1;
        }

        public bool FlyTo(int X, int Y, int x, int y, string action, int delay, int speed)
        {
            return this.FlyTo(X, Y, x, y, action, delay, speed, null);
        }

        public bool FlyTo(int X, int Y, int x, int y, string action, int delay, int speed, LivingCallBack callback)
        {
            this.m_game.AddAction(new LivingFlyToAction(this, X, Y, x, y, action, delay, speed, callback));
            this.m_game.AddAction(new LivingFallingAction(this, x, y, 0, action, delay, 0, callback));
            return true;
        }

        public List<Rectangle> GetDirectBoudRect()
        {
            return new List<Rectangle> { ((this.m_direction > 0) ? new Rectangle(this.X - base.Bound.X, this.Y + base.Bound.Y, base.Bound.Width, base.Bound.Height) : new Rectangle(this.X + base.Bound.X, this.Y + base.Bound.Y, base.Bound.Width, base.Bound.Height)), ((this.m_direction > 0) ? new Rectangle(this.X - base.Bound1.X, this.Y + base.Bound1.Y, base.Bound1.Width, base.Bound1.Height) : new Rectangle(this.X + base.Bound1.X, this.Y + base.Bound1.Y, base.Bound1.Width, base.Bound1.Height)) };
        }

        public Rectangle GetDirectDemageRect()
        {
            if (this.m_direction <= 0)
            {
                return new Rectangle(this.X + this.m_demageRect.X, this.Y + this.m_demageRect.Y, this.m_demageRect.Width, this.m_demageRect.Height);
            }
            return new Rectangle(this.X - this.m_demageRect.X, this.Y + this.m_demageRect.Y, this.m_demageRect.Width, this.m_demageRect.Height);
        }

        public double getHertAddition(SqlDataProvider.Data.ItemInfo item)
        {
            if (item == null)
            {
                return 0.0;
            }
            double num = item.Template.Property7;
            double y = item.StrengthenLevel + item.LianGrade;
            double a = (num * Math.Pow(1.1, y)) - num;
            return (Math.Round(a) + num);
        }

        public bool GetSealState()
        {
            return this.m_isSeal;
        }

        public void GetShootForceAndAngle(ref int x, ref int y, int bombId, int minTime, int maxTime, int bombCount, float time, ref int force, ref int angle)
        {
            if (minTime < maxTime)
            {
                BallInfo info = BallMgr.FindBall(bombId);
                if ((this.m_game != null) && (info != null))
                {
                    Map map = this.m_game.Map;
                    Point shootPoint = this.GetShootPoint();
                    float num = (float) (x - shootPoint.X);
                    float num2 = (float) (y - shootPoint.Y);
                    float af = map.airResistance * info.DragIndex;
                    float f = (map.gravity * info.Weight) * info.Mass;
                    float num5 = map.wind * info.Wind;
                    float mass = info.Mass;
                    for (float i = time; i <= 4f; i += 0.6f)
                    {
                        double num8 = ComputeVx((double) num, mass, af, num5, i);
                        double num9 = ComputeVy((double) num2, mass, af, f, i);
                        if ((num9 < 0.0) && ((num8 * this.m_direction) > 0.0))
                        {
                            double num10 = Math.Sqrt((num8 * num8) + (num9 * num9));
                            if (num10 < 2000.0)
                            {
                                force = (int) num10;
                                angle = (int) ((Math.Atan(num9 / num8) / 3.1415926535897931) * 180.0);
                                if (num8 < 0.0)
                                {
                                    angle += 180;
                                }
                                break;
                            }
                        }
                    }
                    x = shootPoint.X;
                    y = shootPoint.Y;
                }
            }
        }

        public Point GetShootPoint()
        {
            if (this is SimpleBoss)
            {
                if (this.m_direction <= 0)
                {
                    return new Point(this.X + ((SimpleBoss) this).NpcInfo.FireX, this.Y + ((SimpleBoss) this).NpcInfo.FireY);
                }
                return new Point(this.X - ((SimpleBoss) this).NpcInfo.FireX, this.Y + ((SimpleBoss) this).NpcInfo.FireY);
            }
            if (this.m_direction <= 0)
            {
                return new Point((this.X + this.m_rect.X) - 5, (this.Y + this.m_rect.Y) - 5);
            }
            return new Point((this.X - this.m_rect.X) + 5, (this.Y + this.m_rect.Y) - 5);
        }

        public bool IconPicture(eMirariType type, bool result)
        {
            this.m_game.SendPlayerPicture(this, (int) type, result);
            return true;
        }

        public bool IsFriendly(Living living)
        {
            return (!(living is Player) && (living.Team == this.Team));
        }

        public bool JumpTo(int x, int y, string action, int delay, int type)
        {
            return this.JumpTo(x, y, action, delay, type, 20, null);
        }

        public bool JumpTo(int x, int y, string ation, int delay, int type, LivingCallBack callback)
        {
            return this.JumpTo(x, y, ation, delay, type, 20, callback);
        }

        public bool JumpTo(int x, int y, string action, int delay, int type, int speed, LivingCallBack callback)
        {
            Point point = base.m_map.FindYLineNotEmptyPoint(x, y);
            if (point.Y < this.Y)
            {
                this.m_game.AddAction(new LivingJumpAction(this, point.X, point.Y, speed, action, delay, type, callback));
                return true;
            }
            return false;
        }

        public bool JumpTo(int x, int y, string action, int delay, int type, int speed, LivingCallBack callback, int value)
        {
            Point point = base.m_map.FindYLineNotEmptyPointDown(x, y);
            if ((point.Y >= this.Y) && (value != 1))
            {
                return false;
            }
            this.m_game.AddAction(new LivingJumpAction(this, point.X, point.Y, speed, action, delay, type, callback));
            return true;
        }

        public bool JumpToSpeed(int x, int y, string action, int delay, int type, int speed, LivingCallBack callback)
        {
            Point point = base.m_map.FindYLineNotEmptyPoint(x, y);
            int num = point.Y;
            this.m_game.AddAction(new LivingJumpAction(this, point.X, point.Y, speed, action, delay, type, callback));
            return true;
        }

        public int MakeCritical(Living living)
        {
            return this.MakeDamage(living);
        }

        public int MakeCriticalDamage(Living p, int baseDamage)
        {
            double lucky = this.Lucky;
            Random random = new Random();
            if (((75000.0 * lucky) / (lucky + 800.0)) > random.Next(0x186a0))
            {
                return (int) ((0.5 + (lucky * 0.0003)) * baseDamage);
            }
            return 0;
        }

        protected int MakeDamage(Living target)
        {
            double num9;
            if (target.Config.IsChristmasBoss)
            {
                return 1;
            }
            double baseDamage = this.BaseDamage;
            double baseGuard = target.BaseGuard;
            double defence = target.Defence;
            double attack = this.Attack;
            if (target.AddArmor && ((target as Player).DeputyWeapon != null))
            {
                int num5 = (int) this.getHertAddition((target as Player).DeputyWeapon);
                baseGuard += num5;
                defence += num5;
            }
            if (this.IgnoreArmor)
            {
                baseGuard = 0.0;
                defence = 0.0;
            }
            float currentDamagePlus = this.CurrentDamagePlus;
            float currentShootMinus = this.CurrentShootMinus;
            double num8 = (0.95 * (baseGuard - (3 * this.Grade))) / ((500.0 + baseGuard) - (3 * this.Grade));
            if ((defence - this.Lucky) < 0.0)
            {
                num9 = 0.0;
            }
            else
            {
                num9 = (0.95 * (defence - this.Lucky)) / ((600.0 + defence) - this.Lucky);
            }
            double num10 = (((baseDamage * (1.0 + (attack * 0.001))) * (1.0 - ((num8 + num9) - (num8 * num9)))) * currentDamagePlus) * currentShootMinus;
            new Point(this.X, this.Y);
            if (num10 < 0.0)
            {
                return 1;
            }
            return (int) num10;
        }

        public int MakeDamage(Living target, bool them = false)
        {
            double baseGuard = target.BaseGuard;
            double defence = target.Defence;
            double attack = this.Attack;
            if (target.AddArmor && ((target as Player).DeputyWeapon != null))
            {
                int num4 = (int) this.getHertAddition((target as Player).DeputyWeapon);
                baseGuard += num4;
                defence += num4;
            }
            if (this.IgnoreArmor)
            {
                baseGuard = 0.0;
                defence = 0.0;
            }
            float currentDamagePlus = this.CurrentDamagePlus;
            float currentShootMinus = this.CurrentShootMinus;
            double num7 = (0.95 * (baseGuard - (3 * this.Grade))) / ((500.0 + baseGuard) - (3 * this.Grade));
            double num8 = 0.0;
            if ((defence - this.Lucky) < 0.0)
            {
                num8 = 0.0;
            }
            else
            {
                num8 = (0.95 * (defence - this.Lucky)) / ((600.0 + defence) - this.Lucky);
            }
            double num9 = (((this.BaseDamage * (1.0 + (attack * 0.001))) * (1.0 - ((num7 + num8) - (num7 * num8)))) * currentDamagePlus) * currentShootMinus;
            new Point(this.X, this.Y);
            if (num9 < 0.0)
            {
                return 1;
            }
            return (int) num9;
        }

        public int MakeDamageBoss(Living p)
        {
            double baseDamage = this.BaseDamage;
            double baseGuard = p.BaseGuard;
            double defence = p.Defence;
            double attack = this.Attack;
            if (this.IgnoreArmor)
            {
                baseGuard = 0.0;
                defence = 0.0;
            }
            float currentDamagePlus = this.CurrentDamagePlus;
            float currentShootMinus = this.CurrentShootMinus;
            double num7 = (0.95 * (p.BaseGuard - (3 * this.Grade))) / ((500.0 + p.BaseGuard) - (3 * this.Grade));
            double num8 = 0.0;
            if ((p.Defence - this.Lucky) < 0.0)
            {
                num8 = 0.0;
            }
            else
            {
                num8 = (0.95 * (p.Defence - this.Lucky)) / ((600.0 + p.Defence) - this.Lucky);
            }
            double num9 = (((baseDamage * (1.0 + (attack * 0.001))) * (1.0 - ((num7 + num8) - (num7 * num8)))) * currentDamagePlus) * currentShootMinus;
            Rectangle directDemageRect = p.GetDirectDemageRect();
            double num10 = Math.Sqrt((double) (((directDemageRect.X - this.X) * (directDemageRect.X - this.X)) + ((directDemageRect.Y - this.Y) * (directDemageRect.Y - this.Y))));
            num9 *= 1.0 - ((num10 / ((double) Math.Abs((int) ((this.X + p.X) - this.X)))) / 4.0);
            if (((this.X + p.X) - this.X) <= 0)
            {
                num9 *= 1.0 - (num10 / ((double) Math.Abs((int) ((this.X + p.X) - this.X))));
            }
            if (num9 < 0.0)
            {
                return 1;
            }
            return (int) num9;
        }

        public bool MoveTo(int x, int y, string action, int delay)
        {
            return this.MoveTo(x, y, action, delay, (LivingCallBack) null);
        }

        public bool MoveTo(int x, int y, string action, int delay, LivingCallBack callback)
        {
            if ((base.m_x != x) || (base.m_y != y))
            {
                if ((x < 0) || (x > base.m_map.Bound.Width))
                {
                    return false;
                }
                List<Point> path = new List<Point>();
                int num = base.m_x;
                int num2 = base.m_y;
                int direction = (x > num) ? 1 : -1;
                while (((x - num) * direction) > 0)
                {
                    Point item = base.m_map.FindNextWalkPoint(num, num2, direction, STEP_X, STEP_Y);
                    if (item != Point.Empty)
                    {
                        path.Add(item);
                        num = item.X;
                        num2 = item.Y;
                    }
                    else
                    {
                        break;
                    }
                }
                if (path.Count > 0)
                {
                    this.m_game.AddAction(new LivingMoveToAction(this, path, action, delay, 4, callback));
                    return true;
                }
            }
            return false;
        }

        public bool MoveTo(int x, int y, string action, int delay, int speed)
        {
            return this.MoveTo(x, y, action, "", speed, delay, null, 0);
        }

        public bool MoveTo(int x, int y, string action, int delay, LivingCallBack callback, int speed)
        {
            return this.MoveTo(x, y, action, "", speed, delay, callback, 0);
        }

        public bool MoveTo(int x, int y, string action, int delay, string sAction, int speed)
        {
            return this.MoveTo(x, y, action, delay, sAction, speed, null);
        }

        public bool MoveTo(int x, int y, string action, int delay, string sAction, int speed, LivingCallBack callback)
        {
            return this.MoveTo(x, y, action, delay, sAction, speed, callback, 0);
        }

        public bool MoveTo(int x, int y, string action, string sAction, int speed, int delay, LivingCallBack callback)
        {
            return this.MoveTo(x, y, action, sAction, speed, delay, callback, 0);
        }

        public bool MoveTo(int x, int y, string action, int delay, string sAction, int speed, LivingCallBack callback, int delayCallback)
        {
            if ((base.m_x != x) || (base.m_y != y))
            {
                if ((x < 0) || (x > base.m_map.Bound.Width))
                {
                    return false;
                }
                List<Point> path = new List<Point>();
                int num = base.m_x;
                int num2 = base.m_y;
                int direction = (x > num) ? 1 : -1;
                if (!(action == "fly"))
                {
                    while (((x - num) * direction) > 0)
                    {
                        Point item = base.m_map.FindNextWalkPoint(num, num2, direction, speed * STEP_X, speed * STEP_Y);
                        if (!(item != Point.Empty))
                        {
                            break;
                        }
                        path.Add(item);
                        num = item.X;
                        num2 = item.Y;
                    }
                }
                else
                {
                    Point point = new Point(x, y);
                    Point point2 = new Point(num, num2);
                    Point point3 = new Point(x - point2.X, y - point2.Y);
                    while (point3.Length() > speed)
                    {
                        point3.Normalize(speed);
                        point2 = new Point(point2.X + point3.X, point2.Y + point3.Y);
                        point3 = new Point(x - point2.X, y - point2.Y);
                        if (!(point2 != Point.Empty))
                        {
                            path.Add(point);
                            break;
                        }
                        path.Add(point2);
                    }
                }
                if (path.Count > 0)
                {
                    this.m_game.AddAction(new LivingMoveToAction(this, path, action, delay, speed, sAction, callback, delayCallback));
                    return true;
                }
            }
            return false;
        }

        public bool MoveTo(int x, int y, string action, string sAction, int speed, int delay, LivingCallBack callback, int delayCallback)
        {
            if ((base.m_x != x) || (base.m_y != y))
            {
                if ((x < 0) || (x > base.m_map.Bound.Width))
                {
                    return false;
                }
                List<Point> path = new List<Point>();
                int num = base.m_x;
                int num2 = base.m_y;
                int direction = (x > num) ? 1 : -1;
                Point item = new Point(num, num2);
                if (x >= base.m_x)
                {
                }
                if (y >= base.m_y)
                {
                }
                if (!this.Config.IsFly)
                {
                    while (((x - num) * direction) > 0)
                    {
                        item = base.m_map.FindNextWalkPointDown(num, num2, direction, speed * STEP_X, speed * STEP_Y);
                        if (!(item != Point.Empty))
                        {
                            break;
                        }
                        path.Add(item);
                        num = item.X;
                        num2 = item.Y;
                    }
                }
                else
                {
                    Point point = new Point(x - item.X, y - item.Y);
                    while (point.Length() > speed)
                    {
                        point = point.Normalize(speed);
                        item = new Point(item.X + point.X, item.Y + point.Y);
                        point = new Point(x - item.X, y - item.Y);
                        if (!(item != Point.Empty))
                        {
                            path.Add(new Point(x, y));
                            break;
                        }
                        path.Add(item);
                    }
                }
                if (path.Count > 0)
                {
                    this.m_game.AddAction(new LivingMoveToAction2(this, path, action, sAction, speed, delay, callback, delayCallback));
                    return true;
                }
            }
            return false;
        }

        public bool MoveTo2(int x, int y, string action, int delay, string sAction, int speed, LivingCallBack callback)
        {
            if ((base.m_x != x) || (base.m_y != y))
            {
                Point point;
                if ((x < 0) || (x > base.m_map.Bound.Width))
                {
                    return false;
                }
                List<Point> path = new List<Point>();
                int num = base.m_x;
                int num2 = base.m_y;
                int direction = (x > num) ? 1 : -1;
                if (!(action == "fly"))
                {
                    while (((x - num) * direction) > 0)
                    {
                        point = base.m_map.FindNextWalkPoint(num, num2, direction, STEP_X, STEP_Y);
                        if (!(point != Point.Empty))
                        {
                            break;
                        }
                        path.Add(point);
                        num = point.X;
                        num2 = point.Y;
                    }
                }
                else
                {
                    point = new Point(x, y);
                    Point point2 = new Point(num - point.X, num2 - point.Y);
                    point = new Point(point.X + point2.X, y + point2.X);
                    point2 = new Point(x - point.X, x - point.Y);
                    if (point == Point.Empty)
                    {
                    }
                    point = new Point(x, y);
                    path.Add(point);
                }
                if (path.Count > 0)
                {
                    this.m_game.AddAction(new LivingMoveToAction(this, path, action, delay, speed, callback));
                    return true;
                }
            }
            return false;
        }

        public bool MoveToSpeed(int x, int y, string action, int delay, int speed, LivingCallBack callback)
        {
            if ((base.m_x != x) || (base.m_y != y))
            {
                if ((x < 0) || (x > base.m_map.Bound.Width))
                {
                    return false;
                }
                List<Point> path = new List<Point>();
                int num = base.m_x;
                int num2 = base.m_y;
                int direction = (x > num) ? 1 : -1;
                while (((x - num) * direction) > 0)
                {
                    Point item = base.m_map.FindNextWalkPoint(num, num2, direction, (speed / 2) - 2, STEP_Y);
                    if (item != Point.Empty)
                    {
                        path.Add(item);
                        num = item.X;
                        num2 = item.Y;
                    }
                    else
                    {
                        break;
                    }
                }
                if (path.Count > 0)
                {
                    this.m_game.AddAction(new LivingMoveToAction(this, path, action, delay, speed, callback));
                    return true;
                }
            }
            return false;
        }

        public bool MoveToSpeed(int x, int y, string action, int delay, int speed, LivingCallBack callback, string sAction)
        {
            if ((base.m_x != x) || (base.m_y != y))
            {
                if ((x < 0) || (x > base.m_map.Bound.Width))
                {
                    return false;
                }
                List<Point> path = new List<Point>();
                int num = base.m_x;
                int num2 = base.m_y;
                int direction = (x > num) ? 1 : -1;
                while (((x - num) * direction) > 0)
                {
                    Point item = base.m_map.FindNextWalkPoint(num, num2, direction, (speed / 2) - 2, STEP_Y);
                    if (item != Point.Empty)
                    {
                        path.Add(item);
                        num = item.X;
                        num2 = item.Y;
                    }
                    else
                    {
                        break;
                    }
                }
                if (path.Count > 0)
                {
                    this.m_game.AddAction(new LivingMoveToAction(this, path, action, delay, speed, sAction, callback, 0));
                    return true;
                }
            }
            return false;
        }

        public void NoFly(bool value)
        {
            if (this.m_syncAtTime)
            {
                this.m_game.SendGamePlayerProperty(this, "nofly", value.ToString());
            }
        }

        public virtual void OnAfterKillingLiving(Living target, int damageAmount, int criticalAmount)
        {
            if (target.Team != this.Team)
            {
                this.CurrentIsHitTarget = true;
                this.TotalHurt += damageAmount + criticalAmount;
                if (!target.IsLiving)
                {
                    this.TotalKill++;
                }
                this.m_game.CurrentTurnTotalDamage = damageAmount + criticalAmount;
                this.m_game.TotalHurt += damageAmount + criticalAmount;
            }
            if (this.AfterKillingLiving != null)
            {
                this.AfterKillingLiving(this, target, damageAmount, criticalAmount);
            }
        }

        public virtual void OnAfterTakedBomb()
        {
        }

        public void OnAfterTakedDamage(Living target, int damageAmount, int criticalAmount)
        {
            if (this.AfterKilledByLiving != null)
            {
                this.AfterKilledByLiving(this, target, damageAmount, criticalAmount);
            }
        }

        public virtual void OnAfterTakedFrozen()
        {
        }

        protected void OnBeforeTakedDamage(Living source, ref int damageAmount, ref int criticalAmount)
        {
            if (this.BeforeTakeDamage != null)
            {
                this.BeforeTakeDamage(this, source, ref damageAmount, ref criticalAmount);
            }
        }

        protected void OnBeginNewTurn()
        {
            if (this.BeginNextTurn != null)
            {
                this.BeginNextTurn(this);
            }
        }

        protected void OnBeginSelfTurn()
        {
            if (this.BeginSelfTurn != null)
            {
                this.BeginSelfTurn(this);
            }
        }

        protected void OnDied()
        {
            if (this.Died != null)
            {
                this.Died(this);
            }
            if ((this is Player) && (this.Game is PVEGame))
            {
                ((PVEGame) this.Game).DoOther();
            }
        }

        public void OnSmallMap(bool state)
        {
            if (this.m_syncAtTime)
            {
                this.m_game.SendGamePlayerProperty(this, "onSmallMap", state.ToString());
            }
        }

        protected void OnStartAttacked()
        {
            if (this.BeginAttacked != null)
            {
                this.BeginAttacked(this);
            }
        }

        protected void OnStartAttacking()
        {
            if (this.BeginAttacking != null)
            {
                this.BeginAttacking(this);
            }
        }

        protected void OnStopAttacking()
        {
            if (this.EndAttacking != null)
            {
                this.EndAttacking(this);
            }
        }

        public void OnTakedDamage(Living source, ref int damageAmount, ref int criticalAmount)
        {
            if (this.TakePlayerDamage != null)
            {
                this.TakePlayerDamage(this, source, ref damageAmount, ref criticalAmount);
            }
        }

        public virtual bool PetTakeDamage(Living source, ref int damageAmount, ref int criticalAmount, string msg)
        {
            if (this.Config.IsHelper && ((this is SimpleNpc) || (this is SimpleBoss)))
            {
                return false;
            }
            bool flag = false;
            if (this.m_blood > 0)
            {
                this.m_blood -= damageAmount + criticalAmount;
                if (this.m_blood <= 0)
                {
                    this.Die();
                }
                flag = true;
            }
            return flag;
        }

        public virtual void PickBall(Ball ball)
        {
            ball.Die();
            string currentAction = ball.CurrentAction;
            ball.PlayMovie(ball.ActionMapping[currentAction], 0x3e8, 0);
        }

        public virtual void PickBox(Box box)
        {
            box.UserID = base.Id;
            box.Die();
            if (this.m_syncAtTime)
            {
                this.m_game.SendGamePickBox(this, box.Id, 0, "");
            }
        }

        public bool PlayerBeat(Living target, string action, int demageAmount, int criticalAmount, int delay)
        {
            if (!((target != null) && target.IsLiving))
            {
                return false;
            }
            demageAmount = this.MakeDamage(target);
            this.OnBeforeTakedDamage(target, ref demageAmount, ref criticalAmount);
            this.StartAttacked();
            this.m_game.AddAction(new LivingBeatAction(this, target, demageAmount, criticalAmount, action, delay, 1, 0));
            return true;
        }

        public void PlayMovie(string action, int delay, int MovieTime)
        {
            this.m_game.AddAction(new LivingPlayeMovieAction(this, action, delay, MovieTime));
        }

        public void PlayMovie(string action, int delay, int MovieTime, LivingCallBack call)
        {
            this.m_game.AddAction(new LivingPlayeMovieAction(this, action, delay, MovieTime));
        }

        public void PrepareAttackGemLilit()
        {
            if (this.AttackGemLimit > 0)
            {
                this.AttackGemLimit--;
            }
        }

        public void PrepareDefendGem()
        {
            if ((this.DefenFisrtGem > 0) && (this.DefenSecondGem > 0))
            {
                int[] numArray = new int[] { this.DefenFisrtGem, this.DefenSecondGem };
                int index = this.rand.Next(numArray.Length);
                this.DefendActiveGem = numArray[index];
            }
            else
            {
                this.DefendActiveGem = this.DefenFisrtGem;
            }
        }

        public override void PrepareNewTurn()
        {
            this.ShootMovieDelay = 0;
            this.CurrentDamagePlus = 1f;
            this.CurrentShootMinus = 1f;
            this.IgnoreArmor = false;
            this.ControlBall = false;
            this.NoHoleTurn = false;
            this.CurrentIsHitTarget = false;
            this.PrepareAttackGemLilit();
            this.PrepareDefendGem();
            this.OnBeginNewTurn();
        }

        public virtual void PrepareSelfTurn()
        {
            this.OnBeginSelfTurn();
        }

        public bool RangeAttacking(int fx, int tx, string action, int delay, bool directDamage)
        {
            return this.RangeAttacking(fx, tx, action, delay, true, directDamage, null);
        }

        public bool RangeAttacking(int fx, int tx, string action, int delay, List<Player> players)
        {
            if (base.IsLiving)
            {
                this.m_game.AddAction(new LivingRangeAttackingAction(this, fx, tx, action, delay, players));
                return true;
            }
            return false;
        }

        public bool RangeAttacking(int fx, int tx, string action, int delay, bool removeFrost, bool directDamage, List<Player> players)
        {
            if (base.IsLiving)
            {
                this.m_game.AddAction(new LivingRangeAttackingAction2(this, fx, tx, action, delay, removeFrost, directDamage, players));
                return true;
            }
            return false;
        }

        public virtual int ReducedBlood(int value)
        {
            this.m_blood += value;
            if (this.m_blood > this.m_maxBlood)
            {
                this.m_blood = this.m_maxBlood;
            }
            if (this.m_syncAtTime)
            {
                this.m_game.SendGameUpdateHealth(this, 1, value);
            }
            if (this.m_blood <= 0)
            {
                this.Die();
            }
            return value;
        }

        public virtual void Reset()
        {
            this.m_blood = this.m_maxBlood;
            this.m_isFrost = false;
            this.m_isHide = false;
            this.m_isNoHole = false;
            base.m_isLiving = true;
            this.TurnNum = 0;
            this.TotalHurt = 0;
            this.TotalKill = 0;
            this.TotalShootCount = 0;
            this.TotalHitTargetCount = 0;
        }

        public void Say(string msg, int type, int delay)
        {
            this.m_game.AddAction(new LivingSayAction(this, msg, type, delay, 0x3e8));
        }

        public void Say(string msg, int type, int delay, int finishTime)
        {
            this.m_game.AddAction(new LivingSayAction(this, msg, type, delay, finishTime));
        }

        public void Seal(Player player, int type, int delay)
        {
            this.m_game.AddAction(new LivingSealAction(this, player, type, delay));
        }

        public void SetHidden(bool state)
        {
            if (this.m_syncAtTime)
            {
                this.m_game.SendGamePlayerProperty(this, "visible", state.ToString());
            }
        }

        public void SetIceFronze(Living living)
        {
            new IceFronzeEffect(2).Start(this);
            this.BeginNextTurn -= new LivingEventHandle(this.SetIceFronze);
        }

        public void SetIndian(bool state)
        {
            if (this.m_syncAtTime)
            {
                this.m_game.SendPlayerPicture(this, 0x22, state);
            }
        }

        public void SetNiutou(bool state)
        {
            if (this.m_syncAtTime)
            {
                this.m_game.SendPlayerPicture(this, 0x21, state);
            }
        }

        public void SetOffsetY(int p)
        {
            this.m_game.method_34(this, "offsetY", p.ToString());
        }

        public void SetRelateDemagemRect(int x, int y, int width, int height)
        {
            this.m_demageRect.X = x;
            this.m_demageRect.Y = y;
            this.m_demageRect.Width = width;
            this.m_demageRect.Height = height;
        }

        public void SetSeal(bool state)
        {
            if (this.m_isSeal != state)
            {
                this.m_isSeal = state;
                if (this.m_syncAtTime)
                {
                    this.m_game.SendGamePlayerProperty(this, "silenceMany", state.ToString());
                }
            }
        }

        public void SetSeal(bool state, int type)
        {
            if (this.m_isSeal != state)
            {
                this.m_isSeal = state;
                if (this.m_syncAtTime)
                {
                    this.m_game.SendGameUpdateSealState(this);
                }
            }
        }

        public void SetTargeting(bool state)
        {
            if (this.m_syncAtTime)
            {
                this.m_game.SendPlayerPicture(this, 7, state);
            }
        }

        public void SetupPetEffect()
        {
            this.m_petEffects = new PetEffectInfo();
            this.m_petEffects.CritActive = false;
            this.m_petEffects.ActivePetHit = false;
            this.m_petEffects.PetDelay = 0;
            this.m_petEffects.PetBaseAtt = 0;
            this.m_petEffects.CurrentUseSkill = 0;
            this.m_petEffects.ActiveGuard = false;
        }

        public void SetVisible(bool state)
        {
            this.m_game.method_34(this, "visible", state.ToString());
        }

        public void SetXY(int x, int y, int delay)
        {
            this.m_game.AddAction(new LivingDirectSetXYAction(this, x, y, delay));
        }

        public bool Shoot(int bombId, int x, int y, int force, int angle, int bombCount, int delay)
        {
            this.m_game.AddAction(new LivingShootAction(this, bombId, x, y, force, angle, bombCount, delay, 0, 0f, 0));
            return true;
        }

        public bool ShootImp(int bombId, int x, int y, int force, int angle, int bombCount, int shootCount)
        {
            BallInfo info = BallMgr.FindBall(bombId);
            Tile shape = BallMgr.FindTile(bombId);
            BombType ballType = BallMgr.GetBallType(bombId);
            if (info == null)
            {
                return false;
            }
            GSPacketIn pkg = new GSPacketIn(0x5b, base.Id) {
                Parameter1 = base.Id
            };
            pkg.WriteByte(2);
            pkg.WriteInt(bombCount);
            float num = 0f;
            for (int i = 0; i < bombCount; i++)
            {
                double num3 = 1.0;
                int num4 = 0;
                switch (i)
                {
                    case 1:
                        num3 = 0.9;
                        num4 = -5;
                        break;

                    case 2:
                        num3 = 1.1;
                        num4 = 5;
                        break;
                }
                int vx = (int) ((force * num3) * Math.Cos((((double) (angle + num4)) / 180.0) * 3.1415926535897931));
                int vy = (int) ((force * num3) * Math.Sin((((double) (angle + num4)) / 180.0) * 3.1415926535897931));
                int physicalId = this.m_game.PhysicalId;
                this.m_game.PhysicalId = physicalId + 1;
                SimpleBomb phy = new SimpleBomb(physicalId, ballType, this, this.m_game, info, shape, this.ControlBall);
                phy.SetXY(x, y);
                phy.setSpeedXY(vx, vy);
                base.m_map.AddPhysical(phy);
                phy.StartMoving();
                pkg.WriteInt(0);
                pkg.WriteInt(0);
                pkg.WriteBoolean(phy.DigMap);
                pkg.WriteInt(phy.Id);
                pkg.WriteInt(x);
                pkg.WriteInt(y);
                pkg.WriteInt(vx);
                pkg.WriteInt(vy);
                pkg.WriteInt(phy.BallInfo.ID);
                if (this.FlyingPartical != 0)
                {
                    pkg.WriteString(this.FlyingPartical.ToString());
                }
                else
                {
                    pkg.WriteString(info.FlyingPartical);
                }
                pkg.WriteInt((phy.BallInfo.Radii * 0x3e8) / 4);
                pkg.WriteInt(((int) phy.BallInfo.Power) * 0x3e8);
                pkg.WriteInt(phy.Actions.Count);
                foreach (BombAction action in phy.Actions)
                {
                    pkg.WriteInt(action.TimeInt);
                    pkg.WriteInt(action.Type);
                    pkg.WriteInt(action.Param1);
                    pkg.WriteInt(action.Param2);
                    pkg.WriteInt(action.Param3);
                    pkg.WriteInt(action.Param4);
                }
                num = Math.Max(num, phy.LifeTime);
            }
            this.m_game.SendToAll(pkg);
            if ((bombCount == this.countBoom) && (bombCount == 3))
            {
                this.m_game.AddAction(new FightAchievementAction(this, 8, this.Direction, 0x4b0));
            }
            int num8 = (int) (((num + 1f) + (bombCount / 3)) * 1000f);
            this.m_game.WaitTime((int) (((num + 2f) + (bombCount / 3)) * 1000f));
            this.LastLifeTimeShoot = num8;
            return true;
        }

        public bool ShootPoint(int x, int y, int bombId, int minTime, int maxTime, int bombCount, float time, int delay)
        {
            this.m_game.AddAction(new LivingShootAction(this, bombId, x, y, 0, 0, bombCount, minTime, maxTime, time, delay));
            return true;
        }

        public bool ShootPoint(int x, int y, int bombId, int minTime, int maxTime, int bombCount, float time, int delay, LivingCallBack callBack)
        {
            this.m_game.AddAction(new LivingShootAction(this, bombId, x, y, 0, 0, bombCount, minTime, maxTime, time, delay, callBack));
            return true;
        }

        public void SpeedMultX(int value)
        {
            if (this.m_syncAtTime)
            {
                this.m_game.SendGamePlayerProperty(this, "speedX", value.ToString());
            }
        }

        public void SpeedMultX(int value, string _tpye)
        {
            if (this.m_syncAtTime)
            {
                this.m_game.SendGamePlayerProperty(this, _tpye, value.ToString());
            }
        }

        public void SpeedMultY(int value)
        {
            if (this.m_syncAtTime)
            {
                this.m_game.SendGamePlayerProperty(this, "speedY", value.ToString());
            }
        }

        public void StartAttacked()
        {
            this.OnStartAttacked();
        }

        public virtual void StartAttacking()
        {
            if (!this.m_isAttacking)
            {
                this.m_isAttacking = true;
                this.OnStartAttacking();
            }
        }

        public override void StartMoving()
        {
            this.StartMoving(0, 30);
        }

        public virtual void StartMoving(int delay, int speed)
        {
            if (!this.Config.IsFly)
            {
                if (base.m_map.IsEmpty(this.X, this.Y))
                {
                    this.FallFrom(this.X, this.Y, null, delay, 0, speed);
                }
                base.StartMoving();
            }
        }

        public virtual void StopAttacking()
        {
            if (this.m_isAttacking)
            {
                this.m_isAttacking = false;
                this.OnStopAttacking();
            }
        }

        public virtual bool TakeDamage(Living source, ref int damageAmount, ref int criticalAmount, string msg)
        {
            if ((this.Config.IsHelper && ((this is SimpleNpc) || (this is SimpleBoss))) && (source is Player))
            {
                return false;
            }
            bool flag = false;
            if (!this.IsFrost && (this.m_blood > 0))
            {
                if ((source != this) || (source.Team == this.Team))
                {
                    this.OnBeforeTakedDamage(source, ref damageAmount, ref criticalAmount);
                    this.StartAttacked();
                }
                int num = ((damageAmount + criticalAmount) < 0) ? 1 : (damageAmount + criticalAmount);
                if (this is Player)
                {
                    int reduceDamePlus = (this as Player).PlayerDetail.PlayerCharacter.ReduceDamePlus;
                    int num3 = (num * reduceDamePlus) / 100;
                    num -= num3;
                }
                this.m_blood -= num;
                int num4 = (this.m_maxBlood * 30) / 100;
                if ((((this is Player) && (this.m_blood < num4)) && (this.m_blood > 0)) && (this as Player).PlayerDetail.UseKingBlessHelpStraw(this.m_game.RoomType))
                {
                    (this as Player).PlayerDetail.SendMessage(string.Format("{0} trong l\x00fac nguy nan sử đụng Rơm cứu mạng hồi phục {1} m\x00e1u.", (this as Player).PlayerDetail.PlayerCharacter.NickName, num4));
                    this.AddBlood(num4);
                }
                if (this.m_syncAtTime)
                {
                    if ((this is SimpleBoss) && (((SimpleBoss) this).NpcInfo.ID == 0))
                    {
                        this.m_game.SendGameUpdateHealth(this, 6, num);
                    }
                    else
                    {
                        this.m_game.SendGameUpdateHealth(this, 1, num);
                    }
                }
                this.OnAfterTakedDamage(source, damageAmount, criticalAmount);
                if ((this.m_blood <= 0) && (this.m_game.RoomType != eRoomType.FightFootballTime))
                {
                    if ((criticalAmount > 0) && (this is Player))
                    {
                        this.m_game.AddAction(new FightAchievementAction(source, 7, source.Direction, 0x4b0));
                    }
                    this.Die();
                }
                source.OnAfterKillingLiving(this, damageAmount, criticalAmount);
                flag = true;
            }
            this.EffectList.StopEffect(typeof(IceFronzeEffect));
            this.EffectList.StopEffect(typeof(HideEffect));
            this.EffectList.StopEffect(typeof(NoHoleEffect));
            return flag;
        }

        public bool WalkTo(int x, int y, string action, int delay, LivingCallBack callback)
        {
            if ((base.m_x != x) || (base.m_y != y))
            {
                if ((x < 0) || (x > base.m_map.Bound.Width))
                {
                    return false;
                }
                List<Point> path = new List<Point>();
                int num = base.m_x;
                int num2 = base.m_y;
                int direction = (x > num) ? 1 : -1;
                while (((x - num) * direction) > 0)
                {
                    Point item = base.m_map.FindNextWalkPoint(num, num2, direction, STEP_X, STEP_Y);
                    if (item != Point.Empty)
                    {
                        path.Add(item);
                        num = item.X;
                        num2 = item.Y;
                    }
                    else
                    {
                        break;
                    }
                }
                if (path.Count > 0)
                {
                    this.m_game.AddAction(new LivingMoveToAction(this, path, action, delay, 3, callback));
                    return true;
                }
            }
            return false;
        }

        public string ActionStr
        {
            get
            {
                return this.m_action;
            }
            set
            {
                this.m_action = value;
            }
        }

        public bool AutoBoot
        {
            get
            {
                return this.m_autoBoot;
            }
            set
            {
                this.m_autoBoot = value;
            }
        }

        public bool BiKhoa
        {
            get
            {
                return this.bikhoa;
            }
            set
            {
                this.bikhoa = value;
            }
        }

        public bool BlockTurn
        {
            get
            {
                return this.bool_7;
            }
            set
            {
                this.bool_7 = value;
            }
        }

        public int Blood
        {
            get
            {
                return this.m_blood;
            }
            set
            {
                this.m_blood = value;
            }
        }

        public LivingConfig Config
        {
            get
            {
                return this.m_config;
            }
            set
            {
                this.m_config = value;
            }
        }

        public int Direction
        {
            get
            {
                return this.m_direction;
            }
            set
            {
                if (this.m_direction != value)
                {
                    this.m_direction = value;
                    base.SetRect(-this.m_rect.X - this.m_rect.Width, this.m_rect.Y, this.m_rect.Width, this.m_rect.Height);
                    base.SetRectBomb(-this.m_rectBomb.X - this.m_rectBomb.Width, this.m_rectBomb.Y, this.m_rectBomb.Width, this.m_rectBomb.Height);
                    this.SetRelateDemagemRect(-this.m_demageRect.X - this.m_demageRect.Width, this.m_demageRect.Y, this.m_demageRect.Width, this.m_demageRect.Height);
                    if (this.m_syncAtTime)
                    {
                        this.m_game.SendLivingUpdateDirection(this);
                    }
                }
            }
        }

        public int DoAction
        {
            get
            {
                return this.m_doAction;
            }
            set
            {
                if (this.m_doAction != value)
                {
                    this.m_doAction = value;
                }
            }
        }

        public Game.Logic.Effects.EffectList EffectList
        {
            get
            {
                return this.m_effectList;
            }
        }

        public int FallCount
        {
            get
            {
                return this.m_FallCount;
            }
            set
            {
                this.m_FallCount = value;
            }
        }

        public FightBufferInfo FightBuffers
        {
            get
            {
                return this.m_fightBufferInfo;
            }
            set
            {
                this.m_fightBufferInfo = value;
            }
        }

        public int FindCount
        {
            get
            {
                return this.m_FindCount;
            }
            set
            {
                this.m_FindCount = value;
            }
        }

        public int FireX { get; set; }

        public int FireY { get; set; }

        public BaseGame Game
        {
            get
            {
                return this.m_game;
            }
        }

        public bool IsAttacking
        {
            get
            {
                return this.m_isAttacking;
            }
        }

        public bool IsFrost
        {
            get
            {
                return this.m_isFrost;
            }
            set
            {
                if (this.m_isFrost != value)
                {
                    this.m_isFrost = value;
                    if (this.m_syncAtTime)
                    {
                        this.m_game.SendGameUpdateFrozenState(this);
                    }
                }
            }
        }

        public bool IsHide
        {
            get
            {
                return this.m_isHide;
            }
            set
            {
                if (this.m_isHide != value)
                {
                    this.m_isHide = value;
                    if (this.m_syncAtTime)
                    {
                        this.m_game.SendGameUpdateHideState(this);
                    }
                }
            }
        }

        public bool IsNoHole
        {
            get
            {
                return this.m_isNoHole;
            }
            set
            {
                if (this.m_isNoHole != value)
                {
                    this.m_isNoHole = value;
                    if (this.m_syncAtTime)
                    {
                        this.m_game.SendGameUpdateNoHoleState(this);
                    }
                }
            }
        }

        public bool isPet
        {
            get
            {
                return this.m_isPet;
            }
            set
            {
                this.m_isPet = value;
            }
        }

        public bool IsSay { get; set; }

        public int MaxBlood
        {
            get
            {
                return this.m_maxBlood;
            }
            set
            {
                this.m_maxBlood = value;
            }
        }

        public string ModelId
        {
            get
            {
                return this.m_modelId;
            }
        }

        public string Name
        {
            get
            {
                return this.m_name;
            }
        }

        public bool NguyHiem
        {
            get
            {
                return this.m_nguyhiem;
            }
            set
            {
                this.m_nguyhiem = value;
            }
        }

        public Game.Logic.PetEffects.PetEffectList PetEffectList
        {
            get
            {
                return this.m_petEffectList;
            }
        }

        public PetEffectInfo PetEffects
        {
            get
            {
                return this.m_petEffects;
            }
            set
            {
                this.m_petEffects = value;
            }
        }

        public int PictureTurn
        {
            get
            {
                return this.m_pictureTurn;
            }
            set
            {
                this.m_pictureTurn = value;
            }
        }

        public bool SetSeal2
        {
            get
            {
                return this.m_isSeal;
            }
            set
            {
                if (this.m_isSeal != value)
                {
                    this.m_isSeal = value;
                    if (this.m_syncAtTime)
                    {
                        this.m_game.SendGameUpdateSealState(this);
                    }
                }
            }
        }

        public int SpecialSkillDelay
        {
            get
            {
                return this.m_specialSkillDelay;
            }
            set
            {
                this.m_specialSkillDelay = value;
            }
        }

        public int State
        {
            get
            {
                return this.m_state;
            }
            set
            {
                if (this.m_state != value)
                {
                    this.m_state = value;
                    if (this.m_syncAtTime)
                    {
                        this.m_game.SendLivingUpdateAngryState(this);
                    }
                }
            }
        }

        public bool SyncAtTime
        {
            get
            {
                return this.m_syncAtTime;
            }
            set
            {
                this.m_syncAtTime = value;
            }
        }

        public int Team
        {
            get
            {
                return this.m_team;
            }
        }

        public eLivingType Type
        {
            get
            {
                return this.m_type;
            }
            set
            {
                this.m_type = value;
            }
        }

        public bool VaneOpen
        {
            get
            {
                return this.m_vaneOpen;
            }
            set
            {
                this.m_vaneOpen = value;
            }
        }
    }
}

