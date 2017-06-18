using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AT.Character;
using AT.Battle;

public class ItemSlot : MonoBehaviour {
	public enum NoDropReason
	{
		//the only case this drop will be rejected, is if there is no action left and the inventory view is in battle mode.
		NO_ACTION_LEFT_FOR_UNEQUIP,
		NONE__CAN_ACTUALLY_DROP,
		CANT_UNEQUIP_TO_HERE,
	}

	public static ItemSlot currentlyDraggingSlot;

	public InventoryItem currentItem;
	public Sprite defaultPanelImage;

	public void SetInventoryItem(InventoryItem item) {
		currentItem = item;
		if (item == null) {
			GetComponent<OptButton> ().UnsetTooltipInfo ();
		} else {
			item.DressOptButtonForTooltip (GetComponent<OptButton> (), Tooltip.TooltipPosition.TOP, 5);
		}
		RefreshPanelSprite ();
	}
	public void ClearItem() {
		currentItem = null;
		RefreshPanelSprite ();
	}

	public InventoryItem CurrentItem {
		get { return currentItem; }
	}

	public virtual InventoryItem GiveUpItem() {
		InventoryItem ret = currentItem;
		ClearItem ();
		InventoryView.instance.RefreshCharacter ();
		return ret;
	}

	public void RefreshPanelSprite() {
		if (currentItem != null) {
			Sprite s = IconDispenser.instance.SpriteFromIconName(currentItem.IconType);
			PanelImage ().sprite = s;
		} else {
			PanelImage ().sprite = defaultPanelImage;
		}

		if (PanelImage ().sprite != null) {

			PanelImage ().color = new Color (1f, 1f, 1f, 1f);
		} else {
			PanelImage ().color = new Color (1f, 1f, 1f, 0f);
		}
	}


	public virtual void Start () {
		OptButton opt = GetComponent<OptButton> ();
		opt.OnOptBeganDrag += BeganDragging;
		opt.OnOptDropped += DroppedItem;
		opt.OnOptEndedDrag += EndedDraggin;


		RefreshPanelSprite ();

		if (InventoryView.instance == null) {
			Debug.LogError ("Inventory slot requires that there be an active " + typeof(InventoryView).ToString () + " singleton in the scene.");
		}
		if (EquipmentAnimationDispenser.instance == null) {
			Debug.LogError ("Inventory slot requires that there be an active " + typeof(EquipmentAnimationDispenser).ToString () + " singleton in the scene.");
		}
		if (IconDispenser.instance == null) {
			Debug.LogError ("Inventory slot requires that there be an active " + typeof(IconDispenser).ToString () + " singleton in the scene.");
		}
	}

	void BeganDragging(OptButton slot) {
		if (InventoryView.instance.interactivityDisabled)
			return;
		if(currentItem != null) {
			currentlyDraggingSlot = this;

			Sprite s = IconDispenser.instance.SpriteFromIconName(currentItem.IconType);
			PanelImage ().color = new Color (1f, 1f, 1f, .4f);
			Cursor.SetCursor( s.texture, new Vector2(s.texture.width/2, s.texture.height/2), CursorMode.Auto);
		}
	}

	//This happens after the drop happens
	void EndedDraggin(OptButton slot) {
		currentlyDraggingSlot = null;
		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		RefreshPanelSprite();
	}

