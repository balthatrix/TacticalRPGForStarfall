using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

public enum AnimationEventType {
	LOOP,
	ATTACK_HIT_MOMENT,
	ATTACK_SWING_MOMENT,
	CAST_PREPARE_MOMENT,
	CAST_RELEASE_MOMENT
}

/// <summary>
/// Animation transform.  Causes triggers to be sent through recursively to any children
/// This is helpful for syncing animations with one set trigger call for modular animations, with armour/weapons/etc...
/// </summary>
public class AnimationTransform : MonoBehaviour {



	Animator anim;
	public bool isRoot;
	void Awake() {
		anim = GetComponent<Animator> ();
		// States on layer 0:

	
	}

	void Start() {
	//	StartCoroutine (Demo ());
	}

	IEnumerator Demo() {
		while (true) {
			yield return new WaitForSeconds(1.6f);
			Play("MainhandAttackUp");
			yield return new WaitForSeconds(1.6f);
			Play("MainhandAttackRight");
			yield return new WaitForSeconds(1.6f);
			Play("MainhandAttackDown");
			yield return new WaitForSeconds(1.6f);
			Play("MainhandAttackLeft");
		}
	}



	#if UNITY_EDITOR
	public void DebugStateNames() {
		
		if (anim != null) {
			AnimatorState last=null;
			AnimatorController controller = anim.runtimeAnimatorController as AnimatorController;

			if (controller != null) {
				foreach (AnimatorControllerLayer l in controller.layers) {
				
					foreach (ChildAnimatorState s in l.stateMachine.states) {
						last = s.state;
						Debug.Log ("State " + last.name);
					}
				}
			} else {
				Debug.LogWarning (name + " didn't have a controller? ");
			}
		}
	}
	#endif

	public void Play(string stateName) {
		
		LastPlay = stateName;

		if (anim != null && anim.runtimeAnimatorController != null) {
//			Debug.Log ("playing " + stateName);
			anim.Play (stateName);
		} else {
//			Debug.Log ("no animator on " + name);
		}

		//check to see if any children have animation transforms, and call closest animator on them as well.
		for (int i = 0; i < transform.childCount; i++) {
			Transform child = transform.GetChild (i);
			AnimationTransform at = child.GetComponent<AnimationTransform> ();
			if (at != null) {
				at.Play (stateName);
			}
		}
	}

	public void SetAlpha(float a) {
		SpriteRenderer sr = GetComponent<SpriteRenderer> ();

		if (sr != null) {
			sr.color = new Color (sr.color.r, sr.color.g, sr.color.b, a);
		}

		OnEachChild ((ch) => ch.SetAlpha (a));

	}

	public void Idle() {
		string lp = LastPlay;
		string idleDir = "Down";
		if (lp.Contains ("Up")) {
			idleDir = "Up";
		} else if (lp.Contains ("Right")) {
			idleDir = "Right";

		} else if (lp.Contains ("Down")) {
			idleDir = "Down";
		} else if (lp.Contains ("Left")) {
			idleDir = "Left";
		} else {
			Debug.LogError (lp + " did not contain a direction.  So can't play an idle direction properly!");
		}

		Play ("Idle" + idleDir);
	}

	public string LastPlay { get; set; }


	public void SyncToRoot(AnimatorStateInfo info) {
		if (anim != null && !isRoot)
			anim.Play (0, -1, info.normalizedTime);
		Debug.Log ("normal tyme: " + info.normalizedTime);
		OnEachChild ((AnimationTransform child) => child.SyncToRoot (info));
	}

	public Animator ClosestAnimator() {
		if (anim != null)
			return anim;

		//check to see if any children have animators.  return in you find one
		for (int i = 0; i < transform.childCount; i++) {
			Transform child = transform.GetChild (i);
			Animator a = child.GetComponent<Animator> ();
			if (a != null) {
				return a;
			}
		}

		//check to see if any children have animation transforms, and call closest animator on them as well.
		for (int i = 0; i < transform.childCount; i++) {
			Transform child = transform.GetChild (i);
			AnimationTransform at = child.GetComponent<AnimationTransform> ();
			if (at != null) {
				Animator a = at.ClosestAnimator ();
				if (a != null)
					return a;
			}
		}

		return null;
	}

	private float previousSpeed;
	private bool paused;
	public void Pause() {
		if (paused)
			return;
		if (anim != null) {
			previousSpeed = anim.speed;
			anim.speed = 0f;
		}
		OnEachChild ((AnimationTransform at) => at.Pause());
		paused = true;
	}
	public void Unpause() {
		if (anim != null) {
			anim.speed = previousSpeed;
		}
		OnEachChild ((AnimationTransform at) => at.Unpause());
		paused = false;
	}

	public void OnEachChild(PerformOnChildren action) {
		for (int i = 0; i < transform.childCount; i++) {

			Transform child = transform.GetChild (i);
			AnimationTransform at = child.GetComponent<AnimationTransform> ();
			if (at != null) {
				action (at);
			}
		}
	}

	public delegate void PerformOnChildren(AnimationTransform at);


	public delegate void AnimationEventAction(AnimationEventType type, string animationName);

	/// <summary>
	///Occurs when on animation event is fired by the animator.  events only need to be tied to the root animation clips (i.e. naked body), since 
	/// that represent what actually happening with all the children.
	/// Regardless, just in case this transform is only a conduit (doesn't have an animator itself), it triggers the parent transform event as well.
	/// </summary>
	public event AnimationEventAction OnAnimationEvent;


	/// <summary>
	/// Used by all animation events as the function to call from editor settings.
	/// </summary>
	/// <param name="type">Type.</param>
	public void EventFired(AnimationEventType type) {
//		if(anim != null)
//			Debug.Log ("event fired by "+ name + ": " + type + "@"+ anim.GetNextAnimatorStateInfo(0).normalizedTime);

		if (OnAnimationEvent != null) {
			OnAnimationEvent (type,LastPlay);
		}

		if (transform.parent != null) {
			AnimationTransform paren = transform.parent.GetComponent<AnimationTransform> ();

			if (paren != null) {
				paren.EventFired (type);
			}
		}
	}



	public delegate void TriggerSetAction(string trigger);


	public event TriggerSetAction OnTriggerSet;

	public void TriggerSet(string trigger) {
		if (OnTriggerSet != null) {
			OnTriggerSet (trigger);
		}

	}
}
