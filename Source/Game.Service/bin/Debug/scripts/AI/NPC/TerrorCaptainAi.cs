using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Game.Logic;

namespace GameServerScript.AI.NPC
{
    public class TerrorCaptainAi : ABrain
    {
        private int m_attackTurn = 0;

        private int currentCount = 0;

        private int Dander = 0;

        private List<SimpleNpc> Children = new List<SimpleNpc>();

        private int npcID = 1309;

        #region NPC 说话内容
        private static string[] AllAttackChat = new string[] {
            "Đại địa <br/> chấn .... <br/> chấn ......",

            "Các ngươi đang tự tìm tới cái chết đấy!",

            "Siêu động đất bất khả chiến bại <br/> ...... GRR ... GRR ......"
        };

        private static string[] ShootChat = new string[]{
            "Coi anh bắn nè.",

            "Vỡ mặt này!"
        };

        private static string[] KillPlayerChat = new string[]{
            "Xuống đi ngục đi cưng!",

            "Ngươi nghĩ có thể đánh bại ta?",

            "Luyện thêm đi trước khi đòi đánh bại ta"
        };

        private static string[] CallChat = new string[]{
            "Vệ binh! <br/> bảo vệ! ! ",
                  
            "Các em, Gà! <br/> giúp tôi!"
        };

        private static string[] ShootedChat = new string[]{
            "Ôi đệch ...",

            "Dám bắn ta à GRRR ...",

            "Rát quá, ta không tha thứ!!!"
        };

        private static string[] JumpChat = new string[]{
             "Bắn ta à? Thử đi, <br/> bắn vào ta đi!",

             "Lên cao nào! Nhảy cao nào!",

             "Cao! <br/> lên cao nào!"
        };

        private static string[] KillAttackChat = new string[]{
             "Ngươi nghĩ lại gần ta mà sống sót à?",

             "Một thằng ngu lao vào chỗ chết",

             "Tới gần ta làm gì hả?",

             "Trẻ ranh chưa tới tuổi chạm vào mông ta!!!"
        };
        #endregion

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
            bool result = false;
            int maxdis = 0;
            foreach (Player player in Game.GetAllFightPlayers())
            {
                if (player.IsLiving && player.X > 500 && player.X < 1050)
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
                KillAttack(500, 1050);

                return;
            }

            if (m_attackTurn == 0)
            {
                AllAttack();
                m_attackTurn++;
            }
            else if (m_attackTurn == 1)
            {
                PersonalAttack();
                m_attackTurn++;
            }
            else
            {
                Summon();
                m_attackTurn = 0;
            }
        }

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }

        private void KillAttack(int fx, int tx)
        {
            ChangeDirection(3);
            int index = Game.Random.Next(0, KillAttackChat.Length);
            Body.Say(KillAttackChat[index], 1, 1000);
            Body.CurrentDamagePlus = 100f;
            Body.PlayMovie("beat2", 3000, 0);
            Body.RangeAttacking(fx, tx, "cry", 5000, null);
        }

        private void AllAttack()
        {
            ChangeDirection(3);
            Body.CurrentDamagePlus = 2f;
            int index = Game.Random.Next(0, AllAttackChat.Length);
            Body.Say(AllAttackChat[index], 1, 0);
            Body.FallFrom(Body.X, 509, null, 1000, 1, 12);
            Body.PlayMovie("beat2", 1000, 0);
            Body.RangeAttacking(Body.X - 1000, Body.X + 1000, "cry", 4000, null);
        }

        private void PersonalAttack()
        {
            ChangeDirection(3);
            int index = Game.Random.Next(0, ShootChat.Length);
            Body.Say(ShootChat[index], 1, 0);
            int dis = Game.Random.Next(670, 880);
            int direction = Body.Direction;
            Body.MoveTo(dis, Body.Y, "walk", 1000, "", 4, new LivingCallBack(NextAttack));
            Body.ChangeDirection(Game.FindlivingbyDir(Body), 9000);
        }

        private void Summon()
        {
            ChangeDirection(3);
            Body.JumpTo(Body.X, Body.Y - 300, "Jump", 1000, 1);
            int index = Game.Random.Next(0, CallChat.Length);
            Body.Say(CallChat[index], 1, 3300);
            Body.PlayMovie("call", 3500, 0);

            Body.CallFuction(new LivingCallBack(CreateChild), 4000);

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

            Body.CurrentDamagePlus = 1f;

            if (target != null)
            {
                //int mtX = Game.Random.Next(target.X - 50, target.X + 50);

                if (Body.ShootPoint(target.X, target.Y, 61, 1000, 10000, 1, 1, 2200))
                {
                    Body.PlayMovie("beat", 1700, 0);
                }

                if (Body.ShootPoint(target.X, target.Y, 61, 1000, 10000, 1, 1, 3200))
                {
                    Body.PlayMovie("beat", 2700, 0);
                }
				
				if (Body.ShootPoint(target.X, target.Y, 61, 1000, 10000, 1, 1, 4200))
                {
                    Body.PlayMovie("beat", 3700, 0);
                }
            }
        }

        private void ChangeDirection(int count)
        {
            int direction = Body.Direction;
            for (int i = 0; i < count; i++)
            {
                Body.ChangeDirection(-direction, i * 200 + 100);
                Body.ChangeDirection(direction, (i + 1) * 100 + i * 200);
            }
        }

        public void CreateChild()
        {
            ((SimpleBoss)Body).CreateChild(npcID, 520, 530, -1, 430, 6);
        }


    }
}
