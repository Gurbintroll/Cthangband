// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using System;
using System.Windows.Forms;

namespace Cthangband.Debug
{
    /// <summary>
    /// An editor for entity types
    /// </summary>
    internal partial class EntityTypeEditor : Form
    {
        /// <summary>
        /// Create an instance
        /// </summary>
        public EntityTypeEditor()
        {
            InitializeComponent();
        }

        protected virtual void SaveResources()
        {
            // Do nothing
        }

        private void BindingSource1_CurrentChanged(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = bindingSource1.Current;
            if (bindingSource1.Current == null)
            {
                return;
            }
            EntityType et = (EntityType)bindingSource1.Current;
            label1.Text = et.Character.ToString();
            label1.ForeColor = UI.Gui.ColorData[et.Colour];
        }

        private void PropertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (bindingSource1.Current == null)
            {
                return;
            }
            EntityType et = (EntityType)bindingSource1.Current;
            label1.Text = et.Character.ToString();
            label1.ForeColor = UI.Gui.ColorData[et.Colour];
        }

        private void SaveToolStripButton_Click(object sender, EventArgs e)
        {
            SaveResources();
        }
    }
}