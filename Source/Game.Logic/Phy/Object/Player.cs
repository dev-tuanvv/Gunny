namespace Game.Logic.Phy.Object
{
    using Bussiness.Managers;
    using Game.Logic;
    using Game.Logic.Actions;
    using Game.Logic.Effects;
    using Game.Logic.PetEffects;
    using Game.Logic.Phy.Maths;
    using Game.Logic.Spells;
    using SqlDataProvider.Data;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading;

    public class Player : TurnedLiving
    {
        public int BossCardCount;
        public int CanTakeOut;
        private static readonly int CARRY_TEMPLATE_ID = 0x2720;
        private int deputyWeaponResCount;
        public bool FinishTakeCard;
        public int GainGP;
        public int GainOffer;
        public bool HasPaymentTakeCard;
        private Dictionary<int, int> ItemFightBag;
        public bool LockDirection;
        private int m_AddWoundBallId;
        private int m_ballCount;
        private bool m_canGetProp;
        private BallInfo m_currentBall;
        private int m_changeSpecialball;
        private SqlDataProvider.Data.ItemInfo m_DeputyWeapon;
        private int m_energy;
        private int m_flyCoolDown;
        private SqlDataProvider.Data.ItemInfo m_Healstone;
        private bool m_isActive;
        private int m_loadingProcess;
        private int m_mainBallId;
        private int m_MultiBallId;
        private int m_oldx;
        private int m_oldy;
        private UsersPetinfo m_pet;
        private IGamePlayer m_player;
        private int m_prop;
        private int m_shootCount;
        private int m_spBallId;
        private ArrayList m_tempBoxes;
        private ItemTemplateInfo m_weapon;
        public bool Ready;
        public Point TargetPoint;
        public int TotalAllCure;
        public int TotalAllExperience;
        public int TotalAllHitTargetCount;
        public int TotalAllHurt;
        public int TotalAllKill;
        public int TotalAllScore;
        public int TotalAllShootCount;
        public int TotalCure;

        public event PlayerEventHandle AfterPlayerShooted;

        public event PlayerEventHandle BeforePlayerShoot;

        public event PlayerEventHandle CollidByObject;

        public event PlayerEventHandle LoadingCompleted;

        public event PlayerEventHandle PlayerBeginMoving;

        public event PlayerEventHandle PlayerBuffSkillPet;

        public event PlayerEventHandle PlayerCure;

        public event PlayerEventHandle PlayerGuard;

        public event PlayerEventHandle PlayerShoot;

        public Player(IGamePlayer player, int id, BaseGame game, int team, int maxBlood) : base(id, game, team, "", "", maxBlood, 0, 1)
        {
            this.m_tempBoxes = new ArrayList();
            this.m_flyCoolDown = 2;
            base.m_rect = new Rectangle(-15, -20, 30, 30);
            this.ItemFightBag = new Dictionary<int, int>();
            this.m_player = player;
            this.m_player.GamePlayerId = id;
            this.m_isActive = true;
            this.m_canGetProp = true;
            base.Grade = player.PlayerCharacter.Grade;
            if (base.AutoBoot)
            {
                base.VaneOpen = true;
            }
            else
            {
                base.VaneOpen = player.PlayerCharacter.IsWeakGuildFinish(9);
            }
            this.m_pet = player.Pet;
            if (((this.m_pet != null) && (game != null)) && (game.RoomType != eRoomType.FightFootballTime))
            {
                base.isPet = true;
                base.PetEffects.PetBaseAtt = this.GetPetBaseAtt();
                this.InitPetSkillEffect(this.m_pet);
            }
            this.InitFightBuffer(player.FightBuffs);
            this.TotalAllHurt = 0;
            this.TotalAllHitTargetCount = 0;
            this.TotalAllShootCount = 0;
            this.TotalAllKill = 0;
            this.TotalAllExperience = 0;
            this.TotalAllScore = 0;
            this.TotalAllCure = 0;
            this.m_DeputyWeapon = this.m_player.SecondWeapon;
            this.m_Healstone = this.m_player.Healstone;
            this.ChangeSpecialBall = 0;
            if (this.m_DeputyWeapon != null)
            {
                this.deputyWeaponResCount = (this.m_DeputyWeapon.StrengthenLevel + 1) + this.m_DeputyWeapon.LianGrade;
            }
            else
            {
                this.deputyWeaponResCount = 1;
            }
            this.m_weapon = this.m_player.MainWeapon;
            if (this.m_weapon != null)
            {
                BallConfigInfo info = BallConfigMgr.FindBall(this.m_weapon.TemplateID);
                this.m_mainBallId = info.Common;
                this.m_spBallId = info.Special;
                this.m_AddWoundBallId = info.CommonAddWound;
                this.m_MultiBallId = info.CommonMultiBall;
            }
            this.m_loadingProcess = 0;
            this.m_prop = 0;
            this.InitEqupedEffect(this.m_player.EquipEffect);
            this.m_energy = ((this.m_player.PlayerCharacter.AgiAddPlus + this.m_player.PlayerCharacter.Agility) / 30) + 240;
            base.m_maxBlood = this.m_player.PlayerCharacter.hp;
            if (base.FightBuffers.ConsortionAddMaxBlood > 0)
            {
                base.m_maxBlood += (base.m_maxBlood * base.FightBuffers.ConsortionAddMaxBlood) / 100;
            }
            base.m_maxBlood += ((this.m_player.PlayerCharacter.HpAddPlus + base.FightBuffers.WorldBossHP) + base.FightBuffers.WorldBossHP_MoneyBuff) + base.PetEffects.MaxBlood;
        }

        public bool CanUseItem(ItemTemplateInfo item)
        {
            return ((this.m_energy >= item.Property4) && (base.IsAttacking || (!base.IsLiving && (base.Team == base.m_game.CurrentLiving.Team))));
        }

        public bool CanUseItem(ItemTemplateInfo item, int place)
        {
            if (this.m_currentBall.IsSpecial())
            {
                return false;
            }
            if (!(base.IsLiving || (place != -1)))
            {
                return (base.psychic >= item.Property7);
            }
            if ((!base.IsLiving && (place != -1)) && (base.Team == base.m_game.CurrentLiving.Team))
            {
                return true;
            }
            if (this.m_energy < item.Property4)
            {
                return false;
            }
            return (base.IsAttacking || ((!base.IsLiving && (base.Team == base.m_game.CurrentLiving.Team)) && this.IsActive));
        }

        public bool CanUseSkill(int Id)
        {
            if (this.m_pet != null)
            {
                foreach (string str in this.m_pet.SkillEquip.Split(new char[] { '|' }))
                {
                    if (int.Parse(str.Split(new char[] { ',' })[0]) == Id)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void capnhatstate(string loai1, string loai2)
        {
            base.m_game.capnhattrangthai(this, loai1, loai2);
        }

        public override void CollidedByObject(Physics phy)
        {
            base.CollidedByObject(phy);
            if (phy is SimpleBomb)
            {
                this.OnCollidedByObject();
            }
        }

        public bool CheckCanUseItem(ItemTemplateInfo item)
        {
            switch (item.TemplateID)
            {
                case 0x2711:
                    if (!this.ItemFightBag.ContainsKey(0x2713) || !this.ItemFightBag.ContainsKey(0x2712))
                    {
                        if (this.ItemFightBag.ContainsKey(0x2711) && (this.ItemFightBag[0x2711] >= 2))
                        {
                            return false;
                        }
                        break;
                    }
                    return false;

                case 0x2712:
                    if (!this.ItemFightBag.ContainsKey(0x2713) || !this.ItemFightBag.ContainsKey(0x2711))
                    {
                        if (this.ItemFightBag.ContainsKey(0x2712) && (this.ItemFightBag[0x2712] >= 2))
                        {
                            return false;
                        }
                        break;
                    }
                    return false;

                case 0x2713:
                    if (!this.ItemFightBag.ContainsKey(0x2728) && !this.ItemFightBag.ContainsKey(0x2729))
                    {
                        if (this.ItemFightBag.ContainsKey(0x2711) && this.ItemFightBag.ContainsKey(0x2712))
                        {
                            return false;
                        }
                        break;
                    }
                    return false;

                case 0x2729:
                    if (this.ItemFightBag.ContainsKey(0x2713) || this.ItemFightBag.ContainsKey(0x2728))
                    {
                        return false;
                    }
                    break;
            }
            if (!this.ItemFightBag.ContainsKey(item.TemplateID))
            {
                this.ItemFightBag.Add(item.TemplateID, 1);
            }
            else
            {
                Dictionary<int, int> dictionary;
                int num2;
                (dictionary = this.ItemFightBag)[num2 = item.TemplateID] = dictionary[num2] + 1;
            }
            return true;
        }

        public bool CheckShootPoint(int x, int y)
        {
            if (Math.Abs((int) (this.X - x)) > 100)
            {
                string userName = this.m_player.PlayerCharacter.UserName;
                string nickName = this.m_player.PlayerCharacter.NickName;
                this.m_player.Disconnect();
                return false;
            }
            return true;
        }

        public void DeadLink()
        {
            this.m_isActive = false;
            if (base.IsLiving)
            {
                this.Die();
            }
        }

        public void Dichuyentucthoi(int x, int y, int delay)
        {
            Point point = new Point(x - this.X, y - this.Y);
            base.m_game.AddAction(new dichuyennhanh(this, new Point(this.X + point.X, this.Y + point.Y), delay));
        }

        public override void Die()
        {
            if (base.IsLiving)
            {
                base.m_y -= 70;
                base.Die();
            }
        }

        public int GetPetBaseAtt()
        {
            try
            {
                string[] strArray = this.m_pet.SkillEquip.Split(new char[] { '|' });
                for (int i = 0; i < strArray.Length; i++)
                {
                    PetSkillInfo info = PetMgr.FindPetSkill(Convert.ToInt32(strArray[i].Split(new char[] { ',' })[0]));
                    if ((info != null) && (info.Damage > 0))
                    {
                        return info.Damage;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("______________GetPetBaseAtt ERROR______________");
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.StackTrace);
                Console.WriteLine("_______________________________________________");
                return 0;
            }
            return 0;
        }

        private int GetTurnDelay()
        {
            return (int) ((1600.0 - ((1200.0 * base.Agility) / (base.Agility + 1200.0))) + (base.Attack / 10.0));
        }

        public void InitEqupedEffect(List<SqlDataProvider.Data.ItemInfo> equpedEffect)
        {
            base.EffectList.StopAllEffect();
            foreach (SqlDataProvider.Data.ItemInfo info in equpedEffect)
            {
                int index = 0;
                int num2 = 0;
                RuneTemplateInfo info2 = RuneMgr.FindRuneByTemplateID(info.TemplateID);
                string[] strArray = info2.Attribute1.Split(new char[] { '|' });
                string[] strArray2 = info2.Attribute2.Split(new char[] { '|' });
                if (info.Hole1 > info2.BaseLevel)
                {
                    if (strArray.Length > 1)
                    {
                        index = 1;
                    }
                    if (strArray2.Length > 1)
                    {
                        num2 = 1;
                    }
                }
                int type = info2.Type1;
                int count = Convert.ToInt32(strArray[index]);
                int probability = info2.Rate1;
                switch (type)
                {
                    case 0x27:
                    {
                        base.ReduceCritFisrtGem = count;
                        base.ReduceCritSecondGem = count;
                        int num6 = info2.Type2;
                        if (base.DefenFisrtGem == 0)
                        {
                            base.DefenFisrtGem = num6;
                        }
                        else
                        {
                            base.DefenSecondGem = num6;
                        }
                        break;
                    }
                    case 0x25:
                    //case 0x27:
                        type = info2.Type2;
                        count = Convert.ToInt32(strArray2[num2]);
                        probability = info2.Rate2;
                        break;

                    case 1:
                    {
                        new AddAttackEffect(count, probability).Start(this);
                        continue;
                    }
                    case 2:
                    {
                        new AddDefenceEffect(count, probability, type).Start(this);
                        continue;
                    }
                    case 3:
                    {
                        new AddAgilityEffect(count, probability).Start(this);
                        continue;
                    }
                    case 4:
                    {
                        new AddLuckyEffect(count, probability).Start(this);
                        continue;
                    }
                    case 5:
                    {
                        new AddDamageEffect(count, probability).Start(this);
                        continue;
                    }
                    case 6:
                    {
                        new ReduceDamageEffect(count, probability, type).Start(this);
                        continue;
                    }
                    case 7:
                    {
                        new AddBloodEffect(count, probability).Start(this);
                        continue;
                    }
                    case 8:
                    {
                        new FatalEffect(count, probability).Start(this);
                        continue;
                    }
                    case 9:
                    {
                        new IceFronzeEquipEffect(count, probability).Start(this);
                        continue;
                    }
                    case 10:
                    {
                        new NoHoleEquipEffect(count, probability, type).Start(this);
                        continue;
                    }
                    case 11:
                    {
                        new AtomBombEquipEffect(count, probability).Start(this);
                        continue;
                    }
                    case 12:
                    {
                        new ArmorPiercerEquipEffect(count, probability).Start(this);
                        continue;
                    }
                    case 13:
                    {
                        new AvoidDamageEffect(count, probability, type).Start(this);
                        continue;
                    }
                    case 14:
                    {
                        new MakeCriticalEffect(count, probability).Start(this);
                        continue;
                    }
                    case 15:
                    {
                        new AssimilateDamageEffect(count, probability, type).Start(this);
                        continue;
                    }
                    case 0x10:
                    {
                        new AssimilateBloodEffect(count, probability).Start(this);
                        continue;
                    }
                    case 0x11:
                    {
                        new SealEquipEffect(count, probability).Start(this);
                        continue;
                    }
                    case 0x12:
                    {
                        new AddTurnEquipEffect(count, probability, info2.TemplateID).Start(this);
                        continue;
                    }
                    case 0x13:
                    {
                        new AddDanderEquipEffect(count, probability, type).Start(this);
                        continue;
                    }
                    case 20:
                    {
                        new ReflexDamageEquipEffect(count, probability).Start(this);
                        continue;
                    }
                    case 0x15:
                    {
                        new ReduceStrengthEquipEffect(count, probability).Start(this);
                        continue;
                    }
                    case 0x16:
                    {
                        new ContinueReduceBloodEquipEffect(count, probability).Start(this);
                        continue;
                    }
                    case 0x17:
                    {
                        new LockDirectionEquipEffect(count, probability).Start(this);
                        continue;
                    }
                    case 0x18:
                    {
                        new AddBombEquipEffect(count, probability).Start(this);
                        continue;
                    }
                    case 0x19:
                    {
                        new ContinueReduceDamageEquipEffect(count, probability).Start(this);
                        continue;
                    }
                    case 0x1a:
                    {
                        new RecoverBloodEffect(count, probability, type).Start(this);
                        continue;
                    }
                }
                Console.WriteLine("Not Found Effect: " + type);
            }
        }

        public void InitFightBuffer(List<BufferInfo> buffers)
        {
            foreach (BufferInfo info in buffers)
            {
                switch (info.Type)
                {
                    case 0x65:
                        base.FightBuffers.ConsortionAddBloodGunCount = info.Value;
                        break;

                    case 0x66:
                        base.FightBuffers.ConsortionAddDamage = info.Value;
                        break;

                    case 0x67:
                        base.FightBuffers.ConsortionAddCritical = info.Value;
                        break;

                    case 0x68:
                        base.FightBuffers.ConsortionAddMaxBlood = info.Value;
                        break;

                    case 0x69:
                        base.FightBuffers.ConsortionAddProperty = info.Value;
                        break;

                    case 0x6a:
                        base.FightBuffers.ConsortionReduceEnergyUse = info.Value;
                        break;

                    case 0x6b:
                        base.FightBuffers.ConsortionAddEnergy = info.Value;
                        break;

                    case 0x6c:
                        base.FightBuffers.ConsortionAddEffectTurn = info.Value;
                        break;

                    case 0x6d:
                        base.FightBuffers.ConsortionAddOfferRate = info.Value;
                        break;

                    case 110:
                        base.FightBuffers.ConsortionAddPercentGoldOrGP = info.Value;
                        break;

                    case 0x6f:
                        base.FightBuffers.ConsortionAddSpellCount = info.Value;
                        break;

                    case 0x70:
                        base.FightBuffers.ConsortionReduceDander = info.Value;
                        break;

                    case 400:
                        base.FightBuffers.WorldBossHP = info.Value;
                        break;

                    case 0x191:
                        base.FightBuffers.WorldBossAttrack = info.Value;
                        break;

                    case 0x192:
                        base.FightBuffers.WorldBossHP_MoneyBuff = info.Value;
                        break;

                    case 0x193:
                        base.FightBuffers.WorldBossAttrack_MoneyBuff = info.Value;
                        break;

                    case 0x194:
                        base.FightBuffers.WorldBossMetalSlug = info.Value;
                        break;

                    case 0x195:
                        base.FightBuffers.WorldBossAncientBlessings = info.Value;
                        break;

                    case 0x196:
                        base.FightBuffers.WorldBossAddDamage = info.Value;
                        break;

                    default:
                        Console.WriteLine(string.Format("Not Found FightBuff Type {0} Value {1}", info.Type, info.Value));
                        break;
                }
            }
        }

        public void InitPetSkillEffect(UsersPetinfo pet)
        {
            foreach (string str in pet.SkillEquip.Split(new char[] { '|' }))
            {
                int skillID = int.Parse(str.Split(new char[] { ',' })[0]);
                PetSkillInfo info = PetMgr.FindPetSkill(skillID);
                if (info == null)
                {
                    break;
                }
                string[] strArray3 = info.ElementIDs.Split(new char[] { ',' });
                int coldDown = info.ColdDown;
                int probability = info.Probability;
                int delay = info.Delay;
                int gameType = info.GameType;
                foreach (string str2 in strArray3)
                {
                    switch (str2)
                    {
                        case "1017":
                            new PetStopMovingEquipEffect(2, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1018":
                        case "1019":
                        case "1020":
                        case "1046":
                        case "1047":
                        case "1048":
                            new PetAddDefendEquipEffect(2, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1021":
                            new PetNoHoleEquipEffect(2, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1132":
                            new PetReduceAttackEquipEffect(2, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1038":
                            new PetFatalEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1070":
                            new PetRemovePlusDameEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1072":
                        case "1073":
                            new PetPlusDameEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1049":
                        case "1050":
                            new PetPlusGuardEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1120":
                        case "1121":
                        case "1241":
                        case "1242":
                            new PetAddGuardEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1055":
                            new PetClearPlusGuardEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1106":
                            new PetPlusOneMpEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1076":
                        case "1077":
                            new PetAttackAroundEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1082":
                            new PetAlwayNoHoleEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1083":
                        case "1084":
                            new PetAddAttackEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1085":
                        case "1086":
                            new PetAddLuckAllMatchEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1087":
                        case "1088":
                            new PetReduceDefendEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1089":
                            new PetClearV3BatteryEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1200":
                            new PetPlusAllTwoMpEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1223":
                        case "1253":
                        case "1263":
                            new PetPlusTwoMpEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1135":
                        case "1231":
                        case "1246":
                        case "1247":
                            new PetReduceTakeDamageEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1029":
                        case "1030":
                        case "1031":
                        case "1032":
                        case "1033":
                        case "1034":
                        case "1232":
                        case "1233":
                        case "1234":
                            new PetReduceDamageEquipEffect(1, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1067":
                        case "1068":
                        case "1228":
                        case "1229":
                            new PetMakeDamageEquipEffect(2, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1113":
                        case "1114":
                        case "1236":
                        case "1237":
                            new PetLuckMakeDamageEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1117":
                            new PetRemoveDamageEquipEffect(1, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1115":
                        case "1116":
                        case "1243":
                        case "1244":
                            new PetAddBloodEquipEffect(1, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1238":
                        case "1239":
                        case "1240":
                            new PetAddDamageEquipEffect(2, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1043":
                        case "1044":
                        case "1045":
                            new PetUnlimitAddBloodEquipEffect(2, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1058":
                        case "1059":
                            new PetRevertBloodAllPlayerAroundEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1026":
                        case "1027":
                            new PetAddBloodAllPlayerAroundEquipEffect(3, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1063":
                            new PetAddBloodForSelfEquipEffect(3, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1028":
                            new PetAddBloodAllPlayerAroundEquipEffect(4, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1064":
                            new PetAddBloodForSelfEquipEffect(4, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1080":
                        case "1081":
                            new PetAddBloodForTeamEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1118":
                        case "1119":
                            new PetAddDefendByCureEquipEffect(2, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1254":
                        case "1255":
                        case "1256":
                            new PetReduceTargetAttackEquipEffect(3, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1257":
                        case "1258":
                            new PetAddGuardForTeamEquipEffect(2, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1259":
                        case "1260":
                            new PetRecoverBloodForTeamEquipEffect(2, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1268":
                        case "1269":
                            new PetRecoverMPForTeamEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1266":
                        case "1267":
                            new PetRemoveTagertMPEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1261":
                        case "1262":
                            new PetAddGodLuckEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1264":
                        case "1265":
                            new PetAddGodDamageEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1270":
                        case "1271":
                            new PetReduceTargetBloodEquipEffect(2, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1272":
                        case "1273":
                        case "1274":
                            new PetAtomBombEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1040":
                        case "1041":
                        case "1042":
                            new PetAddLuckLimitTurnEquipEffect(2, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1022":
                        case "1023":
                            new PetActiveGuardForTeamEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1024":
                        case "1025":
                            new PetActiveDamageForTeamEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1056":
                        case "1057":
                            new PetRecoverBloodForTeamInMapEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1074":
                        case "1075":
                            new PetSecondWeaponBonusPointEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1078":
                        case "1079":
                            new PetGuardSecondWeaponRecoverBloodEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1107":
                            new PetAddHighMPEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1109":
                        case "1110":
                            new PetClearHighMPEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1092":
                        case "1093":
                            new PetBonusAttackForTeamEquipEffect(3, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1094":
                        case "1095":
                            new PetBonusDefendForTeamEquipEffect(3, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1096":
                        case "1097":
                            new PetBonusAgilityForTeamEquipEffect(3, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1098":
                        case "1099":
                            new PetBonusLuckyForTeamEquipEffect(3, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1100":
                        case "1101":
                            new PetBonusMaxHpForTeamEquipEffect(3, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1222":
                            new PetStopMovingAllEnemyEffect(2, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1220":
                        case "1221":
                            new PetReduceGuardAllEnemyEquipEffect(2, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1214":
                        case "1215":
                            new PetBonusGuardBeginMatchEquipEffect(2, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1216":
                        case "1217":
                            new PetBonusMaxBloodBeginMatchEquipEffect(2, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1210":
                        case "1211":
                            new PetReduceBloodAllBattleEquipEffectcs(3, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1212":
                        case "1213":
                            new PetAddCritRateEquipEffect(3, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1208":
                        case "1209":
                            new PetReduceMpAllEnemyEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1204":
                        case "1205":
                            new PetReduceAttackAllEnemyEquipEffect(2, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1206":
                        case "1207":
                            new PetReduceDefendAllEnemyEquipEffect(2, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1201":
                        case "1202":
                        case "1203":
                            new PetReduceBaseDamageTargetEquipEffect(3, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1150":
                        case "1151":
                        case "1152":
                            new PetBurningBloodTargetEquipEffect(3, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1153":
                        case "1154":
                            new PetAddDamageEquipEffect(3, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1155":
                        case "1156":
                            new PetReduceBaseGuardEquipEffect(3, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1174":
                        case "1175":
                            new PetAttackedRecoverBloodEquipEffect(3, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1176":
                        case "1177":
                            new PetEnemyAttackBurningBloodEquipEffect(3, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1163":
                        case "1164":
                            new PetBonusAttackBeginMatchEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1165":
                        case "1166":
                            new PetBonusBaseDamageBeginMatchEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1161":
                        case "1162":
                            new PetDamageAllEnemyEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1178":
                        case "1179":
                        case "1180":
                            new PetBuffAttackEquipEffect(3, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1181":
                        case "1182":
                        case "1183":
                            new PetBuffBaseGuardForTeamEquipEffect(2, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1184":
                        case "1185":
                            new PetAddDamageEquipEffect(200, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1186":
                        case "1187":
                            new PetReduceBloodAllBattleEquipEffectcs(200, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1188":
                        case "1189":
                            new PetClearHellIceEquipEffectcs(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1190":
                        case "1191":
                            new PetBonusAgilityBeginMatchEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1192":
                        case "1193":
                            new PetBonusDefendTeamBeginMatchEquipEffect(0, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1198":
                        case "1299":
                            new PetAddCritRateEquipEffect(1, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1196":
                        case "1197":
                            new PetAddDamageEquipEffect(1, probability, gameType, skillID, delay, str2).Start(this);
                            break;

                        case "1194":
                        case "1195":
                            new PetBuffAttackEquipEffect(1, probability, gameType, skillID, delay, str2).Start(this);
                            break;
                    }
                }
            }
        }

        public bool IsCure()
        {
            switch (this.Weapon.TemplateID)
            {
                case 0x4268:
                case 0x4269:
                case 0x426a:
                case 0x426d:
                case 0x426f:
                case 0x4272:
                case 0x42cc:
                case 0x42ce:
                    return true;

                case 0x426b:
                case 0x426c:
                case 0x426e:
                case 0x4270:
                case 0x4271:
                    return false;

                case 0x42cd:
                    return false;
            }
            return false;
        }

        public override void OnAfterKillingLiving(Living target, int damageAmount, int criticalAmount)
        {
            base.OnAfterKillingLiving(target, damageAmount, criticalAmount);
            if (target is Player)
            {
                this.m_player.OnKillingLiving(base.m_game, 1, target.Id, target.IsLiving, damageAmount + criticalAmount);
            }
            else
            {
                int id = 0;
                if (target is SimpleBoss)
                {
                    SimpleBoss boss = target as SimpleBoss;
                    id = boss.NpcInfo.ID;
                }
                if (target is SimpleNpc)
                {
                    SimpleNpc npc = target as SimpleNpc;
                    id = npc.NpcInfo.ID;
                }
                this.m_player.OnKillingLiving(base.m_game, 2, id, target.IsLiving, damageAmount + criticalAmount);
            }
        }

        protected void OnAfterPlayerShoot()
        {
            if (this.AfterPlayerShooted != null)
            {
                this.AfterPlayerShooted(this);
            }
        }

        protected void OnBeforePlayerShoot()
        {
            if (this.BeforePlayerShoot != null)
            {
                this.BeforePlayerShoot(this);
            }
        }

        protected void OnCollidedByObject()
        {
            if (this.CollidByObject != null)
            {
                this.CollidByObject(this);
            }
        }

        protected void OnLoadingCompleted()
        {
            if (this.LoadingCompleted != null)
            {
                this.LoadingCompleted(this);
            }
        }

        public void OnPlayerBuffSkillPet()
        {
            if (this.PlayerBuffSkillPet != null)
            {
                this.PlayerBuffSkillPet(this);
            }
        }

        public void OnPlayerCure()
        {
            if (this.PlayerCure != null)
            {
                this.PlayerCure(this);
            }
        }

        public void OnPlayerGuard()
        {
            if (this.PlayerGuard != null)
            {
                this.PlayerGuard(this);
            }
        }

        protected void OnPlayerMoving()
        {
            if (this.PlayerBeginMoving != null)
            {
                this.PlayerBeginMoving(this);
            }
        }

        public void OnPlayerShoot()
        {
            if (this.PlayerShoot != null)
            {
                this.PlayerShoot(this);
            }
        }

        public void OpenBox(int boxId)
        {
            Box box = null;
            foreach (Box box2 in this.m_tempBoxes)
            {
                if (box2.Id == boxId)
                {
                    box = box2;
                    break;
                }
            }
            if ((box != null) && (box.Item != null))
            {
                SqlDataProvider.Data.ItemInfo item = box.Item;
                switch (item.TemplateID)
                {
                    case -1100:
                        this.m_player.AddGiftToken(item.Count);
                        break;

                    case -300:
                        this.m_player.AddMedal(item.Count);
                        break;

                    case -200:
                        this.m_player.AddMoney(item.Count);
                        this.m_player.LogAddMoney(AddMoneyType.Box, AddMoneyType.Box_Open, this.m_player.PlayerCharacter.ID, item.Count, this.m_player.PlayerCharacter.Money);
                        break;

                    case -100:
                        this.m_player.AddGold(item.Count);
                        break;

                    default:
                        if (item.Template.CategoryID == 10)
                        {
                            if (!this.m_player.AddTemplate(item, eBageType.FightBag, item.Count, eGameView.RouletteTypeGet))
                            {
                                this.m_player.SendMessage("H\x00e0nh trang chiến đấu đ\x00e3 đầy.");
                            }
                        }
                        else
                        {
                            this.m_player.AddTemplate(item, eBageType.TempBag, item.Count, eGameView.dungeonTypeGet);
                        }
                        break;
                }
                this.m_tempBoxes.Remove(box);
            }
        }

        public void PetUseKill(int skillID)
        {
            if (this.CanUseSkill(skillID))
            {
                PetSkillInfo info = PetMgr.FindPetSkill(skillID);
                if ((base.PetMP > 0) && (base.PetMP >= info.CostMP))
                {
                    if (info.NewBallID != -1)
                    {
                        base.m_delay += info.Delay;
                        this.SetBall(info.NewBallID);
                    }
                    base.PetMP -= info.CostMP;
                    if (info.DamageCrit > 0)
                    {
                        base.PetEffects.CritActive = true;
                        base.CurrentDamagePlus += info.DamageCrit / 100;
                    }
                    base.PetEffects.IsPetUseSkill = true;
                    base.PetEffects.CurrentUseSkill = skillID;
                    base.m_game.SendPetUseKill(this);
                    this.OnPlayerBuffSkillPet();
                }
                else
                {
                    this.m_player.SendMessage("Ma Ph\x00e1p kh\x00f4ng đủ.");
                }
            }
        }

        public override void PickBox(Box box)
        {
            this.m_tempBoxes.Add(box);
            base.PickBox(box);
        }

        public override void PrepareNewTurn()
        {
            this.ItemFightBag.Clear();
            if (base.CurrentIsHitTarget)
            {
                base.TotalHitTargetCount++;
            }
            this.m_energy = (((int) base.Agility) / 30) + 240;
            if (base.FightBuffers.ConsortionAddEnergy > 0)
            {
                this.m_energy += base.FightBuffers.ConsortionAddEnergy;
            }
            base.PetEffects.CurrentUseSkill = 0;
            base.PetEffects.PetDelay = 0;
            base.SpecialSkillDelay = 0;
            this.m_shootCount = 1;
            this.m_ballCount = 1;
            base.EffectTrigger = false;
            base.PetEffectTrigger = false;
            this.m_flyCoolDown--;
            this.SetCurrentWeapon(this.PlayerDetail.MainWeapon);
            if (this.m_currentBall.ID != this.m_mainBallId)
            {
                this.m_currentBall = BallMgr.FindBall(this.m_mainBallId);
            }
            if (base.m_game.RoomType == eRoomType.FightFootballTime)
            {
                this.m_currentBall = BallMgr.FindBall(0x18);
            }
            if (!base.IsLiving)
            {
                this.StartGhostMoving();
                this.TargetPoint = Point.Empty;
            }
            if (!base.PetEffects.StopMoving)
            {
                base.SpeedMultX(3);
            }
            base.PrepareNewTurn();
        }

        public override void PrepareSelfTurn()
        {
            base.PrepareSelfTurn();
            base.DefaultDelay = base.m_delay;
            this.m_flyCoolDown--;
            if (base.IsFrost)
            {
                base.AddDelay(this.GetTurnDelay());
            }
        }

        public void PrepareShoot(byte speedTime)
        {
            int turnWaitTime = base.m_game.GetTurnWaitTime();
            int num2 = (speedTime > turnWaitTime) ? turnWaitTime : speedTime;
            base.AddDelay(num2 * 20);
            base.TotalShootCount++;
        }

        public bool ReduceEnergy(int value)
        {
            if (value > this.m_energy)
            {
                return false;
            }
            this.m_energy -= value;
            return true;
        }

        public override void Reset()
        {
            if ((base.m_game.RoomType == eRoomType.Dungeon) || (base.m_game.RoomType == eRoomType.SpecialActivityDungeon))
            {
                base.m_game.Cards = new int[0x15];
            }
            else
            {
                base.m_game.Cards = new int[9];
            }
            base.Dander = 0;
            base.PetMP = 10;
            base.psychic = 40;
            base.IsLiving = true;
            this.FinishTakeCard = false;
            this.m_Healstone = this.m_player.Healstone;
            this.m_changeSpecialball = 0;
            this.m_DeputyWeapon = this.m_player.SecondWeapon;
            this.m_weapon = this.m_player.MainWeapon;
            BallConfigInfo info = BallConfigMgr.FindBall(this.m_weapon.TemplateID);
            this.m_mainBallId = info.Common;
            this.m_spBallId = info.Special;
            this.m_AddWoundBallId = info.CommonAddWound;
            this.m_MultiBallId = info.CommonMultiBall;
            base.BaseDamage = this.m_player.GetBaseAttack();
            base.BaseGuard = this.m_player.GetBaseDefence();
            base.Attack = this.m_player.PlayerCharacter.Attack;
            base.Defence = this.m_player.PlayerCharacter.Defence;
            base.Agility = this.m_player.PlayerCharacter.Agility;
            base.Lucky = this.m_player.PlayerCharacter.Luck;
            base.m_maxBlood = this.m_player.PlayerCharacter.hp;
            base.BaseDamage += this.m_player.PlayerCharacter.DameAddPlus + base.FightBuffers.WorldBossAttrack_MoneyBuff;
            if (base.FightBuffers.ConsortionAddDamage > 0)
            {
                base.BaseDamage += base.FightBuffers.ConsortionAddDamage;
            }
            base.BaseGuard += this.m_player.PlayerCharacter.GuardAddPlus + base.PetEffects.BonusGuard;
            base.Attack += this.m_player.PlayerCharacter.AttackAddPlus + base.PetEffects.BonusAttack;
            base.Defence += this.m_player.PlayerCharacter.DefendAddPlus + base.PetEffects.BonusDefend;
            base.Agility += this.m_player.PlayerCharacter.AgiAddPlus + base.PetEffects.BonusAgility;
            base.Lucky += this.m_player.PlayerCharacter.LuckAddPlus + base.PetEffects.BonusLucky;
            base.m_maxBlood = this.m_player.PlayerCharacter.hp;
            base.Attack += this.m_player.PlayerCharacter.StrengthEnchance;
            base.Defence += this.m_player.PlayerCharacter.StrengthEnchance;
            base.Agility += this.m_player.PlayerCharacter.StrengthEnchance;
            base.Lucky += this.m_player.PlayerCharacter.StrengthEnchance;
            if (base.FightBuffers.ConsortionAddMaxBlood > 0)
            {
                base.m_maxBlood += (base.m_maxBlood * base.FightBuffers.ConsortionAddMaxBlood) / 100;
            }
            base.m_maxBlood += ((this.m_player.PlayerCharacter.HpAddPlus + base.FightBuffers.WorldBossHP) + base.FightBuffers.WorldBossHP_MoneyBuff) + base.PetEffects.MaxBlood;
            if (base.FightBuffers.ConsortionAddProperty > 0)
            {
                base.Attack += base.FightBuffers.ConsortionAddProperty;
                base.Defence += base.FightBuffers.ConsortionAddProperty;
                base.Agility += base.FightBuffers.ConsortionAddProperty;
                base.Lucky += base.FightBuffers.ConsortionAddProperty;
            }
            this.m_energy = (((int) base.Agility) / 30) + 240;
            if (base.FightBuffers.ConsortionAddEnergy > 0)
            {
                this.m_energy += base.FightBuffers.ConsortionAddEnergy;
            }
            this.m_currentBall = BallMgr.FindBall(this.m_mainBallId);
            this.m_shootCount = 1;
            this.m_ballCount = 1;
            this.m_prop = 0;
            base.CurrentIsHitTarget = false;
            this.TotalCure = 0;
            base.TotalHitTargetCount = 0;
            base.TotalHurt = 0;
            base.TotalKill = 0;
            base.TotalShootCount = 0;
            this.LockDirection = false;
            this.GainGP = 0;
            this.GainOffer = 0;
            this.Ready = false;
            this.PlayerDetail.ClearTempBag();
            base.m_delay = this.GetTurnDelay();
            this.LoadingProcess = 0;
            base.Reset();
        }

        public void SetBall(int ballId)
        {
            this.SetBall(ballId, false);
        }

        public void SetBall(int ballId, bool special)
        {
            if (ballId != this.m_currentBall.ID)
            {
                if (BallMgr.FindBall(ballId) != null)
                {
                    this.m_currentBall = BallMgr.FindBall(ballId);
                }
                base.m_game.SendGameUpdateBall(this, special);
            }
        }

        public void SetCurrentWeapon(ItemTemplateInfo item)
        {
            this.m_weapon = item;
            BallConfigInfo info = BallConfigMgr.FindBall(this.m_weapon.TemplateID);
            if (this.ChangeSpecialBall > 0)
            {
                info = BallConfigMgr.FindBall(0x112fc);
            }
            this.m_mainBallId = info.Common;
            this.m_spBallId = info.Special;
            this.m_AddWoundBallId = info.CommonAddWound;
            this.m_MultiBallId = info.CommonMultiBall;
            this.SetBall(this.m_mainBallId);
        }

        public override void SetXY(int x, int y)
        {
            if ((base.m_x != x) || (base.m_y != y))
            {
                int num = Math.Abs((int) (base.m_x - x));
                base.m_x = x;
                base.m_y = y;
                if (base.IsLiving)
                {
                    this.m_energy -= Math.Abs((int) (base.m_x - x));
                    if (num > 0)
                    {
                        this.OnPlayerMoving();
                    }
                }
                else
                {
                    Rectangle rect = base.m_rect;
                    rect.Offset(base.m_x, base.m_y);
                    foreach (Physics physics in base.m_map.FindPhysicalObjects(rect, this))
                    {
                        if (physics is Box)
                        {
                            Box box = physics as Box;
                            this.PickBox(box);
                            this.OpenBox(box.Id);
                        }
                    }
                }
            }
        }

        public bool Shoot(int x, int y, int force, int angle)
        {
            if (this.m_shootCount == 1)
            {
                base.PetEffects.ActivePetHit = true;
            }
            if (this.m_shootCount > 0)
            {
                this.OnPlayerShoot();
                int iD = this.m_currentBall.ID;
                if ((this.m_ballCount == 1) && !this.IsSpecialSkill)
                {
                    if (this.Prop == 0x4e22)
                    {
                        iD = this.m_MultiBallId;
                    }
                    if (this.Prop == 0x4e28)
                    {
                        iD = this.m_AddWoundBallId;
                    }
                }
                this.OnBeforePlayerShoot();
                if (this.IsSpecialSkill)
                {
                    base.SpecialSkillDelay = 0x7d0;
                }
                if (base.ShootImp(iD, x, y, force, angle, this.m_ballCount, this.ShootCount))
                {
                    if (iD == 4)
                    {
                        base.m_game.AddAction(new FightAchievementAction(this, 2, base.Direction, 0x4b0));
                    }
                    this.m_shootCount--;
                    if ((this.m_shootCount <= 0) || !base.IsLiving)
                    {
                        this.StopAttacking();
                        base.AddDelay(this.m_currentBall.Delay);
                        base.AddDander(20);
                        base.AddPetMP(10);
                        this.m_prop = 0;
                        if (this.CanGetProp)
                        {
                            int gold = 0;
                            int money = 0;
                            int giftToken = 0;
                            int medal = 0;
                            int honor = 0;
                            int hardCurrency = 0;
                            int token = 0;
                            int dragonToken = 0;
                            int magicStonePoint = 0;
                            List<SqlDataProvider.Data.ItemInfo> list = null;
                            if (DropInventory.FireDrop(base.m_game.RoomType, ref list) && (list != null))
                            {
                                foreach (SqlDataProvider.Data.ItemInfo info in list)
                                {
                                    ShopMgr.FindSpecialItemInfo(info, ref gold, ref money, ref giftToken, ref medal, ref honor, ref hardCurrency, ref token, ref dragonToken, ref magicStonePoint);
                                    if (((info != null) && base.VaneOpen) && (info.TemplateID > 0))
                                    {
                                        if (info.Template.CategoryID == 10)
                                        {
                                            if (!this.PlayerDetail.AddTemplate(info, eBageType.FightBag, info.Count, eGameView.RouletteTypeGet))
                                            {
                                                this.m_player.SendMessage("H\x00e0nh trang chiến đấu đ\x00e3 đầy.");
                                            }
                                        }
                                        else
                                        {
                                            this.PlayerDetail.AddTemplate(info, eBageType.TempBag, info.Count, eGameView.dungeonTypeGet);
                                        }
                                    }
                                }
                                this.PlayerDetail.AddGold(gold);
                                this.PlayerDetail.AddMoney(money);
                                this.PlayerDetail.LogAddMoney(AddMoneyType.Game, AddMoneyType.Game_Shoot, this.PlayerDetail.PlayerCharacter.ID, money, this.PlayerDetail.PlayerCharacter.Money);
                                this.PlayerDetail.AddGiftToken(giftToken);
                                this.PlayerDetail.AddMedal(medal);
                            }
                        }
                    }
                    this.OnAfterPlayerShoot();
                    return true;
                }
            }
            return false;
        }

        public override void Skip(int spendTime)
        {
            if (base.IsAttacking)
            {
                base.Game.SendSkipNext(this);
                this.m_prop = 0;
                base.AddDelay(100);
                base.AddDander(20);
                base.AddPetMP(10);
                base.Skip(spendTime);
            }
        }

        public override void StartAttacking()
        {
            if (!base.IsAttacking)
            {
                if (((this.m_Healstone != null) && (base.m_blood < base.m_maxBlood)) && this.m_player.RemoveHealstone())
                {
                    this.AddBlood(this.m_Healstone.Template.Property2);
                }
                base.AddDelay(this.GetTurnDelay());
                base.StartAttacking();
            }
        }

        public Point StartFalling(bool direct)
        {
            return this.StartFalling(direct, 0, Living.MOVE_SPEED * 10);
        }

        public virtual Point StartFalling(bool direct, int delay, int speed)
        {
            Point p = base.m_map.FindYLineNotEmptyPoint(this.X, this.Y);
            if (p == Point.Empty)
            {
                p = new Point(this.X, base.m_game.Map.Bound.Height + 1);
            }
            if (p.Y == this.Y)
            {
                return Point.Empty;
            }
            if (direct)
            {
                base.SetXY(p);
                if (base.m_map.IsOutMap(p.X, p.Y))
                {
                    base.Die();
                }
                return p;
            }
            base.m_game.AddAction(new LivingFallingAction(this, p.X, p.Y, speed, null, delay, 0, null));
            return p;
        }

        public void StartGhostMoving()
        {
            if (!this.TargetPoint.IsEmpty)
            {
                Point point = new Point(this.TargetPoint.X - this.X, this.TargetPoint.Y - this.Y);
                if (point.Length() > 160.0)
                {
                    point.Normalize(160);
                }
                base.m_game.AddAction(new GhostMoveAction(this, new Point(this.X + point.X, this.Y + point.Y)));
            }
        }

        public override void StartMoving()
        {
            if (base.m_map != null)
            {
                Point point = base.m_map.FindYLineNotEmptyPoint(base.m_x, base.m_y);
                if (point.IsEmpty)
                {
                    if (base.m_map.Ground != null)
                    {
                        base.m_y = base.m_map.Ground.Height;
                    }
                }
                else
                {
                    base.m_x = point.X;
                    base.m_y = point.Y;
                }
                if (point.IsEmpty)
                {
                    base.m_syncAtTime = false;
                    this.Die();
                }
            }
        }

        public override void StartMoving(int delay, int speed)
        {
            if (base.m_map != null)
            {
                Point point = base.m_map.FindYLineNotEmptyPoint(base.m_x, base.m_y);
                if (point.IsEmpty)
                {
                    base.m_y = base.m_map.Ground.Height;
                }
                else
                {
                    base.m_x = point.X;
                    base.m_y = point.Y;
                }
                base.StartMoving(delay, speed);
                if (point.IsEmpty)
                {
                    base.m_syncAtTime = false;
                    this.Die();
                }
            }
        }

        public void StartRotate(int rotation, int speed, string endPlay, int delay)
        {
            base.m_game.AddAction(new LivingRotateTurnAction(this, rotation, speed, endPlay, delay));
        }

        public void StartSpeedMoving(int x, int y, int delay)
        {
            Point point = new Point(x - this.X, y - this.Y);
            base.m_game.AddAction(new PlayerMoveAction(this, new Point(this.X + point.X, this.Y + point.Y), delay));
        }

        public void StartSpeedMult(int x, int y)
        {
            this.StartSpeedMult(x, y, 0xbb8);
        }

        public void StartSpeedMult(int x, int y, int delay)
        {
            Point point = new Point(x - this.X, y - this.Y);
            base.m_game.AddAction(new PlayerSpeedMultAction(this, new Point(this.X + point.X, this.Y + point.Y), delay));
        }

        public void StartSpeedMult(int x, int y, int delay, int speed)
        {
            Point point = new Point(x - this.X, y - this.Y);
            base.m_game.AddAction(new PlayerSetXYAction(this, new Point(this.X + point.X, this.Y + point.Y), delay, speed));
        }

        public override bool TakeDamage(Living source, ref int damageAmount, ref int criticalAmount, string msg)
        {
            if (((source == this) || (source.Team == base.Team)) && ((damageAmount + criticalAmount) >= base.m_blood))
            {
                damageAmount = base.m_blood - 1;
                criticalAmount = 0;
            }
            bool flag = base.TakeDamage(source, ref damageAmount, ref criticalAmount, msg);
            if (base.IsLiving)
            {
                base.AddDander((((damageAmount * 2) / 5) + 5) / 2);
            }
            return flag;
        }

        public void UseFlySkill()
        {
            if ((this.m_flyCoolDown > 0) && (base.Game.RoomType == eRoomType.BattleRoom))
            {
                this.m_flyCoolDown--;
                base.m_game.SendPlayerUseProp(this, -2, -2, CARRY_TEMPLATE_ID);
                this.SetBall(3);
            }
            else
            {
                base.m_game.SendPlayerUseProp(this, -2, -2, CARRY_TEMPLATE_ID);
                this.SetBall(3);
            }
        }

        public bool UseItem(ItemTemplateInfo item)
        {
            if (this.CanUseItem(item))
            {
                this.m_energy -= item.Property4;
                base.m_delay += item.Property5;
                base.m_game.SendPlayerUseProp(this, -2, -2, item.TemplateID, this);
                SpellMgr.ExecuteSpell(base.m_game, this, item);
                return true;
            }
            return false;
        }

        public bool UseItem(ItemTemplateInfo item, int place)
        {
            if (!this.CanUseItem(item, place))
            {
                return false;
            }
            if (base.IsLiving)
            {
                this.ReduceEnergy(item.Property4);
                base.AddDelay(item.Property5);
            }
            else if (place == -1)
            {
                base.psychic -= item.Property7;
                base.Game.CurrentLiving.AddDelay(item.Property5);
            }
            base.m_game.method_39(this, -2, -2, item.TemplateID);
            SpellMgr.ExecuteSpell(base.m_game, base.m_game.CurrentLiving as Player, item);
            if ((item.Property6 == 1) && base.IsAttacking)
            {
                this.StopAttacking();
                base.m_game.CheckState(0);
            }
            return true;
        }

        public void UseSecondWeapon()
        {
            if (this.CanUseItem(this.m_DeputyWeapon.Template))
            {
                if (this.m_DeputyWeapon.Template.Property3 == 0x1f)
                {
                    int count = (int) base.getHertAddition(this.m_DeputyWeapon);
                    new AddGuardEquipEffect(count, 1).Start(this);
                    this.OnPlayerGuard();
                }
                else
                {
                    this.SetCurrentWeapon(this.m_DeputyWeapon.Template);
                    this.OnPlayerCure();
                }
                this.ShootCount = 1;
                this.m_energy -= this.m_DeputyWeapon.Template.Property4;
                base.m_delay += this.m_DeputyWeapon.Template.Property5;
                base.m_game.SendPlayerUseProp(this, -2, -2, this.m_DeputyWeapon.Template.TemplateID);
                if (this.deputyWeaponResCount > 0)
                {
                    this.deputyWeaponResCount--;
                    base.m_game.SendUseDeputyWeapon(this, this.deputyWeaponResCount);
                }
            }
        }

        public void UseSpecialSkill()
        {
            if (base.Dander >= 200)
            {
                this.SetBall(this.m_spBallId, true);
                this.m_ballCount = this.m_currentBall.Amount;
                base.SetDander(0);
            }
        }

        public int BallCount
        {
            get
            {
                return this.m_ballCount;
            }
            set
            {
                if (this.m_ballCount != value)
                {
                    this.m_ballCount = value;
                }
            }
        }

        public bool CanGetProp
        {
            get
            {
                return this.m_canGetProp;
            }
            set
            {
                if (this.m_canGetProp != value)
                {
                    this.m_canGetProp = value;
                }
            }
        }

        public BallInfo CurrentBall
        {
            get
            {
                return this.m_currentBall;
            }
        }

        public int ChangeSpecialBall
        {
            get
            {
                return this.m_changeSpecialball;
            }
            set
            {
                this.m_changeSpecialball = value;
            }
        }

        public SqlDataProvider.Data.ItemInfo DeputyWeapon
        {
            get
            {
                return this.m_DeputyWeapon;
            }
        }

        public int deputyWeaponCount
        {
            get
            {
                return this.deputyWeaponResCount;
            }
        }

        public int Energy
        {
            get
            {
                return this.m_energy;
            }
            set
            {
                this.m_energy = value;
            }
        }

        public int flyCount
        {
            get
            {
                return this.m_flyCoolDown;
            }
        }

        public bool IsActive
        {
            get
            {
                return this.m_isActive;
            }
        }

        public bool IsSpecialSkill
        {
            get
            {
                return (this.m_currentBall.ID == this.m_spBallId);
            }
        }

        public int LoadingProcess
        {
            get
            {
                return this.m_loadingProcess;
            }
            set
            {
                if (this.m_loadingProcess != value)
                {
                    this.m_loadingProcess = value;
                    if (this.m_loadingProcess >= 100)
                    {
                        this.OnLoadingCompleted();
                    }
                }
            }
        }

        public int OldX
        {
            get
            {
                return this.m_oldx;
            }
            set
            {
                this.m_oldx = value;
            }
        }

        public int OldY
        {
            get
            {
                return this.m_oldy;
            }
            set
            {
                this.m_oldy = value;
            }
        }

        public UsersPetinfo Pet
        {
            get
            {
                return this.m_pet;
            }
        }

        public IGamePlayer PlayerDetail
        {
            get
            {
                return this.m_player;
            }
        }

        public int Prop
        {
            get
            {
                return this.m_prop;
            }
            set
            {
                this.m_prop = value;
            }
        }

        public int ShootCount
        {
            get
            {
                return this.m_shootCount;
            }
            set
            {
                if (this.m_shootCount != value)
                {
                    this.m_shootCount = value;
                    base.m_game.SendGameUpdateShootCount(this);
                }
            }
        }

        public ItemTemplateInfo Weapon
        {
            get
            {
                return this.m_weapon;
            }
        }
    }
}

