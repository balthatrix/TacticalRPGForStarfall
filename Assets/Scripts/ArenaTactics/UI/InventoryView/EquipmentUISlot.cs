using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT.Character;
using AT.Battle;

public class EquipmentUISlot : ItemSlot {
	public EquipmentSlotType slotType;

	public override void Start() {
		base.Start ();

		OptButton opt = GetComponent<OptButton> ();
		opt.OnOptMousedOver += IndicateDroppable;
		opt.OnOptMousedOut += UnindicateDroppable;

	}


	public void IndicateDroppable(OptButton self) {	
		if (currentlyDraggingSlot != null) {
			NoDropReason reason = DropRestrictReasonEquipSlot(currentlyDraggingSlot.CurrentItem);
			switch (reason) {
			case NoDropReason.NONE__CAN_ACTUALLY_DROP:
				if (InventoryView.instance.IsBattleMode) {
					string tip = "Equip " + currentlyDraggingSlot.CurrentItem.Name;
					if (InventoryView.instance.CurrentActor.UsedInteraction ()) {
						tip += " (Action)";
					} else {
						tip += " (Interaction)";
					}
					UIManager.instance.Tooltip.SetText (tip);
					UIManager.instance.Tooltip.Show (transform as RectTransform);
				}
				break;
			case NoDropReason.NOT_EQUIPMENT:
				UIManager.instance.Tooltip.SetText (currentlyDraggingSlot.CurrentItem.Name  + " is not equipment");
				UIManager.instance.Tooltip.Show (transform as RectTransform);
				break;
			case NoDropReason.NO_FIT: 
				UIManager.instance.Tooltip.SetText (currentlyDraggingSlot.CurrentItem.Name  + " doesn't fit here");
				UIManager.instance.Tooltip.Show (transform as RectTransform);
				break;
			case NoDropReason.NOT_YOUR_TURN:
				UIManager.instance.Tooltip.SetText ("Not " + InventoryView.instance.CurrentActor.CharSheet.Name + "'s turn");
				UIManager.instance.Tooltip.Show (transform as RectTransform);
				break;

			case NoDropReason.NO_ACTION_LEFT:
				UIManager.instance.Tooltip.SetText ("No actions left");
				UIManager.instance.Tooltip.Show (transform as RectTransform);
				break;
			}

		}
	}

	public void UnindicateDroppable(OptButton self) {
		if (currentlyDraggingSlot != null) {
			//TODO: cancel the illustration that you may NOT drop here.

		}
	}

