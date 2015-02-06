using System.Collections.Generic;
using System.Linq;
using BenebotV3.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable once CheckNamespace
namespace BenebotV3
{
    public class RiotAPI : AbstractAPI
    {
        private const string apihead = "https://euw.api.pvp.net/api/lol/euw/v1.4/";
        private const string IdByName = "summoner/by-name/";
        private const string RankedInfo = "https://euw.api.pvp.net/api/lol/euw/v2.5/league/by-summoner/";
        private const string Entry = "/entry";
        private readonly string _apitail;

        const string Rt5 = "RANKED_TEAM_5x5";
        const string Rt3 = "RANKED_TEAM_3x3";
        const string Sq = "RANKED_SOLO_5x5";

        const string NewRt5 = "Ranked Team 5v5";
        const string NewRt3 = "Ranked Team 3v3";
        const string NewSq = "Solo Queue";

        const string BRONZE = "BRONZE";
        const string SILVER = "SILVER";
        const string GOLD = "GOLD";
        const string PLATINUM = "PLATINUM";
        const string DIAMOND = "DIAMOND";
        const string CHALLENGER = "CHALLENGER";
        const string MASTER = "MASTER";


        const string Bronze = "Bronze";
        const string Silver = "Silver";
        const string Gold = "Gold";
        const string Platinum = "Platinum";
        const string Diamond = "Diamond";
        const string Challenger = "Challenger";
        const string Master = "Master";

        public RiotAPI()
        {
            _apitail = "?api_key=" + Settings.Default.RiotAPIKey;
        }

        public override string CallAPI(string input)
        {
            long type;
            var one = long.TryParse(input, out type);
            return one ? SummonerIdByName(input).id.ToString() : GetRankedInfo(SummonerIdByName(input));
        }

        public SummonerDto SummonerIdByName(string name)
        {
            var json = WebTalker.HttpGet(apihead + IdByName + name + _apitail);
            if (json.Equals("")) return null;
            var sums = JObject.Parse(json).SelectToken(name.ToLower().Replace(" ", string.Empty)).ToString();
            var obj = JsonConvert.DeserializeObject<SummonerDto>(sums);
            return obj;
        }

        public string GetRankedInfo(SummonerDto sum)
        {
            if (sum == null || sum.id == 0)
                return "";
            var json = WebTalker.HttpGet(RankedInfo + sum.id + Entry + _apitail);
            if (json.Equals("")) return "RITO ERROR PLS";
            var renked = JObject.Parse(json).SelectToken(sum.id.ToString()).ToString();
            var obj = JsonConvert.DeserializeObject<List<LeagueDto>>(renked);

            var s = "-\n" + sum.name + ":";

            foreach (var leagueDto in obj.Where(leagueDto => leagueDto.entries != null))
            {
                if (leagueDto.queue.Equals(Sq))
                    s += string.Format("\n{0}: {1} {2} {3}LP", leagueDto.queue, leagueDto.tier, leagueDto.entries.First().division, leagueDto.entries.First().leaguePoints);
                else
                    s += string.Format("\n{0}: {1} {2} {3} {4}LP", leagueDto.queue, leagueDto.entries.First().playerOrTeamName, leagueDto.tier, leagueDto.entries.First().division, leagueDto.entries.First().leaguePoints);
            }

            s = s.Replace(Rt5, NewRt5).Replace(Rt3, NewRt3).Replace(Sq, NewSq).Replace(BRONZE, Bronze).Replace(SILVER, Silver).Replace(GOLD, Gold).Replace(PLATINUM, Platinum).Replace(DIAMOND, Diamond).Replace(CHALLENGER, Challenger).Replace(MASTER, Master);

            return s;
        }
    }

    public class SummonerDto
    {
        public long id { get; set; }
        public string name { get; set; }
        public int profileIconId { get; set; }
        public long revisionDate { get; set; }
        public long summonerLevel { get; set; }
    }

    public class SummonerNameListDto
    {
        public List<SummonerDto> summoners { get; set; }
    }

    public class LeagueDto
    {
        public List<LeagueEntryDto> entries { get; set; }
        public string name { get; set; }
        public string participantId { get; set; }
        public string queue { get; set; }
        public string tier { get; set; }
    }

    public class LeagueEntryDto
    {
        public string division { get; set; }
        public bool isFreshBlood { get; set; }
        public bool isHotStreak { get; set; }
        public bool isInactive { get; set; }
        public bool isVeteran { get; set; }
        public int leaguePoints { get; set; }
        public MiniSeriesDto MiniSeries { get; set; }
        public string playerOrTeamId { get; set; }
        public string playerOrTeamName { get; set; }
        public int wins { get; set; }
    }

    public class MiniSeriesDto
    {
        public int losses { get; set; }
        public string progress { get; set; }
        public int target { get; set; }
        public int wins { get; set; }
    }

}


