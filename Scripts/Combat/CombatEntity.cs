using System;
using JrpgGame.Character;

namespace JrpgGame.Combat
{
    public class CombatEntity
    {
        public string Name { get; set; }
        public bool IsPlayer { get; set; }
        public int Level { get; set; }
        public int Hp { get; set; }
        public int MaxHp { get; set; }
        public int Mp { get; set; }
        public int MaxMp { get; set; }
        public int Speed { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public float Evasion { get; set; }
        public bool IsDefending { get; set; }
        public bool IsAlive => Hp > 0;

        public CharacterData? SourceCharacter { get; set; }

        public CombatEntity(string name, bool isPlayer, int hp, int mp, int speed, int attack, int defense, float evasion = 0.05f)
        {
            Name = name;
            IsPlayer = isPlayer;
            Hp = hp;
            MaxHp = hp;
            Mp = mp;
            MaxMp = mp;
            Speed = speed;
            Attack = attack;
            Defense = defense;
            Evasion = evasion;
        }

        public CombatEntity(string name, bool isPlayer, int hp, int maxHp, int mp, int maxMp, int speed, int attack, int defense, float evasion = 0.05f)
        {
            Name = name;
            IsPlayer = isPlayer;
            Hp = hp;
            MaxHp = maxHp;
            Mp = mp;
            MaxMp = maxMp;
            Speed = speed;
            Attack = attack;
            Defense = defense;
            Evasion = evasion;
        }

        public static CombatEntity FromCharacter(CharacterData character)
        {
            var stats = character.GetTotalStats();
            var entity = new CombatEntity(
                character.Name,
                true,
                character.CurrentHp,
                character.GetTotalMaxHp(),
                character.CurrentMp,
                character.GetTotalMaxMp(),
                stats.Speed,
                character.GetTotalPhysicalAttack(),
                stats.Defense,
                character.GetTotalEvasion()
            )
            {
                Level = character.Level,
                SourceCharacter = character
            };
            return entity;
        }

        public void TakeDamage(int rawDamage)
        {
            int effectiveDef = IsDefending ? Defense * 2 : Defense;
            int actualDamage = Math.Max(1, rawDamage - effectiveDef);
            Hp = Math.Max(0, Hp - actualDamage);
        }

        public void Heal(int amount)
        {
            Hp = Math.Min(MaxHp, Hp + amount);
        }

        public bool SpendMp(int amount)
        {
            if (Mp >= amount)
            {
                Mp -= amount;
                return true;
            }
            return false;
        }
    }
}
