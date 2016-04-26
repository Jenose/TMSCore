﻿using System;
using System.IO;
using System.Text;
using TMSCore.Interfaces;
using TMSCore.Utilities;

namespace LoginServer.OuterNetwork
{
    public abstract class ARecvPacket : IRecvPacket
    {
        public BinaryReader Reader;
        public NetworkSession Session;

        public void Process(NetworkSession session)
        {
            Session = session;

            using (Reader = new BinaryReader(new MemoryStream(Session.Buffer)))
            {
                Read();
            }

            try
            {
                Process();
            }
            catch (Exception ex)
            {
                Log.WarnException("ARecvPacket", ex);
            }
        }

        public abstract void Read();

        public abstract void Process();

        protected int ReadD()
        {
            try
            {
                return Reader.ReadInt32();
            }
            catch (Exception)
            {
                Log.Warn("Missing D for: {0}", GetType());
            }
            return 0;
        }

        protected int ReadC()
        {
            try
            {
                return Reader.ReadByte() & 0xFF;
            }
            catch (Exception)
            {
                Log.Warn("Missing C for: {0}", GetType());
            }
            return 0;
        }

        protected int ReadH()
        {
            try
            {
                return Reader.ReadInt16() & 0xFFFF;
            }
            catch (Exception)
            {
                Log.Warn("Missing H for: {0}", GetType());
            }
            return 0;
        }

        protected double ReadDf()
        {
            try
            {
                return Reader.ReadDouble();
            }
            catch (Exception)
            {
                Log.Warn("Missing DF for: {0}", GetType());
            }
            return 0;
        }

        protected float ReadF()
        {
            try
            {
                return Reader.ReadSingle();
            }
            catch (Exception)
            {
                Log.Warn("Missing F for: {0}", GetType());
            }
            return 0;
        }

        protected long ReadQ()
        {
            try
            {
                return Reader.ReadInt64();
            }
            catch (Exception)
            {
                Log.Warn("Missing Q for: {0}", GetType());
            }
            return 0;
        }

        protected String ReadS(int len)
        {
            Encoding encoding = Encoding.UTF8;
            String result = String.Empty;
            try
            {
                result = encoding.GetString(ReadB(len));
            }
            catch (Exception)
            {
                Log.Warn("Missing S for: {0}", GetType());
            }
            return result;
        }

        protected String ReadS()
        {
            Encoding encoding = Encoding.UTF8;
            String result = String.Empty;
            try
            {
                int len = ReadH();
                result = encoding.GetString(ReadB(len));
            }
            catch (Exception)
            {
                Log.Warn("Missing S for: {0}", GetType());
            }
            return result;
        }

        protected byte[] ReadB(int length)
        {
            byte[] result = new byte[length];
            try
            {
                Reader.Read(result, 0, length);
            }
            catch (Exception)
            {
                Log.Warn("Missing byte[] for: {0}", GetType());
            }
            return result;
        }
    }
}