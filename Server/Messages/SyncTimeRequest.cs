using System;
using DarkMultiPlayerCommon;
using MessageStream2;

namespace DarkMultiPlayerServer.Messages
{
    public class SyncTimeRequest
    {
        public static void HandleSyncTimeRequest(ClientObject client, byte[] messageData)
        {
            ServerMessage newMessage = new ServerMessage();
            newMessage.type = ServerMessageType.SYNC_TIME_REPLY;
            using (MessageWriter mw = new MessageWriter())
            {
                using (MessageReader mr = new MessageReader(messageData))
                {
                    //Client send and universe time
                    long clientSendTime = mr.Read<long>();
                    // Check if the client is requesting a time in the past (= revert)
                    {
                        double clientUniverseTime = double.PositiveInfinity;
                        try // For compatibility with protocol 45
                        {
                            clientUniverseTime = mr.Read<double>();
                        }
                        catch { }
                        finally
                        {

                            if (clientUniverseTime < client.lastSubspaceTime)
                            {
                                DarkLog.Normal("Detected revert by client " + client.playerName);
                            }
                        }
                    }
                    mw.Write<long>(clientSendTime);
                    //Server receive time
                    mw.Write<long>(DateTime.UtcNow.Ticks);
                    newMessage.data = mw.GetMessageBytes();
                }
            }
            ClientHandler.SendToClient(client, newMessage, true);
        }
    }
}

