using UnityEngine;
using System.Collections;
using AT.Battle;
using AT.Character;
using AT;
using System.Collections.Generic;
using System.Linq;

[RequireComponent (typeof(TileMovement))]
[RequireComponent (typeof(AnimationTransform))]
public class CharacterAttacker : MonoBehaviour {
	TileMovement tileMovement;
	Animator animator;
	AnimationTransform animationTransform;


	private AudioSource characterAudioSource;


	void Awake() {
		animationTransform = GetComponent<AnimationTransform> ();
		tileMovement = GetComponent<TileMovement> ();
		characterAudioSource = GetComponent<AudioSource>();

		animationTransform.OnAnimationEvent += HandleAnimationEvent;
	}

	void OnDestroy() {
		animationTransform.OnAnimationEvent -= HandleAnimationEvent;
	}

	// Use this for initialization
	void Start () {
		GetComponent<Actor>().OnWillPerform += SetupAttackAnimation;
	}

	public Attack lastAttack;

	public void SetupAttackAnimation(Action a) {

		if (a is Attack) {
			lastAttack = (Attack)a;

			Actor target = lastAttack.TargetActor ();
			string name = AnimNameFromTarget (target, lastAttack.TypeChoice.IsOffhand());
			DoAttackAnimation (name);
		}
	}

	string AnimNameFromTarget(Actor t, bool offhand=false) {
		string prefix = "Mainhand_";
		if (offhand)
			prefix = "Offhand_";
		Vector3 directions = t.transform.position - transform.position;

		if (Mathf.Abs (directions.x) < Mathf.Abs (directions.y)) {
			//Attacking up or down
			if (directions.y > 0f) {
				return prefix  +  "AttackUp";
			} else {
				return  prefix + "AttackDown";
			}
		} else {
			//Attacking left or right.
			if (directions.x > 0f) {
				return  prefix + "AttackRight";
			} else {
				return  prefix + "AttackLeft";
			}
		}
	}

	bool attacking = false;
	public void DoAttackAnimation(string name) {
		AttackAnimationBegan ();
		animationTransform.Play (name);
	}

	public void HandleAnimationEvent(AnimationEventType type, string animationName) {
		if (!attacking)
			return;
		switch (type) {
		case AnimationEventType.ATTACK_SWING_MOMENT:
//			if (lastAttack.TypeChoice.IsRanged () & lastAttack.WeaponUsed.IsThrown ()) {
//				//nothing
//			} else {
				WeaponSwingFXType swingFx = lastAttack.WeaponUsed.SwingSoundFX;
				characterAudioSource.clip = SoundDispenser.instance.SwingFXFromType (swingFx);
				characterAudioSource.Play ();
//			}
			break;
		case AnimationEventType.ATTACK_HIT_MOMENT:
			//this is where it should split off and launch the weapon if the attack was ranged
			if (lastAttack.TypeChoice.IsRanged ()) {
				GenericWeapon wp = lastAttack.WeaponUsed;
				GameObject missileAnimPrefab = MissileAnimationPrefabDispenser.instance.GetAnimationPrefabByName (wp.MissileAnimationName);
				if (missileAnimPrefab != null) {
					LaunchMissileAndSetup (missileAnimPrefab);
				} else {
					AttackAnimationHitMoment ();
				}
			} else {
				
				AttackAnimationHitMoment ();
			}

			break;
		case AnimationEventType.LOOP:
			if (rangedAttacking) {
				animationTransform.Idle ();
			} else {
				AttackAnimationEnded ();
			}

			break;
		}

	}

	public void AttackAnimationBegan() {
		attacking = true;
		if (OnAttackAnimationBegan != null) {
			OnAttackAnimationBegan (this);
		}
	}

	public void AttackAnimationEnded() {
//		Debug.LogError ("Ended anims!");
		attacking = false;
		animationTransform.Idle ();
		if (OnAttackAnimationEnded != null) {
			OnAttackAnimationEnded (this);
		}

	}

	private bool rangedAttacking = false;
	private MissileScript lastMissileScript;
	public void LaunchMissileAndSetup(GameObject missilePrefab) {
		rangedAttacking = true;
		GameObject missileCopy = Instantiate (missilePrefab);
		missileCopy.transform.position = GetComponent<TileMovement> ().avatar.position;
		MissileScript missile = missileCopy.GetComponent<MissileScript> ();


		//this means miss
		if (lastAttack.resultingEffect == null) {
			List<ATTile> tiles = lastAttack.TargetActor ().TileMovement.occupying.TilesWithinRange(1);
			missile.LaunchAt(tiles [Random.Range (0, tiles.Count)]);
		} else {
			missile.LaunchAt (lastAttack.TargetActor ().TileMovement.occupying);

		}

//		UIManager.instance.cameraController.LockOn (missile.transform);

		GenericWeapon unequipped = lastAttack
			.actor
			.CharSheet
			.Unequip (lastAttack.WeaponUsed) as GenericWeapon;
		
		missile.OnConnectedWithTarget += (MissileScript self) => {
			
			AttackAnimationHitMoment();
			rangedAttacking = false;
			AttackAnimationEnded();
			if (lastAttack.TypeChoice.IsRanged() && lastAttack.WeaponUsed.IsThrown()) {
				self.targetTile.AddItemToGround(unequipped);
			}
			//Add item to ground of the last tile hit.

		};

		missile.OnHitBlocker += (MissileScript self, ATTile blocker) => {
			
			lastAttack.resultingEffect = null;
			AttackAnimationHitMoment();
			rangedAttacking = false;
			AttackAnimationEnded();
			if (lastAttack.TypeChoice.IsRanged() && lastAttack.WeaponUsed.IsThrown()) {
				self.previousHitTile.AddItemToGround(unequipped);
			}
			//Add item to ground of the player
		};

		lastMissileScript = missile;
	}

	public void AttackAnimationHitMoment() {
//		Debug.LogError ("hit moment!");
		if (lastAttack.resultingEffect != null) {
			StartCoroutine (ImpactEmphasize ());



		} else {
			//lastAttack.TargetActor ().InstantiateMissText ();
		}
		if (OnHitMoment != null) {
//			Debug.LogError ("hit moment callls!");
			OnHitMoment (this);
		}
	}

	public IEnumerator ImpactEmphasize() {
		animationTransform.Pause ();
		lastAttack.TargetActor ().CharSheet.TakeDamageEffect (lastAttack.resultingEffect);

//		characterAudioSource.clip = hit; //This should happen via damage
//		characterAudioSource.Play ();
		yield return new WaitForSeconds (.4f);//should be loosely the same as the shudder time.  Perhaps 
		animationTransform.Unpause ();
	}


	public delegate void ImpactMadeAction(CharacterAttacker animationInst);
	public event ImpactMadeAction OnHitMoment;


	public delegate void AttackAnimationBeganAction(CharacterAttacker animationInst);
	public event AttackAnimationBeganAction OnAttackAnimationBegan;


	public delegate void AttackAnimationEndedAction(CharacterAttacker animationInst);
	public event AttackAnimationEndedAction OnAttackAnimationEnded;
}
