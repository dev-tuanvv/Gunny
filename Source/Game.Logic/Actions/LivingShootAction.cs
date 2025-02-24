﻿namespace Game.Logic.Actions
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;

    public class LivingShootAction : BaseAction
    {
        private LivingCallBack livingCallBack_0;
        private int m_angle;
        private int m_bombCount;
        private int m_bombId;
        private int m_force;
        private Living m_living;
        private int m_maxTime;
        private int m_minTime;
        private float m_Time;
        private int m_tx;
        private int m_ty;

        public LivingShootAction(Living living, int bombId, int x, int y, int force, int angle, int bombCount, int minTime, int maxTime, float time, int delay) : base(delay, 0x3e8)
        {
            this.m_living = living;
            this.m_bombId = bombId;
            this.m_tx = x;
            this.m_ty = y;
            this.m_force = force;
            this.m_angle = angle;
            this.m_bombCount = bombCount;
            this.m_bombId = bombId;
            this.m_minTime = minTime;
            this.m_maxTime = maxTime;
            this.m_Time = time;
        }

        public LivingShootAction(Living living, int bombId, int x, int y, int force, int angle, int bombCount, int minTime, int maxTime, float time, int delay, LivingCallBack callBack) : base(delay, 0x3e8)
        {
            this.m_living = living;
            this.m_bombId = bombId;
            this.m_tx = x;
            this.m_ty = y;
            this.m_force = force;
            this.m_angle = angle;
            this.m_bombCount = bombCount;
            this.m_bombId = bombId;
            this.m_minTime = minTime;
            this.m_maxTime = maxTime;
            this.m_Time = time;
            this.livingCallBack_0 = callBack;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            if ((this.m_living is SimpleBoss) || (this.m_living is SimpleNpc))
            {
                this.m_living.GetShootForceAndAngle(ref this.m_tx, ref this.m_ty, this.m_bombId, this.m_minTime, this.m_maxTime, this.m_bombCount, this.m_Time, ref this.m_force, ref this.m_angle);
            if (this.m_living is Player && this.m_minTime == 1001 && this.m_maxTime == 10001)
            {
                this.m_living.GetShootForceAndAngle(ref this.m_tx, ref this.m_ty, this.m_bombId, this.m_minTime, this.m_maxTime, this.m_bombCount, this.m_Time, ref this.m_force, ref this.m_angle);
            }
            }
            if (((this.m_living is Player) && (this.m_minTime == 0x3e9)) && (this.m_maxTime == 0x2711))
            {
                this.m_living.GetShootForceAndAngle(ref this.m_tx, ref this.m_ty, this.m_bombId, this.m_minTime, this.m_maxTime, this.m_bombCount, this.m_Time, ref this.m_force, ref this.m_angle);
            }
            if (this.m_living.ShootImp(this.m_bombId, this.m_tx, this.m_ty, this.m_force, this.m_angle, this.m_bombCount, 0) && (this.livingCallBack_0 != null))
            {
                this.m_living.CallFuction(this.livingCallBack_0, this.m_living.LastLifeTimeShoot);
            }
            base.Finish(tick);
        }
    }
}

