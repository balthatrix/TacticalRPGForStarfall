using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT.Character;
using AT.Battle;

public class EquipmentAnimation : MonoBehaviour {

	public AnimationTransform rootAnimationTransform;
	private Animator root;

	public EquipmentAnimatorMapping[] animators;

	[System.Serializable]
	public class EquipmentAnimatorMapping {
		public EquipmentSlotType type;
		public Animator animator;
		public EquipmentAnimationControllerName current = EquipmentAnimationControllerName.NOT_SET;
	}

	public EquipmentSlotType TypeFromAnimator(Animator a) {
		foreach (EquipmentAnimatorMapping map in animators) {
			if (map.animator == a) {
				return map.type;
			}
		}
		Debug.LogError ("No type gotten from animator: " + a.name);
		return EquipmentSlotType.ERROR;
	}

	public Animator AnimatorFromEquipmentType(EquipmentSlotType t) {
		foreach (EquipmentAnimatorMapping map in animators) {
			if (map.type == t){
				return map.animator;
			}
		}
		return null;
	}

	public EquipmentAnimatorMapping MappingFromEquipmentType(EquipmentSlotType t) {
		foreach (EquipmentAnimatorMapping map in animators) {
			if (map.type == t){
				return map;
			}
		}
		return null;
	}

	public void SetMappingName(EquipmentSlotType t, EquipmentAnimationControllerName name) {
		foreach (EquipmentAnimatorMapping map in animators) {
			if (map.type == t){

				map.current = name;
				break;
			}
		}
	}

	void Awake() {
		rootAnimationTransform = GetComponent<AnimationTransform> ();
		root = rootAnimationTransform.ClosestAnimator ();
		foreach (EquipmentAnimatorMapping anim in animators) {
			anim.current = EquipmentAnimationControllerName.NOT_SET;
		}
	}

	void Start() {
		
		//StartCoroutine (TestMidDelay ());
//		Debug.LogError("refreshin anima");
		//initial syncing of animations
		RefreshAnimations();
//		RefreshAnimations();
		if (MapManager.instance != null)
			GetComponent<TileInhabitant> ().InitialOrderInhabitantLayer (MapManager.instance);
		else
			Debug.LogWarning ("Can't initial order sprites on " + name + "without a map instance");
//		SyncPaperDoll(GetComponent<AT.Battle.Actor>().CharSheet.PaperDoll);
		//ChangeOutAnimation (EquipmentSlotType.BODY_OVERRIDE, EquipmentAnimationControllerName.ORC_OVERRIDE);

		Actor a = GetComponent<Actor> ();
		Sheet character = a.CharSheet;

		//During runtime, this behavior will automatically sync when the sheet equips/unequips
		character.OnUnequipped += UnequippedUpdateMapping;
		character.OnEquipped += EquippedUpdateMapping;

	}

	public void UnequippedUpdateMapping(Sheet s, EquipmentSlotType slotType, Equipment e) {
		EquipmentAnimatorMapping map = MappingFromEquipmentType (slotType);
		if (map != null) { //can be null if the slot type is not a weapon, body, or head gear type as of 04/2017
//			Debug.Log(slotType + ": will remove");
			ClearAnimation(slotType);
			RefreshAnimations ();
		}
	}

	public void EquippedUpdateMapping(Sheet s, EquipmentSlotType slotType, Equipment e) {
		EquipmentAnimatorMapping map = MappingFromEquipmentType (slotType);
		if (map != null) { //can be null if the slot type is not a weapon, body, or head gear type as of 04/2017
//			Debug.Log(slotType + ": will add!");
			map.animator = AnimatorFromEquipmentType(slotType);
			map.current = e.AnimationControllerName;
			RefreshAnimations ();
		}
	}

	public void RefreshAnimations() {
		SyncPaperDoll(GetComponent<AT.Battle.Actor>().CharSheet.PaperDoll);


	}

