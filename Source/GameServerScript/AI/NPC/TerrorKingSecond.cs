﻿using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;
using Game.Logic;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
    public class TerrorKingSecond : ABrain
    {
        private int m_attackTurn = 0;

        private int m_turn = 0;

        private PhysicalObj m_wallLeft = null;

        private PhysicalObj m_wallRight = null;

        private int IsEixt = 0;

        private int npcID = 1310;

        #region NPC 说话内容
        private static string[] AllAttackChat = new string[] { 
             "Trận động đất, bản thân mình! ! <br/> bạn vui lòng Ay giúp đỡ",
       
             "Hạ vũ khí xuống!",
       
             "Xem nếu bạn có thể đủ khả năng, một số ít!！"
        };

        private static string[] ShootChat = new string[]{
             "Cho bạn biết những gì một cú sút vết nứt!",
                               
             "Gửi cho bạn một quả bóng - bạn phải chọn Vâng",

             "Nhóm của bạn của những người dân thường ngu dốt và thấp"
        };

        private static string[] ShootedChat = new string[]{
           "Ah ~ ~ Tại sao bạn tấn công? <br/> tôi đang làm gì?",
                   
            "Oh ~ ~ nó thực sự đau khổ! Tại sao tôi phải chiến đấu? <br/> tôi phải chiến đấu ..."

        };

        private static string[] KillPlayerChat = new string[]{
             "Mathias không kiểm soát tôi!",       

             "Đây là thách thức số phận của tôi!",

             "Không! !Đây không phải là ý chí của tôi ..." 
        };

        private static string[] AddBooldChat = new string[]{
            "Xoắn ah xoay ~ <br/>xoắn ah xoay ~ ~ ~",
               
            "~ Hallelujah <br/>Luyaluya ~ ~ ~",
                
            "Yeah Yeah Yeah, <br/> để thoải mái!"
         
        };

        private static string[] KillAttackChat = new string[]{
            "Con rồng trong thế giới! !"
        };

        private static string[] FrostChat = new string[]{
            "Hương vị này",
               
            "Hãy để bạn bình tĩnh",
               
            "Bạn đã giận dữ với tôi."
              
        };

        private static string[] WallChat = new string[]{
             "Chúa, cho tôi sức mạnh!",

             "Tuyệt vọng, xem tường thủy tinh của tôi!"
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

        public override void OnStartAttacking()
        {
            base.OnStartAttacking();
            bool result = false;
            int maxdis = 0;
            foreach (Player player in Game.GetAllFightPlayers())
            {
                if (player.IsLiving && player.X > 620 && player.X < 1160)
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
                KillAttack(620, 1160);
                return;
            }

            if (m_attackTurn == 0)
            {
                AllAttack();
                if (IsEixt == 1)
                {
                    m_wallLeft.CanPenetrate = true;
                    m_wallRight.CanPenetrate = true;
                    Game.RemovePhysicalObj(m_wallLeft, true);
                    Game.RemovePhysicalObj(m_wallRight, true);
                    IsEixt = 0;
                }
                m_attackTurn++;
            }
            else if (m_attackTurn == 1)
            {
                FrostAttack();
                m_attackTurn++;
            }
            else if (m_attackTurn == 2)
            {
                ProtectingWall();
                m_attackTurn++;
            }
            else
            {

                CriticalStrikes();
                m_attackTurn = 0;
            }
        }

        private void CriticalStrikes()
        {
            Player target = Game.GetFrostPlayerRadom();
            List<Player> players = Game.GetAllFightPlayers();
            List<Player> NotFrostPlayers = new List<Player>();
            foreach (Player player in players)
            {
                if (player.IsFrost == false)
                {
                    NotFrostPlayers.Add(player);
                }
            }

            ((SimpleBoss)Body).CurrentDamagePlus = 30;
            if (NotFrostPlayers.Count != players.Count)
            {
                if (NotFrostPlayers.Count != 0)
                {
                    Body.PlayMovie("beat1", 0, 0);
                    Body.RangeAttacking(Body.X - 10000, Body.X + 10000, "beat1", 1500, NotFrostPlayers);
                }
                else
                {
                    Body.PlayMovie("beat1", 0, 0);
                    Body.RangeAttacking(Body.X - 10000, Body.X + 10000, "beat1", 1500, null);
                }
            }
            else
            {
                Body.Say("Nhỏ mà họ đã cho tôi một bài học tốt đối phương!", 1, 3300);
                Body.PlayMovie("renew", 3500, 0);
                Body.CallFuction(new LivingCallBack(CreateChild), 6000);
            }
        }

        private void FrostAttack()
        {
            int mtX = Game.Random.Next(798, 980);
            Body.MoveTo(mtX, Body.Y, "walk", 0, "", 4, new LivingCallBack(NextAttack));
        }

        private void AllAttack()
        {
            Body.CurrentDamagePlus = 1.5f;
            if (m_turn == 0)
            {
                int index = Game.Random.Next(0, AllAttackChat.Length);
                Body.Say(AllAttackChat[index], 1, 13000);
                Body.PlayMovie("beat1", 15000, 0);
                Body.RangeAttacking(Body.X - 1000, Body.X + 1000, "cry", 17000, null);
                m_turn++;
            }
            else
            {
                int index = Game.Random.Next(0, AllAttackChat.Length);
                Body.Say(AllAttackChat[index], 1, 0);
                Body.PlayMovie("beat1", 1000, 0);
                Body.RangeAttacking(Body.X - 1000, Body.X + 1000, "cry", 3000, null);
            }
        }

        private void KillAttack(int fx, int tx)
        {
            int index = Game.Random.Next(0, KillAttackChat.Length);
            if (m_turn == 0)
            {
                Body.CurrentDamagePlus = 100f;
                Body.Say(KillAttackChat[index], 1, 13000);
                Body.PlayMovie("beat1", 15000, 0);
                Body.RangeAttacking(fx, tx, "cry", 17000, null);
                m_turn++;
            }
            else
            {
                Body.CurrentDamagePlus = 100f;
                Body.Say(KillAttackChat[index], 1, 0);
                Body.PlayMovie("beat1", 2000, 0);
                Body.RangeAttacking(fx, tx, "cry", 4000, null);
            }
        }

        private void ProtectingWall()
        {
            if (IsEixt == 0)
            {
                m_wallLeft = ((PVEGame)Game).CreatePhysicalObj(Body.X - 65, 510, "wallLeft", "com.mapobject.asset.WaveAsset_01_left", "1", 1, 0);
                m_wallRight = ((PVEGame)Game).CreatePhysicalObj(Body.X + 65, 510, "wallLeft", "com.mapobject.asset.WaveAsset_01_right", "1", 1, 0);
                m_wallLeft.SetRect(-165, -169, 43, 330);
                m_wallRight.SetRect(128, -165, 41, 330);
                IsEixt = 1;
            }
            int index = Game.Random.Next(0, WallChat.Length);
            Body.Say(WallChat[index], 1, 0);
        }

        public void CreateChild()
        {
            Body.PlayMovie("renew", 100, 2000);
            ((SimpleBoss)Body).CreateChild(npcID, 520, 530, -1, 400, 6);
        }

        private void NextAttack()
        {
            int count = Game.Random.Next(1, 2);
            for (int i = 0; i < count; i++)
            {
                Player target = Game.FindRandomPlayer();

                int index = Game.Random.Next(0, ShootChat.Length);
                Body.Say(ShootChat[index], 1, 0);

                if (target.X > Body.X)
                {
                    Body.ChangeDirection(1, 500);
                }
                else
                {
                    Body.ChangeDirection(-1, 500);
                }

                if (target != null && target.IsFrost == false)
                {
                    if (Body.ShootPoint(target.X, target.Y, 1, 1000, 10000, 1, 1.5f, 2000))
                    {
                        Body.PlayMovie("beat2", 1500, 0);
                    }
                }
            }
        }
        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }
    }
}
