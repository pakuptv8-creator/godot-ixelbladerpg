using System;
using System.Collections.Generic;

namespace JrpgGame.Character
{
    public class CharacterData
    {
        public string Name { get; set; } = "Герой";
        public RaceData Race { get; set; }
        public List<TraitData> Traits { get; set; } = new List<TraitData>();

        public Stats BaseStats { get; set; } = new Stats(10, 10, 10, 10);

        public int Level { get; set; } = 1;
        public int Experience { get; set; } = 0;

        public int CurrentHp { get; set; }
        public int CurrentMp { get; set; }

        public string Archetype { get; set; } = "Воин"; // Воин, Маг, Разбойник
        public string Subclass { get; set; } = ""; // e.g., Берсерк, Паладин, Пиромант, Некромант, Тень

        public List<string> LearnedSkills { get; set; } = new List<string>();

        public CharacterData(string name, RaceData race, List<TraitData> traits)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "Герой" : name;
            Race = race ?? RaceData.GetAvailableRaces()[0];
            if (traits != null)
            {
                foreach (var t in traits)
                {
                    if (t != null) Traits.Add(t);
                }
            }

            ResetHealthAndManaToMax();
        }

        public Stats GetTotalStats()
        {
            Stats total = BaseStats != null ? BaseStats.Clone() : new Stats(10, 10, 10, 10);
            if (Race?.StatBonus != null)
            {
                total += Race.StatBonus;
            }

            if (Traits != null)
            {
                foreach (var trait in Traits)
                {
                    if (trait != null && trait.StatBonus != null)
                    {
                        total += trait.StatBonus;
                    }
                }
            }

            return total;
        }

        public int GetTotalMaxHp()
        {
            return GetTotalStats().MaxHp;
        }

        public int GetTotalMaxMp()
        {
            return GetTotalStats().MaxMp;
        }

        public int GetTotalPhysicalAttack()
        {
            int baseAtk = GetTotalStats().PhysicalAttack;
            if (Traits != null)
            {
                foreach (var trait in Traits)
                {
                    if (trait != null) baseAtk -= trait.AttackPenalty;
                }
            }
            return Math.Max(1, baseAtk);
        }

        public float GetTotalEvasion()
        {
            float evasion = GetTotalStats().Evasion;
            if (Traits != null)
            {
                foreach (var trait in Traits)
                {
                    if (trait != null) evasion += trait.EvasionBonus;
                }
            }
            return evasion;
        }

        public void ResetHealthAndManaToMax()
        {
            CurrentHp = GetTotalMaxHp();
            CurrentMp = GetTotalMaxMp();
        }
    }
}
