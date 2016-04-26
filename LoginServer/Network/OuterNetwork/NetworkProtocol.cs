using Crypto;
using Hik.Communication.Scs.Communication.Messages;
using Hik.Communication.Scs.Communication.Protocols;
using System;
using System.Collections.Generic;
using System.IO;
using TMSCore.Utilities;

namespace LoginServer.OuterNetwork
{
    public class NetworkProtocol : IScsWireProtocol
    {
        /// <summary>
        /// 
        /// </summary>
        NetworkSession Session;

        /// <summary>
        /// 
        /// </summary>
        protected MemoryStream Stream = new MemoryStream();

        public NetworkProtocol(NetworkSession session)
        {
            Session = session;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="receivedBytes"></param>
        /// <returns></returns>
        public IEnumerable<IScsMessage> CreateMessages(byte[] receivedBytes)
        {
            //Log.Debug("RAW DECRYPT: {0}{1}", Environment.NewLine, receivedBytes.FormatHex());

            byte[] data = new byte[receivedBytes.Length - 4];
            Buffer.BlockCopy(receivedBytes, 4, data, 0, data.Length);
            Session.m_ClientCrypt.Decrypt(data);

            Stream.Write(data, 0, data.Length);
            List<IScsMessage> messages = new List<IScsMessage>();
            while (ReadMessage(messages)) ;

            return messages;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        private bool ReadMessage(List<IScsMessage> messages)
        {
            Stream.Position = 0;

            if (Stream.Length < 2)
                return false;

            byte[] header = new byte[2];
            Stream.Read(header, 0, 2);

            ushort opcode = BitConverter.ToUInt16(header, 0);

            NetworkMessage message = new NetworkMessage
            {
                OpCode = (short)opcode,
                Data = new byte[Stream.Length - 2]
            };

            Stream.Read(message.Data, 0, (int)Stream.Length - 2);

            messages.Add(message);

            TrimStream();

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void TrimStream()
        {
            if (Stream.Position == Stream.Length)
            {
                Stream = new MemoryStream();
                return;
            }

            byte[] remaining = new byte[Stream.Length - Stream.Position];
            Stream.Read(remaining, 0, remaining.Length);
            Stream = new MemoryStream();
            Stream.Write(remaining, 0, remaining.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public byte[] GetBytes(IScsMessage message)
        {
            return ((NetworkMessage)message).Data;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
        }
    }
}
