﻿namespace Game.Server.SceneMarryRooms.TankHandle
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameObjects;
    using Game.Server.Packets;
    using Game.Server.SceneMarryRooms;
    using System;

    [MarryCommandAttbute(11)]
    public class GunsaluteCommand : IMarryCommandHandler
    {
        public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            if (player.CurrentMarryRoom != null)
            {
                packet.ReadInt();
                if (((ItemMgr.FindItemTemplate(packet.ReadInt()) != null) && !player.CurrentMarryRoom.Info.IsGunsaluteUsed) && ((player.CurrentMarryRoom.Info.GroomID == player.PlayerCharacter.ID) || (player.CurrentMarryRoom.Info.BrideID == player.PlayerCharacter.ID)))
                {
                    player.CurrentMarryRoom.ReturnPacketForScene(player, packet);
                    player.CurrentMarryRoom.Info.IsGunsaluteUsed = true;
                    GSPacketIn @in = player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("GunsaluteCommand.Successed1", new object[] { player.PlayerCharacter.NickName }));
                    player.CurrentMarryRoom.SendToPlayerExceptSelfForScene(@in, player);
                    GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(player.CurrentMarryRoom.Info.GroomID, true, player.CurrentMarryRoom.Info);
                    GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(player.CurrentMarryRoom.Info.BrideID, true, player.CurrentMarryRoom.Info);
                    using (PlayerBussiness bussiness = new PlayerBussiness())
                    {
                        bussiness.UpdateMarryRoomInfo(player.CurrentMarryRoom.Info);
                    }
                    return true;
                }
            }
            return false;
        }
    }
}

