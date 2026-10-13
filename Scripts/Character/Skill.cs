namespace JrpgGame.Character
{
    public enum SkillType
    {
        Damage,
        Heal,
        Buff
    }

    public class Skill
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MpCost { get; set; }
        public int Power { get; set; }
        public SkillType Type { get; set; }
        public int RequiredLevel { get; set; }
        public string RequiredSubclass { get; set; }

        public Skill(string id, string name, string description, int mpCost, int power, SkillType type, int requiredLevel = 1, string requiredSubclass = "")
        {
            Id = id;
            Name = name;
            Description = description;
            MpCost = mpCost;
            Power = power;
            Type = type;
            RequiredLevel = requiredLevel;
            RequiredSubclass = requiredSubclass;
        }
    }
}
