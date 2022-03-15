// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.StaticData;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Cthangband.Debug
{
    internal sealed class GenericEntityTypeEditor<T> : EntityTypeEditor where T : EntityType
    {
        private Dictionary<string, T> _dict;
        private List<T> _list;

        public void LoadList(Dictionary<string, T> dictionary, string title)
        {
            _dict = dictionary;
            _list = new List<T>();
            foreach (KeyValuePair<string, T> pair in dictionary)
            {
                _list.Add(pair.Value);
            }
            bindingSource1.DataSource = _list;
            Text = title;
        }

        protected override void SaveResources()
        {
            _dict.Clear();
            _list.Sort();
            foreach (T t in _list)
            {
                if (_dict.ContainsKey(t.Name))
                {
                    MessageBox.Show(
                        string.Format("More than one entity has the name \"{0}\". The duplicate has been renamed.",
                                      t.Name), "Duplicate Name", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    t.Name = Guid.NewGuid().ToString();
                }
                _dict.Add(t.Name, t);
            }
            StaticResources.Instance.SaveForRecompile();
        }
    }
}