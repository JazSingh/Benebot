using BenebotV3.Properties;
using System;
using System.IO;

namespace BenebotV3
{
    public class UserManagingCommands : AbstractCommands
    {
        private UserManager _manager;
        private static Random r = new Random(new Guid().GetHashCode());

        public UserManagingCommands(UserManager userManager)
        {
            _manager = userManager;
        }

        public override string GetResponse(string c, Summoner s)
        {
            var parts = c.Split();
            if (!Commands.ContainsKey(parts[0])) return "";
            if (!HasRights(Commands[parts[0]].AuthRank, s.Rank)) return "";

            var args = string.Empty;
            for (var i = 1; i < parts.Length; i++)
                args += string.Format("{0} ", parts[i]);

            args = args.Trim();
            switch (parts[0])
            {
                case "!ban":
                    return  Ban(args);
                case "!unban":
                    return UnBan(args);
                case "!register":
                    return Register(s.Name);
                case "!sukkel":
                    return string.Format("{0} is een sukkel", RandomFromRoom());
                case "!setrank":
                    return SetRank(args);
                case "!beasts3sukkelsoftheday":
                    return string.Format("{0}, {1} en {2} zijn sukkels!", RandomFromRoom(), RandomFromRoom(), RandomFromRoom());
                default:
                    return "";
            }
        }

        private string SetRank(string args)
        {
            var parts = args.Split();
            var user = string.Empty;
            for (int i = 0; i < parts.Length - 1; i++)
                user += parts[i];
            return _manager.SetRank(user, parts[parts.Length-1]);
        }

        private string Ban(string user)
        {
            return _manager.Ban(user);
        }

        private string UnBan(string user)
        {
            return _manager.UnBan(user);
        }

        private string Register(string newUser)
        {
            return _manager.Register(newUser);
        }

        public string RandomFromRoom()
        {
            var max = _manager.InRoom.Count;
            return _manager.InRoom[r.Next(0, max)];
        }

        protected override string[] GetContents()
        {
            return File.ReadAllLines(Settings.Default.UsermanCom);
        }

        protected override AbstractCommand GetCommand()
        {
            return new SingleResponseCommand();
        }
    }
}


