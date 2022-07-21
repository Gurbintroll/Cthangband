using System;

namespace Cthangband.Commands
{
    [Serializable]
    internal class WizardCommand : ICommand
    {
        public char Key => 'W';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            if (player.IsWizard)
            {
                WizardCommandHandler wizard = new WizardCommandHandler(player, level);
                wizard.DoCmdWizard();
            }
            else
            {
                WizardCommandHandler wizard = new WizardCommandHandler(player, level);
                wizard.DoCmdWizmode();
            }
        }
    }
}
