namespace Game.Server.Packets.Client
{
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.Quests;
    using System;

    [PacketHandler(0xb3, "任务完成")]
    public class QuestFinishHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int id = packet.ReadInt();
            int selectedItem = packet.ReadInt();
            BaseQuest baseQuest = client.Player.QuestInventory.FindQuest(id);
            bool flag = false;
            if (baseQuest != null)
            {
                flag = client.Player.QuestInventory.Finish(baseQuest, selectedItem);
            }
            if (flag)
            {
                GSPacketIn @in = new GSPacketIn(0xb3, client.Player.PlayerCharacter.ID);
                @in.WriteInt(id);
                client.Out.SendTCP(@in);
            }
            return 1;
        }
    }
}

