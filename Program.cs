using System;

namespace BenebotV3
{
    class Program
    {
        static void Main(string[] args)
        {
            var benebot = new Benebot();
            benebot.Connection = new LoLConnection(benebot);
            benebot.Start();
            while (true)
            {
                var s = Console.ReadLine();
                switch (s)
                {
                    case "disconnect":
                        benebot.Connection.Disconnect();
                        break;
                    case "connect": benebot.Start();
                        break;
                    default: benebot.SendMessage(s);
                        break;
                }
            }
        }
    }
}