	//This happens before the drag end happens.
	public virtual void DroppedItem(OptButton opt) {
		if (currentlyDraggingSlot != null) {
			
			bool updated = false;
			NoDropReason reason = DropRestrictReasonItemSlot (currentlyDraggingSlot.CurrentItem, currentlyDraggingSlot);
			switch (reason) {
			case NoDropReason.CANT_UNEQUIP_TO_HERE:
				Debug.Log ("shit, can unequip here ni");
				break;
			case NoDropReason.NO_ACTION_LEFT_FOR_UNEQUIP:
				Debug.Log ("No action left my friend.");
				break;
			case NoDropReason.NONE__CAN_ACTUALLY_DROP:
				Debug.Log ("You can drop!");


				if (currentlyDraggingSlot is EquipmentUISlot) { //slot is an equipment slot

					EquipmentUISlot sl = currentlyDraggingSlot as EquipmentUISlot;
					if (InventoryView.instance.IsBattleMode) {
						//actions need to be performed...


						if (CurrentItem == null) {
							AT.Battle.Unequip action = BattleUnequip (sl.slotType);
							action.Perform ();
						} else {
							AT.Battle.Equip action = BattleEquip (CurrentItem, sl.slotType);
							action.Perform ();
						}
						InventoryView.instance.RefreshUi ();
					} else {
						//The state can just be set.


						if (CurrentItem == null) {
//							AT.Battle.Unequip action = BattleUnequip (sl.slotType);
//							Debug.Log("here, will unequp");

							SetInventoryItem (currentlyDraggingSlot.CurrentItem);

							currentlyDraggingSlot.SetInventoryItem (null);

						} else {
							InventoryItem swap = CurrentItem;

							InventoryView.instance.currentCharacter.Unequip (
								(currentlyDraggingSlot as EquipmentUISlot).slotType, 
								currentlyDraggingSlot.CurrentItem as Equipment);
							SetInventoryItem (currentlyDraggingSlot.CurrentItem);
							currentlyDraggingSlot.SetInventoryItem (swap);
						}
						InventoryView.instance.RefreshCharacter();
					}
				} else { ///TODO: add case for if the slot is a ground slot (only valid in battle)
					InventoryItem swap = CurrentItem;
					SetInventoryItem (currentlyDraggingSlot.CurrentItem);
					currentlyDraggingSlot.SetInventoryItem (swap);
					InventoryView.instance.RefreshCharacter ();
				}
				break;
			}
		}
	}

	//context: item was dropped on this slot.  Ite might be equipment or another item slot
	public NoDropReason DropRestrictReasonItemSlot(InventoryItem item, ItemSlot other) {
		if (other is EquipmentUISlot) {
			if (InventoryView.instance.IsBattleMode &&
			    InventoryView.instance.CurrentActor.UsedAction () &&
			    InventoryView.instance.CurrentActor.UsedInteraction ()) {
				return NoDropReason.NO_ACTION_LEFT_FOR_UNEQUIP;
			}

			if (CurrentItem == null) {
				return NoDropReason.NONE__CAN_ACTUALLY_DROP;
			}

			EquipmentUISlot otherSlot = other as EquipmentUISlot;
			//check if my item is compatible with the slot.

										

			if ((CurrentItem as Equipment).FittingSlotTypes.Contains (otherSlot.slotType)) {
				
				return NoDropReason.NONE__CAN_ACTUALLY_DROP;

			} else {
				return NoDropReason.CANT_UNEQUIP_TO_HERE;
			}
		} else {
			return NoDropReason.NONE__CAN_ACTUALLY_DROP;
		}
	}


	public AT.Battle.Equip BattleEquip (InventoryItem i, EquipmentSlotType t) {
		AT.Battle.Equip action = new AT.Battle.Equip (InventoryView.instance.CurrentActor);
		(action.ActionOptions [0] as ItemsOnPersonOption).chosenChoice = new InventoryItemChoice (i);
		(action.ActionOptions [1] as EquipmentSlotTypeOption).chosenChoice = new EquipmentSlotTypeChoice (t);
		return action;
	}

	public AT.Battle.Unequip BattleUnequip (EquipmentSlotType t) {
		AT.Battle.Unequip action = new AT.Battle.Unequip (InventoryView.instance.CurrentActor);
		(action.ActionOptions [0] as EquipmentSlotTypeOption).chosenChoice = new EquipmentSlotTypeChoice (t);

		return action;
	}

	public Image PanelImage() {
		return transform.GetChild (0).GetComponent<Image> ();
	}

}
