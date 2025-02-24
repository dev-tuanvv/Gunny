namespace Game.Logic
{
    using Bussiness;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Reflection;

    public class PetMgr
    {
        private static Dictionary<string, PetConfig> _configs;
        private static Dictionary<int, PetExpItemPriceInfo> _expItemPrices;
        private static Dictionary<int, PetLevel> _levels;
        private static Dictionary<int, PetFightPropertyInfo> _petFightProp;
        private static Dictionary<int, PetStarExpInfo> _petStarExp;
        private static Dictionary<int, PetSkillElementInfo> _skillElements;
        private static Dictionary<int, PetSkillInfo> _skills;
        private static Dictionary<int, PetSkillTemplateInfo> _skillTemplates;
        private static Dictionary<int, PetTemplateInfo> _templateIds;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock;
        private static ThreadSafeRandom rand;

        public static string ActiveEquipSkill(int Level)
        {
            string str = "0,0";
            int num = 1;
            if ((Level >= 20) && (Level < 30))
            {
                num++;
            }
            if ((Level >= 30) && (Level < 50))
            {
                num += 2;
            }
            if ((Level >= 50) && (Level < 60))
            {
                num += 3;
            }
            if (Level == 60)
            {
                num += 4;
            }
            for (int i = 1; i < num; i++)
            {
                str = str + "|0," + i;
            }
            return str;
        }

        public static List<UsersPetinfo> CreateAdoptList(int userID, int playerLevel)
        {
            int num = Convert.ToInt32(FindConfig("AdoptCount").Value);
            List<UsersPetinfo> list = new List<UsersPetinfo>();
            List<PetTemplateInfo> info = null;
            int place = 0;
            while (place < num)
            {
                if (DropInventory.GetPetDrop(0x265, 1, ref info) && (info != null))
                {
                    int num3 = rand.Next(info.Count);
                    UsersPetinfo item = CreatePet(info[num3], userID, place, playerLevel);
                    item.IsExit = true;
                    list.Add(item);
                    place++;
                }
            }
            return list;
        }

        public static List<UsersPetinfo> CreateFirstAdoptList(int userID, int playerLevel)
        {
            List<int> list = new List<int> { 0x187cd, 0x1aedd, 0x1d5ed, 0x1fcfd };
            List<UsersPetinfo> list2 = new List<UsersPetinfo>();
            for (int i = 0; i < list.Count; i++)
            {
                UsersPetinfo item = CreatePet(FindPetTemplate(list[i]), userID, i, playerLevel);
                item.IsExit = true;
                list2.Add(item);
            }
            return list2;
        }

        public static UsersPetinfo CreateNewPet()
        {
            string[] strArray = FindConfig("NewPet").Value.Split(new char[] { ',' });
            int index = rand.Next(strArray.Length);
            PetTemplateInfo info = FindPetTemplate(Convert.ToInt32(strArray[index]));
            UsersPetinfo pet = new UsersPetinfo();
            int starLevel = info.StarLevel;
            PetConfig config = FindConfig("PropertiesRate");
            if (config != null)
            {
                int.Parse(config.Value);
            }
            pet.ID = 0;
            pet.BloodGrow = info.HighBloodGrow;
            pet.AttackGrow = info.HighAttackGrow;
            pet.DefenceGrow = info.HighDefenceGrow;
            pet.AgilityGrow = info.HighAgilityGrow;
            pet.LuckGrow = info.HighLuckGrow;
            pet.DamageGrow = info.HighDamageGrow;
            pet.GuardGrow = info.HighGuardGrow;
            pet.Hunger = 0x2710;
            pet.TemplateID = info.TemplateID;
            pet.Name = info.Name;
            pet.UserID = -1;
            pet.Place = -1;
            pet.Level = 60;
            pet.Skill = UpdateSkillPet(60, info.TemplateID, 60);
            pet.SkillEquip = ActiveEquipSkill(1);
            CreateNewPropPet(ref pet);
            return pet;
        }

        public static void CreateNewPropPet(ref UsersPetinfo pet)
        {
            PetConfig config = FindConfig("PropertiesRate");
            int num = (config == null) ? 100 : int.Parse(config.Value);
            double[] numArray = new double[5];
            PetTemplateInfo info = FindPetTemplate(pet.TemplateID);
            if (info != null)
            {
                double[] numArray2 = new double[] { (double) info.HighBlood, (double) info.HighAttack, (double) info.HighDefence, (double) info.HighAgility, (double) info.HighLuck };
                double[] array = new double[] { (double) info.HighBloodGrow, (double) info.HighAttackGrow, (double) info.HighDefenceGrow, (double) info.HighAgilityGrow, (double) info.HighLuckGrow };
                double[] numArray4 = numArray2;
                double[] numArray5 = getAddedPropArr(1.0, array);
                getAddedPropArr(2.0, array);
                getAddedPropArr(3.0, array);
                for (int i = 0; i < numArray4.Length; i++)
                {
                    numArray4[i] += (pet.Level - 1) * numArray5[i];
                    numArray4[i] = Math.Ceiling((double) (numArray4[i] / ((double) num)));
                }
                numArray = numArray4;
                for (int j = 0; j < numArray.Length; j++)
                {
                    switch (j)
                    {
                        case 0:
                            pet.Blood = (int) numArray[j];
                            break;

                        case 1:
                            pet.Attack = (int) numArray[j];
                            break;

                        case 2:
                            pet.Defence = (int) numArray[j];
                            break;

                        case 3:
                            pet.Agility = (int) numArray[j];
                            break;

                        case 4:
                            pet.Luck = (int) numArray[j];
                            break;
                    }
                }
                pet.BloodGrow = info.HighBloodGrow;
                pet.AttackGrow = info.HighAttackGrow;
                pet.DefenceGrow = info.HighDefenceGrow;
                pet.AgilityGrow = info.HighAgilityGrow;
                pet.LuckGrow = info.HighLuckGrow;
            }
        }

        public static UsersPetinfo CreatePet(PetTemplateInfo info, int userID, int place, int playerLevel)
        {
            UsersPetinfo pet = new UsersPetinfo();
            int starLevel = info.StarLevel;
            PetConfig config = FindConfig("PropertiesRate");
            if (config != null)
            {
                int.Parse(config.Value);
            }
            pet.ID = 0;
            pet.BloodGrow = info.HighBloodGrow;
            pet.AttackGrow = info.HighAttackGrow;
            pet.DefenceGrow = info.HighDefenceGrow;
            pet.AgilityGrow = info.HighAgilityGrow;
            pet.LuckGrow = info.HighLuckGrow;
            pet.DamageGrow = info.HighDamageGrow;
            pet.GuardGrow = info.HighGuardGrow;
            pet.Hunger = 0x2710;
            pet.TemplateID = info.TemplateID;
            pet.Name = info.Name;
            pet.UserID = userID;
            pet.Place = place;
            pet.Level = 1;
            pet.Skill = UpdateSkillPet(1, info.TemplateID, playerLevel);
            pet.SkillEquip = ActiveEquipSkill(1);
            CreateNewPropPet(ref pet);
            return pet;
        }

        public static PetConfig FindConfig(string key)
        {
            if (_configs == null)
            {
                Init();
            }
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (_configs.ContainsKey(key))
                {
                    return _configs[key];
                }
            }
            catch
            {
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return null;
        }

        public static PetExpItemPriceInfo FindPetExpItemPrice(int count)
        {
            if (_expItemPrices == null)
            {
                Init();
            }
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (_expItemPrices.ContainsKey(count))
                {
                    return _expItemPrices[count];
                }
            }
            catch
            {
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return null;
        }

        public static PetLevel FindPetLevel(int level)
        {
            if (_levels == null)
            {
                Init();
            }
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (_levels.ContainsKey(level))
                {
                    return _levels[level];
                }
            }
            catch
            {
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return null;
        }

        public static PetSkillInfo FindPetSkill(int SkillID)
        {
            if (_skills == null)
            {
                Init();
            }
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (_skills.ContainsKey(SkillID))
                {
                    return _skills[SkillID];
                }
            }
            catch
            {
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return null;
        }

        public static PetSkillElementInfo FindPetSkillElement(int SkillID)
        {
            if (_skillElements == null)
            {
                Init();
            }
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (_skillElements.ContainsKey(SkillID))
                {
                    return _skillElements[SkillID];
                }
            }
            catch
            {
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return null;
        }

        public static PetTemplateInfo FindPetTemplate(int TemplateID)
        {
            if (_templateIds == null)
            {
                Init();
            }
            foreach (PetTemplateInfo info in _templateIds.Values)
            {
                if (info.TemplateID == TemplateID)
                {
                    return info;
                }
            }
            return null;
        }

        public static PetTemplateInfo FindPetTemplateById(int ID)
        {
            if (_templateIds == null)
            {
                Init();
            }
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (_templateIds.ContainsKey(ID))
                {
                    return _templateIds[ID];
                }
            }
            catch
            {
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return null;
        }

        public static List<PetSkillElementInfo> GameNeedPetSkill()
        {
            if (_skillElements == null)
            {
                Init();
            }
            List<PetSkillElementInfo> list = new List<PetSkillElementInfo>();
            Dictionary<string, PetSkillElementInfo> dictionary = new Dictionary<string, PetSkillElementInfo>();
            foreach (PetSkillElementInfo info in _skillElements.Values)
            {
                if (!(dictionary.Keys.Contains<string>(info.EffectPic) || string.IsNullOrEmpty(info.EffectPic)))
                {
                    list.Add(info);
                    dictionary.Add(info.EffectPic, info);
                }
            }
            return list;
        }

        private static double[] getAddedPropArr(double number, double[] array)
        {
            double[] numArray = new double[5];
            numArray[0] = array[0] * Math.Pow(2.0, number - 1.0);
            for (int i = 1; i < 5; i++)
            {
                numArray[i] = array[i] * Math.Pow(1.5, number - 1.0);
            }
            return numArray;
        }

        public static int GetGP(int level, int playerLevel)
        {
            for (int i = 1; i <= playerLevel; i++)
            {
                if (level == FindPetLevel(i).Level)
                {
                    return FindPetLevel(i).GP;
                }
            }
            return 0;
        }

        public static int GetLevel(int GP, int playerLevel)
        {
            if (GP >= FindPetLevel(playerLevel).GP)
            {
                return playerLevel;
            }
            for (int i = 1; i <= playerLevel; i++)
            {
                if (GP < FindPetLevel(i).GP)
                {
                    if ((i - 1) != 0)
                    {
                        return (i - 1);
                    }
                    return 1;
                }
            }
            return 1;
        }

        public static PetFightPropertyInfo GetPetFightProperty(int level)
        {
            if (_petFightProp == null)
            {
                Init();
            }
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (_petFightProp.ContainsKey(level))
                {
                    return _petFightProp[level];
                }
            }
            catch
            {
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return null;
        }

        public static PetSkillTemplateInfo[] GetPetSkillByKindID(int KindID)
        {
            List<PetSkillTemplateInfo> list = new List<PetSkillTemplateInfo>();
            foreach (PetSkillTemplateInfo info in _skillTemplates.Values)
            {
                if (info.KindID == KindID)
                {
                    list.Add(info);
                }
            }
            return list.ToArray();
        }

        public static List<int> GetPetSkillByKindID(int KindID, int lv, int playerLevel)
        {
            List<int> list = new List<int>();
            List<string> list2 = new List<string>();
            PetSkillTemplateInfo[] petSkillByKindID = GetPetSkillByKindID(KindID);
            int num = (lv > playerLevel) ? playerLevel : lv;
            for (int i = 1; i <= num; i++)
            {
                foreach (PetSkillTemplateInfo info in petSkillByKindID)
                {
                    if (info.MinLevel == i)
                    {
                        foreach (string str in info.DeleteSkillIDs.Split(new char[] { ',' }))
                        {
                            list2.Add(str);
                        }
                        list.Add(info.SkillID);
                    }
                }
            }
            foreach (string str2 in list2)
            {
                if (!string.IsNullOrEmpty(str2))
                {
                    int item = int.Parse(str2);
                    list.Remove(item);
                }
            }
            list.Sort();
            return list;
        }

        public static PetSkillTemplateInfo GetPetSkillTemplate(int ID)
        {
            if (_skillTemplates == null)
            {
                Init();
            }
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (_skillTemplates.ContainsKey(ID))
                {
                    return _skillTemplates[ID];
                }
            }
            catch
            {
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return null;
        }

        public static PetStarExpInfo GetPetStarExp(int oldId)
        {
            if (_petStarExp == null)
            {
                Init();
            }
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (_petStarExp.ContainsKey(oldId))
                {
                    return _petStarExp[oldId];
                }
            }
            catch
            {
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return null;
        }

        public static bool Init()
        {
            try
            {
                _configs = new Dictionary<string, PetConfig>();
                _levels = new Dictionary<int, PetLevel>();
                _skillElements = new Dictionary<int, PetSkillElementInfo>();
                _skills = new Dictionary<int, PetSkillInfo>();
                _skillTemplates = new Dictionary<int, PetSkillTemplateInfo>();
                _templateIds = new Dictionary<int, PetTemplateInfo>();
                _expItemPrices = new Dictionary<int, PetExpItemPriceInfo>();
                _petFightProp = new Dictionary<int, PetFightPropertyInfo>();
                _petStarExp = new Dictionary<int, PetStarExpInfo>();
                m_lock = new ReaderWriterLock();
                rand = new ThreadSafeRandom();
                return LoadPetMgr(_configs, _levels, _skillElements, _skills, _skillTemplates, _templateIds, _expItemPrices, _petFightProp, _petStarExp);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("PetInfoMgr", exception);
                }
                return false;
            }
        }

        private static bool LoadPetMgr(Dictionary<string, PetConfig> Config, Dictionary<int, PetLevel> Level, Dictionary<int, PetSkillElementInfo> SkillElement, Dictionary<int, PetSkillInfo> Skill, Dictionary<int, PetSkillTemplateInfo> SkillTemplate, Dictionary<int, PetTemplateInfo> TemplateId, Dictionary<int, PetExpItemPriceInfo> PetExpItemPrice, Dictionary<int, PetFightPropertyInfo> PetFightProp, Dictionary<int, PetStarExpInfo> PetStarExp)
        {
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                PetConfig[] allPetConfig = bussiness.GetAllPetConfig();
                PetLevel[] allPetLevel = bussiness.GetAllPetLevel();
                PetSkillElementInfo[] allPetSkillElementInfo = bussiness.GetAllPetSkillElementInfo();
                PetSkillInfo[] allPetSkillInfo = bussiness.GetAllPetSkillInfo();
                PetSkillTemplateInfo[] allPetSkillTemplateInfo = bussiness.GetAllPetSkillTemplateInfo();
                PetTemplateInfo[] allPetTemplateInfo = bussiness.GetAllPetTemplateInfo();
                PetExpItemPriceInfo[] allPetExpItemPriceInfoInfo = bussiness.GetAllPetExpItemPriceInfoInfo();
                PetFightPropertyInfo[] allPetFightProp = bussiness.GetAllPetFightProp();
                PetStarExpInfo[] allPetStarExp = bussiness.GetAllPetStarExp();
                foreach (PetExpItemPriceInfo info in allPetExpItemPriceInfoInfo)
                {
                    if (!PetExpItemPrice.ContainsKey(info.Count))
                    {
                        PetExpItemPrice.Add(info.Count, info);
                    }
                }
                foreach (PetConfig config in allPetConfig)
                {
                    if (!Config.ContainsKey(config.Name))
                    {
                        Config.Add(config.Name, config);
                    }
                }
                foreach (PetLevel level in allPetLevel)
                {
                    if (!Level.ContainsKey(level.Level))
                    {
                        Level.Add(level.Level, level);
                    }
                }
                foreach (PetSkillElementInfo info2 in allPetSkillElementInfo)
                {
                    if (!SkillElement.ContainsKey(info2.ID))
                    {
                        SkillElement.Add(info2.ID, info2);
                    }
                }
                foreach (PetSkillTemplateInfo info3 in allPetSkillTemplateInfo)
                {
                    if (!SkillTemplate.ContainsKey(info3.ID))
                    {
                        SkillTemplate.Add(info3.ID, info3);
                    }
                }
                foreach (PetTemplateInfo info4 in allPetTemplateInfo)
                {
                    if (!TemplateId.ContainsKey(info4.ID))
                    {
                        TemplateId.Add(info4.ID, info4);
                    }
                }
                foreach (PetSkillInfo info5 in allPetSkillInfo)
                {
                    if (!Skill.ContainsKey(info5.ID))
                    {
                        Skill.Add(info5.ID, info5);
                    }
                }
                foreach (PetFightPropertyInfo info6 in allPetFightProp)
                {
                    if (!PetFightProp.ContainsKey(info6.ID))
                    {
                        PetFightProp.Add(info6.ID, info6);
                    }
                }
                foreach (PetStarExpInfo info7 in allPetStarExp)
                {
                    if (!PetStarExp.ContainsKey(info7.OldID))
                    {
                        PetStarExp.Add(info7.OldID, info7);
                    }
                }
            }
            return true;
        }

        public static int OldTemplate(int TemplateID)
        {
            int templateID = TemplateID;
            PetTemplateInfo info = FindPetTemplate(templateID - 1);
            PetTemplateInfo info2 = FindPetTemplate(templateID - 2);
            if (info2 != null)
            {
                return info2.TemplateID;
            }
            if (info != null)
            {
                templateID = info.TemplateID;
            }
            return templateID;
        }

        public static void PlusPetProp(UsersPetinfo pet, int min, int max, ref int blood, ref int attack, ref int defence, ref int agility, ref int lucky)
        {
            PetConfig config = FindConfig("PropertiesRate");
            int num = (config == null) ? 100 : int.Parse(config.Value);
            double num2 = pet.BloodGrow / num;
            double num3 = pet.AttackGrow / num;
            double num4 = pet.DefenceGrow / num;
            double num5 = pet.AgilityGrow / num;
            double num6 = pet.LuckGrow / num;
            double num7 = pet.Blood;
            double num8 = pet.Attack;
            double num9 = pet.Defence;
            double num10 = pet.Agility;
            double luck = pet.Luck;
            for (int i = min + 1; i <= max; i++)
            {
                num7 += num2;
                num8 += num3;
                num9 += num4;
                num10 += num5;
                luck += num6;
            }
            blood = (int) num7;
            attack = (int) num8;
            defence = (int) num9;
            agility = (int) num10;
            lucky = (int) luck;
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<string, PetConfig> config = new Dictionary<string, PetConfig>();
                Dictionary<int, PetLevel> level = new Dictionary<int, PetLevel>();
                Dictionary<int, PetSkillElementInfo> skillElement = new Dictionary<int, PetSkillElementInfo>();
                Dictionary<int, PetSkillInfo> skill = new Dictionary<int, PetSkillInfo>();
                Dictionary<int, PetSkillTemplateInfo> skillTemplate = new Dictionary<int, PetSkillTemplateInfo>();
                new Dictionary<int, PetTemplateInfo>();
                Dictionary<int, PetTemplateInfo> templateId = new Dictionary<int, PetTemplateInfo>();
                Dictionary<int, PetExpItemPriceInfo> petExpItemPrice = new Dictionary<int, PetExpItemPriceInfo>();
                Dictionary<int, PetFightPropertyInfo> petFightProp = new Dictionary<int, PetFightPropertyInfo>();
                Dictionary<int, PetStarExpInfo> petStarExp = new Dictionary<int, PetStarExpInfo>();
                if (LoadPetMgr(config, level, skillElement, skill, skillTemplate, templateId, petExpItemPrice, petFightProp, petStarExp))
                {
                    m_lock.AcquireWriterLock(-1);
                    try
                    {
                        _configs = config;
                        _levels = level;
                        _skillElements = skillElement;
                        _skills = skill;
                        _skillTemplates = skillTemplate;
                        _templateIds = templateId;
                        _expItemPrices = petExpItemPrice;
                        _petFightProp = petFightProp;
                        _petStarExp = petStarExp;
                        return true;
                    }
                    catch
                    {
                    }
                    finally
                    {
                        m_lock.ReleaseWriterLock();
                    }
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("PetMgr", exception);
                }
            }
            return false;
        }

        public static int TemplateReset(int TemplateID)
        {
            int templateID = TemplateID;
            PetTemplateInfo info = FindPetTemplate(templateID - 1);
            PetTemplateInfo info2 = FindPetTemplate(templateID - 2);
            if (info != null)
            {
                return info.TemplateID;
            }
            if (info2 != null)
            {
                templateID = info2.TemplateID;
            }
            return templateID;
        }

        public static int UpdateEvolution(int TemplateID, int lv)
        {
            int templateID = TemplateID;
            int num2 = Convert.ToInt32(FindConfig("EvolutionLevel1").Value);
            int num3 = Convert.ToInt32(FindConfig("EvolutionLevel2").Value);
            FindPetTemplate(templateID);
            PetTemplateInfo info = FindPetTemplate(templateID + 1);
            PetTemplateInfo info2 = FindPetTemplate(templateID + 2);
            if ((info2 != null) && (info != null))
            {
                if ((lv >= num2) && (lv < num3))
                {
                    return info.TemplateID;
                }
                if (lv >= num3)
                {
                    templateID = info2.TemplateID;
                }
                return templateID;
            }
            if ((info != null) && (lv >= num3))
            {
                templateID = info.TemplateID;
            }
            return templateID;
        }

        public static string UpdateSkillPet(int Level, int TemplateID, int playerLevel)
        {
            PetTemplateInfo info = FindPetTemplate(TemplateID);
            if (info == null)
            {
                log.Error("Pet not found: " + TemplateID);
                return "";
            }
            List<int> list = GetPetSkillByKindID(info.KindID, Level, playerLevel);
            string str = list[0] + ",0";
            for (int i = 1; i < list.Count; i++)
            {
                object obj2 = str;
                str = string.Concat(new object[] { obj2, "|", list[i], ",", i });
            }
            return str;
        }
    }
}

