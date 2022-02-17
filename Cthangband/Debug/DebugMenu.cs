using Cthangband.Enumerations;
using Cthangband.Projection;
using Cthangband.Spells;
using Cthangband.StaticData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Cthangband.Debug
{
    /// <summary>
    /// The debug menu
    /// </summary>
    internal sealed partial class DebugMenu : Form
    {
        /// <summary>
        /// Create a debug menu
        /// </summary>
        public DebugMenu()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            using (GenericEntityTypeEditor<FloorTileType> ete = new GenericEntityTypeEditor<FloorTileType>())
            {
                ete.LoadList(StaticResources.Instance.FloorTileTypes, "Editing FloorTileTypes");
                ete.ShowDialog(this);
            }
        }

        private void Button10_Click(object sender, EventArgs e)
        {
            using (GenericEntityTypeEditor<WandFlavour> ete = new GenericEntityTypeEditor<WandFlavour>())
            {
                ete.LoadList(StaticResources.Instance.WandFlavours, "Editing Wand Flavours");
                ete.ShowDialog(this);
            }
        }

        private void Button11_Click(object sender, EventArgs e)
        {
            using (GenericEntityTypeEditor<RingFlavour> ete = new GenericEntityTypeEditor<RingFlavour>())
            {
                ete.LoadList(StaticResources.Instance.RingFlavours, "Editing Ring Flavours");
                ete.ShowDialog(this);
            }
        }

        private void Button12_Click(object sender, EventArgs e)
        {
            using (GenericEntityTypeEditor<RodFlavour> ete = new GenericEntityTypeEditor<RodFlavour>())
            {
                ete.LoadList(StaticResources.Instance.RodFlavours, "Editing Rod Flavours");
                ete.ShowDialog(this);
            }
        }

        private void Button13_Click(object sender, EventArgs e)
        {
            using (GenericEntityTypeEditor<ScrollFlavour> ete = new GenericEntityTypeEditor<ScrollFlavour>())
            {
                ete.LoadList(StaticResources.Instance.ScrollFlavours, "Editing Scroll Flavours");
                ete.ShowDialog(this);
            }
        }

        private void Button14_Click(object sender, EventArgs e)
        {
            using (GenericEntityTypeEditor<StaffFlavour> ete = new GenericEntityTypeEditor<StaffFlavour>())
            {
                ete.LoadList(StaticResources.Instance.StaffFlavours, "Editing Staff Flavours");
                ete.ShowDialog(this);
            }
        }

        private void Button15_Click(object sender, EventArgs e)
        {
            Dictionary<string, BaseMonsterRace> monsters = StaticResources.Instance.BaseMonsterRaces;
            Dictionary<Colour, Dictionary<char, string>> table = new Dictionary<Colour, Dictionary<char, string>>();
            List<char> allChars = new List<char>();
            List<Colour> allColours = new List<Colour>();
            foreach (object colour in Enum.GetValues(typeof(Colour)))
            {
                allColours.Add((Colour)colour);
                table.Add((Colour)colour, new Dictionary<char, string>());
            }
            foreach (KeyValuePair<string, BaseMonsterRace> pair in monsters)
            {
                BaseMonsterRace monster = pair.Value;
                Colour colour = monster.Colour;
                char character = monster.Character;
                Dictionary<char, string> row = table[colour];
                if (allColours.Contains(colour) == false)
                {
                    allColours.Add(colour);
                }
                if (row.ContainsKey(character) == false)
                {
                    row.Add(character, monster.FriendlyName);
                }
                else
                {
                    row[character] = row[character] + "|" + monster.FriendlyName;
                }
                if (allChars.Contains(character) == false)
                {
                    allChars.Add(character);
                }
            }
            allColours.Sort();
            allChars.Sort();
            FileInfo file = new FileInfo("Monster Distribution.csv");
            using (FileStream stream = file.OpenWrite())
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    string line = "";
                    foreach (Colour colour in allColours)
                    {
                        line += ";";
                        line += colour.ToString();
                    }
                    writer.WriteLine(line);
                    foreach (char character in allChars)
                    {
                        line = new string(character, 1);
                        foreach (Colour colour in allColours)
                        {
                            line += ";";
                            if (table[colour].ContainsKey(character))
                            {
                                line += table[colour][character];
                            }
                        }
                        writer.WriteLine(line);
                    }
                }
            }
        }

        private void Button16_Click(object sender, EventArgs e)
        {
            using (GenericEntityTypeEditor<BaseFixedartifact> ete = new GenericEntityTypeEditor<BaseFixedartifact>())
            {
                ete.LoadList(StaticResources.Instance.BaseFixedartifacts, "Editing Base Fixed Artifacts");
                ete.ShowDialog(this);
            }
        }

        private void Button17_Click(object sender, EventArgs e)
        {
            using (GenericEntityTypeEditor<BaseVaultType> ete = new GenericEntityTypeEditor<BaseVaultType>())
            {
                ete.LoadList(StaticResources.Instance.BaseVaultTypes, "Editing Base Vault Types");
                ete.ShowDialog(this);
            }
        }

        private void Button18_Click(object sender, EventArgs e)
        {
            for (int c = 0; c < 15; c++)
            {
                string className = Profession.ClassInfo[c].Title;
                string[][] cells = new string[9][];
                for (int j = 0; j < 9; j++)
                {
                    cells[j] = new string[51];
                    for (int i = 0; i < 51; i++)
                    {
                        cells[j][i] = "";
                    }
                }
                for (int r = 0; r < 9; r++)
                {
                    SpellList spells = new SpellList((Realm)r, c);
                    for (int i = 0; i < 32; i++)
                    {
                        Spell spell = spells[i];
                        if (spell.Level < 99)
                        {
                            if (!string.IsNullOrEmpty(cells[r][spell.Level]))
                            {
                                cells[r][spell.Level] += "<br>";
                            }
                            if (i >= 16)
                            {
                                cells[r][spell.Level] += "<em>";
                            }
                            cells[r][spell.Level] += spell.Name;
                            if (i >= 16)
                            {
                                cells[r][spell.Level] += "</em>";
                            }
                        }
                    }
                }
                for (int j = 0; j < 9; j++)
                {
                    for (int i = 0; i < 51; i++)
                    {
                        cells[j][i] = "<td>" + cells[j][i] + "</td>";
                    }
                }
                FileInfo file = new FileInfo($"{className}.html");
                using (FileStream stream = file.OpenWrite())
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.WriteLine("<html>");
                        writer.WriteLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"manual.css\">");
                        writer.WriteLine("<body>");
                        writer.WriteLine($"<h1>{className}</h1>");
                        writer.WriteLine("<p>Blah Blah Blah</p>");
                        writer.WriteLine("<p><table align=center>");
                        writer.WriteLine("<tr><th>Level</th><th>Life</th><th>Sorcery</th><th>Nature</th><th>Chaos</th><th>Death</th><th>Tarot</th><th>Folk</th><th>Corporeal</th></tr>");
                        for (int level = 1; level < 51; level++)
                        {
                            string line = $"<tr><td>{level}</td>";
                            for (int i = 1; i < 9; i++)
                            {
                                line += cells[i][level];
                            }
                            line += "</tr>";
                            writer.WriteLine(line);
                        }
                        writer.WriteLine("</table></p>");
                    }
                }
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            using (GenericEntityTypeEditor<ProjectileGraphic> ete = new GenericEntityTypeEditor<ProjectileGraphic>())
            {
                ete.LoadList(StaticResources.Instance.ProjectileGraphics, "Editing ProjectileGraphics");
                ete.ShowDialog(this);
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            var list = StaticResources.Instance.BaseVaultTypes;
            foreach (var vault in list.Values)
            {
                if (vault.Text.Length != vault.Width * vault.Height)
                {
                    MessageBox.Show($"{vault.Name} : {vault.Width}x{vault.Height} : {vault.Width * vault.Height} : {vault.Text.Length}");
                }
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            for (int c = 0; c < 15; c++)
            {
                Profession cc = Profession.ClassInfo[c];
                string className = cc.Title;
                FileInfo file = new FileInfo($"{className}.html");
                using (FileStream stream = file.OpenWrite())
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.WriteLine("<p><table align=center>");
                        writer.WriteLine("<tr><th>Hit Dice</th><th>Experience Factor</th></tr>");
                        writer.WriteLine($"<tr><td>{cc.HitDieBonus:+0;-0;+0}</td><td>{cc.ExperienceFactor}%</td></tr>");
                        writer.WriteLine("</table></p>");
                        writer.WriteLine("<p><table align=center>");
                        writer.WriteLine("<tr><th>Strength</th><th>Intelligence</th><th>Wisdom</th><th>Dexterity</th><th>Constitution</th><th>Charisma</th></tr>");
                        writer.WriteLine($"<tr><td>{cc.AbilityBonus[0]:+0;-0;+0}</td><td>{cc.AbilityBonus[1]:+0;-0;+0}</td><td>{cc.AbilityBonus[2]:+0;-0;+0}</td><td>{cc.AbilityBonus[3]:+0;-0;+0}</td><td>{cc.AbilityBonus[4]:+0;-0;+0}</td><td>{cc.AbilityBonus[5]:+0;-0;+0}</td></tr>");
                        writer.WriteLine("</table></p>");
                        writer.WriteLine("<p><table align=center>");
                        writer.WriteLine("<tr><th></th><th>Devices</th><th>Disarming</th><th>Melee</th><th>Ranged</th><th>Saves</th><th>Searching</th><th>Stealth</th></tr>");
                        writer.WriteLine($"<tr><td>Base Bonus</td><td>{cc.BaseDeviceBonus}%</td><td>{cc.BaseDisarmBonus}%</td><td>{cc.BaseMeleeAttackBonus}%</td><td>{cc.BaseRangedAttackBonus}%</td><td>{cc.BaseSaveBonus}%</td><td>{cc.BaseSearchBonus}%</td><td>{cc.BaseStealthBonus}%</td></tr>");
                        writer.WriteLine($"<tr><td>Bonus per Level</td><td>{cc.DeviceBonusPerLevel / 10.0}%</td><td>{cc.DisarmBonusPerLevel / 10.0}%</td><td>{cc.MeleeAttackBonusPerLevel / 10.0}%</td><td>{cc.RangedAttackBonusPerLevel / 10.0}%</td><td>{cc.SaveBonusPerLevel / 10.0}%</td><td>{cc.SearchBonusPerLevel / 10.0}%</td><td>{cc.StealthBonusPerLevel / 10.0}%</td></tr>");
                        writer.WriteLine("</table></p>");
                    }
                }
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            var color = this.label1.BackColor == Color.Black ? Color.Silver : Color.Black;
            this.label1.BackColor = color;
            this.label2.BackColor = color;
            this.label3.BackColor = color;
            this.label4.BackColor = color;
            this.label5.BackColor = color;
            this.label6.BackColor = color;
            this.label7.BackColor = color;
            this.label8.BackColor = color;
            this.label9.BackColor = color;
            this.label10.BackColor = color;
            this.label11.BackColor = color;
            this.label12.BackColor = color;
            this.label13.BackColor = color;
            this.label14.BackColor = color;
            this.label15.BackColor = color;
            this.label16.BackColor = color;
            this.label17.BackColor = color;
            this.label18.BackColor = color;
            this.label19.BackColor = color;
            this.label20.BackColor = color;
            this.label21.BackColor = color;
            this.label22.BackColor = color;
            this.label23.BackColor = color;
            this.label24.BackColor = color;
            this.label25.BackColor = color;
            this.label26.BackColor = color;
            this.label27.BackColor = color;
            this.label28.BackColor = color;
            this.label29.BackColor = color;
            this.label30.BackColor = color;
            this.label31.BackColor = color;
            this.label32.BackColor = color;
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            using (GenericEntityTypeEditor<Animation> ete = new GenericEntityTypeEditor<Animation>())
            {
                ete.LoadList(StaticResources.Instance.Animations, "Editing Animations");
                ete.ShowDialog(this);
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            using (GenericEntityTypeEditor<BaseMonsterRace> ete = new GenericEntityTypeEditor<BaseMonsterRace>())
            {
                ete.LoadList(StaticResources.Instance.BaseMonsterRaces, "Editing Base Monster Races");
                ete.ShowDialog(this);
            }
        }

        private void Button5_Click_1(object sender, EventArgs e)
        {
            using (GenericEntityTypeEditor<BaseRareItemType> ete = new GenericEntityTypeEditor<BaseRareItemType>())
            {
                ete.LoadList(StaticResources.Instance.BaseRareItemTypes, "Editing Base Rare Item Types");
                ete.ShowDialog(this);
            }
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            using (GenericEntityTypeEditor<BaseItemType> ete = new GenericEntityTypeEditor<BaseItemType>())
            {
                ete.LoadList(StaticResources.Instance.BaseItemTypes, "Editing Base Item Types");
                ete.ShowDialog(this);
            }
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            using (GenericEntityTypeEditor<AmuletFlavour> ete = new GenericEntityTypeEditor<AmuletFlavour>())
            {
                ete.LoadList(StaticResources.Instance.AmuletFlavours, "Editing Amulet Flavours");
                ete.ShowDialog(this);
            }
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            using (GenericEntityTypeEditor<MushroomFlavour> ete = new GenericEntityTypeEditor<MushroomFlavour>())
            {
                ete.LoadList(StaticResources.Instance.MushroomFlavours, "Editing Mushroom Flavours");
                ete.ShowDialog(this);
            }
        }

        private void Button9_Click(object sender, EventArgs e)
        {
            using (GenericEntityTypeEditor<PotionFlavour> ete = new GenericEntityTypeEditor<PotionFlavour>())
            {
                ete.LoadList(StaticResources.Instance.PotionFlavours, "Editing Potion Flavours");
                ete.ShowDialog(this);
            }
        }
    }
}