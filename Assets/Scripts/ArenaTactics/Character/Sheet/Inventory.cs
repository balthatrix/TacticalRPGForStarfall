using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT.Serialization;
using System.Linq;

namespace AT.Character {

    [System.Serializable]
	public class InventoryWrapper : Wrapper {
        public Wrapper[] items;
        public override SerializedObject GetInstance() {
            Inventory ret = new Inventory();
            ret.items = items.ToList().Select((i) => {
				if(i != null)
                	return i.GetInstance() as InventoryItem;
				else
					return null;
            }).ToArray();

            return ret;
        }
	}


	public class Inventory : SerializedObject {
		public static readonly int MAX_INVENTORY_SLOTS = 18;
		public  InventoryItem[] items;

		public Inventory() {
			items = new InventoryItem[MAX_INVENTORY_SLOTS];
		}

		public Wrapper GetSerializableWrapper() {
			InventoryWrapper ret = new InventoryWrapper ();
			ret.items = items.Select((i)=> { 
				if(i != null) 
					return i.GetSerializableWrapper();
				else 
					return null;
			}).ToArray();

			return ret;
		}

		public void AddItem(InventoryItem element) {
			if (NoRoomLeft)
				Debug.LogError ("No room left to add!");
			for (int i = 0; i < items.Length; i++) {
				if (items [i] == null) {
					items [i] = element;
					break;
				}
			}
		}

		public void RemoveItem(InventoryItem element) {
			for (int i = 0; i < items.Length; i++) {
				if (items [i] == element) {
					items [i] = null;
					break;
				}
			}
		}

		public List<InventoryItem> ListOfItems() {
			List<InventoryItem> ret = items.Where ((i) => i != null).ToList ();
			return ret;
		}


		public bool NoRoomLeft {
			get { 
				return ListOfItems ().Count >= MAX_INVENTORY_SLOTS;
			}
		}
	}

}