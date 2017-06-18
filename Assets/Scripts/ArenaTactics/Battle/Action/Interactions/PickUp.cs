using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AT.Character;
using AT.Character.Situation;


namespace AT {

namespace Battle {  
	public class Interaction : Action {
		public Interaction(Actor actor =null) : base(actor) {
			this.IsInteraction = true;
			OnBegan += (a) => {
				if(a.actor.UsedInteraction()) {
					Debug.Log("SIR YOU MUST PAY YOUR ACTION!");
					a.IsInteraction = false;
				}
			};
		}
	}

	public class PickUp : Interaction {
		public override void DecorateOption(ActionButtonNode n) {
				n.spriteLabel = IconDispenser.instance.SpriteFromIconName (IconName.PICK_UP);
		}



		public PickUp(Actor actor=null) : base(actor) {
			this.ActionOptions.Add (new ItemPickupOption ());
		}





		public override void Perform() {
			CallOnBegan ();

			ItemPickupOption opt = ActionOptions [0] as ItemPickupOption;

			InventoryItemChoice choice = opt.chosenChoice as InventoryItemChoice;

			actor.CharSheet.inventory.AddItem (choice.item);

			TileMovement tm = actor.GetComponent<TileMovement> ();
			tm.occupying.RemoveItemFromGround (choice.item);

			CallOnFinished ();
		}

	}

}
}