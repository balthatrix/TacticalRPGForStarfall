using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AT.Character;
using AT.Character.Situation;


namespace AT {

	namespace Battle {  
		

		public class DropItem : Interaction {



			public DropItem(Actor actor=null) : base(actor) {
				this.ActionOptions.Add (new ItemsOnPersonOption ());
			}

			public override void DecorateOption(ActionButtonNode n) {
				n.spriteLabel = IconDispenser.instance.SpriteFromIconName (IconName.DROP);
			}

			public override void Perform() {
				CallOnBegan ();

				ItemsOnPersonOption opt = ActionOptions [0] as ItemsOnPersonOption;

				InventoryItemChoice choice = opt.chosenChoice as InventoryItemChoice;

				if (actor.CharSheet.inventory.ListOfItems ().Contains (choice.item)) {
					actor.CharSheet.inventory.RemoveItem (choice.item);
				} else if (actor.CharSheet.PaperDoll.slots.ContainsValue (choice.item as Equipment)) {
					actor.CharSheet.PaperDoll.Unequip(choice.item as Equipment, actor.CharSheet);
				}



				TileMovement tm = actor.GetComponent<TileMovement> ();
				tm.occupying.AddItemToGround (choice.item);

				CallOnFinished ();
			}

		}

	}
}