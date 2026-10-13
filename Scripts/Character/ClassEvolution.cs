using System;
using System.Collections.Generic;

namespace JrpgGame.Character
{
    public class ClassEvolution
    {
        public static int GetExpForNextLevel(int currentLevel)
        {
            return currentLevel * 100;
        }

        public static bool AddExperience(CharacterData character, int expGained, out bool leveledUp, out bool unlockedSubclassChoice)
        {
            leveledUp = false;
            unlockedSubclassChoice = false;

            if (character == null) return false;

            character.Experience += expGained;
            int reqExp = GetExpForNextLevel(character.Level);

            while (character.Experience >= reqExp)
            {
                character.Experience -= reqExp;
                character.Level++;
                leveledUp = true;

                // Level up stat growth
                character.BaseStats.Strength += 2;
                character.BaseStats.Agility += 2;
                character.BaseStats.Intelligence += 2;
                character.BaseStats.Vitality += 2;

                character.ResetHealthAndManaToMax();

                if (character.Level >= 10 && string.IsNullOrEmpty(character.Subclass))
                {
                    unlockedSubclassChoice = true;
                }

                reqExp = GetExpForNextLevel(character.Level);
            }

            return leveledUp;
        }

        public static List<string> GetAvailableSubclasses(string archetype)
        {
            return archetype switch
            {
                ClassNames.Warrior => new List<string> { ClassNames.Berserker, ClassNames.Paladin },
                ClassNames.Mage => new List<string> { ClassNames.Pyromancer, ClassNames.Necromancer },
                ClassNames.Rogue => new List<string> { ClassNames.Shadow, ClassNames.Assassin },
                _ => new List<string>()
            };
        }

        public static bool SelectSubclass(CharacterData character, string subclass)
        {
            if (character.Level < 10) return false;
            var available = GetAvailableSubclasses(character.Archetype);
            if (!available.Contains(subclass)) return false;

            character.Subclass = subclass;
            return true;
        }

        public static List<Skill> GetAvailableSkills()
        {
            return new List<Skill>
            {
                // Basic Skills
                new Skill("heavy_strike", "Тяжелый удар", "Наносит повышенный физический урон.", 5, 25, SkillType.Damage, 1),
                new Skill("fireball", "Огненный шар", "Наносит магический урон огнем.", 8, 30, SkillType.Damage, 1),
                new Skill("quick_stab", "Быстрый укол", "Быстрая атака с высокой точностью.", 4, 20, SkillType.Damage, 1),
                new Skill("heal", "Лечение", "Восстанавливает здоровье целевому персонажу.", 10, 35, SkillType.Heal, 2),

                // Subclass Skills (Level 10+)
                new Skill("berserk_rage", "Ярость Берсерка", "Мощнейший физический урон.", 15, 60, SkillType.Damage, 10, ClassNames.Berserker),
                new Skill("holy_shield", "Святой щит / Касание", "Восстанавливает здоровье светлой магией.", 20, 70, SkillType.Heal, 10, ClassNames.Paladin),
                new Skill("inferno", "Инферно", "Испепеляющее пламя.", 25, 80, SkillType.Damage, 10, ClassNames.Pyromancer),
                new Skill("drain_life", "Высасывание жизни", "Темное заклинание наносящее урон и лечащее.", 20, 50, SkillType.Damage, 10, ClassNames.Necromancer),
                new Skill("shadow_strike", "Удар из тени", "Смертоносный урон из укрытия.", 18, 75, SkillType.Damage, 10, ClassNames.Shadow)
            };
        }

        public static List<Skill> GetLearnableSkillsForCharacter(CharacterData character)
        {
            var result = new List<Skill>();
            foreach (var skill in GetAvailableSkills())
            {
                if (character.Level >= skill.RequiredLevel)
                {
                    if (string.IsNullOrEmpty(skill.RequiredSubclass) || skill.RequiredSubclass == character.Subclass)
                    {
                        result.Add(skill);
                    }
                }
            }
            return result;
        }
    }
}
