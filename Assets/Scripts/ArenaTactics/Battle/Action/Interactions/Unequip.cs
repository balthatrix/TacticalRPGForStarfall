using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AT.Character;
using AT.Character.Situation;

using System.Linq;


namespace AT {

namespace Battle {  


	public class Unequip : Interaction {


		public Unequip(Actor actor=null) : base(actor) {
			

			//the slot to unequip
			this.ActionOptions.Add (new EquipmentSlotTypeOption ()); //Should filter based on slots that the item has.
			this.ActionOptions[0].choiceFilters.Add((choices, action) => {
				List<IActionOptionChoice> ret = new List<IActionOptionChoice>();
				foreach(IActionOptionChoice choice in choices) {
					EquipmentSlotType potential = (choice as EquipmentSlotTypeChoice).slotType;
					if(action.actor.CharSheet.PaperDoll.EquippedOn(potential) != null) {
						ret.Add(choice);
					}
				}
				if(ret.Count == 0) {
//					action. = "Nothing to de-equip";
					ActionOptions[0].lastReasonForNoChoices = "Nothing to unequip";
				}

				return ret;
			});
		}
		public override void DecorateOption(ActionButtonNode n) {
//			n.spriteLabel = IconDispenser.instance.SpriteFromIconName (IconN);
		}



		public override void Perform() {
			CallOnBegan ();

			Debug.LogError ("AM I AN INTERACTION uunnnnnnEQUOPISD???!! " + IsInteraction);



			EquipmentSlotTypeOption slotopt = ActionOptions [0] as EquipmentSlotTypeOption;
			EquipmentSlotTypeChoice slotchoice = slotopt.chosenChoice as EquipmentSlotTypeChoice;
			EquipmentSlotType targetSlot = slotchoice.slotType;

			//if there's already an item on the slot, remove it, and get the slot it's currently on, and the item.
			Equipment unequipped = actor.CharSheet.PaperDoll.EquippedOn(targetSlot);

			if (unequipped != null) {
				Debug.LogError ("unequipps");
				actor.CharSheet.Unequip (unequipped);
			} 

			actor.CharSheet.inventory.AddItem (unequipped);


			CallOnFinished ();
		}

	}

}
}