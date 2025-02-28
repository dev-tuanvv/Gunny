﻿using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Game.Logic;
using System.Drawing;
using Game.Logic.Actions;
using Game.Logic.Effects;
using Bussiness;


namespace GameServerScript.AI.NPC
{
    public class ThirteenHardBrynBoss : ABrain
    {
        private int m_attackTurn = 0;

        private PhysicalObj m_npc;

        private PhysicalObj m_moive;

        private int isSay = 0;

        #region NPC 说话内容
        private static string[] AllAttackChat = new string[] {
            LanguageMgr.GetTranslation("Sư tử rống..."),

            LanguageMgr.GetTranslation("Sức mạnh của chúa rừng !"),

            LanguageMgr.GetTranslation("Sự đau đớn tột độ !")
        };

        private static string[] ShootChat = new string[]{
            LanguageMgr.GetTranslation("Nén đá dấu tay ...!"),

            LanguageMgr.GetTranslation("Vũ khí của thần bộ lạc !")  
        };

        private static string[] KillPlayerChat = new string[]{
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg6"),

            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg7")
        };

        private static string[] CallChat = new string[]{
            LanguageMgr.GetTranslation("Vật tổ ..."),

            LanguageMgr.GetTranslation("Kill !")

        };

        private static string[] JumpChat = new string[]{
             LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg10"),

             LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg11"),

             LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg12")
        };

        private static string[] KillAttackChat = new string[]{
             LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg13"),

              LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg14")
        };

        private static string[] ShootedChat = new string[]{
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg15"),

            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg16")

        };

        private static string[] DiedChat = new string[]{
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg17")
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

            isSay = 0;
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
                if (player.IsLiving && player.X > 950 && player.X < 1250 && player.Y > 666)
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
                KillAttack(950, 1250);

                return;
            }

            if (m_attackTurn == 0)
            {
                SummonA();
                m_attackTurn++;
            }
            else if (m_attackTurn == 1)
            {
                CreateMovie();// call buff dame and guard
                m_attackTurn++;
            }
            else if (m_attackTurn == 2)
            {
                // add buff dame and guard
                PersonalAttack();
                m_attackTurn++;
            }
            else
            {
                PersonalAttack2();
                m_attackTurn = 0;
            }
        }

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }

        private void KillAttack(int fx, int tx)
        {
            int index = Game.Random.Next(0, KillAttackChat.Length);
            Body.Say(KillAttackChat[index], 1, 1000);
            Body.CurrentDamagePlus = 20;
            Body.PlayMovie("beatC", 3000, 0);
            Body.RangeAttacking(fx, tx, "cry", 5000, null);
        }

        private void PersonalAttack()
        {
            m_moive.PlayMovie("beatA", 1000, 1000);
            Body.CallFuction(CallEffectB, 1000);
        }
        private void CallEffectB()
        {
            bool canAdd = false;
            Player[] players = Game.GetAllPlayers();
            List<Player> playerArr = new List<Player>();
            foreach (Player player in players)
            {
                if (player.X > 990 && player.X < 1315 && player.Y < 606)
                {
                    canAdd = true;
                    playerArr.Add(player);
                    break;
                }
            }
            if (canAdd)
            {
                foreach (Player p in players)
                {
                    p.AddEffect(new DamageEffect(2), 0);
                    p.AddEffect(new GuardEffect(2), 0);
                }
            }
            Body.RangeAttacking(Body.X - 1500, Body.X + 1500, "cry", 1500, playerArr);
        }

        private void PersonalAttack2()
        {
            //Player target = Game.FindRandomPlayer();
            int index = Game.Random.Next(0, ShootChat.Length);
            Body.Say(ShootChat[index], 1, 0);
            Body.CallFuction(DoudbleAttack, 2600);
            Body.CallFuction(DoudbleAttack, 6500);
        }
        private void DoudbleAttack()
        {
            Player target = Game.FindRandomPlayer();
            Body.CurrentDamagePlus = 1.8f;
            if (target != null)
            {
                int mtX = Game.Random.Next(target.X - 10, target.X + 10);
                if (Body.ShootPoint(target.X, target.Y, 55, 1000, 10000, 1, 1.5f, 2550))
                {
                    Body.PlayMovie("beatB", 1700, 0);
                }
            }
        }
        private void SummonA()
        {
            //int index = Game.Random.Next(0, CallChat.Length);
            //Body.Say(CallChat[index], 1, 3300);            
            Body.PlayMovie("callA", 3500, 0);
            Body.RangeAttacking(Body.X - 1500, Body.X + 1500, "cry", 5500, null);
            Body.CallFuction(GoMovie, 5500);
            Body.CallFuction(MovingPlayer, 6500);
        }
        private void MovingPlayer()
        {
            Player[] players = Game.GetAllPlayers();
            foreach (Player p in players)
            {
                int disX = Game.Random.Next(900, 1350);
                p.StartSpeedMult(disX, p.Y);
            }
        }
        private void GoMovie()
        {
            List<Player> targets = Game.GetAllFightPlayers();
            foreach (Player p in targets)
            {
                ((PVEGame)Game).Createlayer(p.X, p.Y, "boom", "game.living.Living126", "beatA", 1, 0);
            }
        }
        public void CreateMovie()
        {
            m_moive = ((PVEGame)Game).CreatePhysicalObj(1146, 566, "moiveA", "asset.game.ten.jitan", "born", 1, 0);
            Body.CallFuction(SummonB, 1000);
        }
        private void SummonB()
        {
            //int index = Game.Random.Next(0, CallChat.Length);
            //Body.Say(CallChat[index], 1, 3300); 
            Body.PlayMovie("callA", 3500, 0);
            Body.RangeAttacking(Body.X - 1500, Body.X + 1500, "cry", 5500, null);
            Body.CallFuction(GoMovie, 5500);
            Body.CallFuction(CallEffectA, 6500);
        }
        private void CallEffectA()
        {
            List<Player> players = Game.GetAllLivingPlayers();
            int count = 0;
            foreach (Player p in players)
            {
                if (count == 0)
                {
                    //players.Remove(p);
                    p.AddEffect(new ReduceStrengthEffect(3, 110), 0);
                }
                if (count == 1)
                {
                    //players.Remove(p);
                    p.AddEffect(new LockDirectionEffect(3), 0);
                }
                count++;
            }
        }
        public override void OnKillPlayerSay()
        {
            base.OnKillPlayerSay();
            int index = Game.Random.Next(0, KillPlayerChat.Length);
            Body.Say(KillPlayerChat[index], 1, 0, 2000);
        }

        public override void OnShootedSay()
        {
            int index = Game.Random.Next(0, ShootedChat.Length);
            if (isSay == 0 && Body.IsLiving == true)
            {
                Body.Say(ShootedChat[index], 1, 900, 0);
                isSay = 1;
            }

            if (!Body.IsLiving)
            {
                index = Game.Random.Next(0, DiedChat.Length);
                Body.Say(DiedChat[index], 1, 900 - 800, 2000);
                //Game.AddAction(new FocusAction(Body.X, Body.Y - 90, 0, delay - 900, 4000));
            }
        }
    }
}
