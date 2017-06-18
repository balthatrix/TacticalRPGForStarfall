using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT.Character;
using AT.Battle;
using System.Linq;

namespace AT.Battle {
	/// <summary>
	/// Represents the choices an actor can make about what type of attack will be made....
	/// </summary>
	public class ItemPickupOption : ActionOption {
		public ItemPickupOption () : base() {
		}

		public override List<IActionOptionChoice> GetChoicesUnfiltered(Actor actor, Action pickup) {

			if (actor.CharSheet.inventory.NoRoomLeft) {
				lastReasonForNoChoices = "No room left for any items.";
				return new List<IActionOptionChoice> ();
			}

			TileMovement tm = actor.GetComponent<TileMovement> ();
			List<IActionOptionChoice> ret = null;
			if (tm.occupying == null) {
				ret = new List<IActionOptionChoice> ();
			} else {
				//Debug.LogError (tm.occupying.onTheGround.Count);
				ret = tm.occupying.OnTheGround.Select ((item) => new InventoryItemChoice (item) as IActionOptionChoice).ToList();
			}


			if (ret.Count == 0) {
				lastReasonForNoChoices = "No items on the ground.";
			}

			return ret;
		}
	}


	/// <summary>
	///  Attack type choice. Represents the choice that an entity can, or has made about the type of attack that will be made
	/// </summary>
	public class InventoryItemChoice : IActionOptionChoice {


		public InventoryItem item;

		public InventoryItemChoice(InventoryItem item) {
			this.item = item;
		}

		public string ValueLabel() {
			return item.Name;
		}
		public void DecorateOption(ActionButtonNode n) {
//			Debug.LogWarning (GetType () + " didn't override decorate option.");
		}

	}


}