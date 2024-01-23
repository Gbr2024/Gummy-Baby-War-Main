using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WeirdBrothers.ThirdPersonController
{
    public class WBPlayerInventory
    {
        private List<WBItem> _itemLists;

        public WBPlayerInventory()
        {
            _itemLists = new List<WBItem>();
        }

        public void AddItem(WBItem item)
        {
            var listItem = _itemLists.Find(x => x.ItemName == item.ItemName);
            if (listItem != null)
            {
                listItem.ItemAmount += item.ItemAmount;
                return;
            }
            _itemLists.Add(item);
        }

        public void UpdateItem(WBItem item)
        {
            var listItem = _itemLists.Find(x => x.ItemName == item.ItemName);
            if (listItem != null)
            {
                listItem.ItemAmount = item.ItemAmount;
            }
        }

        public int GetAmmo(string itemName)
        {
            var listItem = _itemLists.Find(x => x.ItemName == itemName);
            if (listItem != null)
                return listItem.ItemAmount;
            return 0;
        }
    }
}