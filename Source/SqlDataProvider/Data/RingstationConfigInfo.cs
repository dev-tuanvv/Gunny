namespace SqlDataProvider.Data
{
    using System;
    using System.Runtime.CompilerServices;

    public class RingstationConfigInfo
    {
        public int AwardBattleByRank(int rank, bool isWin)
        {
            if ((rank == 0) && isWin)
            {
                return 10;
            }
            if (!((rank != 0) || isWin))
            {
                return 5;
            }
            if (!string.IsNullOrEmpty(this.AwardFightLost))
            {
                if (string.IsNullOrEmpty(this.AwardFightWin))
                {
                    return 0;
                }
                string[] strArray = this.AwardFightLost.Split(new char[] { '|' });
                if (isWin)
                {
                    strArray = this.AwardFightWin.Split(new char[] { '|' });
                }
                if (strArray.Length < 3)
                {
                    return 0;
                }
                string[] strArray2 = strArray;
                int index = 0;
                while (index < strArray2.Length)
                {
                    string[] strArray3 = strArray2[index].Split(new char[] { ',' });
                    if (strArray3.Length >= 2)
                    {
                        int num4 = int.Parse(strArray3[0].Split(new char[] { '-' })[0]);
                        int num5 = int.Parse(strArray3[0].Split(new char[] { '-' })[1]);
                        if ((rank >= num4) && (rank <= num5))
                        {
                            return int.Parse(strArray3[1]);
                        }
                        index++;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            return 0;
        }

        public int AwardNumByRank(int rank)
        {
            if (rank == 0)
            {
                return 0;
            }
            if ((rank < 30) && (rank > 0))
            {
                return (this.AwardNum - (10 * rank));
            }
            return (this.AwardNum - 350);
        }

        public bool IsEndTime()
        {
            return (this.AwardTime.Date < DateTime.Now.Date);
        }

        public string AwardFightLost { get; set; }

        public string AwardFightWin { get; set; }

        public int AwardNum { get; set; }

        public DateTime AwardTime { get; set; }

        public int buyCount { get; set; }

        public int buyPrice { get; set; }

        public int cdPrice { get; set; }

        public int ChallengeNum { get; set; }

        public string ChampionText { get; set; }

        public int ID { get; set; }

        public bool IsFirstUpdateRank { get; set; }
    }
}

