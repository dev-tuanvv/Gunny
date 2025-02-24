using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
    public class TwelveTankNpc : ABrain
    {
        private int m_attackTurn = 0;
		
		private Living m_living;
		
		protected int m_maxBlood;
		
        protected int m_blood;

        #region NPC 说话内容
        private static string[] AllAttackChat = new string[] { 
            "要地震喽！！<br/>各位请扶好哦",
       
            "把你武器震下来！",
       
            "看你们能还经得起几下！！"
        };

        private static string[] ShootChat = new string[]{
             "让你知道什么叫百发百中！",
                               
             "送你一个球~你可要接好啦",

             "你们这群无知的低等庶民"
        };

        private static string[] ShootedChat = new string[]{
           "哎呀~~你们为什么要攻击我？<br/>我在干什么？",
                   
            "噢~~好痛!我为什么要战斗？<br/>我必须战斗…"

        };

        private static string[] AddBooldChat = new string[]{
            "扭啊扭~<br/>扭啊扭~~",
               
            "哈利路亚~<br/>路亚路亚~~",
                
            "呀呀呀，<br/>好舒服啊！"
         
        };

        private static string[] KillAttackChat = new string[]{
            "君临天下！！"
        };

        #endregion

        public override void OnBeginSelfTurn()
        {
            base.OnBeginSelfTurn();
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
            m_body.CurrentDamagePlus = 1;
            m_body.CurrentShootMinus = 1;
        }

        public override void OnCreated()
        {
            base.OnCreated();
        }
		
		public int MaxBlood
        {
            get { return m_maxBlood; }
        }

        public int Blood
        {
            get { return m_blood; }

            set { m_blood = value; }
        }
		
        public override void OnStartAttacking()
        {
            base.OnStartAttacking();
            bool result = false;
            int maxdis = 0;
			m_maxBlood = 10000;
            foreach (Player player in Game.GetAllFightPlayers())
            {
                if (player.IsLiving && player.X > 0 && player.X < 0)
                {
                    int dis = (int)Body.Distance(player.X, player.Y);
                    if (dis > maxdis)
                    {
                        maxdis = dis;
                    }
                    result = true;
                }
            }

            if (result)
            {
                KillAttack(0, 0);
                return;
            }

            /*if (Body.Blood < 9000)
            {
                CryA();
				m_attackTurn++;
            }
            else if (Body.Blood < 7000)
            {
                CryB();
				m_attackTurn++;
            }
			else if (Body.Blood < 5000)
            {
               CryC();
			   m_attackTurn++;
            }
            else if (Body.Blood < 2000)
            {
                CryD();
				m_attackTurn = 0;
            }*/
        }
		
		private void KillAttack(int fx, int tx)
        {
            Body.CurrentDamagePlus = 10;
            int index = Game.Random.Next(0, KillAttackChat.Length);
            Body.Say(KillAttackChat[index], 1, 1000);
            Body.PlayMovie("beatC", 3000, 0);
            Body.RangeAttacking(fx, tx, "cry", 4000, null);
        }

        private void CryA()
        {
            Body.PlayMovie("cryA", 1000, 0);
			Body.IsFriendly(Body);
        }

        private void CryB()
        {
            Body.PlayMovie("cryAtoB", 1000, 0);
			Body.IsFriendly(Body);
        }
		private void CryC()
        {
            Body.PlayMovie("cryB", 1000, 0);
			Body.IsFriendly(Body);
        }

        private void CryD()
        {
            Body.PlayMovie("cryBtoC", 1000, 0);
			Body.IsFriendly(Body);
        }
		

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }
    }
}
