using Game.Logic.Actions;
using Game.Logic.Effects;
using Game.Logic.Phy.Actions;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Maths;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
namespace Game.Logic.Phy.Object
{
    public class SimpleBomb : BombObject
    {
        private Living m_owner;
        private BaseGame m_game;
        protected Tile m_shape;
        protected int m_radius;
        protected int m_petRadius;
        protected double m_power;
        protected List<BombAction> m_actions;
        protected List<BombAction> m_petActions;
        protected BombType m_type;
        protected bool m_controled;
        private float m_lifeTime;
        private BallInfo m_info;
        private bool m_bombed;
        private bool digMap;
        public bool DigMap
        {
            get
            {
                return this.digMap;
            }
        }
        public BallInfo BallInfo
        {
            get
            {
                return this.m_info;
            }
        }
        public Living Owner
        {
            get
            {
                return this.m_owner;
            }
        }
        public List<BombAction> PetActions
        {
            get
            {
                return this.m_petActions;
            }
        }
        public List<BombAction> Actions
        {
            get
            {
                return this.m_actions;
            }
        }
        public float LifeTime
        {
            get
            {
                return this.m_lifeTime;
            }
        }
        public SimpleBomb(int id, BombType type, Living owner, BaseGame game, BallInfo info, Tile shape, bool controled) : base(id, (float)info.Mass, (float)info.Weight, (float)info.Wind, (float)info.DragIndex)
        {
            this.m_owner = owner;
            this.m_game = game;
            this.m_info = info;
            this.m_shape = shape;
            this.m_type = type;
            this.m_power = info.Power;
            this.m_radius = info.Radii;
            this.m_controled = controled;
            this.m_bombed = false;
            this.m_petRadius = 80;
            this.m_lifeTime = 0f;
            if (this.m_info.IsSpecial())
            {
                this.digMap = false;
                return;
            }
            this.digMap = true;
        }
        public override void StartMoving()
        {
            base.StartMoving();
            this.m_actions = new List<BombAction>();
            this.m_petActions = new List<BombAction>();
            int arg_27_0 = this.m_game.LifeTime;
            while (this.m_isMoving && this.m_isLiving)
            {
                this.m_lifeTime += 0.04f;
                Point pos = base.CompleteNextMovePoint(0.04f);
                base.MoveTo(pos.X, pos.Y);
                if (this.m_isLiving)
                {
                    if (Math.Round((double)(this.m_lifeTime * 100f)) % 40.0 == 0.0 && pos.Y > 0)
                    {
                        this.m_game.AddTempPoint(pos.X, pos.Y);
                    }
                    if (this.m_controled && base.vY > 0f)
                    {
                        Living player = this.m_map.FindNearestEnemy(this.m_x, this.m_y, 150.0, this.m_owner);
                        if (player != null)
                        {
                            Point v;
                            if (player is SimpleBoss)
                            {
                                Rectangle dis = player.GetDirectDemageRect();
                                v = new Point(dis.X - this.m_x, dis.Y - this.m_y);
                            }
                            else
                            {
                                v = new Point(player.X - this.m_x, player.Y - this.m_y);
                            }
                            v = v.Normalize(1000);
                            base.setSpeedXY(v.X, v.Y);
                            base.UpdateForceFactor(0f, 0f, 0f);
                            this.m_controled = false;
                            this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.CHANGE_SPEED, v.X, v.Y, 0, 0));
                        }
                    }
                }
                if (this.m_bombed)
                {
                    this.m_bombed = false;
                    this.BombImp();
                }
            }
        }
        protected override void CollideObjects(Physics[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                Physics phy = list[i];
                phy.CollidedByObject(this);
                this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.PICK, phy.Id, 0, 0, 0));
            }
        }
        protected override void CollideGround()
        {
            base.CollideGround();
            this.Bomb();
        }
        public void Bomb()
        {
            this.StopMoving();
            this.m_isLiving = false;
            this.m_bombed = true;
            this.m_owner.Game.SendFightStatus(this.m_owner, 0);
        }
        private void BombImp()
        {
            Point m_point = this.GetCollidePoint();
            List<Living> playersAround = this.m_map.FindHitByHitPiont(m_point, this.m_radius);
            foreach (Living p in playersAround)
            {
                if (p.IsNoHole || p.NoHoleTurn)
                {
                    p.NoHoleTurn = true;
                    if (!this.m_info.IsSpecial())
                    {
                        this.digMap = false;
                    }
                }
                p.SyncAtTime = false;
            }
            this.m_owner.SyncAtTime = false;
            try
            {
                if (this.digMap)
                {
                    this.m_map.Dig(this.m_x, this.m_y, this.m_shape, null);
                }
                this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.BOMB, this.m_x, this.m_y, this.digMap ? 1 : 0, 0));
                switch (this.m_type)
                {
                    case BombType.FORZEN:
                        using (List<Living>.Enumerator enumerator2 = playersAround.GetEnumerator())
                        {
                            while (enumerator2.MoveNext())
                            {
                                Living p2 = enumerator2.Current;
                                if (this.m_owner is SimpleBoss && new IceFronzeEffect(100).Start(p2))
                                {
                                    this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.FORZEN, p2.Id, 0, 0, 0));
                                }
                                else
                                {
                                    if (new IceFronzeEffect(2).Start(p2))
                                    {
                                        this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.FORZEN, p2.Id, 0, 0, 0));
                                    }
                                    else
                                    {
                                        this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.FORZEN, -1, 0, 0, 0));
                                        this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.CHANGE_STATE, p2.Id, 0, 0, 0));
                                    }
                                }
                            }
                            goto IL_93D;
                        }
                        break;
                    case BombType.FLY:
                        break;
                    case BombType.CURE:
                        using (List<Living>.Enumerator enumerator3 = playersAround.GetEnumerator())
                        {
                            while (enumerator3.MoveNext())
                            {
                                Living p3 = enumerator3.Current;
                                double plus;
                                if (this.m_map.FindPlayers(this.GetCollidePoint(), this.m_radius))
                                {
                                    plus = 0.4;
                                }
                                else
                                {
                                    plus = 1.0;
                                }
                                int blood;
                                if (this.m_info.ID == 10009)
                                {
                                    blood = (int)(this.m_lifeTime * 2000f);
                                }
                                else
                                {
                                    blood = (int)((double)((Player)this.m_owner).PlayerDetail.SecondWeapon.Template.Property7 * Math.Pow(1.1, (double)((Player)this.m_owner).PlayerDetail.SecondWeapon.StrengthenLevel) * plus);
                                    blood += this.m_owner.FightBuffers.ConsortionAddBloodGunCount;
                                    blood += this.m_owner.PetEffects.BonusPoint;
                                }
                                if (p3 is Player)
                                {
                                    p3.AddBlood(blood);
                                    ((Player)p3).TotalCure += blood;
                                    this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.CURE, p3.Id, p3.Blood, blood, 0));
                                }
                                if ((p3 is SimpleBoss || p3 is SimpleNpc) && p3.Config.IsHelper)
                                {
                                    p3.AddBlood(blood);
                                    ((SimpleBoss)p3).TotalCure += blood;
                                    this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.CURE, p3.Id, p3.Blood, blood, 0));
                                }
                            }
                            goto IL_93D;
                        }
                        goto IL_4BB;
                    default:
                        goto IL_4BB;
                }
                if (this.m_y > 10 && this.m_lifeTime > 0.04f)
                {
                    if (!this.m_map.IsEmpty(this.m_x, this.m_y))
                    {
                        PointF v = new PointF(-base.vX, -base.vY);
                        v = v.Normalize(5f);
                        this.m_x -= (int)v.X;
                        this.m_y -= (int)v.Y;
                    }
                    this.m_owner.SetXY(this.m_x, this.m_y);
                    this.m_owner.StartMoving();
                    this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.TRANSLATE, this.m_x, this.m_y, 0, 0));
                    this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.START_MOVE, this.m_owner.Id, this.m_owner.X, this.m_owner.Y, this.m_owner.IsLiving ? 1 : 0));
                    goto IL_93D;
                }
                goto IL_93D;
                IL_4BB:
                int damage = 0;
                int critical = 0;
                foreach (Living p4 in playersAround)
                {
                    if (!this.m_owner.IsFriendly(p4))
                    {
                        damage = this.MakeDamage(p4);
                        critical = 0;
                        if (damage != 0)
                        {
                            if (this.m_game.RoomType == eRoomType.FightFootballTime && p4 is SimpleNpc)
                            {
                                damage = this.GetFightFootballPoint((p4 as SimpleNpc).NpcInfo.ID);
                                if (this.m_owner.Team == 1)
                                {
                                    this.m_game.blueScore += damage;
                                }
                                else
                                {
                                    this.m_game.redScore += damage;
                                }
                                this.m_owner.ScoreArr.Add(damage);
                            }
                            else
                            {
                                critical = this.MakeCriticalDamage(p4, damage);
                            }
                            this.m_owner.OnTakedDamage(this.m_owner, ref damage, ref critical);
                            if (p4.TakeDamage(this.m_owner, ref damage, ref critical, "Fire"))
                            {
                                this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.KILL_PLAYER, p4.Id, damage + critical, (critical != 0) ? 2 : 1, p4.Blood));
                            }
                            else
                            {
                                this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.UNFORZEN, p4.Id, 0, 0, 0));
                            }
                            if (this.m_owner is Player && p4 is SimpleBoss)
                            {
                                this.m_owner.TotalDameLiving += critical + damage;
                            }
                            if (p4 is Player)
                            {
                                int dander = ((Player)p4).Dander;
                                if (this.m_owner.FightBuffers.ConsortionReduceDander > 0)
                                {
                                    dander -= dander * this.m_owner.FightBuffers.ConsortionReduceDander / 100;
                                    ((Player)p4).Dander = dander;
                                }
                                this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.DANDER, p4.Id, dander, 0, 0));
                            }
                            if ((p4 is SimpleBoss || p4 is SimpleNpc) && this.m_game.RoomType != eRoomType.FightFootballTime)
                            {
                                ((PVEGame)this.m_game).OnShooted();
                                if (p4.DoAction > -1)
                                {
                                    this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.DO_ACTION, p4.Id, 0, 0, p4.DoAction));
                                }
                            }
                        }
                        else
                        {
                            if (p4 is SimpleBoss || p4 is SimpleNpc)
                            {
                                this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.DO_ACTION, p4.Id, 0, 0, 0));
                            }
                        }
                        this.m_owner.OnAfterKillingLiving(p4, damage, critical);
                        if (p4.IsLiving)
                        {
                            p4.StartMoving((int)((this.m_lifeTime + 1f) * 1000f), 12);
                            this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.START_MOVE, p4.Id, p4.X, p4.Y, p4.IsLiving ? 1 : 0));
                        }
                    }
                }
                List<Living> playersAroundForPet = this.m_map.FindHitByHitPiont(m_point, this.m_petRadius);
                if (this.m_owner.isPet && this.m_owner.PetEffects.ActivePetHit)
                {
                    foreach (Living liv in playersAroundForPet)
                    {
                        if (liv != this.m_owner)
                        {
                            damage = this.MakePetDamage(liv, m_point);
                            if (damage > 0)
                            {
                                damage = damage * this.m_owner.PetEffects.PetBaseAtt / 100;
                                critical = this.MakeCriticalDamage(liv, damage);
                                if (liv.PetTakeDamage(this.m_owner, ref damage, ref critical, "PetFire"))
                                {
                                    if (liv is Player)
                                    {
                                        this.m_petActions.Add(new BombAction(this.m_lifeTime, ActionType.PET, liv.Id, damage + critical, ((Player)liv).Dander, liv.Blood));
                                    }
                                    else
                                    {
                                        this.m_petActions.Add(new BombAction(this.m_lifeTime, ActionType.PET, liv.Id, damage + critical, 0, liv.Blood));
                                    }
                                }
                            }
                        }
                    }
                    if (playersAroundForPet.Count == 0)
                    {
                        this.m_petActions.Add(new BombAction(0f, ActionType.NULLSHOOT, 0, 0, 0, 0));
                    }
                    this.m_owner.PetEffects.ActivePetHit = false;
                }
                IL_93D:
                this.Die();
            }
            finally
            {
                this.m_owner.SyncAtTime = true;
                foreach (Living p5 in playersAround)
                {
                    p5.SyncAtTime = true;
                }
            }
        }
        protected int MakeDamage(Living target)
        {
            if (target.Config.IsChristmasBoss)
            {
                return 1;
            }
            if (this.m_game.RoomType == eRoomType.FightFootballTime && target is Player)
            {
                return 1;
            }
            double baseDamage = this.m_owner.BaseDamage;
            double baseGuard = target.BaseGuard;
            double defence = target.Defence;
            double attack = this.m_owner.Attack;
            if (target.AddArmor && (target as Player).DeputyWeapon != null)
            {
                int addPoint = (int)target.getHertAddition((target as Player).DeputyWeapon);
                baseGuard += (double)addPoint;
                defence += (double)addPoint;
            }
            if (this.m_owner.IgnoreArmor)
            {
                baseGuard = 0.0;
                defence = 0.0;
            }
            float damagePlus = this.m_owner.CurrentDamagePlus;
            float shootMinus = this.m_owner.CurrentShootMinus;
            double DR = 0.95 * (baseGuard - (double)(3 * this.m_owner.Grade)) / (500.0 + baseGuard - (double)(3 * this.m_owner.Grade));
            double DR2 = (defence - this.m_owner.Lucky >= 0.0) ? (0.95 * (defence - this.m_owner.Lucky) / (600.0 + defence - this.m_owner.Lucky)) : 0.0;
            double DR3 = (double)this.m_owner.FightBuffers.WorldBossAddDamage * (1.0 - (baseGuard / 200.0 + defence * 0.003));
            double damage = (DR3 + baseDamage * (1.0 + attack * 0.001) * (1.0 - (DR + DR2 - DR * DR2))) * (double)damagePlus * (double)shootMinus;
            Point p = new Point(this.X, this.Y);
            double distance = target.Distance(p);
            if (distance >= (double)this.m_radius)
            {
                return 0;
            }
            if (this.m_owner is Player && target is Player && target != this.m_owner)
            {
                this.m_game.AddAction(new FightAchievementAction(this.m_owner, 3, this.m_owner.Direction, 1200));
            }
            damage *= 1.0 - distance / (double)this.m_radius / 4.0;
            if (damage < 0.0)
            {
                return 1;
            }
            return (int)damage;
        }
        protected int MakePetDamage(Living target, Point p)
        {
            if (target.Config.IsWorldBoss || this.m_owner.Game.RoomType == eRoomType.ActivityDungeon)
            {
                return 0;
            }
            if (target.Config.IsChristmasBoss)
            {
                return 1;
            }
            double baseDamage = this.m_owner.BaseDamage;
            double baseGuard = target.BaseGuard;
            double defence = target.Defence;
            double attack = this.m_owner.Attack;
            if (target.AddArmor && (target as Player).DeputyWeapon != null)
            {
                int addPoint = (int)target.getHertAddition((target as Player).DeputyWeapon);
                baseGuard += (double)addPoint;
                defence += (double)addPoint;
            }
            if (this.m_owner.IgnoreArmor)
            {
                baseGuard = 0.0;
                defence = 0.0;
            }
            float damagePlus = this.m_owner.CurrentDamagePlus;
            double DR = 0.95 * (baseGuard - (double)(3 * this.m_owner.Grade)) / (500.0 + baseGuard - (double)(3 * this.m_owner.Grade));
            double DR2 = (defence - this.m_owner.Lucky >= 0.0) ? (0.95 * (defence - this.m_owner.Lucky) / (600.0 + defence - this.m_owner.Lucky)) : 0.0;
            double damage = baseDamage * (1.0 + attack * 0.001) * (1.0 - (DR + DR2 - DR * DR2)) * (double)damagePlus;
            if (damage < 0.0)
            {
                return 1;
            }
            return (int)damage;
        }
        protected int MakeCriticalDamage(Living target, int baseDamage)
        {
            double lucky = this.m_owner.Lucky;
            bool canHit = lucky * 45.0 / (800.0 + lucky) + (double)this.m_owner.PetEffects.CritRate > (double)this.m_game.Random.Next(100);
            if (this.m_owner.PetEffects.CritActive)
            {
                canHit = true;
                this.m_owner.PetEffects.CritActive = false;
            }
            if (canHit)
            {
                int totalReduceCrit = target.ReduceCritFisrtGem + target.ReduceCritSecondGem;
                int CritDamage = (int)((0.5 + lucky * 0.00015) * (double)baseDamage);
                CritDamage = CritDamage * (100 - totalReduceCrit) / 100;
                if (this.m_owner.FightBuffers.ConsortionAddCritical > 0)
                {
                    CritDamage += this.m_owner.FightBuffers.ConsortionAddCritical;
                }
                return CritDamage;
            }
            return 0;
        }
        protected int GetFightFootballPoint(int livingID)
        {
            switch (livingID)
            {
                case 10005:
                    return 1;
                case 10006:
                    return 2;
                case 10007:
                    return 3;
                case 10008:
                    return 4;
                case 10009:
                    return 5;
                default:
                    return 1;
            }
        }
        protected override void FlyoutMap()
        {
            this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.FLY_OUT, 0, 0, 0, 0));
            base.FlyoutMap();
        }
    }
}
