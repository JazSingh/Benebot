using System;

namespace BenebotV3
{
    public class SingleResponseCommand : AbstractCommand
    {
        public override AbstractCommand ParseString(string s)
        {
            var parts = s.Split('~');
            var src = new SingleResponseCommand()
            {
                Call = parts[0],
                Response = parts[1],
                AuthRank = parts[2],
                Cooldown = int.Parse(parts[3]),
                API = GetAPI(parts[4]),
                TimeStamp = DateTime.Now
            };
            return src;
        }

    }
}