	/// <summary>
	/// Checks if the item slot dropped can be droppped, and acts accordingly. 
	/// Look at ItemSlot subclass to find see what functionality this overrides,
	/// and if you are curious how the cursor is unset by the item dragged.
	/// </summary>
	/// <param name="self">Self.</param>
	public override void DroppedItem(OptButton self) {		
		if (currentlyDraggingSlot != null) {
			bool updated = false;
			NoDropReason reason = DropRestrictReasonEquipSlot (currentlyDraggingSlot.CurrentItem, currentlyDraggingSlot);
			switch (reason) {
			case NoDropReason.NONE__CAN_ACTUALLY_DROP:
				//if battle mode...
				if (InventoryView.instance.IsBattleMode) {
					AT.Battle.Equip action = new AT.Battle.Equip (InventoryView.instance.CurrentActor);
					(action.ActionOptions [0] as ItemsOnPersonOption).chosenChoice = new InventoryItemChoice (currentlyDraggingSlot.CurrentItem);
					(action.ActionOptions [1] as EquipmentSlotTypeOption).chosenChoice = new EquipmentSlotTypeChoice (slotType);
					action.Perform ();
					if (CurrentItem != null) {
						InventoryItem swap = CurrentItem;
						SetInventoryItem (currentlyDraggingSlot.CurrentItem);
						currentlyDraggingSlot.SetInventoryItem (swap);
					} else {
						SetInventoryItem (currentlyDraggingSlot.CurrentItem);
						currentlyDraggingSlot.ClearItem ();	
					}
				} else {
					if (CurrentItem != null) {
						if (currentlyDraggingSlot is EquipmentUISlot ) {
							EquipmentUISlot sl = currentlyDraggingSlot as EquipmentUISlot;
							if (sl.EquipmentFits (CurrentItem as Equipment)) {
								InventoryItem swap = currentlyDraggingSlot.CurrentItem;
								currentlyDraggingSlot.SetInventoryItem (CurrentItem);
								SetInventoryItem (swap);
							} else {
								InventoryView.instance.inventoryDisplay.AddToInventory (CurrentItem);
								SetInventoryItem (currentlyDraggingSlot.CurrentItem);
								currentlyDraggingSlot.ClearItem ();
							}
//							
////							
//
//							SetInventoryItem (swap);
						} else {
//							InventoryItem swap = currentlyDraggingSlot.CurrentItem;
//							currentlyDraggingSlot.SetInventoryItem (CurrentItem);
//							InventoryView.instance.inventoryDisplay.AddToInventory (CurrentItem);
							InventoryItem swap = currentlyDraggingSlot.CurrentItem;
							currentlyDraggingSlot.SetInventoryItem (CurrentItem);
							SetInventoryItem (swap);
						}


					} else {
						//just set the state as needed directly
						SetInventoryItem (currentlyDraggingSlot.CurrentItem);
						currentlyDraggingSlot.ClearItem ();


//						InventoryView.instance.RefreshUi ();
					}

				}
				updated = true;
				break;
			case NoDropReason.NOT_EQUIPMENT:
				Debug.Log ("you can't drop non-equipmnet on a equpment slot");
				break;
			case NoDropReason.NO_FIT: 
				Debug.Log ((currentlyDraggingSlot.CurrentItem as Equipment).DefaultFitSlotType + "  doesn't match " + slotType);
				break;
			case NoDropReason.NOT_YOUR_TURN:
				Debug.Log ("Not your turn, my sone");
				break;
			
			case NoDropReason.NO_ACTION_LEFT:
				Debug.Log ("You can't act");
				break;
			}

			if (updated && InventoryView.instance.IsBattleMode) {
				//in this case, the action takes care of syncing character stuff
				InventoryView.instance.RefreshUi ();
			} else {
				InventoryView.instance.RefreshCharacter ();
				
			}

			UIManager.instance.Tooltip.Hide ();
		}
	}


	public enum NoDropReason
	{
		NONE__CAN_ACTUALLY_DROP,
		NOT_EQUIPMENT,
		NOT_YOUR_TURN,
		NO_ACTION_LEFT,
		NO_INVENTORY_SPACE,
		NO_FIT
	}

	public NoDropReason DropRestrictReasonEquipSlot (InventoryItem i, ItemSlot dragging=null) {
		if (dragging == null) {
			dragging = currentlyDraggingSlot;
		}


		if (i is Equipment) {
			Equipment e = i as Equipment;
			if(EquipmentFits(e)) {
				if (InventoryView.instance.IsBattleMode) {
					if (InventoryView.instance.currentCharacter != BattleManager.instance.CurrentlyTakingTurn.CharSheet) {
						return NoDropReason.NOT_YOUR_TURN;
					} else if (BattleManager.instance.CurrentlyTakingTurn.UsedInteraction() &&
								BattleManager.instance.CurrentlyTakingTurn.UsedAction()) {
						return NoDropReason.NO_ACTION_LEFT;
					}
				}
				if (dragging is EquipmentUISlot) {
					//check if currently dragging is
					EquipmentUISlot sl = dragging as EquipmentUISlot;
					if ((CurrentItem == null) || !sl.EquipmentFits (CurrentItem as Equipment)) {	
						if (InventoryView.instance.currentCharacter.inventory.NoRoomLeft) {
							return NoDropReason.NO_INVENTORY_SPACE;
						}
					}
//					EquipmentUISlot.NoDropReason reasonCantSwap = sl.DropRestrictReasonEquipSlot(CurrentItem, this);
//					if (reasonCantSwap != EquipmentUISlot.NoDropReason.NONE__CAN_ACTUALLY_DROP) {
//						if (InventoryView.instance.currentCharacter.inventory.NoRoomLeft) {
//							return NoDropReason.NO_INVENTORY_SPACE;
//						}
//					}
				} 

				return NoDropReason.NONE__CAN_ACTUALLY_DROP;
			} else {
				return NoDropReason.NO_FIT;
			}

		} else {
			return NoDropReason.NOT_EQUIPMENT;
		}


	}

	public bool EquipmentFits(Equipment e) {
		return e.FittingSlotTypes.Contains (this.slotType);
	}
}
