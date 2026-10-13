using System.Collections.Generic;

namespace JrpgGame.Character
{
    public class TraitData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Stats StatBonus { get; set; }
        public float EvasionBonus { get; set; }
        public int AttackPenalty { get; set; }

        public TraitData(string name, string description, Stats statBonus, float evasionBonus = 0f, int attackPenalty = 0)
        {
            Name = name;
            Description = description;
            StatBonus = statBonus ?? new Stats(0, 0, 0, 0);
            EvasionBonus = evasionBonus;
            AttackPenalty = attackPenalty;
        }

        public static List<TraitData> GetAvailableTraits()
        {
            return new List<TraitData>
            {
                new TraitData("Скрытный", "Бонус к увороту (+10%), но небольшое снижение силы атаки.", new Stats(0, 3, 0, 0), 0.10f, 2),
                new TraitData("Богатырь", "Повышенная сила и здоровье.", new Stats(4, 0, 0, 3)),
                new TraitData("Мудрец", "Повышенный интеллект и мана.", new Stats(0, 0, 5, 1))
            };
        }
    }
}
