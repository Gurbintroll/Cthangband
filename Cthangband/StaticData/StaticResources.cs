// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Debug;
using Cthangband.Enumerations;
using Cthangband.Projection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Cthangband.StaticData
{
    /// <summary>
    /// Singleton class containing objects loaded from an embedded resource
    /// </summary>
    [Serializable]
    internal sealed class StaticResources
    {
        /// <summary>
        /// The singleton instance
        /// </summary>
        public static StaticResources Instance
        {
            get;
            private set;
        }

        public Dictionary<string, AmuletFlavour> AmuletFlavours
        {
            get;
            private set;
        }

        /// <summary>
        /// Animations for spells and effects
        /// </summary>
        public Dictionary<string, Animation> Animations
        {
            get;
            private set;
        }

        public Dictionary<string, BaseFixedartifact> BaseFixedartifacts
        {
            get;
            private set;
        }

        public Dictionary<string, BaseItemType> BaseItemTypes
        {
            get;
            private set;
        }

        public Dictionary<string, BaseMonsterRace> BaseMonsterRaces
        {
            get;
            private set;
        }

        public Dictionary<string, BaseRareItemType> BaseRareItemTypes
        {
            get;
            private set;
        }

        public Dictionary<string, BaseVaultType> BaseVaultTypes
        {
            get;
            private set;
        }

        /// <summary>
        /// Types of floor tile
        /// </summary>
        public Dictionary<string, FloorTileType> FloorTileTypes
        {
            get;
            private set;
        }

        public Dictionary<string, MushroomFlavour> MushroomFlavours
        {
            get;
            private set;
        }

        public Dictionary<string, PotionFlavour> PotionFlavours
        {
            get;
            private set;
        }

        /// <summary>
        /// Graphics for projectiles
        /// </summary>
        public Dictionary<string, ProjectileGraphic> ProjectileGraphics
        {
            get;
            private set;
        }

        public Dictionary<string, RingFlavour> RingFlavours
        {
            get;
            private set;
        }

        public Dictionary<string, RodFlavour> RodFlavours
        {
            get;
            private set;
        }

        public Dictionary<string, ScrollFlavour> ScrollFlavours
        {
            get;
            private set;
        }

        public Dictionary<string, StaffFlavour> StaffFlavours
        {
            get;
            private set;
        }

        public Dictionary<string, WandFlavour> WandFlavours
        {
            get;
            private set;
        }

        /// <summary>
        /// Load the dictionaries from the binary resource file
        /// </summary>
        public static void LoadOrCreate(Action<int> Callback)
        {
            Instance = new StaticResources
            {
                BaseMonsterRaces = ReadEntitiesFromCsv(new BaseMonsterRace())
            };
            Callback(1);
            Instance.BaseItemTypes = ReadEntitiesFromCsv(new BaseItemType());
            Callback(1);
            Instance.BaseFixedartifacts = ReadEntitiesFromCsv(new BaseFixedartifact());
            Callback(1);
            Instance.BaseRareItemTypes = ReadEntitiesFromCsv(new BaseRareItemType());
            Callback(1);
            Instance.BaseVaultTypes = ReadEntitiesFromCsv(new BaseVaultType());
            Callback(1);
            Instance.FloorTileTypes = ReadEntitiesFromCsv(new FloorTileType());
            Callback(1);
            Instance.Animations = ReadEntitiesFromCsv(new Animation());
            Callback(1);
            Instance.ProjectileGraphics = ReadEntitiesFromCsv(new ProjectileGraphic());
            Callback(1);
            Instance.AmuletFlavours = ReadEntitiesFromCsv(new AmuletFlavour());
            Callback(1);
            Instance.MushroomFlavours = ReadEntitiesFromCsv(new MushroomFlavour());
            Callback(1);
            Instance.PotionFlavours = ReadEntitiesFromCsv(new PotionFlavour());
            Callback(1);
            Instance.WandFlavours = ReadEntitiesFromCsv(new WandFlavour());
            Callback(1);
            Instance.ScrollFlavours = ReadEntitiesFromCsv(new ScrollFlavour());
            Callback(1);
            Instance.StaffFlavours = ReadEntitiesFromCsv(new StaffFlavour());
            Callback(1);
            Instance.RingFlavours = ReadEntitiesFromCsv(new RingFlavour());
            Callback(1);
            Instance.RodFlavours = ReadEntitiesFromCsv(new RodFlavour());
            Callback(1);
        }

        /// <summary>
        /// Save the current data to a binary file that can be embedded as a resource next compilation
        /// </summary>
        public void SaveForRecompile()
        {
            WriteEntitiesToCsv(BaseMonsterRaces, new BaseMonsterRace());
            WriteEntitiesToCsv(BaseItemTypes, new BaseItemType());
            WriteEntitiesToCsv(BaseFixedartifacts, new BaseFixedartifact());
            WriteEntitiesToCsv(BaseRareItemTypes, new BaseRareItemType());
            WriteEntitiesToCsv(BaseVaultTypes, new BaseVaultType());
            WriteEntitiesToCsv(FloorTileTypes, new FloorTileType());
            WriteEntitiesToCsv(Animations, new Animation());
            WriteEntitiesToCsv(ProjectileGraphics, new ProjectileGraphic());
            WriteEntitiesToCsv(AmuletFlavours, new AmuletFlavour());
            WriteEntitiesToCsv(MushroomFlavours, new MushroomFlavour());
            WriteEntitiesToCsv(PotionFlavours, new PotionFlavour());
            WriteEntitiesToCsv(WandFlavours, new WandFlavour());
            WriteEntitiesToCsv(RingFlavours, new RingFlavour());
            WriteEntitiesToCsv(RodFlavours, new RodFlavour());
            WriteEntitiesToCsv(ScrollFlavours, new ScrollFlavour());
            WriteEntitiesToCsv(StaffFlavours, new StaffFlavour());
        }

        private static Dictionary<string, T> ReadEntitiesFromCsv<T>(T sample) where T : EntityType, new()
        {
            Dictionary<string, T> dictionary = new Dictionary<string, T>();
            PropertyInfo[] properties = sample.GetType().GetProperties();
            Assembly assembly = Assembly.GetExecutingAssembly();
            string[] names = assembly.GetManifestResourceNames();
            string name = sample.GetType().Name;
            string resourceName = $"Cthangband.Data.{name}s.csv";
            foreach (string match in names)
            {
                if (match == resourceName)
                {
                    {
                        using (Stream ms = assembly.GetManifestResourceStream(resourceName))
                        {
                            using (StreamReader reader = new StreamReader(ms))
                            {
                                string[] header = reader.ReadLine().Split(',');
                                Dictionary<string, int> headerNames = new Dictionary<string, int>();
                                for (int i = 0; i < header.Length; i++)
                                {
                                    headerNames.Add(header[i], i);
                                }
                                if (reader.EndOfStream == false)
                                {
                                    do
                                    {
                                        string line = reader.ReadLine();
                                        if (string.IsNullOrEmpty(line))
                                        {
                                            continue;
                                        }
                                        string[] values = line.Split(',');
                                        T entity = new T();
                                        foreach (PropertyInfo p in properties)
                                        {
                                            if (headerNames.ContainsKey(p.Name))
                                            {
                                                if (!p.CanWrite)
                                                {
                                                    continue;
                                                }
                                                string stringValue = values[headerNames[p.Name]].FromCsvFriendly();
                                                switch (p.PropertyType.Name)
                                                {
                                                    case "Colour":
                                                        p.SetValue(entity, Enum.Parse(typeof(Colour), stringValue));
                                                        break;

                                                    case "Char":
                                                        p.SetValue(entity, Convert.ToChar(stringValue));
                                                        break;

                                                    case "String":
                                                        p.SetValue(entity, stringValue);
                                                        break;

                                                    case "AttackEffect":
                                                        p.SetValue(entity, Enum.Parse(typeof(AttackEffect), stringValue));
                                                        break;

                                                    case "FloorTileAlterAction":
                                                        p.SetValue(entity, Enum.Parse(typeof(FloorTileAlterAction), stringValue));
                                                        break;

                                                    case "FixedArtifactId":
                                                        p.SetValue(entity, Enum.Parse(typeof(FixedArtifactId), stringValue));
                                                        break;

                                                    case "FloorTileTypeCategory":
                                                        p.SetValue(entity, Enum.Parse(typeof(FloorTileTypeCategory), stringValue));
                                                        break;

                                                    case "ItemCategory":
                                                        p.SetValue(entity, Enum.Parse(typeof(ItemCategory), stringValue));
                                                        break;

                                                    case "RareItemType":
                                                        p.SetValue(entity, Enum.Parse(typeof(Enumerations.RareItemType), stringValue));
                                                        break;

                                                    case "AttackType":
                                                        p.SetValue(entity, Enum.Parse(typeof(AttackType), stringValue));
                                                        break;

                                                    case "Int32":
                                                        p.SetValue(entity, Convert.ToInt32(stringValue));
                                                        break;

                                                    case "Boolean":
                                                        p.SetValue(entity, Convert.ToBoolean(stringValue));
                                                        break;

                                                    case "MonsterAttack":
                                                        break;

                                                    default:
                                                        MessageBox.Show($"Unrecognised property type: {p.PropertyType.Name}");
                                                        break;
                                                }
                                            }
                                        }
                                        dictionary.Add(entity.Name, entity);
                                    } while (reader.EndOfStream == false);
                                }
                            }
                            ms.Close();
                        }
                    }
                }
            }
            return dictionary;
        }

        private static void WriteEntitiesToCsv<T>(Dictionary<string, T> dictionary, T sample)
            where T : EntityType, new()
        {
            string name = sample.GetType().Name;
            PropertyInfo[] properties = sample.GetType().GetProperties();
            string path = Path.GetDirectoryName(Application.ExecutablePath);
            DirectoryInfo saveDir = new DirectoryInfo(path);
            if (saveDir.Parent != null)
            {
                saveDir = saveDir.Parent;
            }
            if (saveDir != null)
            {
                if (saveDir.Parent != null)
                {
                    saveDir = saveDir.Parent;
                }
                if (saveDir != null)
                {
                    if (saveDir != null)
                    {
                        saveDir = saveDir.GetDirectories("Data")[0];
                    }
                    if (saveDir == null)
                    {
                        return;
                    }
                    if (dictionary.Count == 0)
                    {
                        return;
                    }
                    try
                    {
                        //Save a new file - if we get an error at this point then we've not trashed anything
                        FileStream fs = new FileStream(Path.Combine(saveDir.FullName, $"{name}s.new"), FileMode.Create,
                            FileAccess.Write);
                        StreamWriter writer = new StreamWriter(fs);
                        bool header = false;
                        foreach (T entry in dictionary.Values)
                        {
                            if (header == false)
                            {
                                string headerLine = "Name";
                                foreach (PropertyInfo property in properties)
                                {
                                    if (property.Name != "Name")
                                    {
                                        if (!string.IsNullOrEmpty(headerLine))
                                        {
                                            headerLine += ",";
                                        }
                                        headerLine += property.Name;
                                    }
                                }
                                header = true;
                                writer.WriteLine(headerLine);
                            }
                            string dataLine = $"{entry.Name.ToCsvFriendly()}";
                            foreach (PropertyInfo property in properties)
                            {
                                if (property.Name != "Name")
                                {
                                    dataLine += ",";
                                    dataLine += property.GetValue(entry).ToString().ToCsvFriendly();
                                }
                            }
                            writer.WriteLine(dataLine);
                        }
                        writer.Dispose();
                        fs.Close();
                        fs.Dispose();
                        // Rename the existing file to get it out of the way
                        FileInfo file = new FileInfo(Path.Combine(saveDir.FullName, $"{name}s.csv"));
                        if (file.Exists)
                        {
                            file.MoveTo(Path.Combine(saveDir.FullName, $"{name}s.old"));
                        }
                        // Rename the new file to make it the existing one
                        file = new FileInfo(Path.Combine(saveDir.FullName, $"{name}s.new"));
                        file.MoveTo(Path.Combine(saveDir.FullName, $"{name}s.csv"));
                        // Assuming we got here without error, delete the old file because we are okay
                        file = new FileInfo(Path.Combine(saveDir.FullName, $"{name}s.old"));
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Unable to save static resources because:" + Environment.NewLine + ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}