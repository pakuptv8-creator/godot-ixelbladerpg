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
                new RaceData(RaceType.Human, "Человек", "Сбалансированная раса во всех аспектах.", new Stats(2, 2, 2, 2)),
                new RaceData(RaceType.Elf, "Эльф", "Высокая ловкость и интеллект, но меньшая живучесть.", new Stats(0, 4, 4, 0)),
                new RaceData(RaceType.Dwarf, "Дварф", "Высокая сила и живучесть, но меньшая ловкость.", new Stats(4, 0, 0, 4))
            };
        }
    }
}
