using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol.x.muc;
using BenebotV3.Properties;
using System.Security.Cryptography;

namespace BenebotV3
{
    public class LoLConnection : IConnection
    {
        private readonly XmppClientConnection _connection;

        private readonly string _resource;
        private const string Lolapp = "lolapp.me";

        private readonly string _room;
        private MucManager _chatroom;
        private Jid _chatJid;

        private Benebot _parent;

        public LoLConnection(Benebot parent)
        {
            _parent = parent;

            _room = Settings.Default.LoLRoom;
            _resource = Settings.Default.LoLResource;

            _connection = new XmppClientConnection
            {
                Username      = Settings.Default.LoLUsername,
                Password      = string.Format("AIR_{0}", Settings.Default.LoLPassword),
                Status        = Settings.Default.LoLStatus,
                UseSSL        = true,
                AutoRoster    = true,
                AutoPresence  = true,
                Server        = Settings.Default.LoLServer,
                ConnectServer = Settings.Default.LoLConnectServer,
                Port          = Settings.Default.LoLPort,
                Resource      = _resource
            };

            _connection.OnLogin    += OnLoggedIn;
            _connection.OnPresence += OnReceivePrescence;
            _connection.OnMessage  += OnMessageReceive;

            _connection.OnAuthError += (sender, element) => Console.WriteLine(element.ToString());
        }

        private void OnMessageReceive(object sender, Message msg)
        {
            if (string.IsNullOrEmpty(msg.From.Resource) || string.IsNullOrEmpty(msg.Body)) return;
            MessageReceived(msg.From.Resource, msg.Body);
        }

        private void OnReceivePrescence(object sender, Presence pres)
        {
            if (pres.From.Resource == null) return;
            if (RoomJoin(pres))
                PresenceReceived(pres.From.Resource, 1);
            else if(RoomLeave(pres))
                PresenceReceived(pres.From.Resource, 0);
        }

        private bool RoomJoin(Presence pres)
        {
            return pres.From.Resource != null &&
                   pres.Type.ToString().Equals("available") &&
                   !(pres.From.Resource.Equals(_resource) || pres.From.Resource.Equals(Lolapp));
        }

        private bool RoomLeave(Presence pres)
        {
            return pres.From.Resource != null &&
                   pres.Type.ToString().Equals("unavailable");
        }

        private void OnLoggedIn(object sender)
        {
            OnLogin();
        }

        private void JoinChatRoom()
        {
            _chatJid = new Jid(string.Format("pu~{0}@lvl.pvp.net", Hasher.Hash(_room)));
            _chatroom = new MucManager(_connection);
            _chatroom.AcceptDefaultConfiguration(_chatJid);
            _chatroom.JoinRoom(_chatJid, _connection.Username);
            _connection.SendMyPresence();
            SendMessage("Benebot V3 Alpha 8^)");
        }

        public bool Open()
        {
            _connection.Open();
            return true;
        }

        public bool Disconnect()
        {
            _connection.Close();
            return true;
        }

        public void OnLogin()
        {
            JoinChatRoom();
        }

        public void MessageReceived(string from, string message)
        {
            Action a = () => _parent.MessageReceived(from, message);
            Task t = new Task(a);
            t.Start();
        }

        public void PresenceReceived(string from, int status)
        {
            _parent.PresenceReceived(from, status);
        }

        public void OnError()
        {
            throw new NotImplementedException();
        }

        public void SendMessage(string s)
        {
            _connection.Send(new Message(_chatJid, _connection.MyJID, MessageType.groupchat, s));
        }
    }

    internal class Hasher
    {
        public static string Hash(string s)
        {
            var sha1 = SHA1.Create();
            var hashData = sha1.ComputeHash(Encoding.Default.GetBytes(s));
            var returnValue = new StringBuilder();
            
            foreach (var t in hashData)
                returnValue.Append(t.ToString("x2"));

            return returnValue.ToString();
        }
    }

}


