using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationListener : MonoBehaviour {


	AnimationTransform animationTransform;
	Animator rootBody;

	// Use this for initialization
	void Start () {
		animationTransform = GetComponent<AnimationTransform> ();	
		rootBody = animationTransform.ClosestAnimator ();
		animationTransform.OnAnimationEvent += AnimationEventHappened;

		//animationTransform.SetTrigger ("walkDown");
		//rootBody.speed = 0f;
	}

	private void AnimationEventHappened(AnimationEventType type, string name) {
//		Debug.LogError ("look " + type);
//
//		animationTransform.SetTrigger ("idle");
//		animationTransform.SetTrigger ("walkLeft");
	}

	void Update () {
		AnimatorStateInfo state = rootBody.GetCurrentAnimatorStateInfo (0);
		Debug.Log ("o : " + state.normalizedTime);

	}


}
