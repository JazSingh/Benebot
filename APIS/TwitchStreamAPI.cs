using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace BenebotV3
{
    public class TwitchStreamAPI : AbstractAPI
    {
        private const string Uri = "https://api.twitch.tv/kraken/streams/";
        private const string Streambase = "twitch.tv/";
        private readonly List<string> _streamers;
        private readonly List<string> _summoners;

        public TwitchStreamAPI()
        {
            _streamers = new List<string>();
            _summoners = new List<string>();
            var all = File.ReadAllLines("benestreams.txt");
            foreach (var s in all.Select(line => line.Split('~')))
            {
                _streamers.Add(s[1]);
                _summoners.Add(s[0]);
            }
        }
        public override string CallAPI(string input)
        {
            return GetStreams();
        }

        public string GetStreams()
        {
            var on = new List<string>();
            var output = "Online Streams";
            for (var i = 0; i < _streamers.Count; i++)
            {
                var json = WebTalker.HttpGet(Uri + _streamers[i]);
                dynamic s = JObject.Parse(json);
                if (s.stream != null)
                    on.Add(_summoners[i] + ": " + Streambase + _streamers[i] + "\n");
            }
            output += string.Format(" ({0}/{1}):{2}", @on.Count, _streamers.Count, Environment.NewLine);
            output = @on.Aggregate(output, (current, s) => current + s);
            return !@on.Any() ? "No streams online! (0/" + _streamers.Count + ")" : output.Remove(output.Length - 1);
        }
    }
}


