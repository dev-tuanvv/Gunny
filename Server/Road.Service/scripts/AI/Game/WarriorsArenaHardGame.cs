﻿using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
namespace GameServerScript.AI.Game
{
    class WarriorsArenaHardGame : APVEGameControl
    {
        public override void OnCreated()
        {
            // Game.SetupMissions("13201, 13202, 13203, 13204");
            // Game.TotalMissionCount = 4;
			Game.SetupMissions("13204");
            Game.TotalMissionCount = 1;
        }

        public override void OnPrepated()
        {
            base.OnPrepated();

            //Game.SessionId = 0;
        }

        public override int CalculateScoreGrade(int score)
        {
            base.CalculateScoreGrade(score);

            if (score > 900)
            {
                return 3;
            }
            else if (score > 825)
            {
                return 2;
            }
            else if (score > 725)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public override void OnGameOverAllSession()
        {
            base.OnGameOverAllSession();
        }
    }
}
