﻿using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Game.Logic;

namespace GameServerScript.AI.NPC
{
    public class SeizeNpcAi : ABrain
    {
	
        private int m_attackTurn = 0;

        private int currentCount = 0;
     
        public override void OnBeginSelfTurn()
        {
            base.OnBeginSelfTurn();
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();


        }

        public override void OnCreated()
        {
            base.OnCreated();
        }

        public override void OnStartAttacking()
        {
            base.OnStartAttacking();
        }

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }

        private void KillAttack(int fx, int tx)
        {

        }

        private void AllAttack()
        {

        }

        private void PersonalAttack()
        {

        }

        private void Summon()
        {


        }

        private void NextAttack()
        {

        }

        private void ChangeDirection(int count)
        {

        }

        public void CreateChild()
        {

        }


    }
	
}
