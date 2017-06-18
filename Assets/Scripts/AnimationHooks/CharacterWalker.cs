using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterWalker : MonoBehaviour {
	AnimationTransform animationTransform;

	private List<string> currentPath;

	Animator infoHook;
	void Awake () {
		animationTransform = GetComponent<AnimationTransform> ();
		infoHook = animationTransform.ClosestAnimator ();

		currentPath = new List<string> ();
		RoundPositionToNearestWhole ();

		animationTransform.OnAnimationEvent += AnimationEventHappened;

	}

	void Start() {
		//animationTransform.Play ("WalkLeft");
		StartCoroutine (TestWalking ());
	}

	IEnumerator TestWalking() {
		yield return new WaitForSeconds (2f);

		//test out the cue
//		AddToCue ("WalkUp");
//		AddToCue ("WalkUp");
//		AddToCue ("WalkDown");
//		AddToCue ("WalkLeft");
//		AddToCue ("WalkLeft");
//		AddToCue ("WalkLeft");
//		AddToCue ("WalkDown");
//		AddToCue ("WalkDown");
//		AddToCue ("WalkDown");
//		AddToCue ("WalkRight");
//		AddToCue ("WalkRight");
//		AddToCue ("WalkRight");
//		AddToCue ("WalkLeft");
//		AddToCue ("WalkUp");
//		AddToCue ("WalkUp");
//		AddToCue ("WalkLeft");
//		AddToCue ("WalkDown");
//		AddToCue ("WalkLeft");




		//test out pausing
//		while (true) {
//			yield return new WaitForSeconds (2f);
//			animationTransform.Pause ();
//
//			yield return new WaitForSeconds (2f);
//
//			animationTransform.Unpause ();
//		}
	}


	private void AnimationEventHappened(AnimationEventType type, string animationName) {

		if (type == AnimationEventType.LOOP) {
			if (Moving && animationTransform.LastPlay != null && animationTransform.LastPlay.Contains("Walk")) {
//				Debug.Log ("Move cycle!");
				MovementCycled ();
			} 
		}

	}

	public bool Moving {
		get { 
			return (currentPath.Count > 0); 

		}
	}

	public void AddToCue(string direction) {

		bool willStart = (currentPath.Count == 0);
		currentPath.Add (direction);
		if (willStart) {
			StartCue ();
		}
	}

	bool interruptedFlagged = false;
	public void Interrupt() {
//		currentPath.Clear ();
//		RoundPositionToNearestWhole ();
//		MovementEnded ();
//
		if (Moving) {
			interruptedFlagged = true;

//			Debug.LogError ("interupted moving!");
		} 


	}

	string currentWalkingDirection;
	Vector3 sinceMovedInSameDirection;
	void StartCue() {
		string first = currentPath.First();
		MoveInNewDirection (first);
		if (OnBeganWalking != null) {
			OnBeganWalking (first);
		}
	}

	bool continuing = false;
	void MoveInNewDirection(string dir) {
		currentWalkingDirection = dir;
		animationTransform.Play (dir);
		sinceMovedInSameDirection = transform.position;
	}

	IEnumerator StartChangeAtNextFrame(string nextDirection) {
		yield return new WaitForEndOfFrame ();

		MoveInNewDirection (nextDirection);
	}

	void MovementCycled() {
		
		currentPath.Remove (currentWalkingDirection);
		if (interruptedFlagged) {
			currentPath.Clear ();
			interruptedFlagged = false;
		}
		if (currentPath.Count > 0) {
			string previous = currentWalkingDirection;
			string nextDirection = currentPath [0];
			if (nextDirection != previous) {
				continuing = false;
				StartCoroutine (StartChangeAtNextFrame (nextDirection));
			} else {
				continuing = true;
			}
			//continued here, which mean continuing along paths,
			//is distinct from the meaning than bool 'continuing' above, which means the same direction is being gone. 
			if (OnContinuedWalking != null) {
				OnContinuedWalking (nextDirection);
			}
		} else {
			MovementEnded ();
		}

		RoundPositionToNearestWhole ();
	}

	void MovementEnded() {
		currentWalkingDirection = null;
		//animationTransform.Pause ();
		Idle();
		if(OnEndedWalking != null)
			OnEndedWalking ();
	}

	public void Idle() {
		string lp = animationTransform.LastPlay;
		string idleDir = "Down";
		if (lp != null) {
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
		}

		animationTransform.Play ("Idle" + idleDir);
	}


	void Update() {
		AnimatorStateInfo info = infoHook.GetCurrentAnimatorStateInfo (0);

		SetPosition (info.normalizedTime);
	}

	void SetPosition( float offset) {

		if (offset > 1f && !continuing) {
			offset = offset - (int)offset;
		}

		Vector3 amendment = Vector3.zero;
		switch (currentWalkingDirection) {
		case "WalkUp":
			//			Debug.Log ("Moving up " + completionRatio);
			amendment=  new Vector3 (0f, offset, 0f);
			break;
		case "WalkDown":
			//			Debug.Log ("Moving down " + completionRatio);
			amendment= new Vector3 (0f, -offset, 0f);
			break;
		case "WalkLeft":
			//			Debug.Log ("Moving left " + completionRatio);
			amendment=  new Vector3 (-offset, 0f, 0f);
			break;
		case "WalkRight":
			//			Debug.Log ("Moving right " + completionRatio);
			amendment= new Vector3 (offset, 0f, 0f);
			break;
		default:
			return;
		}


		transform.position = sinceMovedInSameDirection + amendment;
	}

	void RoundPositionToNearestWhole() {
		transform.position = new Vector3 (
			Mathf.Round (transform.position.x), 
			Mathf.Round (transform.position.y), 
			0f
		);
	}


	public delegate void  BeganWalkingAction(string dir); 
	public event BeganWalkingAction OnBeganWalking;

	public delegate void  ContinuedWalkingAction(string dir); 
	public event ContinuedWalkingAction OnContinuedWalking;


	public delegate void  EndedWalkingAction(); 
	public event EndedWalkingAction OnEndedWalking;
}
