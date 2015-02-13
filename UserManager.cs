using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BenebotV3.Properties;

namespace BenebotV3
{
    public class UserManager
    {
        private Benebot _parent;
        private Dictionary<string, Summoner> _registeredUsers;
        private List<long> _blackList;
        public List<string> InRoom = new List<string>();
        private readonly RiotAPI _api;


        public UserManager(Benebot p)
        {
            _parent = p;
            LoadUsers();
            _api = new RiotAPI();
        }

        public void LoadUsers()
        {
            Console.WriteLine("Loading Users...");
            _registeredUsers = new Dictionary<string, Summoner>();
            var sums = File.ReadAllLines(Settings.Default.Users);
            foreach (var user in sums.Select(Summoner.ParseUser))
                _registeredUsers.Add(user.Name, user);

            _blackList = new List<long>();
            sums = File.ReadAllLines(Settings.Default.Blacklist);
            foreach (var sum in sums)
                _blackList.Add(long.Parse(sum));
            Console.WriteLine("Users Loaded!" + Environment.NewLine);
        }

        public string Ban(string name)
        {
            if (!_registeredUsers.ContainsKey(name)) return string.Format("{0} is not a registered user!", name);
            if (_blackList.Contains(_registeredUsers[name].Summonerid))
                return string.Format("{0} is already banned!", name);
            _blackList.Add(_registeredUsers[name].Summonerid);
            FlushBans();
            return string.Format("{0} banned!", name);
        }

        public string UnBan(string name)
        {
            if (!_registeredUsers.ContainsKey(name)) return string.Format("{0} is not a registered user!", name);
            if (!_blackList.Contains(_registeredUsers[name].Summonerid))
                return string.Format("{0} isn't banned!", name);
            _blackList.Remove(_registeredUsers[name].Summonerid);
            FlushBans();
            return string.Format("{0} unbanned", name);
        }

        public string Register(string name) 
        {
            if (_registeredUsers.ContainsKey(name)) return string.Format("{0} is already a registered user!", name);
            var summoner = _api.SummonerIdByName(name);
            var id = summoner.id;
            var oldName = NameFromId(id);
            if (!oldName.Equals(string.Empty)) return Update(oldName, id, name);
            var newUser = new Summoner(id.ToString(), summoner.name, "normal");
            _registeredUsers.Add(newUser.Name, newUser);
            FlushUsers();
            return string.Format("{0} registered!", newUser.Name);
        }

        private string Update(string oldName, long id, string name)
        {
            if (oldName.Equals(string.Empty)) return string.Format("User with id: {0} not registered!", id);
            var sum = _registeredUsers[oldName];
            _registeredUsers.Remove(oldName);
            sum.Name = name;
            _registeredUsers.Add(name, sum);
            FlushUsers();
            return string.Format("{2} => {0} updated to {1}", oldName, name, id);
        }

        private string NameFromId(long id)
        {
            try
            {
                foreach (
                    var registeredUser in
                        _registeredUsers.Where(registeredUser => registeredUser.Value.Summonerid == id))
                    return registeredUser.Value.Name;
                throw new UserNotFoundException();
            }
            catch (UserNotFoundException)
            {
                return string.Empty;
            }
        }

        public bool IsRegistered(string name)
        {
            return _registeredUsers.ContainsKey(name);
        }

        public bool IsBlackListed(string name)
        {
            return _blackList.Contains(_registeredUsers[name].Summonerid);
        }

        public Summoner GetUser(string name)
        {
            return !_registeredUsers.ContainsKey(name) ? null : _registeredUsers[name];
        }

        public void JoinedRoom(string name)
        {
            if (InRoom.Contains(name)) return;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("{0} joined", name);
            Console.ForegroundColor = ConsoleColor.White;
            InRoom.Add(name);
        }

        public void LeftRoom(string name)
        {
            if (InRoom.Remove(name))
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("{0} left", name);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public string SetRank(string user, string s)
        {
            if (!_registeredUsers.ContainsKey(user)) return string.Format("User {0} not found!", user);
            _registeredUsers[user].Rank = s;
            FlushUsers();
            return string.Format("New rank for {0} is {1}", user, s);
        }

        private void FlushUsers()
        {
            var s = _registeredUsers.Aggregate(string.Empty, (current, registeredUser) => current + (Summoner.UserToString(registeredUser.Value) + Environment.NewLine));
            File.WriteAllText(Settings.Default.Users, s);
            Console.WriteLine("Registered users flushed to disk.");
        }

        private void FlushBans()
        {
            var s = _blackList.Aggregate(string.Empty, (current, l) => current + (l + Environment.NewLine));
            File.WriteAllText(Settings.Default.Blacklist, s);
            Console.WriteLine("Blacklist flushed to disk.");
        }
    }

    internal class UserNotFoundException : Exception {}
}