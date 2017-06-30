using System;
using System.IO;
using DarkMultiPlayerCommon;
using MessageStream2;

namespace DarkMultiPlayerServer.Messages
{
    public class VesselRemove
    {
        public static void HandleVesselRemoval(ClientObject client, byte[] messageData)
        {
            using (MessageReader mr = new MessageReader(messageData))
            {
                //Don't care about the subspace on the server.
                mr.Read<double>();
                string vesselID = mr.Read<string>();
                bool isDockingUpdate = mr.Read<bool>();
                if (File.Exists(Path.Combine(Server.universeDirectory, "Vessels", vesselID + ".txt")))
                {
                    string vesselName = File.ReadAllLines(Path.Combine(Server.universeDirectory, "Vessels", vesselID + ".txt"))[1].Remove(0, 7);
                    DarkLog.Normal("Removing" + (isDockingUpdate ? " DOCKED " : " ") + "vessel [" + vesselName + "] (" + vesselID + ") from " + client.playerName);
                    lock (Server.universeSizeLock)
                    {
                        File.Delete(Path.Combine(Server.universeDirectory, "Vessels", vesselID + ".txt"));
                    }
                }
                //Relay the message.
                ServerMessage newMessage = new ServerMessage();
                newMessage.type = ServerMessageType.VESSEL_REMOVE;
                newMessage.data = messageData;
                ClientHandler.SendToAll(client, newMessage, false);
            }
        }

        public static void HandleKerbalRemoval(ClientObject client, byte[] messageData)
        {
            using (MessageReader mr = new MessageReader(messageData))
            {
                //Don't care about the subspace on the server.
                mr.Read<double>();
                string kerbalName = mr.Read<string>();
                DarkLog.Normal("Removing kerbal " + kerbalName + " from " + client.playerName);
                if (File.Exists(Path.Combine(Server.universeDirectory, "Kerbals", kerbalName + ".txt")))
                {
                    lock (Server.universeSizeLock)
                    {
                        File.Delete(Path.Combine(Server.universeDirectory, "Kerbals", kerbalName + ".txt"));
                    }
                }
                //Relay the message.
                ServerMessage newMessage = new ServerMessage();
                newMessage.type = ServerMessageType.KERBAL_REMOVE;
                newMessage.data = messageData;
                ClientHandler.SendToAll(client, newMessage, false);
            }
        }
    }
}

