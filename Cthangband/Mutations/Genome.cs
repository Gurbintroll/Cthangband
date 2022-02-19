using Cthangband.Enumerations;
using Cthangband.Mutations.ActiveMutations;
using Cthangband.Mutations.PassiveMutations;
using Cthangband.Mutations.RandomMutations;
using Cthangband.StaticData;
using System;
using System.Collections.Generic;

namespace Cthangband.Mutations
{
    [Serializable]
    internal class Genome
    {
        public readonly List<Mutation> NaturalAttacks = new List<Mutation>();
        public int ArmourClassBonus;
        public bool ChaosGift;
        public int CharismaBonus;
        public bool CharismaOverride;
        public int ConstitutionBonus;
        public int DexterityBonus;
        public bool ElecHit;
        public bool Esp;
        public bool FeatherFall;
        public bool FireHit;
        public bool FreeAction;
        public int InfravisionBonus;
        public int IntelligenceBonus;
        public bool MagicResistance;
        public bool Regen;
        public bool ResFear;
        public bool ResTime;
        public int SearchBonus;
        public int SpeedBonus;
        public int StealthBonus;
        public int StrengthBonus;
        public bool SuppressRegen;
        public bool SustainAll;
        public bool Vulnerable;
        public int WisdomBonus;
        private readonly List<Mutation> _notPossessed = new List<Mutation>();
        private readonly List<Mutation> _possessed = new List<Mutation>();

        public Genome()
        {
            _possessed.Clear();
            // Active Mutations
            _notPossessed.Add(new MutationBanish());
            _notPossessed.Add(new MutationBerserk());
            _notPossessed.Add(new MutationBlink());
            _notPossessed.Add(new MutationBrFire());
            _notPossessed.Add(new MutationColdTouch());
            _notPossessed.Add(new MutationDazzle());
            _notPossessed.Add(new MutationDetCurse());
            _notPossessed.Add(new MutationEarthquake());
            _notPossessed.Add(new MutationEatMagic());
            _notPossessed.Add(new MutationEatRock());
            _notPossessed.Add(new MutationGrowMold());
            _notPossessed.Add(new MutationHypnGaze());
            _notPossessed.Add(new MutationIllumine());
            _notPossessed.Add(new MutationLaserEye());
            _notPossessed.Add(new MutationLauncher());
            _notPossessed.Add(new MutationMidasTch());
            _notPossessed.Add(new MutationMindBlst());
            _notPossessed.Add(new MutationPanicHit());
            _notPossessed.Add(new MutationPolymorph());
            _notPossessed.Add(new MutationRadiation());
            _notPossessed.Add(new MutationRecall());
            _notPossessed.Add(new MutationResist());
            _notPossessed.Add(new MutationShriek());
            _notPossessed.Add(new MutationSmellMet());
            _notPossessed.Add(new MutationSmellMon());
            _notPossessed.Add(new MutationSpitAcid());
            _notPossessed.Add(new MutationSterility());
            _notPossessed.Add(new MutationSwapPos());
            _notPossessed.Add(new MutationTelekines());
            _notPossessed.Add(new MutationVampirism());
            _notPossessed.Add(new MutationVteleport());
            _notPossessed.Add(new MutationWeighMag());
            // Passive Mutations
            _notPossessed.Add(new MutationAlbino());
            _notPossessed.Add(new MutationArthritis());
            _notPossessed.Add(new MutationBlankFac());
            _notPossessed.Add(new MutationElecTouc());
            _notPossessed.Add(new MutationEsp());
            _notPossessed.Add(new MutationFearless());
            _notPossessed.Add(new MutationFireBody());
            _notPossessed.Add(new MutationFleshRot());
            _notPossessed.Add(new MutationHyperInt());
            _notPossessed.Add(new MutationHyperStr());
            _notPossessed.Add(new MutationIllNorm());
            _notPossessed.Add(new MutationInfravis());
            _notPossessed.Add(new MutationIronSkin());
            _notPossessed.Add(new MutationLimber());
            _notPossessed.Add(new MutationMagicRes());
            _notPossessed.Add(new MutationMoronic());
            _notPossessed.Add(new MutationMotion());
            _notPossessed.Add(new MutationPuny());
            _notPossessed.Add(new MutationRegen());
            _notPossessed.Add(new MutationResilient());
            _notPossessed.Add(new MutationResTime());
            _notPossessed.Add(new MutationScales());
            _notPossessed.Add(new MutationShortLeg());
            _notPossessed.Add(new MutationSillyVoi());
            _notPossessed.Add(new MutationSusStats());
            _notPossessed.Add(new MutationVulnElem());
            _notPossessed.Add(new MutationWartSkin());
            _notPossessed.Add(new MutationWings());
            _notPossessed.Add(new MutationXtraEyes());
            _notPossessed.Add(new MutationXtraFat());
            _notPossessed.Add(new MutationXtraLegs());
            _notPossessed.Add(new MutationXtraNois());
            // Random Mutations
            _notPossessed.Add(new MutationAlcohol());
            _notPossessed.Add(new MutationAttAnimal());
            _notPossessed.Add(new MutationAttDemon());
            _notPossessed.Add(new MutationAttDragon());
            _notPossessed.Add(new MutationBanishAll());
            _notPossessed.Add(new MutationBeak());
            _notPossessed.Add(new MutationBersRage());
            _notPossessed.Add(new MutationChaosGift());
            _notPossessed.Add(new MutationCowardice());
            _notPossessed.Add(new MutationDisarm());
            _notPossessed.Add(new MutationEatLight());
            _notPossessed.Add(new MutationFlatulent());
            _notPossessed.Add(new MutationHallu());
            _notPossessed.Add(new MutationHorns());
            _notPossessed.Add(new MutationHpToSp());
            _notPossessed.Add(new MutationInvuln());
            _notPossessed.Add(new MutationNausea());
            _notPossessed.Add(new MutationNormality());
            _notPossessed.Add(new MutationPolyWound());
            _notPossessed.Add(new MutationProdMana());
            _notPossessed.Add(new MutationRawChaos());
            _notPossessed.Add(new MutationRteleport());
            _notPossessed.Add(new MutationScorTail());
            _notPossessed.Add(new MutationSpeedFlux());
            _notPossessed.Add(new MutationSpToHp());
            _notPossessed.Add(new MutationTentacles());
            _notPossessed.Add(new MutationTrunk());
            _notPossessed.Add(new MutationWalkShad());
            _notPossessed.Add(new MutationWarning());
            _notPossessed.Add(new MutationWasting());
            _notPossessed.Add(new MutationWeirdMind());
            _notPossessed.Add(new MutationWraith());
            foreach (Mutation mutation in _notPossessed)
            {
                mutation.Initialise();
            }
        }

