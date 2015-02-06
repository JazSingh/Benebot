using BenebotV3.Properties;
using System.IO;

namespace BenebotV3
{
    public class RandomizedCommands : AbstractCommands
    {
        protected override string[] GetContents()
        {
            return File.ReadAllLines(Settings.Default.RandCom);
        }

        protected override AbstractCommand GetCommand()
        {
            return new MultiResponseCommand();
        }
    }
}

