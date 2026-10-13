using System;

namespace JrpgGame.Character
{
    public class Stats
    {
        public int Strength { get; set; }
        public int Agility { get; set; }
        public int Intelligence { get; set; }
        public int Vitality { get; set; }

        public Stats(int str = 10, int agi = 10, int intel = 10, int vit = 10)
        {
            Strength = str;
            Agility = agi;
            Intelligence = intel;
            Vitality = vit;
        }

        public int MaxHp => Vitality * 10 + 20;
        public int MaxMp => Intelligence * 8 + 10;
        public int Speed => Agility * 2;
        public int PhysicalAttack => Strength * 2 + 5;
        public int MagicAttack => Intelligence * 2 + 5;
        public int Defense => Vitality + (Strength / 2);
        public float Evasion => Agility * 0.01f;

        public static Stats operator +(Stats a, Stats b)
        {
            if (a == null) return b?.Clone() ?? new Stats(0, 0, 0, 0);
            if (b == null) return a.Clone();
            return new Stats(
                a.Strength + b.Strength,
                a.Agility + b.Agility,
                a.Intelligence + b.Intelligence,
                a.Vitality + b.Vitality
            );
        }

        public Stats Clone()
        {
            return new Stats(Strength, Agility, Intelligence, Vitality);
        }
    }
}
