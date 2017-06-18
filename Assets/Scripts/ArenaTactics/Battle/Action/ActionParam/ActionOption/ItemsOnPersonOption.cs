using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT.Character;
using AT.Battle;
using System.Linq;

namespace AT.Battle {
	public class ItemsOnPersonOption : ActionOption {
		public ItemsOnPersonOption () : base() {
		}

		public override List<IActionOptionChoice> GetChoicesUnfiltered(Actor actor, Action pickup) {



	//		TileMovement tm = actor.GetComponent<TileMovement> ();
			List<IActionOptionChoice> ret = actor.CharSheet.inventory.ListOfItems()
				.Select ((item) => new InventoryItemChoice (item) as IActionOptionChoice).ToList();

			List<Equipment> equipment = actor.CharSheet.PaperDoll.slots.Values.ToList();
			List<IActionOptionChoice> equipmentChoices = equipment.Select ((e) => {
				InventoryItemChoice ch = new InventoryItemChoice (e as InventoryItem);
				return ch as IActionOptionChoice;
			}).ToList();

			ret = ret.Concat(equipmentChoices).ToList();

			if (ret.Count == 0) {
				lastReasonForNoChoices = "No items on person.";
			}

			return ret;
		}
	}
}