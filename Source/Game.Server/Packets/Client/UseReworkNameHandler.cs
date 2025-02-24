namespace Game.Server.Packets.Client
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameUtils;
    using SqlDataProvider.Data;
    using System;

    [PacketHandler(0xab, "场景用户离开")]
    public class UseReworkNameHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            byte num = packet.ReadByte();
            int slot = packet.ReadInt();
            string newNickName = packet.ReadString();
            string msg = "";
            PlayerInventory inventory = client.Player.GetInventory((eBageType) num);
            SqlDataProvider.Data.ItemInfo itemAt = inventory.GetItemAt(slot);
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                if (bussiness.RenameNick(client.Player.PlayerCharacter.UserName, client.Player.PlayerCharacter.NickName, newNickName))
                {
                    inventory.RemoveCountFromStack(itemAt, 1);
                }
                else
                {
                    msg = "Thay đổi Nickname thất bại.";
                }
            }
            if (msg != "")
            {
                client.Player.SendMessage(msg);
            }
            return 0;
        }
    }
}

