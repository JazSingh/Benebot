using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Serialization;

namespace BenebotV3
{
    public class TalkAPI : AbstractAPI
    {
        public override string CallAPI(string input)
        {
            return GetResponse(input);
        }

        const string Baseurl = "http://www.pandorabots.com/pandora/talk-xml?botid=ae3b6f03de3422d1&input=";

        public static string GetResponse(string input)
        {
            input = WebUtility.UrlEncode(input);
            var xml = WebTalker.HttpGet(Baseurl + input);

            if (xml == "")
                return "";

            var deserializer = new XmlSerializer(typeof(result));
            TextReader reader = new StringReader(xml);
            var obj = deserializer.Deserialize(reader);
            var resultje = (result)obj;
            reader.Close();

            return resultje.that == null ? "Error: Expected parameter, got none." : resultje.that.Replace("<br>", Environment.NewLine);
        }
    }
    public class result
    {
        public string input { get; set; }
        public string that { get; set; }
    }
}

