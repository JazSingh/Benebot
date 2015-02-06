using System;
using System.Collections.Generic;

namespace BenebotV3
{
    public interface IConnection
    {
        bool Open();
        bool Disconnect();
        void OnLogin();
        void MessageReceived(string from, string message);
        void PresenceReceived(string from, int status);
        void OnError();
        void SendMessage(string s);
    }
}

