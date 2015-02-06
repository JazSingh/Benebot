using System;
using IrcDotNet;

namespace BenebotV3.Connection
{
// ReSharper disable once InconsistentNaming
    class IRCConnection : IConnection
    {
        private IrcClient irc;
        public bool Open()
        {
            throw new NotImplementedException();
        }

        public bool Disconnect()
        {
            throw new NotImplementedException();
        }

        public void OnLogin()
        {
            throw new NotImplementedException();
        }

        public void MessageReceived(string @from, string message)
        {
            throw new NotImplementedException();
        }

        public void PresenceReceived(string @from, int status)
        {
            throw new NotImplementedException();
        }

        public void OnError()
        {
            throw new NotImplementedException();
        }

        public void SendMessage(string s)
        {
            throw new NotImplementedException();
        }
    }
}
