export class Item {
  constructor(id, name, desc, type, buyPrice, sellPrice, value) {
    this.id = id;
    this.name = name;
    this.desc = desc;
    this.type = type;
    this.buyPrice = buyPrice;
    this.sellPrice = sellPrice;
    this.value = value;
  }
}

export class ShopManager {
  constructor(game) {
    this.game = game;
    this.catalog = [
      new Item('hp_potion', 'Зелье Крови', 'Восстанавливает 50 HP.', 'consumable', 30, 15, 50),
      new Item('mp_potion', 'Эликсир Бездны', 'Восстанавливает 30 MP.', 'consumable', 35, 18, 30),
      new Item('dark_sword', 'Темный Клинок', 'Меч, закаленный в тенях (+12 Атака).', 'weapon', 120, 60, 12),
      new Item('cult_armor', 'Окультный Балахон', 'Балахон адепта (+8 Защита).', 'armor', 100, 50, 8)
    ];
  }

  buyItem(itemId) {
    const item = this.catalog.find(i => i.id === itemId);
    if (item && this.game.gold >= item.buyPrice) {
      this.game.gold -= item.buyPrice;
      this.game.inventory.push(item);
      return true;
    }
    return false;
  }

  sellItem(index) {
    if (index >= 0 && index < this.game.inventory.length) {
      const item = this.game.inventory[index];
      this.game.gold += item.sellPrice;
      this.game.inventory.splice(index, 1);
      return true;
    }
    return false;
  }
}
