using Hik.Communication.ScsServices.Service;

namespace InnerLib.Interfaces
{
    [ScsService]
    public interface IInnerService
    {
        void Auth(string key, int serverId);
        void OnlineCount(int count);
    }
}
