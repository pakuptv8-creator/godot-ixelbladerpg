namespace JrpgGame.Character
{
    public enum Archetype
    {
        Warrior,
        Mage,
        Rogue
    }

    public static class ClassNames
    {
        public const string Warrior = "Воин";
        public const string Mage = "Маг";
        public const string Rogue = "Разбойник";

        // Warrior Subclasses
        public const string Berserker = "Берсерк";
        public const string Paladin = "Паладин";

        // Mage Subclasses
        public const string Pyromancer = "Пиромант";
        public const string Necromancer = "Некромант";

        // Rogue Subclasses
        public const string Shadow = "Тень";
        public const string Assassin = "Ассасин";
    }
}
