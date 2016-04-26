using Crypto;
using Hik.Communication.Scs.Communication;
using Hik.Communication.Scs.Communication.Messages;
using Hik.Communication.Scs.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using TMSCore.Interfaces;
using TMSCore.Utilities;

namespace LoginServer.OuterNetwork
{
    public class NetworkSession : ISession
    {
        /// <summary>
        /// 
        /// </summary>
        public static List<NetworkSession> Sessions = new List<NetworkSession>();

        /// <summary>
        /// 
        /// </summary>
        public static Thread SendAllThread = new Thread(SendAll);

        /// <summary>
        /// 
        /// </summary>
        protected static void SendAll()
        {
            while (true)
            {
                for (int i = 0; i < Sessions.Count; i++)
                {
                    try
                    {
                        if (!Sessions[i].Send())
                            Sessions.RemoveAt(i--);
                    }
                    catch (Exception ex)
                    {
                        Log.ErrorException("Connection: SendAll:", ex);
                    }
                }

                Thread.Sleep(10);
            }
            // ReSharper disable FunctionNeverReturns
        }

        /// <summary>
        /// 
        /// </summary>
        protected IScsServerClient Client;

        /// <summary>
        /// 
        /// </summary>
        public byte[] Buffer;

        /// <summary>
        /// 
        /// </summary>
        protected List<byte[]> SendData = new List<byte[]>();

        /// <summary>
        /// 
        /// </summary>
        protected int SendDataSize;

        /// <summary>
        /// 
        /// </summary>
        protected object SendLock = new object();

        /// <summary>
        /// 
        /// </summary>
        public AESCrypt m_ClientCrypt;

        /// <summary>
        /// 
        /// </summary>
        public AESCrypt m_ServerCrypt;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        public NetworkSession(IScsServerClient client)
        {
            Client = client;
            Client.WireProtocol = new NetworkProtocol(this);

            Client.Disconnected += OnDisconnected;
            Client.MessageReceived += OnMessageReceived;

            //new PROTOCOL_BASE_CONNECT_ACK(this).Send(this);
            byte[] hello_buff = new byte[15 + ServerConstants.MAPLE_PATCH.Length];
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream, new UTF8Encoding()))
            {
                short len = (short)(13 + ServerConstants.MAPLE_PATCH.Length);
                byte[] RIV = new byte[4]; // Server Recieve - from client
                byte[] SIV = new byte[4]; // Server Send - to client
                Funcs.Random().NextBytes(RIV);
                Funcs.Random().NextBytes(SIV);

                writer.Write(len);
                writer.Write(ServerConstants.MAPLE_VERSION);
                writer.Write((short)ServerConstants.MAPLE_PATCH.Length);
                writer.Write(Encoding.ASCII.GetBytes(ServerConstants.MAPLE_PATCH));
                writer.Write(RIV);
                writer.Write(SIV);
                writer.Write((byte)7);

                hello_buff = stream.ToArray();
                m_ClientCrypt = new AESCrypt(RIV, ServerConstants.MAPLE_VERSION);
                m_ServerCrypt = new AESCrypt(SIV, ServerConstants.MAPLE_VERSION);
            }

            Log.Debug("SEND HELLO:{0}{1}", Environment.NewLine, hello_buff.FormatHex());
            Client.SendMessage(new NetworkMessage { Data = hello_buff });

            Sessions.Add(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisconnected(object sender, EventArgs e)
        {
            Buffer = null;
            Client = null;
            SendData = null;
            SendLock = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool Send()
        {
            NetworkMessage message;

            if (SendLock == null)
                return false;

            lock (SendLock)
            {
                if (SendData.Count == 0)
                    return Client.CommunicationState == CommunicationStates.Connected;

                message = new NetworkMessage { Data = new byte[SendDataSize] };

                int pointer = 0;
                for (int i = 0; i < SendData.Count; i++)
                {
                    Array.Copy(SendData[i], 0, message.Data, pointer, SendData[i].Length);
                    pointer += SendData[i].Length;
                }

                SendData.Clear();
                SendDataSize = 0;
            }

            try
            {
                Client.SendMessage(message);
            }
            catch
            {
                //Already closed
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMessageReceived(object sender, MessageEventArgs e)
        {
            NetworkMessage message = (NetworkMessage)e.Message;
            Buffer = message.Data;

            if (NetworkOpcode.Recv.ContainsKey(message.OpCode))
            {
                //GlobalLogic.PacketReceived(Account, OpCodes.Recv[message.OpCode], Buffer);
                Console.WriteLine("-------------------------------------------------------------------");
                Log.Debug("Recieve {0}: Buffer({1})", NetworkOpcode.Recv[message.OpCode].Name, Buffer.Length);
                Log.Debug("Data:{0}{1}", Environment.NewLine, Buffer.FormatHex());
                Console.WriteLine("-------------------------------------------------------------------");

                ((ARecvPacket)Activator.CreateInstance(NetworkOpcode.Recv[message.OpCode])).Process(this);
            }
            else
            {
                //GlobalLogic.PacketReceived(Account, null, Buffer);

                string opCodeLittleEndianHex = BitConverter.GetBytes(message.OpCode).ToHex();
                Console.WriteLine("-------------------------------------------------------------------");
                Log.Debug("Unknown Packet Opcode: 0x{0}{1}({2}) [{3}]",
                                 opCodeLittleEndianHex.Substring(2),
                                 opCodeLittleEndianHex.Substring(0, 2),
                                 message.OpCode,
                                 Buffer.Length);

                Log.Debug("Data:\r\n{0}", Buffer.FormatHex());
                Console.WriteLine("-------------------------------------------------------------------");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsValid
        {
            get { return true; }
        }

        public void Close()
        {
            Client.Disconnect();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long Ping()
        {
            Ping ping = new Ping();

            //"tcp://127.0.0.1:27230"
            string ipAddress = Client.RemoteEndPoint.ToString().Substring(6);
            ipAddress = ipAddress.Substring(0, ipAddress.IndexOf(':'));

            PingReply pingReply = ping.Send(ipAddress);

            return (pingReply != null) ? pingReply.RoundtripTime : 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void PushPacket(byte[] data)
        {
            //Already closed
            if (SendLock == null)
                return;

            lock (SendLock)
            {
                short opCode = BitConverter.ToInt16(data, 2);
                /*GlobalLogic.PacketSent(Account,
                                       OpCodes.SendNames.ContainsKey(opCode)
                                           ? OpCodes.SendNames[opCode]
                                           : "unk",
                                       data);*/

                m_ServerCrypt.Encrypt(data);
                SendData.Add(data);
                SendDataSize += data.Length;
            }
        }
    }
}
