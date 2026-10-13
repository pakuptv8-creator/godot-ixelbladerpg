using System.Collections.Generic;
using System.Linq;

namespace JrpgGame.Items
{
    public class InventoryManager
    {
        private static InventoryManager? _instance;
        public static InventoryManager Instance => _instance ??= new InventoryManager();

        public int Gold { get; set; } = 100;
        private readonly List<Item> _items = new List<Item>();

        public IReadOnlyList<Item> Items => _items.AsReadOnly();

        public void AddItem(Item item)
        {
            if (item != null)
            {
                _items.Add(item);
            }
        }

        public bool RemoveItem(Item item)
        {
            return _items.Remove(item);
        }

        public bool RemoveItemById(string itemId)
        {
            var item = _items.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                _items.Remove(item);
                return true;
            }
            return false;
        }

        public bool HasItem(string itemId)
        {
            return _items.Any(i => i.Id == itemId);
        }

        public int GetItemCount(string itemId)
        {
            return _items.Count(i => i.Id == itemId);
        }

        public void Clear()
        {
            _items.Clear();
            Gold = 100;
        }
    }
}
