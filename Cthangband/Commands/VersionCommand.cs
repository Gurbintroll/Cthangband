using Cthangband.StaticData;
using System;
using System.Reflection;

namespace Cthangband.Commands
{
    /// <summary>
    /// Print the version number and build info of the game
    /// </summary>
    [Serializable]
    internal class VersionCommand : ICommand
    {
        public char Key => 'V';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            AssemblyName assembly = Assembly.GetExecutingAssembly().GetName();
            Version version = assembly.Version;
            Profile.Instance.MsgPrint($"You are playing {Constants.VersionName} {version}.");
            Profile.Instance.MsgPrint($"(Build time: {Constants.CompileTime})");
        }

    }
}
