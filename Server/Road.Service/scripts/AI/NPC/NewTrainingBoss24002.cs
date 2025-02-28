﻿using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Game.Logic;

namespace GameServerScript.AI.NPC
{
    public class NewTrainingBoss24002 : ABrain
    {
        private int m_attackTurn = 0;

        public int currentCount = 0;

        public int Dander = 0;        

        public override void OnBeginSelfTurn()
        {
            base.OnBeginSelfTurn();
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();

            Body.CurrentDamagePlus = 1;
            Body.CurrentShootMinus = 1;
            Body.SetRect(((SimpleBoss)Body).NpcInfo.X, ((SimpleBoss)Body).NpcInfo.Y, ((SimpleBoss)Body).NpcInfo.Width, ((SimpleBoss)Body).NpcInfo.Height);

            if (Body.Direction == -1)
            {
                Body.SetRect(((SimpleBoss)Body).NpcInfo.X, ((SimpleBoss)Body).NpcInfo.Y, ((SimpleBoss)Body).NpcInfo.Width, ((SimpleBoss)Body).NpcInfo.Height);
            }
            else
            {
                Body.SetRect(-((SimpleBoss)Body).NpcInfo.X - ((SimpleBoss)Body).NpcInfo.Width, ((SimpleBoss)Body).NpcInfo.Y, ((SimpleBoss)Body).NpcInfo.Width, ((SimpleBoss)Body).NpcInfo.Height);
            }

        }

        public override void OnCreated()
        {
            base.OnCreated();
        }

        public override void OnStartAttacking()
        {
            Body.Direction = Game.FindlivingbyDir(Body);
            if (m_attackTurn == 0)
            {
                m_attackTurn++;
                Body.Say("Bay ra ngoài đi，núp trong đóa ko thịt được ta đâu！", 0, 3000);
                           
            }            
            else
            {
                PersonalAttack();     
            }
           
           
        }
        private void PersonalAttack()
        {   
            int dis = Game.Random.Next(120, 250);
            int direction = Body.Direction;
            //Body.MoveTo(dis, Body.Y, "walk", 1000, new LivingCallBack(NextAttack));
            NextAttack();
            //Body.ChangeDirection(Game.FindlivingbyDir(Body), 5000);
        }
        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }
        
        private void NextAttack()
        {
            Player target = Game.FindRandomPlayer();

            Body.SetRect(0, 0, 0, 0);
            if (target.X > Body.Y)
            {
                Body.ChangeDirection(1, 500);
            }
            else
            {
                Body.ChangeDirection(-1, 500);
            }

            Body.CurrentDamagePlus = 1.8f;

            if (target != null)
            {
                int mtX = Game.Random.Next(target.X - 50, target.X + 50);

                if (Body.ShootPoint(mtX, target.Y, ((SimpleBoss)Body).NpcInfo.CurrentBallId, 1000, 10000, 1, 1, 3200))
                {
                    Body.PlayMovie("beatA", 2700, 0);
                }
            }
        }
                

    }
}
