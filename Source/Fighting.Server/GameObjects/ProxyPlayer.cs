namespace Fighting.Server.GameObjects
{
    using Fighting.Server;
    using Game.Base.Packets;
    using Game.Logic;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Game.Logic.Phy.Object;

    public class ProxyPlayer : IGamePlayer
    {
        public List<BufferInfo> Buffers;
        public string FightFootballStyle;
        private double GPRate;
        public double m_antiAddictionRate;
        private double m_baseAglilty;
        private double m_baseAttack;
        private double m_baseBlood;
        private double m_baseDefence;
        private bool m_canUseProp;
        private bool m_canX2Exp;
        private bool m_canX3Exp;
        private ServerClient m_client;
        private ItemTemplateInfo m_currentWeapon;
        private PlayerInfo m_character;
        private List<SqlDataProvider.Data.ItemInfo> m_equipEffect;
        private List<BufferInfo> m_fightBuffs;
        private int m_gamePlayerId;
        private SqlDataProvider.Data.ItemInfo m_healstone;
        private UserMatchInfo m_matchInfo;
        private UsersPetinfo m_pet;
        private SqlDataProvider.Data.ItemInfo m_secondWeapon;
        public int m_serverid;
        private int m_zoneId;
        private string m_zoneName;
        private double OfferRate;
        private ServerClient serverClient;
        private PlayerInfo info;
        private ItemTemplateInfo itemTemplate;
        private ItemInfo item;
        private double baseAttack;
        private double baseDefence;
        private double baseAgility;
        private double baseBlood;
        private double gprate;
        private double offerrate;
        private double rate;
        private List<BufferInfo> infos;
        private int serverid;

        public ProxyPlayer(ServerClient client, PlayerInfo character, UserMatchInfo matchInfo, ProxyPlayerInfo proxyPlayer, UsersPetinfo pet, List<BufferInfo> infos, List<SqlDataProvider.Data.ItemInfo> euipEffects, List<BufferInfo> fightBuffs)
        {
            this.m_client = client;
            this.m_character = character;
            this.m_matchInfo = matchInfo;
            this.m_pet = pet;
            this.m_serverid = proxyPlayer.ServerId;
            this.m_baseAttack = proxyPlayer.BaseAttack;
            this.m_baseDefence = proxyPlayer.BaseDefence;
            this.m_baseAglilty = proxyPlayer.BaseAgility;
            this.m_baseBlood = proxyPlayer.BaseBlood;
            this.m_currentWeapon = proxyPlayer.GetItemTemplateInfo();
            this.m_secondWeapon = proxyPlayer.GetItemInfo();
            this.m_healstone = proxyPlayer.GetHealstone();
            this.GPRate = proxyPlayer.GPAddPlus;
            this.m_zoneId = proxyPlayer.ZoneId;
            this.m_zoneName = proxyPlayer.ZoneName;
            this.OfferRate = proxyPlayer.OfferAddPlus;
            this.m_antiAddictionRate = proxyPlayer.AntiAddictionRate;
            this.m_equipEffect = euipEffects;
            this.Buffers = infos;
            this.m_fightBuffs = fightBuffs;
            this.FightFootballStyle = proxyPlayer.FightFootballStyle;
        }

        public ProxyPlayer(ServerClient serverClient, PlayerInfo info, ItemTemplateInfo itemTemplate, ItemInfo item, double baseAttack, double baseDefence, double baseAgility, double baseBlood, double gprate, double offerrate, double rate, List<BufferInfo> infos, int serverid)
        {
            // TODO: Complete member initialization
            this.serverClient = serverClient;
            this.info = info;
            this.itemTemplate = itemTemplate;
            this.item = item;
            this.baseAttack = baseAttack;
            this.baseDefence = baseDefence;
            this.baseAgility = baseAgility;
            this.baseBlood = baseBlood;
            this.gprate = gprate;
            this.offerrate = offerrate;
            this.rate = rate;
            this.infos = infos;
            this.serverid = serverid;
        }

        public int AddActiveMoney(int value)
        {
            if (value > 0)
            {
                this.m_client.SendPlayerAddActiveMoney(this.m_character.ID, value);
            }
            return value;
        }

        public int AddDamageScores(int value)
        {
            return value;
        }

        public int AddGold(int value)
        {
            if (value > 0)
            {
                this.m_client.SendPlayerAddGold(this.PlayerCharacter.ID, value);
            }
            return value;
        }

        public int AddGoXu(int value)
        {
            if (value > 0)
            {
                this.m_client.SendPlayerAddGoXu(this.m_character.ID, value);
            }
            return value;
        }

        public int AddGP(int gp)
        {
            if (gp > 0)
            {
                this.m_client.SendPlayerAddGP(this.PlayerCharacter.ID, gp);
            }
            return (int) (this.GPRate * gp);
        }

        public int AddGiftToken(int value)
        {
            if (value > 0)
            {
                this.m_client.SendPlayerAddGiftToken(this.m_character.ID, value);
            }
            return value;
        }

        public int AddHardCurrency(int value)
        {
            return 0;
        }

        public int AddHonor(int value)
        {
            return value;
        }

        public int AddLeagueMoney(int value)
        {
            if (value > 0)
            {
                this.m_client.SendPlayerAddLeagueMoney(this.m_character.ID, value);
            }
            return value;
        }

        public int AddMedal(int value)
        {
            if (value > 0)
            {
                this.m_client.SendPlayerAddMedal(this.m_character.ID, value);
            }
            return value;
        }

        public int AddMoney(int value)
        {
            if (value > 0)
            {
                this.m_client.SendPlayerAddMoney(this.m_character.ID, value);
            }
            return value;
        }

        public int AddOffer(int baseoffer)
        {
            if (baseoffer < 0)
            {
                return baseoffer;
            }
            return (int) ((baseoffer * this.OfferRate) * this.m_antiAddictionRate);
        }

        public void AddPrestige(bool isWin)
        {
            this.m_client.SendPlayerAddPrestige(this.m_character.ID, isWin);
        }

        public bool AddTemplate(SqlDataProvider.Data.ItemInfo cloneItem, eBageType bagType, int count, eGameView typeGet)
        {
            this.m_client.SendPlayerAddTemplate(this.m_character.ID, cloneItem, bagType, count);
            return true;
        }

        public bool ClearFightBag()
        {
            return true;
        }

        public void ClearFightBuffOneMatch()
        {
        }

        public bool ClearTempBag()
        {
            return true;
        }

        public int ConsortiaFight(int consortiaWin, int consortiaLose, Dictionary<int, Player> players, eRoomType roomType, eGameType gameClass, int totalKillHealth, int count)
        {
            this.m_client.SendPlayerConsortiaFight(this.m_character.ID, consortiaWin, consortiaLose, players, roomType, gameClass, totalKillHealth);
            return 0;
        }

        public void Disconnect()
        {
            this.m_client.SendDisconnectPlayer(this.m_character.ID);
        }

        public void FootballTakeOut(bool isWin)
        {
            this.m_client.SendFootballTakeOut(this.PlayerCharacter.ID, isWin);
        }

        public double GetBaseAgility()
        {
            return this.m_baseAglilty;
        }

        public double GetBaseAttack()
        {
            return this.m_baseAttack;
        }

        public double GetBaseBlood()
        {
            return this.m_baseBlood;
        }

        public double GetBaseDefence()
        {
            return this.m_baseDefence;
        }

        public string GetFightFootballStyle(int team)
        {
            if (team == 1)
            {
                return this.FightFootballStyle.Split(new char[] { ';' })[0];
            }
            return this.FightFootballStyle.Split(new char[] { ';' })[1];
        }

        public bool isDoubleAward()
        {
            return false;
        }

        public bool IsPvePermission(int missionId, eHardLevel hardLevel)
        {
            return true;
        }

        public void LogAddMoney(AddMoneyType masterType, AddMoneyType sonType, int userId, int moneys, int SpareMoney)
        {
        }

        public bool MissionEnergyEmpty(int value)
        {
            return false;
        }

        public void OnGameOver(AbstractGame game, bool isWin, int gainXp)
        {
            this.m_client.SendPlayerOnGameOver(this.PlayerCharacter.ID, game.Id, isWin, gainXp);
        }

        public void OnKillingBoss(AbstractGame game, NpcInfo npc, int damage)
        {
            throw new NotImplementedException();
        }

        public void OnKillingLiving(AbstractGame game, int type, int id, bool isLiving, int demage)
        {
            this.m_client.SendPlayerOnKillingLiving(this.m_character.ID, game, type, id, isLiving, demage);
        }

        public void OnMissionOver(AbstractGame game, bool isWin, int MissionID, int turnNum)
        {
            this.m_client.SendPlayerOnMissionOver(this.m_character.ID, game, isWin, MissionID, turnNum);
        }

        public void OutLabyrinth()
        {
        }

        public void OutLabyrinth(bool isWin)
        {
        }

        public int RemoveGold(int value)
        {
            this.m_client.SendPlayerRemoveGold(this.m_character.ID, value);
            return 0;
        }

        public int RemoveGP(int gp)
        {
            this.m_client.SendPlayerRemoveGP(this.PlayerCharacter.ID, gp);
            return gp;
        }

        public int RemoveGiftToken(int value)
        {
            return 0;
        }

        public bool RemoveHealstone()
        {
            this.m_client.SendPlayerRemoveHealstone(this.m_character.ID);
            return false;
        }

        public int RemoveMedal(int value)
        {
            return 0;
        }

        public bool RemoveMissionEnergy(int value)
        {
            return false;
        }

        public int RemoveMoney(int value)
        {
            this.m_client.SendPlayerRemoveMoney(this.m_character.ID, value);
            return 0;
        }

        public int RemoveOffer(int value)
        {
            this.m_client.SendPlayerRemoveOffer(this.m_character.ID, value);
            return value;
        }

        public void SendConsortiaFight(int consortiaID, int riches, string msg)
        {
            this.m_client.SendPlayerSendConsortiaFight(this.m_character.ID, consortiaID, riches, msg);
        }

        public void SendHideMessage(string msg)
        {
            GSPacketIn pkg = new GSPacketIn(3);
            pkg.WriteInt(3);
            pkg.WriteString(msg);
            this.SendTCP(pkg);
        }

        public void SendInsufficientMoney(int type)
        {
        }

        public void SendMessage(string msg)
        {
            GSPacketIn pkg = new GSPacketIn(3);
            pkg.WriteInt(0);
            pkg.WriteString(msg);
            this.SendTCP(pkg);
        }

        public void SendTCP(GSPacketIn pkg)
        {
            this.m_client.SendPacketToPlayer(this.m_character.ID, pkg);
        }

        public bool SetPvePermission(int missionId, eHardLevel hardLevel)
        {
            return true;
        }

        public void UpdateBarrier(int barrier, string pic)
        {
        }

        public void UpdateLabyrinth(int currentFloor, int m_missionInfoId, bool bigAward)
        {
        }

        public void UpdatePveResult(string type, int value, bool isWin)
        {
        }

        public void UpdateRestCount()
        {
            this.m_client.SendUpdateRestCount(this.m_character.ID);
        }

        public bool UseKingBlessHelpStraw(eRoomType roomType)
        {
            return false;
        }

        public bool UsePropItem(AbstractGame game, int bag, int place, int templateId, bool isLiving)
        {
            this.m_client.SendPlayerUsePropInGame(this.PlayerCharacter.ID, bag, place, templateId, isLiving);
            game.Pause(500);
            return false;
        }

        public long AllWorldDameBoss
        {
            get
            {
                return 0L;
            }
        }

        public bool CanUseProp
        {
            get
            {
                return this.m_canUseProp;
            }
            set
            {
                this.m_canUseProp = value;
            }
        }

        public bool CanX2Exp
        {
            get
            {
                return this.m_canX2Exp;
            }
            set
            {
                this.m_canX2Exp = value;
            }
        }

        public bool CanX3Exp
        {
            get
            {
                return this.m_canX3Exp;
            }
            set
            {
                this.m_canX3Exp = value;
            }
        }

        public List<SqlDataProvider.Data.ItemInfo> EquipEffect
        {
            get
            {
                return this.m_equipEffect;
            }
        }

        public List<BufferInfo> FightBuffs
        {
            get
            {
                return this.m_fightBuffs;
            }
        }

        public int GamePlayerId
        {
            get
            {
                return this.m_gamePlayerId;
            }
            set
            {
                this.m_gamePlayerId = value;
                this.m_client.SendGamePlayerId(this);
            }
        }

        public SqlDataProvider.Data.ItemInfo Healstone
        {
            get
            {
                return this.m_healstone;
            }
        }

        public ItemTemplateInfo MainWeapon
        {
            get
            {
                return this.m_currentWeapon;
            }
        }

        public UserMatchInfo MatchInfo
        {
            get
            {
                return this.m_matchInfo;
            }
        }

        public UsersPetinfo Pet
        {
            get
            {
                return this.m_pet;
            }
        }

        public PlayerInfo PlayerCharacter
        {
            get
            {
                return this.m_character;
            }
        }

        public string ProcessLabyrinthAward { get; set; }

        public SqlDataProvider.Data.ItemInfo SecondWeapon
        {
            get
            {
                return this.m_secondWeapon;
            }
        }

        public int ServerID
        {
            get
            {
                return this.m_serverid;
            }
            set
            {
                this.m_serverid = value;
            }
        }

        public long WorldbossBood
        {
            get
            {
                return 0L;
            }
        }

        public int ZoneId
        {
            get
            {
                return this.m_zoneId;
            }
        }

        public string ZoneName
        {
            get
            {
                return this.m_zoneName;
            }
        }
    }
}

