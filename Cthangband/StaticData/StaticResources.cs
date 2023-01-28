// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
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

        public Dictionary<string, BaseArtifact> BaseArtifacts
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
        public static void LoadOrCreate()
        {
            Instance = new StaticResources
            {
                BaseMonsterRaces = ReadEntitiesFromCsv(new BaseMonsterRace())
            };
            Instance.BaseItemTypes = ReadEntitiesFromCsv(new BaseItemType());
            Instance.BaseArtifacts = ReadEntitiesFromCsv(new BaseArtifact());
            Instance.BaseRareItemTypes = ReadEntitiesFromCsv(new BaseRareItemType());
            Instance.BaseVaultTypes = ReadEntitiesFromCsv(new BaseVaultType());
            Instance.FloorTileTypes = ReadEntitiesFromCsv(new FloorTileType());
            Instance.Animations = ReadEntitiesFromCsv(new Animation());
            Instance.ProjectileGraphics = ReadEntitiesFromCsv(new ProjectileGraphic());
            Instance.AmuletFlavours = ReadEntitiesFromCsv(new AmuletFlavour());
            Instance.MushroomFlavours = ReadEntitiesFromCsv(new MushroomFlavour());
            Instance.PotionFlavours = ReadEntitiesFromCsv(new PotionFlavour());
            Instance.WandFlavours = ReadEntitiesFromCsv(new WandFlavour());
            Instance.ScrollFlavours = ReadEntitiesFromCsv(new ScrollFlavour());
            Instance.StaffFlavours = ReadEntitiesFromCsv(new StaffFlavour());
            Instance.RingFlavours = ReadEntitiesFromCsv(new RingFlavour());
            Instance.RodFlavours = ReadEntitiesFromCsv(new RodFlavour());
        }

        /// <summary>
        /// Save the current data to a binary file that can be embedded as a resource next compilation
        /// </summary>
        public void SaveForRecompile()
        {
            WriteEntitiesToCsv(BaseMonsterRaces, new BaseMonsterRace());
            WriteEntitiesToCsv(BaseItemTypes, new BaseItemType());
            WriteEntitiesToCsv(BaseArtifacts, new BaseArtifact());
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
            var dictionary = new Dictionary<string, T>();
            var properties = sample.GetType().GetProperties();
            var assembly = Assembly.GetExecutingAssembly();
            var names = assembly.GetManifestResourceNames();
            var name = sample.GetType().Name;
            var resourceName = $"Cthangband.Data.{name}s.csv";
            foreach (var match in names)
            {
                if (match == resourceName)
                {
                    {
                        using (var ms = assembly.GetManifestResourceStream(resourceName))
                        {
                            using (var reader = new StreamReader(ms))
                            {
                                var header = reader.ReadLine().Split(',');
                                var headerNames = new Dictionary<string, int>();
                                for (var i = 0; i < header.Length; i++)
                                {
                                    headerNames.Add(header[i], i);
                                }
                                if (reader.EndOfStream == false)
                                {
                                    do
                                    {
                                        var line = reader.ReadLine();
                                        if (string.IsNullOrEmpty(line))
                                        {
                                            continue;
                                        }
                                        var values = line.Split(',');
                                        var entity = new T();
                                        foreach (var p in properties)
                                        {
                                            if (headerNames.ContainsKey(p.Name))
                                            {
                                                if (!p.CanWrite)
                                                {
                                                    continue;
                                                }
                                                var stringValue = values[headerNames[p.Name]].FromCsvFriendly();
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

                                                    case "ArtifactId":
                                                        p.SetValue(entity, Enum.Parse(typeof(ArtifactId), stringValue));
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
            var name = sample.GetType().Name;
            var properties = sample.GetType().GetProperties();
            var path = Path.GetDirectoryName(Application.ExecutablePath);
            var saveDir = new DirectoryInfo(path);
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
                        var fs = new FileStream(Path.Combine(saveDir.FullName, $"{name}s.new"), FileMode.Create,
                            FileAccess.Write);
                        var writer = new StreamWriter(fs);
                        var header = false;
                        foreach (var entry in dictionary.Values)
                        {
                            if (header == false)
                            {
                                var headerLine = "Name";
                                foreach (var property in properties)
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
                            var dataLine = $"{entry.Name.ToCsvFriendly()}";
                            foreach (var property in properties)
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
                        var file = new FileInfo(Path.Combine(saveDir.FullName, $"{name}s.csv"));
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