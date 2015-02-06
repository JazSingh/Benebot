using System.IO;
using BenebotV3.Properties;

namespace BenebotV3
{
    public class DynamicCommands : AbstractCommands
    {
        protected override string[] GetContents()
        {
            return File.ReadAllLines(Settings.Default.DynCom);
        }

        protected override AbstractCommand GetCommand()
        {
            return new SingleResponseCommand();
        }
    }


}

