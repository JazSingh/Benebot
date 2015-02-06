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
            Console.ReadLine();
        }
    }
}
