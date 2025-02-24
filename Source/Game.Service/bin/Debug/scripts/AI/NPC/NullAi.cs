using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Game.Logic.Effects;
using Game.Logic;

namespace GameServerScript.AI.NPC
{
    public class NullAi : ABrain
    {
        private int m_attackTurn = 0;
		
		private PhysicalObj moive;
		
		public override void OnBeginSelfTurn()
        {
            base.OnBeginSelfTurn();
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
            Body.CurrentDamagePlus = 1;
            Body.CurrentShootMinus = 1;
        }

        public override void OnCreated()
        {
            base.OnCreated();
        }

        public override void OnStartAttacking()
        {
            base.OnStartAttacking();
			
            if (m_attackTurn == 0)
            {
                Body.PlayMovie("stand", 0, 0);
            }
            else
            {
                Body.PlayMovie("beatA", 1000, 0);
				((PVEGame)Game).SendGameFocus(1000, 400, 0, 0, 1500);
				Body.CallFuction(new LivingCallBack(Beat), 1500);	
				Body.Die(1500);
            }
			m_attackTurn++;
        }

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }

        public void Beat()
        {
            ((PVEGame)Game).ClearAllChild(); 
			moive = ((PVEGame)Game).Createlayer(1000, 400, "moive", "asset.game.nine.daodan", "", 2, 0);
			Body.RangeAttacking(Body.X - 10000, Body.X  + 10000, "cry", 3000, null);
			Body.RangeAttacking(Body.X - 10000, Body.X  + 10000, "cry", 3000, null);
			Body.CallFuction(new LivingCallBack(Remove), 3000);
        }
		
		public void Remove()
        {
		    Game.RemovePhysicalObj(moive, true);
		}
    }
}
