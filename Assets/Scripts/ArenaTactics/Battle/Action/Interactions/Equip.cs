using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AT.Character;
using AT.Character.Situation;

using System.Linq;


namespace AT {

namespace Battle {  


	public class Equip : Interaction {
		
		public override void DecorateOption(ActionButtonNode n) {
			n.spriteLabel = IconDispenser.instance.SpriteFromIconName (IconName.EQUIP);
		}

		public Equip(Actor actor=null) : base(actor) {
			//the item to equip
			this.ActionOptions.Add (new ItemsOnPersonOption ()); //Should filter based on items that are equpment
			this.ActionOptions[0].choiceFilters.Add((choices, action) => {
				
				List<IActionOptionChoice> ret = choices.Where((item)=>(item as InventoryItemChoice).item is Equipment).ToList();
				if(ret.Count == 0) {
					action.ActionOptions[0].lastReasonForNoChoices = "You have no equipment";
				}
				return ret;
			});

			
			//the slot to equip it on
			this.ActionOptions.Add (new EquipmentSlotTypeOption ()); //Should filter based on slots that the item has.
			this.ActionOptions[1].choiceFilters.Add((choices, action) => {
				List<IActionOptionChoice> ret = new List<IActionOptionChoice>();
				ItemsOnPersonOption itemChoiceOpt = (action.ActionOptions[0] as ItemsOnPersonOption);
				InventoryItemChoice itemChosen = itemChoiceOpt.chosenChoice as InventoryItemChoice;
				Equipment equipmentChosen = itemChosen.item as Equipment;
				foreach(IActionOptionChoice choice in choices) {
					EquipmentSlotType potential = (choice as EquipmentSlotTypeChoice).slotType;
					if(equipmentChosen.FittingSlotTypes.Contains(potential)) {
						if(potential != action.actor.CharSheet.PaperDoll.SlotFor(equipmentChosen)) {
							ret.Add(choice);
						} 
					}
				}
				if(ret.Count == 0) {
					ActionOptions[1].lastReasonForNoChoices = "Already equipped";
				}
				
				return ret;
			});
		}


		public override void Perform() {
			CallOnBegan ();

//			Debug.LogError ("AM I AN INTERACTION EQUOPISD???!! " + IsInteraction);


			ItemsOnPersonOption itemopt = ActionOptions [0] as ItemsOnPersonOption;
			InventoryItemChoice itemchoice = itemopt.chosenChoice as InventoryItemChoice;
			Equipment equipment = itemchoice.item as Equipment;

			EquipmentSlotTypeOption slotopt = ActionOptions [1] as EquipmentSlotTypeOption;
			EquipmentSlotTypeChoice slotchoice = slotopt.chosenChoice as EquipmentSlotTypeChoice;
			EquipmentSlotType targetSlot = slotchoice.slotType;

			//if there's already an item on the slot, remove it, and get the slot it's currently on, and the item.
			Equipment unequipped = actor.CharSheet.PaperDoll.EquippedOn(targetSlot);

			if (unequipped != null) {
				actor.CharSheet.Unequip (unequipped);
			} 

			if (actor.CharSheet.inventory.ListOfItems ().Contains (itemchoice.item as Equipment)) {
				//then remove the item from inventory
				actor.CharSheet.inventory.RemoveItem (itemchoice.item);
				if (unequipped != null) {
					actor.CharSheet.inventory.AddItem (unequipped);
				}
			} else {
				//Assume the item is on the paper doll.
				EquipmentSlotType fromSlot = actor.CharSheet.PaperDoll.SlotFor(itemchoice.item as Equipment);
				actor.CharSheet.Unequip (itemchoice.item as Equipment);

				//try to equip the item unequipped on the fromSlot
				if (unequipped.FittingSlotTypes.Contains (fromSlot)) {
					actor.CharSheet.Equip (fromSlot, unequipped);
				} else {
					actor.CharSheet.inventory.AddItem (unequipped);
				}
			}

			actor.CharSheet.Equip (targetSlot, itemchoice.item as Equipment);




			CallOnFinished ();
		}

	}

}
}