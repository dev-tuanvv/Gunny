namespace Game.Logic.Actions
{
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    public class LivingMoveToAction : BaseAction
    {
        private string m_action;
        private LivingCallBack m_callback;
        private int m_delayCallback;
        private int m_index;
        private bool m_isSent;
        private Living m_living;
        private List<Point> m_path;
        private string m_saction;
        private int m_speed;

        public LivingMoveToAction(Living living, List<Point> path, string action, int delay, int speed, LivingCallBack callback) : base(delay, 0)
        {
            this.m_living = living;
            this.m_path = path;
            this.m_action = action;
            this.m_isSent = false;
            this.m_index = 0;
            this.m_callback = callback;
            this.m_speed = speed;
        }

        public LivingMoveToAction(Living living, List<Point> path, string action, int delay, int speed, string sAction, LivingCallBack callback, int delayCallback) : base(delay, 0)
        {
            this.m_living = living;
            this.m_path = path;
            this.m_action = action;
            this.m_saction = sAction;
            this.m_isSent = false;
            this.m_index = 0;
            this.m_callback = callback;
            this.m_speed = speed;
            this.m_delayCallback = delayCallback;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            Point point;
            if (!this.m_isSent)
            {
                this.m_isSent = true;
                point = this.m_path[this.m_path.Count - 1];
                point = this.m_path[this.m_path.Count - 1];
                game.SendLivingMoveTo(this.m_living, this.m_living.X, this.m_living.Y, point.X, point.Y, this.m_action, this.m_speed, this.m_saction);
            }
            this.m_index++;
            if (this.m_index >= this.m_path.Count)
            {
                point = this.m_path[this.m_index - 1];
                if (point.X > this.m_living.X)
                {
                    this.m_living.Direction = 1;
                }
                else
                {
                    this.m_living.Direction = -1;
                }
                point = this.m_path[this.m_index - 1];
                point = this.m_path[this.m_index - 1];
                this.m_living.SetXY(point.X, point.Y);
                if (this.m_callback != null)
                {
                    this.m_living.CallFuction(this.m_callback, this.m_delayCallback);
                }
                base.Finish(tick);
            }
        }
    }
}

