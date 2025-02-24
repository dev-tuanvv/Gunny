namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Server;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0xb0, "添加任务")]
    public class QuestAddHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int num = packet.ReadInt();
            PlayerBussiness bussiness = new PlayerBussiness();
            for (int i = 0; i < num; i++)
            {
                string str;
                QuestInfo singleQuest = QuestMgr.GetSingleQuest(packet.ReadInt());
                QuestDataInfo userQuestSiger = bussiness.GetUserQuestSiger(client.Player.PlayerCharacter.ID, singleQuest.ID);
                if ((userQuestSiger != null) && ((userQuestSiger.RepeatFinish <= 1) && (singleQuest.ID == 7)))
                {
                    return 0;
                }
                client.Player.QuestInventory.AddQuest(singleQuest, out str);
            }
            return 0;
        }
    }
}