        public bool HasMutations => _possessed.Count > 0;

        public List<Mutation> ActivatableMutations(Player player)
        {
            List<Mutation> list = new List<Mutation>();
            foreach (Mutation mutation in _possessed)
            {
                if (string.IsNullOrEmpty(mutation.ActivationSummary(player.Level)))
                {
                    continue;
                }
                list.Add(mutation);
            }
            return list;
        }

        public void GainMutation()
        {
            if (_notPossessed.Count == 0)
            {
                return;
            }
            Profile.Instance.MsgPrint("You change...");
            int total = 0;
            foreach (Mutation mutation in _notPossessed)
            {
                total += mutation.Frequency;
            }
            int roll = Program.Rng.DieRoll(total);
            for (int i = 0; i < _notPossessed.Count; i++)
            {
                roll -= _notPossessed[i].Frequency;
                if (roll > 0)
                {
                    continue;
                }
                Mutation mutation = _notPossessed[i];
                _notPossessed.RemoveAt(i);
                if (_possessed.Count > 0 && mutation.Group != MutationGroup.None)
                {
                    int j = 0;
                    do
                    {
                        if (_possessed[j].Group == mutation.Group)
                        {
                            Mutation other = _possessed[j];
                            _possessed.RemoveAt(j);
                            other.OnLose(this);
                            Profile.Instance.MsgPrint(other.LoseMessage);
                            _notPossessed.Add(other);
                        }
                        else
                        {
                            j++;
                        }
                    } while (j < _possessed.Count);
                }
                _possessed.Add(mutation);
                mutation.OnGain(this);
                Profile.Instance.MsgPrint(mutation.GainMessage);
                SaveGame.Instance.Player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
                SaveGame.Instance.HandleStuff();
                return;
            }
            Profile.Instance.MsgPrint("Oops! Fell out of mutation list!");
        }

        public string[] GetMutationList()
        {
            if (_possessed.Count == 0)
            {
                return new string[0];
            }
            string[] list = new string[_possessed.Count];
            for (int i = 0; i < _possessed.Count; i++)
            {
                list[i] = _possessed[i].HaveMessage;
            }
            return list;
        }

        public void LoseAllMutations()
        {
            if (_possessed.Count == 0)
            {
                return;
            }
            Profile.Instance.MsgPrint("You change...");
            do
            {
                Mutation mutation = _possessed[0];
                _possessed.RemoveAt(0);
                mutation.OnLose(this);
                _notPossessed.Add(mutation);
                Profile.Instance.MsgPrint(mutation.LoseMessage);
            } while (_possessed.Count > 0);
            SaveGame.Instance.Player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            SaveGame.Instance.HandleStuff();
        }

        public void LoseMutation()
        {
            if (_possessed.Count == 0)
            {
                return;
            }
            Profile.Instance.MsgPrint("You change...");
            int total = 0;
            foreach (Mutation mutation in _possessed)
            {
                total += mutation.Frequency;
            }
            int roll = Program.Rng.DieRoll(total);
            for (int i = 0; i < _possessed.Count; i++)
            {
                roll -= _possessed[i].Frequency;
                if (roll > 0)
                {
                    continue;
                }
                Mutation mutation = _possessed[i];
                _possessed.RemoveAt(i);
                mutation.OnLose(this);
                _notPossessed.Add(mutation);
                Profile.Instance.MsgPrint(mutation.LoseMessage);
                return;
            }
            Profile.Instance.MsgPrint("Oops! Fell out of mutation list!");
            SaveGame.Instance.Player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            SaveGame.Instance.HandleStuff();
        }

        public void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            foreach (Mutation mutation in _possessed)
            {
                mutation.OnProcessWorld(saveGame, player, level);
            }
        }
    }
}