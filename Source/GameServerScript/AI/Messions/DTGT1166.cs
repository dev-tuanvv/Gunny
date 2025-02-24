using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;
using Game.Logic;
using SqlDataProvider.Data;
namespace GameServerScript.AI.Messions
{
    public class DTGT1166 : AMissionControl
    {
        private static string[] KillChat = new string[2]
    {
      "Gửi cho bạn trở về nhà!",
      "Một mình, bạn có ảo tưởng có thể đánh bại tôi?"
    };
    private static string[] ShootedChat = new string[2]
    {
      " Đau ah! Đau ...",
      "Quốc vương vạn tuế ..."
    };
    private int IsSay = 0;
    private int bossID = 6123;
    private int npcID = 6121;
    private int npcID2 = 6122;
    private SimpleNpc npc;
    private SimpleBoss m_boss;
    private SimpleBoss m_king;
    private PhysicalObj m_front;
    public override int CalculateScoreGrade(int score)
    {
      base.CalculateScoreGrade(score);
      if (score > 900)
        return 3;
      if (score > 825)
        return 2;
      return score > 725 ? 1 : 0;
    }

    public override void OnPrepareNewSession()
    {
      base.OnPrepareNewSession();
      Game.AddLoadingFile(1, "bombs/61.swf", "tank.resource.bombs.Bomb61");
      Game.AddLoadingFile(2, "image/game/living/Living190.swf", "game.living.Living190");
      int[] npcIds = { bossID, npcID, npcID2 };
      Game.LoadResources(npcIds);
      Game.LoadNpcGameOverResources(npcIds);
      Game.SetMap(1166);
    }

    public override void OnStartGame()
    {
      base.OnStartGame();
      m_boss = Game.CreateBoss(bossID, 1950, 700, -1, 1,"");
      m_front = Game.Createlayer(1100, 1080, "font", "game.living.Living190", "stand", 1, 0);
      m_boss.FallFrom(m_boss.X, m_boss.Y, "", 0, 0, 1000);
      m_boss.SetRelateDemagemRect(-34, -35, 100, 70);
      npc = Game.CreateNpc(npcID, 10, 750, 1, 10);
      npc.FallFrom(npc.X, npc.Y, "", 0, 0, 1000);
      npc.CallFuction(new LivingCallBack(Go), 1200);
      m_king = Game.CreateBoss(npcID2, 10, 750, 1, 1,"");
      m_king.FallFrom(m_king.X, m_king.Y, "", 0, 0, 1000);
      m_king.CallFuction(new LivingCallBack(Run), 2200);
    }

    private void Go()
    {
      npc.MoveTo(605, npc.Y, "walk", 0, "", 7, new LivingCallBack(FlyUp));
    }

    private void FlyUp()
    {
      npc.MoveTo(npc.X, npc.Y - 110, "flyup", 0, "", 3, new LivingCallBack(FlyLR), 2500);
    }

    private void FlyLR()
    {
      npc.MoveTo(npc.X + 170, npc.Y, "flyLR", 0, "", 3, (LivingCallBack) null);
    }

    private void Run()
    {
      m_king.MoveTo(605, m_king.Y, "walk", 0, "", 7, new LivingCallBack(WalkUp));
    }

    private void WalkUp()
    {
      m_king.MoveTo(m_king.X, m_king.Y - 110, "flyup", 0, "", 3, new LivingCallBack(WalkLR), 2500);
    }

    private void WalkLR()
    {
      m_king.MoveTo(m_king.X + 90, m_king.Y, "flyLR", 0, "", 3, (LivingCallBack) null);
    }

    public override void OnNewTurnStarted()
    {
      base.OnNewTurnStarted();
    }

    public override void OnBeginNewTurn()
    {
      base.OnBeginNewTurn();
      IsSay = 0;
    }

    public override bool CanGameOver()
    {
      base.CanGameOver();
      return Game.TurnIndex > Game.MissionInfo.TotalTurn - 1 || !m_boss.IsLiving;
    }

    public override int UpdateUIData()
    {
      if (m_boss == null)
        return 0;
      if (!m_boss.IsLiving)
        return 1;
      else
        return base.UpdateUIData();
    }

    public override void OnGameOver()
    {
      base.OnGameOver();
      if (!m_boss.IsLiving)
        Game.IsWin = true;
      else
        Game.IsWin = false;
        }
    }
}
