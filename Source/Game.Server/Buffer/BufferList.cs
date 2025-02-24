namespace Game.Server.Buffer
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server.GameObjects;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Reflection;

    public class BufferList
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected List<AbstractBuffer> m_buffers;
        protected ArrayList m_clearList;
        private int m_changeCount;
        protected ArrayList m_changedBuffers = new ArrayList();
        protected volatile sbyte m_changesCount;
        private object m_lock;
        private GamePlayer m_player;

        public BufferList(GamePlayer player)
        {
            this.m_player = player;
            this.m_lock = new object();
            this.m_buffers = new List<AbstractBuffer>();
            this.m_clearList = new ArrayList();
        }

        public bool AddBuffer(AbstractBuffer buffer)
        {
            lock (this.m_buffers)
            {
                this.m_buffers.Add(buffer);
            }
            this.OnBuffersChanged(buffer);
            return true;
        }

        public void BeginChanges()
        {
            Interlocked.Increment(ref this.m_changeCount);
        }

        public void CommitChanges()
        {
            int num = Interlocked.Decrement(ref this.m_changeCount);
            if (num < 0)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Inventory changes counter is bellow zero (forgot to use BeginChanges?)!\n\n" + Environment.StackTrace);
                }
                Thread.VolatileWrite(ref this.m_changeCount, 0);
            }
            if ((num <= 0) && (this.m_changedBuffers.Count > 0))
            {
                this.UpdateChangedBuffers();
            }
        }

        public static AbstractBuffer CreateBuffer(BufferInfo info)
        {
            AbstractBuffer buffer = null;
            int type = info.Type;
            if (type <= 0x33)
            {
                switch (type)
                {
                    case 11:
                        return new KickProtectBuffer(info);

                    case 12:
                        return new OfferMultipleBuffer(info);

                    case 13:
                        return new GPMultipleBuffer(info);

                    case 14:
                        return buffer;

                    case 15:
                        return new PropsBuffer(info);

                    case 0x1a:
                        return new HonorBuffer(info);

                    case 0x33:
                        buffer = new SaveLifeBuffer(info);
                        break;
                }
                return buffer;
            }
            switch (type)
            {
                case 0x4a:
                    return new DefendBuffer(info);

                case 0x4b:
                    return new AttackBuffer(info);

                case 0x4c:
                    return new GuardBuffer(info);

                case 0x4d:
                    return new AgiBuffer(info);

                case 0x4e:
                    return new DameBuffer(info);

                case 0x4f:
                    return new HpBuffer(info);

                case 80:
                    return new LuckBuffer(info);

                case 0x65:
                    return new ConsortionAddBloodGunCountBuffer(info);

                case 0x66:
                    return new ConsortionAddDamageBuffer(info);

                case 0x67:
                    return new ConsortionAddCriticalBuffer(info);

                case 0x68:
                    return new ConsortionAddMaxBloodBuffer(info);

                case 0x69:
                    return new ConsortionAddPropertyBuffer(info);

                case 0x6a:
                    return new ConsortionReduceEnergyUseBuffer(info);

                case 0x6b:
                    return new ConsortionAddEnergyBuffer(info);

                case 0x6c:
                    return new ConsortionAddEffectTurnBuffer(info);

                case 0x6d:
                    return new ConsortionAddOfferRateBuffer(info);

                case 110:
                    return new ConsortionAddPercentGoldOrGPBuffer(info);

                case 0x6f:
                    return new ConsortionAddSpellCountBuffer(info);

                case 0x70:
                    return new ConsortionReduceDanderBuffer(info);

                case 0x71:
                    return buffer;

                case 0x72:
                    return new ActivityDungeonBubbleBuffer(info);

                case 0x73:
                    return new ActivityDungeonNetBuffer(info);

                case 400:
                    return new WorldBossHPBuffer(info);

                case 0x191:
                    return new WorldBossAttrackBuffer(info);

                case 0x192:
                    return new WorldBossHP_MoneyBuffBuffer(info);

                case 0x193:
                    return new WorldBossAttrack_MoneyBuffBuffer(info);

                case 0x194:
                    return new WorldBossMetalSlugBuffer(info);

                case 0x195:
                    return new WorldBossAncientBlessingsBuffer(info);

                case 0x196:
                    return new WorldBossAddDamageBuffer(info);
            }
            return buffer;
        }

        public static AbstractBuffer CreateBuffer(ItemTemplateInfo template, int ValidDate)
        {
            BufferInfo info = new BufferInfo {
                TemplateID = template.TemplateID,
                BeginDate = DateTime.Now,
                ValidDate = (ValidDate * 0x18) * 60,
                Value = template.Property2,
                Type = template.Property1,
                ValidCount = 1,
                IsExist = true
            };
            return CreateBuffer(info);
        }

        public static AbstractBuffer CreateBufferHour(ItemTemplateInfo template, int ValidHour)
        {
            BufferInfo info = new BufferInfo {
                TemplateID = template.TemplateID,
                BeginDate = DateTime.Now,
                ValidDate = ValidHour * 60,
                Value = template.Property2,
                Type = template.Property1,
                ValidCount = 1,
                IsExist = true
            };
            return CreateBuffer(info);
        }

        public static AbstractBuffer CreateBufferMinutes(ItemTemplateInfo template, int ValidMinutes)
        {
            BufferInfo info = new BufferInfo {
                TemplateID = template.TemplateID,
                BeginDate = DateTime.Now,
                ValidDate = ValidMinutes,
                Value = template.Property2,
                Type = template.Property1,
                ValidCount = 1,
                IsExist = true
            };
            return CreateBuffer(info);
        }

        public static AbstractBuffer CreateConsortiaBuffer(ConsortiaBufferInfo info)
        {
            BufferInfo info2 = new BufferInfo {
                TemplateID = info.BufferID,
                BeginDate = info.BeginDate,
                ValidDate = info.ValidDate,
                Value = info.Value,
                Type = info.Type,
                ValidCount = 1,
                IsExist = true
            };
            return CreateBuffer(info2);
        }

        public static AbstractBuffer CreatePayBuffer(int type, int Value, int ValidMinutes)
        {
            BufferInfo info = new BufferInfo {
                TemplateID = 0,
                BeginDate = DateTime.Now,
                ValidDate = ValidMinutes,
                Value = Value,
                Type = type,
                ValidCount = 1,
                IsExist = true
            };
            return CreateBuffer(info);
        }

        public static AbstractBuffer CreatePayBuffer(int type, int Value, int ValidMinutes, int id)
        {
            BufferInfo info = new BufferInfo {
                TemplateID = id,
                BeginDate = DateTime.Now,
                ValidDate = ValidMinutes,
                Value = Value,
                Type = type,
                ValidCount = 1,
                IsExist = true
            };
            return CreateBuffer(info);
        }

        public static AbstractBuffer CreateSaveLifeBuffer(int ValidCount)
        {
            BufferInfo info = new BufferInfo {
                TemplateID = 0x2e8f,
                BeginDate = DateTime.Now,
                ValidDate = 0x5a0,
                Value = 30,
                Type = 0x33,
                ValidCount = ValidCount,
                IsExist = true
            };
            return CreateBuffer(info);
        }

        public List<AbstractBuffer> GetAllBufferByTemplate()
        {
            List<AbstractBuffer> list = new List<AbstractBuffer>();
            lock (this.m_lock)
            {
                foreach (AbstractBuffer buffer in this.m_buffers)
                {
                    if (buffer.Info.TemplateID > 100)
                    {
                        list.Add(buffer);
                    }
                }
            }
            return list;
        }

        public List<AbstractBuffer> GetAllBuffers()
        {
            List<AbstractBuffer> list = new List<AbstractBuffer>();
            lock (this.m_lock)
            {
                foreach (AbstractBuffer buffer in this.m_buffers)
                {
                    list.Add(buffer);
                }
            }
            return list;
        }

        public virtual AbstractBuffer GetOfType(Type bufferType)
        {
            lock (this.m_buffers)
            {
                foreach (AbstractBuffer buffer in this.m_buffers)
                {
                    if (buffer.GetType().Equals(bufferType))
                    {
                        return buffer;
                    }
                }
            }
            return null;
        }

        public bool IsConsortiaBuff(int type)
        {
            return ((type > 100) && (type < 0x73));
        }

        public void LoadFromDatabase(int playerId)
        {
            lock (this.m_lock)
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    BufferInfo[] userBuffer = bussiness.GetUserBuffer(playerId);
                    this.BeginChanges();
                    foreach (BufferInfo info in userBuffer)
                    {
                        AbstractBuffer buffer = CreateBuffer(info);
                        if (buffer != null)
                        {
                            buffer.Start(this.m_player);
                        }
                    }
                    foreach (ConsortiaBufferInfo info2 in bussiness.GetUserConsortiaBuffer(this.m_player.PlayerCharacter.ConsortiaID))
                    {
                        AbstractBuffer buffer2 = CreateConsortiaBuffer(info2);
                        if (buffer2 != null)
                        {
                            buffer2.Start(this.m_player);
                        }
                    }
                    this.CommitChanges();
                }
                this.Update();
                this.m_player.ClearFightBuffOneMatch();
            }
        }

        protected void OnBuffersChanged(AbstractBuffer buffer)
        {
            if (!this.m_changedBuffers.Contains(buffer))
            {
                this.m_changedBuffers.Add(buffer);
            }
            if ((this.m_changeCount <= 0) && (this.m_changedBuffers.Count > 0))
            {
                this.UpdateChangedBuffers();
            }
        }

        public bool RemoveBuffer(AbstractBuffer buffer)
        {
            lock (this.m_buffers)
            {
                if (this.m_buffers.Remove(buffer))
                {
                    this.m_clearList.Add(buffer.Info);
                }
            }
            this.OnBuffersChanged(buffer);
            return true;
        }

        public void SaveToDatabase()
        {
            lock (this.m_lock)
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    foreach (AbstractBuffer buffer in this.m_buffers)
                    {
                        bussiness.SaveBuffer(buffer.Info);
                    }
                    foreach (BufferInfo info in this.m_clearList)
                    {
                        bussiness.SaveBuffer(info);
                    }
                    this.m_clearList.Clear();
                }
            }
        }

        public void Update()
        {
            List<AbstractBuffer> allBuffers = this.GetAllBuffers();
            foreach (AbstractBuffer buffer in allBuffers)
            {
                try
                {
                    if (!buffer.Check())
                    {
                        buffer.Stop();
                    }
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                }
            }
        }

        public void UpdateBuffer(AbstractBuffer buffer)
        {
            this.OnBuffersChanged(buffer);
        }

        public void UpdateChangedBuffers()
        {
            List<BufferInfo> list = new List<BufferInfo>();
            Dictionary<int, BufferInfo> bufflist = new Dictionary<int, BufferInfo>();
            foreach (AbstractBuffer buffer in this.m_changedBuffers)
            {
                if (buffer.Info.TemplateID > 100)
                {
                    list.Add(buffer.Info);
                }
            }
            List<AbstractBuffer> allBuffers = this.GetAllBuffers();
            foreach (AbstractBuffer buffer2 in allBuffers)
            {
                if (this.IsConsortiaBuff(buffer2.Info.Type) && this.m_player.IsConsortia())
                {
                    bufflist.Add(buffer2.Info.TemplateID, buffer2.Info);
                }
            }
            BufferInfo[] infos = list.ToArray();
            GSPacketIn pkg = this.m_player.Out.SendUpdateBuffer(this.m_player, infos);
            if (this.m_player.CurrentRoom != null)
            {
                this.m_player.CurrentRoom.SendToAll(pkg, this.m_player);
            }
            this.m_player.Out.SendUpdateConsotiaBuffer(this.m_player, bufflist);
            this.m_changedBuffers.Clear();
            bufflist.Clear();
        }

        public bool UserSaveLifeBuff()
        {
            lock (this.m_buffers)
            {
                for (int i = 0; i < this.m_buffers.Count; i++)
                {
                    if ((this.m_buffers[i].Info.Type == 0x33) && (this.m_buffers[i].Info.ValidCount > 0))
                    {
                        BufferInfo info = this.m_buffers[i].Info;
                        info.ValidCount--;
                        this.OnBuffersChanged(this.m_buffers[i]);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

