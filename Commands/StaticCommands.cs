using System;
using System.IO;
using BenebotV3.Properties;

namespace BenebotV3
{
    public class StaticCommands : AbstractCommands
    {
        protected override string[] GetContents()
        {
            return File.ReadAllLines(Settings.Default.StaticCom);
        }

        protected override AbstractCommand GetCommand()
        {
            return new SingleResponseCommand();
        }
    }
}


