using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BenebotV3
{
    public class Benebot
    {

        private List<AbstractCommands> _commands;
        private UserManager _userManager;
        public IConnection Connection;

        public Benebot()
        {
            _userManager = new UserManager(this);

            Console.WriteLine("Loading Commands...");
            _commands = new List<AbstractCommands>
            {
                new DynamicCommands(),
                new RandomizedCommands(),
                new StaticCommands(),
                new UserManagingCommands(_userManager)
            };
            foreach (var abstractCommands in _commands)
                abstractCommands.CreateCommands();
            Console.WriteLine("Commands Loaded!" + Environment.NewLine);
        }

        private void Reload()
        {
            var room = _userManager.InRoom;
            _userManager = new UserManager(this);
            _userManager.InRoom = room;
            _commands = new List<AbstractCommands>
            {
                new DynamicCommands(),
                new RandomizedCommands(),
                new StaticCommands(),
                new UserManagingCommands(_userManager)
            };
            foreach (var abstractCommands in _commands)
                abstractCommands.CreateCommands();
            Console.WriteLine("Reloaded!");
        }


        public void Start()
        {
            Connection.Open();
        }

        public void SendMessage(string s)
        {
            Connection.SendMessage(s);
        }

        public void MessageReceived(string from, string message)
        {
            if (message[0] != '!' || from.Equals("BeNeBot")) return;
            Console.WriteLine("[{2}] {0}: {1}", from, message, DateTime.Now.ToString("t"));

            string s = "";

            if (message.Split()[0].Equals("!register"))
            {
                Connection.SendMessage(_commands[3].GetResponse(message, new Summoner("0", from, "normal")));
                return;
            }

            if (!_userManager.IsRegistered(from)) { Connection.SendMessage(string.Format("{0} use !register to register or update.", from));}
            if (_userManager.IsBlackListed(from)) return;

            var h = string.Empty;
            if (_userManager.GetUser(from) != null && message.Equals("!help"))
            {
                h = _commands.Aggregate(h, (current1, abstractCommandse) => abstractCommandse.Commands.Where(abstractCommand => abstractCommandse.HasRights(abstractCommand.Value.AuthRank, _userManager.GetUser(@from))).Aggregate(current1, (current, abstractCommand) => current + (current.Equals(string.Empty) ? abstractCommand.Key : string.Format(", {0}", abstractCommand.Key))));
                Connection.SendMessage(h);
                return;
            }

            if (message.Equals("!reload") &&
                _commands[3].HasRights(_commands[3].Commands["!reload"].AuthRank, _userManager.GetUser(from)))
            {
                Reload();
                return;
            }

            foreach (var command in _commands)
            {
                s = command.GetResponse(message, _userManager.GetUser(from));
                if (!string.IsNullOrEmpty(s)) break;
            }

            if(!string.IsNullOrEmpty(s)) Connection.SendMessage(s);

        }

        public void PresenceReceived(string from, int status)
        {
            switch (status)
            {
                case 1:
                    _userManager.JoinedRoom(from); 
                    break;
                case 0:
                    _userManager.LeftRoom(from);
                    break;
            }
        }
    }
}

