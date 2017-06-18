using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT.Character;

public class InventoryDisplay : MonoBehaviour {

	public Transform inventorySlotParent;

	void Awake() {

	}
	void Start() {
		PopulateInventorySlots ();
	}

	private void PopulateInventorySlots() {
//		InventoryView.instance.Show ();
		GameObject original = inventorySlotParent.transform.GetChild (0).gameObject;
		for (int i = 1; i < Inventory.MAX_INVENTORY_SLOTS; i++) {
			Instantiate (original, inventorySlotParent.transform);
		}
//		InventoryView.instance.Hide();
	}

	/// <summary>
	/// Syncs the user interface to inventory of current character.
	/// </summary>
	/// <param name="inv">Inv.</param>
	public void SyncUiWithCharacter(Sheet character) {
		for (int i = 0; i < character.inventory.items.Length; i++) {
			if (character.inventory.items [i] != null) {
				//get the icon from the dispenser to apply to the slot.
				SetInventorySlotSprite (i, 
					character.inventory.items [i]);
			} else {

				ItemSlot slot =  GetInventorySlot (i);
				if (slot != null)
					slot.ClearItem ();
			}
		}
	}


	public void SetInventorySlotSprite(int index, InventoryItem item) {
		if (index >= inventorySlotParent.transform.childCount) {
			Debug.LogError ("Trying to set item " + item.Name + " at non-existent index: " + index);
			return;
		}
		Transform invItemSlot = inventorySlotParent.transform.GetChild (index);
		ItemSlot sl = invItemSlot.GetComponent<ItemSlot> ();
		sl.SetInventoryItem (item);
	}

	public ItemSlot FirstFreeSlot() {
		for (int i = 0; i < inventorySlotParent.transform.childCount; i++) {
			if (inventorySlotParent.transform.GetChild (i).GetComponent<ItemSlot> ().CurrentItem == null) {
				return inventorySlotParent.transform.GetChild (i).GetComponent<ItemSlot> ();
			}
		}
		return null;
	}



	public ItemSlot GetInventorySlot(int index) {
		if (index >= inventorySlotParent.transform.childCount)
			return null;
		
		Transform invItemSlot = inventorySlotParent.transform.GetChild (index);
		ItemSlot sl = invItemSlot.GetComponent<ItemSlot> ();
		return sl;
	}

	public void AddToInventory(InventoryItem item) {
		FirstFreeSlot ().SetInventoryItem (item);
	} 


}
