using System;

namespace BenebotV3
{
    public abstract class AbstractCommand
    {
        public AbstractAPI API
        {
            get;
            set;
        }

        public DateTime TimeStamp
        {
            get; 
            set; 
        }

        public string Call
        {
            get; 
            set;
        }

        public string AuthRank
        {
            get; 
            set;
        }

        public int Cooldown
        {
            get; 
            set;
        }

        public string Response
        {
            get; 
            set;
        }

        protected bool _useAPI;

        public abstract AbstractCommand ParseString(string s);

        public virtual string GetResponse(string parameters)
        {
            return API != null ? API.CallAPI(parameters) : Response.Replace("\\n", Environment.NewLine);
        }

        public bool OnCooldown()
        {
            return DateTime.Now < TimeStamp;
        }

        public void SetCooldown()
        {
            TimeStamp = TimeStamp.AddMilliseconds(Cooldown);
        }

        protected AbstractAPI GetAPI(string s)
        {
            switch (s)
            {
                case "wolfram": return new WolframAPI();
                case "riot": return new RiotAPI();
                case "twitch": return new TwitchStreamAPI();
                case "talk": return new TalkAPI();
                default:
                   return null;
            }
        }
    }
}


