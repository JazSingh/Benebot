using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Serialization;

namespace BenebotV3
{
    public class WolframAPI : AbstractAPI
    {

        private string _baseUrl = "http://api.wolframalpha.com/v2/query?appid=";
        private const string Tail = "&format=plaintext";
        private List<string> podtitlesList;

        public WolframAPI()
        {
            podtitlesList =  new List<string>
            {
                "Result",
                "Solution",
                "Solutions",
                "Approximate result",
                "Decimal approximation",
                "Exact result",
                "Decimal form",
                "Derivative",
                "Results",
                "Indefinite integral",
                "Definite integral"
            };
            var appId = Properties.Settings.Default.WolframAPIKey;
            _baseUrl += appId;
        }

        public string Calculate(string expression)
        {
            var uri = _baseUrl + "&input=" + WebUtility.UrlEncode(expression.Trim()) + Tail;
            var xml = WebTalker.HttpGet(uri);
            if (xml == "")
                return "";
            var deserializer = new XmlSerializer(typeof(queryresult));
            TextReader reader = new StringReader(xml);
            var obj = deserializer.Deserialize(reader);
            var result = (queryresult)obj;
            reader.Close();

            var output = string.Format("Input: {0}", expression);

            var outcome = string.Empty;
            foreach (var podje in result.Pods)
            {
                if (!podtitlesList.Contains(podje.Title)) continue;
                outcome = podje.Subpods.Aggregate(outcome,
                    (current, subpodje) => current + (subpodje.plaintext + Environment.NewLine));
                outcome = outcome.TrimEnd('\r', '\n');
                break;
            }

            output += string.IsNullOrEmpty(outcome) ? "\nNo result found. :(" : "\n" + outcome;

            Debug.WriteLine(output);
            return output;
        }


        // ReSharper disable once InconsistentNaming
        public class queryresult
        {
            [XmlElement("pod")]
            public List<pod> Pods = new List<pod>();
            [XmlAttribute("success")]
            public string Success { get; set; }

        }

        // ReSharper disable once InconsistentNaming
        public class pod
        {
            [XmlElement("subpod")]
            public List<subpod> Subpods = new List<subpod>();
            [XmlAttribute("title")]
            public string Title { get; set; }
        }

        // ReSharper disable once InconsistentNaming
        public class subpod
        {
            // ReSharper disable once InconsistentNaming
            public string plaintext { get; set; }
        }

        public override string CallAPI(string input)
        {
            return Calculate(input);
        }
    }

}


