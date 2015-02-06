using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenebotV3
{
    public class Summoner
    {
        public long Summonerid;
        public string Name;
        public string Rank;

        public Summoner(string id, string n, string r)
        {
            Summonerid = long.Parse(id);
            Name = n;
            Rank = r;
        }

        public static Summoner ParseUser(string s)
        {
            var parts = s.Split('~');
            return new Summoner(parts[0], parts[1], parts[2]);
        }

        public static String UserToString(Summoner b)
        {
            return string.Format("{0}~{1}~{2}", b.Summonerid, b.Name, b.Rank);
        }
    }
}
