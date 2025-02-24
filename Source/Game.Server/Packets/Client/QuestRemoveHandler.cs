namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Quests;
    using System;

    [PacketHandler(0xb1, "删除任务")]
    public class QuestRemoveHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int id = packet.ReadInt();
            BaseQuest quest = client.Player.QuestInventory.FindQuest(id);
            if (quest != null)
            {
                client.Player.QuestInventory.RemoveQuest(quest);
            }
            return 0;
        }
    }
}

