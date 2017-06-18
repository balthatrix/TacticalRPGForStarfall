using UnityEngine;

using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using AT.Battle;
using AT;
/// <summary>
/// TODO: Need to somehow break this out into multiple scripts?
/// </summary>
public class UIManager : MonoBehaviour {


	public TileSelector selector;

	public CameraController cameraController;

	[SerializeField]
	public static UIManager instance;

	public GameObject canvas;

	public GameObject physicalDamageText;
	public GameObject missHitText;
	public GameObject conditionText;
	public GameObject aOOLine;
	public GameObject advantageIndicator;
	public GameObject disadvantageIndicator;	
	public ActionBar actionBar;
	public Text upperRightText;
	public Text upperLeftText;
	public TileInfoPanel tileInfoPanel;

	public Text waitText;
	public RectTransform roundPanel;
	public Text roundText;

	public Tooltip Tooltip {
		get { return Tooltip.instance; }
	}


	public List<KeyCode> registeredKeys;


	void Awake() {
		if (instance == null) {
			instance = this;

			registeredKeys.Add (KeyCode.I);
			registeredKeys.Add (KeyCode.Space);
			registeredKeys.Add (KeyCode.Escape);
//			DontDestroyOnLoad (instance);

		}
	}

	public void ShowSelectorOnTile(ATTile t) {
		selector.transform.position = t.transform.position;
		selector.GetComponent<SpriteRenderer> ().enabled = true;
	}

	public void HideSelector() {
		selector.GetComponent<SpriteRenderer> ().enabled = false;
	}


	public void Pause() {
		Time.timeScale = 0f;
	}

	public void Unpause() {
		Time.timeScale = 1f;
	}


	public void ActionBarEnable() {
		actionBar.GetComponent<CanvasGroup> ().interactable = true;
		actionBar.GetComponent<CanvasGroup> ().alpha = 1f;
	}


	public void ActionBarDisable() {
		actionBar.GetComponent<CanvasGroup> ().interactable = false;
		actionBar.GetComponent<CanvasGroup> ().alpha = .5f;
	}



		

	Ray ray;
	RaycastHit hit;

	// Update is called once per frame
	void Update () {

		foreach (KeyCode key in registeredKeys) {
			if(Input.GetKeyDown(key)) {
				IssueKeyPressed (key);
			}
		}



//		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//		if (Physics.Raycast (ray, out hit)) {
////			Debug.Log ("HIIIIITT:::::" + hit.collider.name);
//		} 
	}

	void IssueKeyPressed(KeyCode kc) {
		if (OnKeyPressed != null) {
			OnKeyPressed (kc);
		}	
	}

	public delegate void  KeyPressedAction(KeyCode k);
	public event KeyPressedAction OnKeyPressed;

	public delegate void  ButtonUiPressedAction(string name);
	public event ButtonUiPressedAction OnButtonUiPressed;





	public bool OverUI {
		get { 
			return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject ();
		}
	}



}
