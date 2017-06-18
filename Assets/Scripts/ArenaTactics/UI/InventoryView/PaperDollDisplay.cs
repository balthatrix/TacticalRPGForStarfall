using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AT.Character;

public class PaperDollDisplay : MonoBehaviour {
	/// <summary>
	/// Paper doll user interface class stores, associates and allows editing for paper doll elements.
	/// </summary>
	[System.Serializable] 
	public class PaperDollUi
	{
		public PaperDollUiElement[] paperDollElements;
		[System.Serializable] 
		public class PaperDollUiElement
		{
			public EquipmentSlotType slotType;
			public EquipmentUISlot uiSlot;
			public Image preview;
		}
	}
	public PaperDollUi paperDollUi;
	public Image bodyOverrideImage;
	public Sprite defaultBodySprite;
	public void SyncUiToCharacter(Sheet character) {

		//This has to happen because paper doll images are dynamically set by equipment animation dispenser
		//which takes n frames to happen, where n : number of animations...
		if (EquipmentAnimationDispenser.instance.paperdollSpritesSet) { 
			ActuallySyncUiToCharacter (character);
		} else {
			cachedChar = character;
			EquipmentAnimationDispenser.instance.OnPaperDollImagesSet += ActuallySyncUiPaperDollAfterReady;
		}



	}

	Sheet cachedChar;
	void ActuallySyncUiPaperDollAfterReady(EquipmentAnimationDispenser inst) {
		ActuallySyncUiToCharacter (cachedChar);
		EquipmentAnimationDispenser.instance.OnPaperDollImagesSet -= ActuallySyncUiPaperDollAfterReady;
	}


	void ActuallySyncUiToCharacter(Sheet character) {
		for (int i = 0; i < paperDollUi.paperDollElements.Length; i++) {
			PaperDollUi.PaperDollUiElement elem = paperDollUi.paperDollElements [i];
			elem.uiSlot.SetInventoryItem(character.PaperDoll.EquippedOn(elem.slotType));

			SetDollPreviewSprite (elem.preview, character.PaperDoll.EquippedOn (elem.slotType), elem.slotType);
		}

		if (character.race != null) {
			EquipmentAnimationControllerName rn = Race.BodyOverrideController (character.race.name);
//			Debug.Log ("race names: " + rn);
			bodyOverrideImage.sprite = EquipmentAnimationDispenser.instance.GetPaperDollSpriteByName (rn);
			if (bodyOverrideImage.sprite == null) {
				bodyOverrideImage.sprite = defaultBodySprite;
			}
		} else {
			bodyOverrideImage.sprite = defaultBodySprite;
		}
		
			
	}

	void SetDollPreviewSprite(Image img, Equipment e, EquipmentSlotType type) {
		if (e == null) {
			img.sprite = null;
		} else {
			if (type == EquipmentSlotType.OFF_HAND && e is GenericWeapon) {
				img.sprite = EquipmentAnimationDispenser.instance.GetPaperDollSpriteByName ((e as GenericWeapon).OffhandAnimationControllerName);
//				img.transform.localScale = new Vector3 (-1f, 1f, 1f);
			} else {
				img.sprite = EquipmentAnimationDispenser.instance.GetPaperDollSpriteByName (e.AnimationControllerName);
//				img.transform.localScale = new Vector3 (1f, 1f, 1f);
			}
		}


		if (img.sprite == null) {
			img.color = new Color (1f, 1f, 1f, 0f);
		} else {
			img.color = new Color (1f, 1f, 1f, 1f);
		} 
	}


}