	bool syncing = false;
	//This prevents multiple sync requests per frame to 'stack'.  only one sync per frame should happen.
	void SyncPaperDoll(PaperDoll pd) {
		if (syncing)
			return;
		StartCoroutine(ReallySync(pd));
	}


	IEnumerator ReallySync(PaperDoll pd) {
		yield return new WaitForEndOfFrame ();
		foreach (EquipmentAnimatorMapping mapping in animators) {
			Equipment equipped = null;
//			Debug.Log ("Syncing " + mapping.type + " animation " + " pd slots size: " + pd.slots.Count);
			if (pd.slots.TryGetValue (mapping.type, out equipped)) {
//				if (equipped == null)
//					Debug.LogError ("equipped is null for  " + mapping.type);
//				else
//					Debug.Log ("equipped is not null for " + mapping.type + ". YAY!");
				if (equipped is GenericWeapon && mapping.type == EquipmentSlotType.OFF_HAND) {
					GenericWeapon wep = equipped as GenericWeapon;
					ChangeOutAnimation (mapping.type, wep.OffhandAnimationControllerName);	
				} else {
					ChangeOutAnimation (mapping.type, equipped.AnimationControllerName);	
				}
			} else {

				ClearAnimation (mapping.type);
			}
		}

		if (GetComponent<AT.Battle.Actor> ().CharSheet.race != null) { //can be null in some test apps.
//			Debug.Log("SYCNGIL!: " + GetComponent<AT.Battle.Actor> ().CharSheet.race.BodyAnimationOverride);
			ChangeOutAnimation (EquipmentSlotType.BODY_OVERRIDE, GetComponent<AT.Battle.Actor> ().CharSheet.race.BodyAnimationOverride);
		} else {
			Debug.LogWarning (GetComponent<AT.Battle.Actor> ().CharSheet.Name + " has no race.  Are you just testing something? ");
		}

		syncing = false;
	}

	IEnumerator TestMidDelay() {
		yield return new WaitForSeconds (1f);

//		AnimatorStateInfo info = root.GetCurrentAnimatorStateInfo (0);
//		ChangeOutAnimation(EquipmentSlotType.MAIN_HAND, EquipmentAnimationControllerName.MAINHAND_HANDAXE);
//		ChangeOutAnimation(EquipmentSlotType.HEAD, EquipmentAnimationControllerName.HEAD_HORNEDHELMET);
//		ChangeOutAnimation(EquipmentSlotType.ARMOUR, EquipmentAnimationControllerName.BODY_CHAINARMOUR);
	}

	public void ChangeOutAnimation(EquipmentSlotType slot, EquipmentAnimationControllerName name) {
		StartCoroutine (EquipmentSwap (slot, name));
	}

	/// <summary>
	/// Swaps equipment out, and syncs it with whatever is currently playing by setting the normalized time.
	/// </summary>
	private IEnumerator EquipmentSwap(EquipmentSlotType slot, EquipmentAnimationControllerName name) {
		rootAnimationTransform.Pause ();
		AnimatorStateInfo info = root.GetCurrentAnimatorStateInfo (0);
		Animator anim = AnimatorFromEquipmentType (slot);

		SetMappingName (slot, name);
		RuntimeAnimatorController rt = EquipmentAnimationDispenser.instance.GetAnimationControllerByName (name);
//		Debug.Log ("swapping "+slot+" animation to this : " + rt.name);
		anim.runtimeAnimatorController = rt;
		anim.Play(rootAnimationTransform.LastPlay, 0, info.normalizedTime);
		yield return new WaitForEndOfFrame ();
		rootAnimationTransform.Unpause ();
	}

	private void ClearAnimation(EquipmentSlotType slot) {
		Animator anim = AnimatorFromEquipmentType (slot);
		EquipmentAnimatorMapping map =  MappingFromEquipmentType (slot);
		map.current = EquipmentAnimationControllerName.NOT_SET;

		anim.runtimeAnimatorController = null;
		anim.GetComponent<SpriteRenderer> ().sprite = null;
	}

}
