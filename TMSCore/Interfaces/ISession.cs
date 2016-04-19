namespace TMSCore.Interfaces
{
    public interface ISession
    {
        bool IsValid { get; }
        void Close();
        void PushPacket(byte[] data);
        long Ping();
    }
}
