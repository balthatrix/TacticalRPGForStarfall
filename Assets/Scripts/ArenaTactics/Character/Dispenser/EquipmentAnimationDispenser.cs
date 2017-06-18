using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using AT.Character;

namespace AT.Character {
	/// <summary>
	/// Name of the controller which contains all the overrides for this animation
	/// </summary>
	public enum EquipmentAnimationControllerName {
		HEAD_HORNEDHELMET,
		MAINHAND_LONGSWORD,
		MAINHAND_HANDAXE,
		OFFHAND_HANDAXE,
		MAINHAND_DAGGER,
		OFFHAND_DAGGER,
		BODY_CHAINARMOUR,
		BODY_LEATHERARMOUR,
		METAL_SHIELD,
		ORC_OVERRIDE,
		NOT_SET,
		BODY_ROBE
	}
}

/// <summary>
/// Equipment animations get their controllers from this.
/// Pieces of equipment should all have animation tags of type EquipmentAnimationControllerName
/// this is used to determine what animation controllers to use.
/// </summary>
public class EquipmentAnimationDispenser : MonoBehaviour {

	public NamedAnimationController[] animationStore;
	public bool paperdollSpritesSet = false;

	[System.Serializable]
	public class NamedAnimationController {
		public EquipmentAnimationControllerName name;
		public AnimatorOverrideController controller;
		public Sprite paperDollSprite;
	}

	public AnimatorOverrideController GetAnimationControllerByName(EquipmentAnimationControllerName name) {
		AnimatorOverrideController con = null;

//		con.clips[0].overrideClip.SampleAnimation
		foreach (NamedAnimationController mapping in animationStore) {
			if (mapping.name == name) {

				con = mapping.controller;
				break;
			}
		} 
		return con;
				
	}

	public Sprite GetPaperDollSpriteByName(EquipmentAnimationControllerName name) {
		Sprite ret = null;

		//		con.clips[0].overrideClip.SampleAnimation
		foreach (NamedAnimationController mapping in animationStore) {
			if (mapping.name == name) {
				//				Debug.LogError ("name returning one : " + name);
				ret = mapping.paperDollSprite;
				break;
			}
		} 


		return ret;

	}


	public AnimatorOverrideController GetAnimationControllerByEquipmentSubtype(EquipmentAnimationControllerName name) {
		return GetAnimationControllerByName (name);
	}





	public static EquipmentAnimationDispenser instance;

	void Awake() {
		if (instance == null) {
			instance = this;
			instance.StartCoroutine (SetPaperDollImages ());
		} else {
			Destroy (this);
		}

	}


	IEnumerator SetPaperDollImages() {
		Sprite last = null;
		foreach (NamedAnimationController mapping in animationStore) {
			
			GetComponent<Animator> ().runtimeAnimatorController = mapping.controller;
			GetComponent<Animator> ().Play("Idle", 0);
			yield return new WaitForEndOfFrame ();

			Sprite proposed = GetComponent<SpriteRenderer> ().sprite;
			if (proposed != last) {

				mapping.paperDollSprite = proposed;
				last = proposed;
			} else {
				mapping.paperDollSprite = null;
			}
		}

		foreach (var val in Util.EnumUtil.GetValues<RaceName>()) {
			
		}

		paperdollSpritesSet = true;
		if (OnPaperDollImagesSet != null) {
			OnPaperDollImagesSet (this);
		}
	}

	public delegate void PaperDollImagesSetAction(EquipmentAnimationDispenser self);
	public event PaperDollImagesSetAction OnPaperDollImagesSet;
}
