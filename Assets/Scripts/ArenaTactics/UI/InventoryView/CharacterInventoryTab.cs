using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AT.Character;
using AT.Battle;

public class CharacterInventoryTab : MonoBehaviour {
	public Sheet character;
	public Text label;

	// Use this for initialization
	void Start () {
//		Debug.LogError("Stargeting as " + character.Name);
		GetComponent<OptToggle> ().OnOptLeftClicked += SwitchCharacter;
		GetComponent<OptToggle> ().OnOptDropped += TakeItem;

		GetComponent<OptToggle> ().OnOptMousedOver += IndicateDroppableness;
		GetComponent<OptToggle> ().OnOptMousedOut += RemoveIndication;

	}

	private void RemoveIndication(OptButton op) {
		UIManager.instance.Tooltip.Hide ();
	}

	private void IndicateDroppableness(OptButton btn) {
		Debug.Log ("this! " + name);
		if (character == InventoryView.instance.currentCharacter)
			return;
		
		if (ItemSlot.currentlyDraggingSlot != null) {
			NoTakeItemReason reason = GetNoTakeItemReason ();
			string tooltipText;
			switch (reason) {
			case NoTakeItemReason.GIVER_OR_TAKER_NOT_ACTING:
				tooltipText = "One of the characters involved in a trade\nneeds to be currently acting.";
				break;
			case NoTakeItemReason.GIVER_NO_INTERACTION:
				tooltipText = "You have no action or interaction left";
				break;
			case NoTakeItemReason.TAKER_NO_INTERACTION:
				tooltipText = "The receiver of this item no action or interaction left";
				break;
			case NoTakeItemReason.TAKER_NO_ROOM:
				tooltipText = "The receiver of this item has no inventory room";
				break;
			case NoTakeItemReason.OUT_OF_RANGE:
				tooltipText = "The two characters need to be within " + GiveItem.GIVE_DISTANCE + " of each other.";
				break;
			case NoTakeItemReason.NO_REASON__CAN_TAKE:
				string giverActionType = "interaction";
				string takerActionType = "interaction";
				if (GiverInDrop.UsedInteraction ()) {
					giverActionType = "action";
				}
				if (TakerInDrop.UsedInteraction ()) {
					takerActionType = "action";
				}
				tooltipText = "This will cost the following actions this round:\n" +
				InventoryView.instance.currentCharacter.Name + ": " + giverActionType + "\n"
				+ character.Name + ": " + takerActionType;
				
				break;
			default:
				tooltipText = "ERROR";
				break;
			}

			if (InventoryView.instance.IsBattleMode) {
				UIManager.instance.Tooltip.SetText (tooltipText);
			} else {
				if (reason == NoTakeItemReason.NO_REASON__CAN_TAKE)
					UIManager.instance.Tooltip.SetText ("Drop to give item");
				else
					UIManager.instance.Tooltip.SetText (tooltipText);
			}

			UIManager.instance.Tooltip.Show (transform as RectTransform, Tooltip.TooltipPosition.RIGHT);

		}
	}

	public void SwitchCharacter(OptButton opt) {
//		Debug.LogError("Switng char!");
		InventoryView.instance.SetCurrentCharacter (character);
	}

	public void TakeItem(OptButton btn) {
		if (character == InventoryView.instance.currentCharacter)
			return;
		
		if (ItemSlot.currentlyDraggingSlot != null) {
			NoTakeItemReason reason = GetNoTakeItemReason ();
			switch (reason) {
			case NoTakeItemReason.GIVER_NO_INTERACTION:
				Debug.LogError ("Giver needs interaction!");
				break;
			case NoTakeItemReason.TAKER_NO_INTERACTION:
				Debug.LogError ("TAKER needs interaction!");
				break;
			case NoTakeItemReason.TAKER_NO_ROOM:
				Debug.LogError ("TAKER has no room left!");
				break;
			case NoTakeItemReason.GIVER_OR_TAKER_NOT_ACTING:
				Debug.LogError ("Giver or taker must be acting.");
				break;
			case NoTakeItemReason.OUT_OF_RANGE:
				Debug.LogError ("Giver and taker must be within " + GiveItem.GIVE_DISTANCE + " squares of each other.");
				break;
			case NoTakeItemReason.NO_REASON__CAN_TAKE:
				Debug.Log ("You may add, my son!");

				//do action
				if (InventoryView.instance.IsBattleMode) {
					GiveItem act = GiveAndTakeAction ();
					act.Perform ();
				} else {
					character.inventory.AddItem (ItemSlot.currentlyDraggingSlot.GiveUpItem ());
				}


				InventoryView.instance.RefreshUi ();
				break;
			default:
				break;
			}
		}
	}

	private GiveItem GiveAndTakeAction() {
		GiveItem ret = new GiveItem (GiverInDrop);
		(ret.ActionOptions [0] as ItemsOnPersonOption).chosenChoice = 
			new InventoryItemChoice(ItemSlot.currentlyDraggingSlot.CurrentItem) as IActionOptionChoice;
		ret.actionTargetParameters [0].chosenTile = TakerInDrop.TileMovement.occupying;
		return ret;
	}

	public Actor GiverInDrop {
		get {
			
			return InventoryView.instance.ActorFromSheet(InventoryView.instance.currentCharacter);
		}
	}

	public Actor TakerInDrop {
		get { 
			return InventoryView.instance.ActorFromSheet (character);
		}
	}

	public NoTakeItemReason GetNoTakeItemReason() {
		
		if (InventoryView.instance.IsBattleMode) {
			Actor taker = TakerInDrop;
			Actor giver = GiverInDrop;

			if (BattleManager.instance.CurrentlyTakingTurn != taker && 
				BattleManager.instance.CurrentlyTakingTurn != giver) {
				return NoTakeItemReason.GIVER_OR_TAKER_NOT_ACTING;
			}
			if (taker.UsedInteraction () && taker.UsedAction()) {
				return NoTakeItemReason.TAKER_NO_INTERACTION;
			}
			if (giver.UsedInteraction () && giver.UsedAction()) {
				return NoTakeItemReason.GIVER_NO_INTERACTION;
			}

			if (giver.TileMovement.occupying.HCostTo (taker.TileMovement.occupying) > GiveItem.GIVE_DISTANCE) {

				return NoTakeItemReason.OUT_OF_RANGE;
			}

			if (taker.CharSheet.inventory.NoRoomLeft) {
				return NoTakeItemReason.TAKER_NO_ROOM;
			}

		}

		//if actor giving has no interaction (only applicable in battle)
		//return NoTakeItemReason.GIVER_NO_INTERACTION

		//if actor this slot is associated to has no interaction (only applicable in battle)
		//return NoTakeItemReason.TAKER_NO_INTERACTION

		//if actor this slot is associated to has no room.  (always applicable)
		//return NoTakeItemReason.TAKER_NO_ROOM
		if(character.inventory.NoRoomLeft) {
			return NoTakeItemReason.TAKER_NO_ROOM;
		}

		return NoTakeItemReason.NO_REASON__CAN_TAKE;
	}



	public enum NoTakeItemReason {
		GIVER_NO_INTERACTION,
		TAKER_NO_INTERACTION,
		TAKER_NO_ROOM,
		NO_REASON__CAN_TAKE,
		OUT_OF_RANGE,
		GIVER_OR_TAKER_NOT_ACTING
	}

}
