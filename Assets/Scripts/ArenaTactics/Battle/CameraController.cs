using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody2D))]
public class CameraController : MonoBehaviour {

	public GameObject cameraCollisionReceiverPrefab;
	private Rigidbody2D rigidBody;
	public GameObject lastReceiver;
	public OptButton reLockCameraButton;

	Transform lockedOn;
	public bool freeCameraMode = false;

	public float nudgeSpeed = 5f;


	// Use this for initialization
	void Start () {
		rigidBody = GetComponent<Rigidbody2D> ();
		reLockCameraButton.OnOptClicked += (button, eventData) => {
			GoLockMode();
		};
		reLockCameraButton.gameObject.SetActive (false);
		//Screen.SetResolution(640, 480, true);

	}

	void Update() {
		if (lockedOn != null && !freeCameraMode) {
			GetComponent<TargetJoint2D> ().target = lockedOn.position - CamOffset;
		}

		if(Input.GetKey(KeyCode.LeftArrow)) {
			GoFreeMode ();
			NudgeLeft ();
		}
			
		if(Input.GetKey(KeyCode.RightArrow)) {
			GoFreeMode ();
			NudgeRight ();
		}

		if(Input.GetKey(KeyCode.UpArrow)) {
			GoFreeMode ();
			NudgeUp();
		}

		if(Input.GetKey(KeyCode.DownArrow)) {
			GoFreeMode ();
			NudgeDown();
		}

		if (Input.GetKeyDown (KeyCode.RightShift)) {
			GoLockMode ();
		}

	}

	void NudgeLeft() {
		if (transform.position.x <= MapManager.instance.minX)
			return;
		transform.position = new Vector3 (
			transform.position.x - (nudgeSpeed * Time.deltaTime), 
			transform.position.y, 
			transform.position.z);
	}
	void NudgeRight() {
		if (transform.position.x >= MapManager.instance.maxX)
			return;
		transform.position = new Vector3 (
			transform.position.x + (nudgeSpeed * Time.deltaTime), 
			transform.position.y, 
			transform.position.z);
	}
	void NudgeDown() {
		if (transform.position.y <= MapManager.instance.minY)
			return;
		transform.position = new Vector3 (
			transform.position.x, 
			transform.position.y - (nudgeSpeed * Time.deltaTime), 
			transform.position.z);
	}
	void NudgeUp() {
		if (transform.position.y >= MapManager.instance.maxY)
			return;
		transform.position = new Vector3 (
			transform.position.x, 
			transform.position.y + (nudgeSpeed * Time.deltaTime), 
			transform.position.z);
	}




	private Vector3 GetVelocityNoZ(Vector3 v3) {
		return new Vector3(v3.x, v3.y, 0f);
	}

	private Vector3 ColliderOffset {
		get { return GetComponent<Collider2D> ().offset; }
	}

	public void MoveTo(Vector3 pos) {
		lockedOn = null;
		GetComponent<TargetJoint2D> ().target = pos - ColliderOffset;
		if (freeCameraMode) {
			CallMoveComplete ();
		} else {
			InitializeMove (pos);
		}
	}

	void InitializeMove(Vector3 pos) {
		Destroy (lastReceiver);
		lastReceiver = Instantiate (cameraCollisionReceiverPrefab);
		lastReceiver.transform.position = pos;

		if (OnMoveBegan != null) {
			OnMoveBegan (this);
		}

		if (lastReceiver.GetComponent<Collider2D> ().IsTouching (GetComponent<Collider2D> ())) {
			CallMoveComplete ();
		}
	}


	Vector3 CamOffset {
		get { 
			return new Vector3(GetComponent<Collider2D> ().offset.x, GetComponent<Collider2D> ().offset.y, 0f); 
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		
		if (other.CompareTag ("CameraCollisionReceiver")) {
			CallMoveComplete ();
		}
	}

	public void CallMoveComplete(){
		rigidBody.velocity = Vector3.zero;
		Destroy (lastReceiver);
		if (OnMoveComplete  != null) {
			OnMoveComplete (this);
		}

	}

	public Vector2 Rounded(Vector2 v) {
		return new Vector2 (Mathf.Round(v.x), Mathf.Round(v.y));
	}

	public void LockOn(Transform t) {
		lockedOn = t;
	}
	public void GoFreeMode() {
		reLockCameraButton.gameObject.SetActive (true);
		CallMoveComplete ();
		GetComponent<Collider2D> ().enabled = false;
		GetComponent<TargetJoint2D> ().enabled = false;
		freeCameraMode = true;

//		CancelLock ();
	}
	public void GoLockMode() {
		reLockCameraButton.gameObject.SetActive (false);
		GetComponent<Collider2D> ().enabled = true;
		GetComponent<TargetJoint2D> ().enabled = true;
		freeCameraMode = false;

		if (lockedOn == null) {
//			Debug.LogError ("initing move~!");
			InitializeMove (GetComponent<TargetJoint2D> ().target  + new Vector2(CamOffset.x, CamOffset.y));
		}
	}

	public Transform CurrentLock {
		get { return lockedOn; }
	}

	public void CancelLock(){
		lockedOn = null;
	}


	public delegate void MoveCompleteAction(CameraController c);
	public event MoveCompleteAction OnMoveComplete;
	public delegate void MoveBeganAction(CameraController c);
	public event MoveBeganAction OnMoveBegan;
}
