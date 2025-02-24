namespace Game.Server.SceneMarryRooms.TankHandle
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server.GameObjects;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using Game.Server.SceneMarryRooms;
    using log4net;
    using System;
    using System.Reflection;

    [MarryCommandAttbute(3)]
    public class ContinuationCommand : IMarryCommandHandler
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            int num2;
            int brideID;
            if (player.CurrentMarryRoom == null)
            {
                return false;
            }
            if ((player.PlayerCharacter.ID != player.CurrentMarryRoom.Info.GroomID) && (player.PlayerCharacter.ID != player.CurrentMarryRoom.Info.BrideID))
            {
                return false;
            }
            int time = packet.ReadInt();
            string[] strArray = GameProperties.PRICE_MARRY_ROOM.Split(new char[] { ',' });
            if (strArray.Length < 3)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("MarryRoomCreateMoney node in configuration file is wrong");
                }
                return false;
            }
            switch (time)
            {
                case 2:
                    num2 = int.Parse(strArray[0]);
                    break;

                case 3:
                    num2 = int.Parse(strArray[1]);
                    break;

                case 4:
                    num2 = int.Parse(strArray[2]);
                    break;

                default:
                    num2 = int.Parse(strArray[2]);
                    time = 4;
                    break;
            }
            if (player.PlayerCharacter.Money < num2)
            {
                player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryApplyHandler.Msg1", new object[0]));
                return false;
            }
            player.RemoveMoney(num2);
            CountBussiness.InsertSystemPayCount(player.PlayerCharacter.ID, num2, 0, 0, 0);
            player.CurrentMarryRoom.RoomContinuation(time);
            GSPacketIn @in = player.Out.SendContinuation(player, player.CurrentMarryRoom.Info);
            if (player.PlayerCharacter.ID == player.CurrentMarryRoom.Info.GroomID)
            {
                brideID = player.CurrentMarryRoom.Info.BrideID;
            }
            else
            {
                brideID = player.CurrentMarryRoom.Info.GroomID;
            }
            GamePlayer playerById = WorldMgr.GetPlayerById(brideID);
            if (playerById != null)
            {
                playerById.Out.SendTCP(@in);
            }
            player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ContinuationCommand.Successed", new object[0]));
            return true;
        }
    }
}

