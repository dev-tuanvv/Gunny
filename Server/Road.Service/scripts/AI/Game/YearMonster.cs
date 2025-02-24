using Game.Logic.AI;

namespace GameServerScript.AI.Game
{
	public class YearMonster : APVEGameControl
	{
		public override void OnCreated()
		{
			Game.SetupMissions("1347");
			Game.TotalMissionCount = 1;
		}

		public override void OnPrepated()
		{
			Game.SessionId = 0;
		}

		public override int CalculateScoreGrade(int score)
		{
			int result;
			if (score > 800)
			{
				result = 3;
			}
			else if (score > 725)
			{
				result = 2;
			}
			else if (score > 650)
			{
				result = 1;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public override void OnGameOverAllSession()
		{
		}
	}
}
