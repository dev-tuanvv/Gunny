namespace Game.Logic
{
    using Game.Base.Packets;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using Game.Logic.Phy.Object;

    public interface IGamePlayer
    {
        int AddActiveMoney(int value);
        int AddDamageScores(int value);
        int AddGold(int value);
        int AddGoXu(int value);
        int AddGP(int gp);
        int AddGiftToken(int value);
        int AddHardCurrency(int value);
        int AddHonor(int value);
        int AddLeagueMoney(int value);
        int AddMedal(int value);
        int AddMoney(int value);
        int AddOffer(int value);
        void AddPrestige(bool isWin);
        bool AddTemplate(SqlDataProvider.Data.ItemInfo cloneItem, eBageType bagType, int count, eGameView gameView);
        bool ClearFightBag();
        void ClearFightBuffOneMatch();
        bool ClearTempBag();
        int ConsortiaFight(int consortiaWin, int consortiaLose, Dictionary<int, Player> players, eRoomType roomType, eGameType gameClass, int totalKillHealth, int count);
        void Disconnect();
        void FootballTakeOut(bool isWin);
        double GetBaseAttack();
        double GetBaseBlood();
        double GetBaseDefence();
        string GetFightFootballStyle(int team);
        bool isDoubleAward();
        bool IsPvePermission(int missionId, eHardLevel hardLevel);
        void LogAddMoney(AddMoneyType masterType, AddMoneyType sonType, int userId, int moneys, int SpareMoney);
        bool MissionEnergyEmpty(int value);
        void OnGameOver(AbstractGame game, bool isWin, int gainXp);
        void OnKillingBoss(AbstractGame game, NpcInfo npc, int damage);
        void OnKillingLiving(AbstractGame game, int type, int id, bool isLiving, int demage);
        void OnMissionOver(AbstractGame game, bool isWin, int MissionID, int TurnNum);
        void OutLabyrinth(bool isWin);
        int RemoveGold(int value);
        int RemoveGP(int gp);
        int RemoveGiftToken(int value);
        bool RemoveHealstone();
        int RemoveMedal(int value);
        bool RemoveMissionEnergy(int value);
        int RemoveMoney(int value);
        int RemoveOffer(int value);
        void SendConsortiaFight(int consortiaID, int riches, string msg);
        void SendHideMessage(string msg);
        void SendInsufficientMoney(int type);
        void SendMessage(string msg);
        void SendTCP(GSPacketIn pkg);
        bool SetPvePermission(int missionId, eHardLevel hardLevel);
        void UpdateBarrier(int barrier, string pic);
        void UpdateLabyrinth(int currentFloor, int m_missionInfoId, bool bigAward);
        void UpdatePveResult(string type, int value, bool isWin);
        void UpdateRestCount();
        bool UseKingBlessHelpStraw(eRoomType roomType);
        bool UsePropItem(AbstractGame game, int bag, int place, int templateId, bool isLiving);

        long AllWorldDameBoss { get; }

        bool CanUseProp { get; set; }

        bool CanX2Exp { get; set; }

        bool CanX3Exp { get; set; }

        List<SqlDataProvider.Data.ItemInfo> EquipEffect { get; }

        List<BufferInfo> FightBuffs { get; }

        int GamePlayerId { get; set; }

        SqlDataProvider.Data.ItemInfo Healstone { get; }

        ItemTemplateInfo MainWeapon { get; }

        UserMatchInfo MatchInfo { get; }

        UsersPetinfo Pet { get; }

        PlayerInfo PlayerCharacter { get; }

        string ProcessLabyrinthAward { get; set; }

        SqlDataProvider.Data.ItemInfo SecondWeapon { get; }

        int ServerID { get; set; }

        long WorldbossBood { get; }

        int ZoneId { get; }

        string ZoneName { get; }
    }
}

