using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
	public class YearMonster1347 : AMissionControl
	{
		private SimpleBoss boss = null;

		private int bossID = 73000;

		private int kill = 0;

		public override int CalculateScoreGrade(int score)
		{
			base.CalculateScoreGrade(score);
			int result;
			if (score > 1750)
			{
				result = 3;
			}
			else if (score > 1675)
			{
				result = 2;
			}
			else if (score > 1600)
			{
				result = 1;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public override void OnPrepareNewSession()
		{
			base.OnPrepareNewSession();
			int[] npcIds = new[]
			{
				bossID
			};
			int[] npcIds2 = new[]
			{
				bossID
			};
			Game.LoadResources(npcIds);
			Game.LoadNpcGameOverResources(npcIds2);
			Game.AddLoadingFile(2, "image/game/effect/15/305b.swf", "asset.game.fifteen.305b");
			Game.AddLoadingFile(2, "image/game/effect/15/305d.swf", "asset.game.fifteen.305d");
			Game.SetMap(1347);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			boss = Game.CreateBoss(bossID, 1171, 468, -1, 1, "");
			boss.SetRelateDemagemRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
		}

		public override bool CanGameOver()
		{
			bool result;
			if (boss != null && !boss.IsLiving)
			{
				kill++;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override int UpdateUIData()
		{
			base.UpdateUIData();
			return kill;
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			if (boss != null && !boss.IsLiving)
			{
				Game.IsWin = true;
			}
			else
			{
				Game.IsWin = false;
			}
			List<Player> allFightPlayers = Game.GetAllFightPlayers();
			foreach (Player current in allFightPlayers)
			{
				current.PlayerDetail.UpdatePveResult("yearmonter", current.TotalDameLiving, Game.IsWin);
			}
		}
	}
}
