using System.Collections.Generic;

namespace JrpgGame.Items
{
    public class ShopManager
    {
        public List<Item> ShopCatalog { get; set; } = new List<Item>();

        public ShopManager()
        {
            // Initialize default shop stock
            ShopCatalog.Add(new Item("potion_hp", "Зелье здоровья", "Восстанавливает 50 HP.", ItemType.Consumable, 20, 10, 50));
            ShopCatalog.Add(new Item("potion_mp", "Зелье маны", "Восстанавливает 30 MP.", ItemType.Consumable, 25, 12, 30));
            ShopCatalog.Add(new Item("iron_sword", "Железный меч", "Обычный надежный меч (+10 Атака).", ItemType.Weapon, 100, 50, 10));
            ShopCatalog.Add(new Item("leather_armor", "Кожаная броня", "Легкая броня (+5 Защита).", ItemType.Armor, 80, 40, 5));
        }

        public bool BuyItem(InventoryManager inventory, Item item)
        {
            if (inventory == null || item == null) return false;
            if (inventory.Gold >= item.BuyPrice)
            {
                inventory.Gold -= item.BuyPrice;
                inventory.AddItem(item);
                return true;
            }
            return false;
        }

        public bool SellItem(InventoryManager inventory, Item item)
        {
            if (inventory == null || item == null) return false;
            if (inventory.HasItem(item.Id))
            {
                inventory.RemoveItem(item);
                inventory.Gold += item.SellPrice;
                return true;
            }
            return false;
        }
    }
}
