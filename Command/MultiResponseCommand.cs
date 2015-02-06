using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace BenebotV3
{
    public class MultiResponseCommand : AbstractCommand
    {
        public List<string> Responses = new List<string>();
        private int _index = 0;

        public override AbstractCommand ParseString(string s)
        {
            var parts = s.Split('~');

            var m = new MultiResponseCommand
            {
                Call = parts[0],
                Responses = ShuffleList(File.ReadAllLines(parts[1]).ToList()),
                AuthRank = parts[2],
                TimeStamp = DateTime.Now,
                Cooldown = int.Parse(parts[3]),
                _useAPI = false,
                
            };
            return m;
        }

        public override string GetResponse(string parameters)
        {
            if (_index != Responses.Count) return Responses[_index++].Replace("\\n", Environment.NewLine);
            Responses = ShuffleList(Responses);
            _index = 0;
            return Responses[_index++].Replace("\\n", Environment.NewLine);
        }

        private List<string> ShuffleList(IEnumerable<string> list)
        {
            for (var i = 0; i < 100; i++)
                list = list.OrderBy(elem => Guid.NewGuid());
            return list.ToList();
        } 

    }
}
