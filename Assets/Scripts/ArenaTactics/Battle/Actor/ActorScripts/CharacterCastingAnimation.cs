using UnityEngine;
using System.Collections;
using AT.Battle;
using AT.Character;
using AT;
using System.Collections.Generic;
using System.Linq;

[RequireComponent (typeof(TileMovement))]
[RequireComponent (typeof(AnimationTransform))]
public class CharacterCastingAnimation : MonoBehaviour {
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
//		GetComponent<Actor>().OnWillPerform += SetupCastAnimation;
	}

	public Cast lastCast;

	public void SetupAndDoCastAnimation(Action a) {

		if (a is Cast) {
			lastCast = a as Cast;
//			Debug.Log ("Nale: " + lastCast.Targets ().First ().name);
			Actor target = lastCast.Targets ().First().FirstOccupant.ActorComponent;
			string name = AnimNameFromTarget (target);
			//Here the code should split off for ranged cases
			DoCastAnimation (name);
		}
	}

	string AnimNameFromTarget(Actor t) {
		
		Vector3 directions = t.transform.position - transform.position;

		if (Mathf.Abs (directions.x) < Mathf.Abs (directions.y)) {
			//Attacking up or down
			if (directions.y > 0f) {
				return "CastUp";
			} else {
				return  "CastDown";
			}
		} else {
			//Attacking left or right.
			if (directions.x > 0f) {
				return  "CastRight";
			} else {
				return  "CastLeft";
			}
		}
	}

	private bool casting = false;
	public void DoCastAnimation(string name) {
		CastAnimationBegan ();
		animationTransform.Play (name);
	}

	public void HandleAnimationEvent(AnimationEventType type, string animationName) {
		if (!casting)
			return;
		switch (type) {
		case AnimationEventType.CAST_PREPARE_MOMENT:
			break;
		case AnimationEventType.CAST_RELEASE_MOMENT:
			Debug.Log ("Casting!");
			CastAnimationReleaseMoment ();
			break;
		case AnimationEventType.LOOP:
			CastAnimationEnded ();

			break;
		}

	}

	public void CastAnimationBegan() {
		casting = true;
		if (OnCastAnimationBegan != null) {
			OnCastAnimationBegan (this);
		}
	}

	public void CastAnimationEnded() {
		//		Debug.LogError ("Ended anims!");
		casting = false;
		animationTransform.Idle ();
		if (OnCastAnimationEnded != null) {
			OnCastAnimationEnded (this);

			foreach (CastAnimationEndedAction oneshot in oneShotCastAnimEndeds) {
				OnCastAnimationEnded -= oneshot;
			}
		}

	}



	public void CastAnimationReleaseMoment() {
		//		Debug.LogError ("hit moment!");
		StartCoroutine (ImpactEmphasize ());



		
		if (OnSpellRelease != null) {
			//			Debug.LogError ("hit moment callls!");
			OnSpellRelease (this);

			foreach (SpellReleaseAction oneshot in oneShotSpellReleases) {
				Debug.Log ("unsubbing one shot!");
				OnSpellRelease -= oneshot;
			}
		}
	}

	public IEnumerator ImpactEmphasize() {
		animationTransform.Pause ();

		//		characterAudioSource.clip = hit; //This should happen via damage
		//		characterAudioSource.Play ();
		yield return new WaitForSeconds (.4f);
		animationTransform.Unpause ();
	}



	List<SpellReleaseAction> oneShotSpellReleases = new List<SpellReleaseAction> ();
	public void OneShotSpellRelease(SpellReleaseAction deleg) {
		OnSpellRelease += deleg;
		oneShotSpellReleases.Add (deleg);
	}

	List<CastAnimationEndedAction> oneShotCastAnimEndeds = new List<CastAnimationEndedAction> ();
	public void OneShotAnimEnded(CastAnimationEndedAction deleg) {
		OnCastAnimationEnded += deleg;
		oneShotCastAnimEndeds.Add (deleg);
	}



	public delegate void SpellReleaseAction(CharacterCastingAnimation animationInst);
	public event SpellReleaseAction OnSpellRelease;


	public delegate void CastAnimationBeganAction(CharacterCastingAnimation animationInst);
	public event CastAnimationBeganAction OnCastAnimationBegan;


	public delegate void CastAnimationEndedAction(CharacterCastingAnimation animationInst);
	public event CastAnimationEndedAction OnCastAnimationEnded;
}

