using System.Collections.Generic;

namespace JrpgGame.Character
{
    public enum RaceType
    {
        Human,
        Elf,
        Dwarf
    }

    public class RaceData
    {
        public RaceType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Stats StatBonus { get; set; }

        public RaceData(RaceType type, string name, string description, Stats statBonus)
        {
            Type = type;
            Name = name;
            Description = description;
            StatBonus = statBonus;
        }

        public static List<RaceData> GetAvailableRaces()
        {
            return new List<RaceData>
            {
                new RaceData(RaceType.Human, "Человек Древних Семлян", "Сбалансированная раса, стойкая к шепоту Бездны.", new Stats(2, 2, 2, 2)),
                new RaceData(RaceType.Elf, "Лунно-Лесной Эльф", "Высокая ловкость и разум, познавшие таинства Звездной Магии.", new Stats(0, 4, 4, 0)),
                new RaceData(RaceType.Dwarf, "Подземный Дварф-Кузнец", "Непревзойденная физическая сила и закаленная живучесть.", new Stats(4, 0, 0, 4))
            };
        }
    }
}
