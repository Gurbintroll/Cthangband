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