namespace JrpgGame.Items
{
    public enum ItemType
    {
        Weapon,
        Armor,
        Consumable,
        KeyItem
    }

    public class Item
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ItemType Type { get; set; }
        public int BuyPrice { get; set; }
        public int SellPrice { get; set; }
        public int ValueModifier { get; set; } // Heal amount for consumable, Attack for weapon, Defense for armor

        public Item(string id, string name, string description, ItemType type, int buyPrice, int sellPrice, int valueModifier)
        {
            Id = id;
            Name = name;
            Description = description;
            Type = type;
            BuyPrice = buyPrice;
            SellPrice = sellPrice;
            ValueModifier = valueModifier;
        }
    }
}
