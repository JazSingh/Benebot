﻿using System.Collections.Generic;
using System.Linq;

namespace BenebotV3
{
    public abstract class AbstractCommands
    {
        public Dictionary<string, AbstractCommand> Commands { get; set; }

        public virtual string GetResponse(string c, Summoner s)
        {
            var parts = c.Split();
            if (!Commands.ContainsKey(parts[0])) return "";

            var args = string.Empty;
            for (var i = 1; i < parts.Length; i++)
                args += string.Format("{0} ", parts[i]);
            var com = Commands[parts[0]];
            return HasRights(com.AuthRank, s) ? com.GetResponse(args.TrimEnd()) : "";
        }

        public bool HasRights(string required, Summoner has)
        {
            if (required.Equals(has.Rank) || required.Equals(has.Name)) return true;
            var authlevels = new List<string> {"admin", "mod", "normal"};
            if (authlevels.Contains(required) && required.Equals(has.Rank)) return true;
            if (required.Equals("mod") && has.Rank.Equals("admin")) return true;
            return required.Equals("normal") && has.Rank.Equals("admin") || has.Rank.Equals("mod");
        }

        public Dictionary<string, AbstractCommand> CreateCommands()
        {
            var command = GetCommand();
            var raw = GetContents();

            Commands = new Dictionary<string, AbstractCommand>();
            foreach (AbstractCommand com in raw.Select(command.ParseString))
                Commands.Add(com.Call, com);

            return Commands;
        }
        protected abstract string[] GetContents();
        protected abstract AbstractCommand GetCommand();

    }

}


