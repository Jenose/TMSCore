using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMSCore.Interfaces;
using TMSCore.Utilities;

namespace LoginServer.OuterNetwork
{
    public abstract class ASendPacket : ISendPacket
    {
        protected byte[] Data;
        protected object WriteLock = new object();

        /*public void Send(Player player)
        {
            Send(player.Connection);
        }*/

        /*public void Send(params Player[] players)
        {
            for (int i = 0; i < players.Length; i++)
                Send(players[i].Connection);
        }*/

        public void Send(params ISession[] states)
        {
            for (int i = 0; i < states.Length; i++)
                Send(states[i]);
        }

        public void Send(ISession state)
        {
            if (state == null || !state.IsValid)
                return;

            if (!NetworkOpcode.Send.ContainsKey(GetType()))
            {
                Log.Warn("UNKNOWN packet opcode: {0}", GetType().Name);
                return;
            }


            lock (WriteLock)
            {
                if (Data == null)
                {
                    try
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            using (BinaryWriter writer = new BinaryWriter(stream, new UTF8Encoding()))
                            {
                                WriteH(writer, 0); //Reserved for length
                                WriteH(writer, NetworkOpcode.Send[GetType()]);
                                Write(writer);
                            }

                            Data = stream.ToArray();
                            BitConverter.GetBytes((short)(Data.Length - 4)).CopyTo(Data, 0);

                            Console.WriteLine("-------------------------------------------------------------------");
                            Log.Debug("SEND {0}: Buffer({1})", GetType().Name, Data.Length);
                            Log.Debug("Data:\n{0}", Data.FormatHex());
                            Console.WriteLine("-------------------------------------------------------------------");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Warn("Can't write packet: {0}", GetType().Name);
                        Log.WarnException("ASendPacket", ex);
                        return;
                    }
                }
            }

            state.PushPacket(Data);
        }

        public abstract void Write(BinaryWriter writer);

        protected void WriteD(BinaryWriter writer, int val)
        {
            writer.Write(val);
        }

        protected void WriteH(BinaryWriter writer, short val)
        {
            writer.Write(val);
        }

        protected void WriteC(BinaryWriter writer, byte val)
        {
            writer.Write(val);
        }

        protected void WriteDf(BinaryWriter writer, double val)
        {
            writer.Write(val);
        }

        protected void WriteF(BinaryWriter writer, float val)
        {
            writer.Write(val);
        }

        protected void WriteQ(BinaryWriter writer, long val)
        {
            writer.Write(val);
        }

        protected void WriteS(BinaryWriter writer, String text)
        {
            Encoding encoding = Encoding.UTF8;
            writer.Write(encoding.GetBytes(text));
        }

        protected void WriteB(BinaryWriter writer, string hex)
        {
            writer.Write(hex.ToBytes());
        }

        protected void WriteB(BinaryWriter writer, byte[] data)
        {
            writer.Write(data);
        }
    }
}
