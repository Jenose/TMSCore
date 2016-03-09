using InnerLib.Interfaces;
using System;

namespace GameServer.InnerNetwork
{
    public class InnerClient : IInnerClient
    {
        public event Action OnAuthed;

        public void Authed()
        {
            if (OnAuthed != null) OnAuthed();
        }
    }
}
