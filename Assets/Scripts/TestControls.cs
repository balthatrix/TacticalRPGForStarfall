using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestControls : MonoBehaviour {

	private Animator animator;
	private List<KeyCode> keysDown;
	private Vector3 posSinceBeganMove;
	private string prevDirection;
	// Use this for initialization
	void Start () {
		keysDown = new List<KeyCode> ();

		animator = GetComponent<Animator> ();

		//animator.SetTrigger ("walkUp");
	}
	
	// Update is called once per frame
	void Update () {


		if(Input.GetKeyDown(KeyCode.W)) {
			keysDown.Add(KeyCode.W);
		}
		if(Input.GetKeyDown(KeyCode.A)) {
			keysDown.Add(KeyCode.A);
		}
		if(Input.GetKeyDown(KeyCode.S)) {
			keysDown.Add(KeyCode.S);
		}
		if(Input.GetKeyDown(KeyCode.D)) {
			keysDown.Add(KeyCode.D);
		}

		if(Input.GetKeyUp(KeyCode.W)) {
			keysDown.Remove(KeyCode.W);
		}
		if(Input.GetKeyUp(KeyCode.A)) {
			keysDown.Remove(KeyCode.A);
		}
		if(Input.GetKeyUp(KeyCode.S)) {
			keysDown.Remove(KeyCode.S);
		}
		if(Input.GetKeyUp(KeyCode.D)) {
			keysDown.Remove(KeyCode.D);
		}


		AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo (0);

		if (state.IsName ("Idle")) {
			if (keysDown.Count > 0) {
				if (prevDirection != null) {
					//This prevents jittery animations, and animations that cause movement to los
					RoundPositionToNearestWhole ();
				}
				switch (keysDown [keysDown.Count - 1]) {
				case KeyCode.W:
					animator.SetTrigger ("walkUp");
					prevDirection = "walkUp";
					break;
				case KeyCode.A:
					animator.SetTrigger ("walkLeft");
					prevDirection = "walkLeft";
					break;
				case KeyCode.S:
					animator.SetTrigger ("walkDown");
					prevDirection = "walkDown";
					break;
				case KeyCode.D:
					animator.SetTrigger ("walkRight");
					prevDirection = "walkRight";
					break;
				}


				BeginMoving (prevDirection, state.normalizedTime);
			}



		} else {
			if (state.normalizedTime > 0f)
				MoveCharacter (prevDirection, state.normalizedTime);

			//EndMoving();
		}

//		 else {
//			if(!(state.IsName("IdleRight") || 
//				state.IsName("IdleLeft") || 
//				state.IsName("IdleUp") ||
//				state.IsName("IdleDown")))
//				animator.SetTrigger ("stopWalk");
//		}0

	}

	void RoundPositionToNearestWhole() {
		
		transform.position = new Vector3 (
			Mathf.Round (transform.position.x),
			Mathf.Round (transform.position.y) + 0.20f,
			0f);
//		switch (direction) {
//		case "walkUp":
//			transform.position = startPos + new Vector3 (0f, 1f, 0f);
//			break;
//		case "walkDown":
//			transform.position = startPos + new Vector3 (0f, -1f, 0f);
//			break;
//		case "walkLeft":
//			transform.position = startPos + new Vector3 (-1f, 0f, 0f);
//			break;
//		case "walkRight":
//			transform.position = startPos + new Vector3 (1f, 0f, 0f);
//			break;
//		}
	}

	void BeginMoving(string direction, float completion) {
		posSinceBeganMove = transform.position;
		MoveCharacter (direction, completion);
	}

	void MoveCharacter(string direction, float completionRatio) {
		switch (direction) {
		case "walkUp":
			transform.position = posSinceBeganMove + new Vector3 (0f, completionRatio, 0f);
			break;
		case "walkDown":
			transform.position = posSinceBeganMove + new Vector3 (0f, -completionRatio, 0f);
			break;
		case "walkLeft":
			transform.position = posSinceBeganMove + new Vector3 (-completionRatio, 0f, 0f);
			break;
		case "walkRight":
			transform.position = posSinceBeganMove + new Vector3 (completionRatio, 0f, 0f);
			break;
		}
	}

}
