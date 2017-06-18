using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleLog : MonoBehaviour {


	[SerializeField]
	OptButton drag;

	[SerializeField]
	RectTransform logWindow;

	[SerializeField]
	RectTransform logBody;

	[SerializeField]
	ScrollRect scrollRect;

	[SerializeField]
	public GameObject textElementPrefab;

	public int maxHeight = 300; //the higher it is, the smaller the window can be
	public int minHeight = 36; //the lower it is, the bigger the log window can be
	private int current;

	void Clear() {
		for (int i = 0; i < logBody.childCount; i++) {
			Destroy (logBody.GetChild (i).gameObject);
		}
	}

	public void Log(string text) {
		GameObject go = Instantiate (textElementPrefab);
		go.GetComponent<TextElement> ().textObject.fontSize = 12;
		go.GetComponent<TextElement> ().textObject.text = text;
		go.transform.SetParent (logBody, false);
		ScrollToBottom ();
	}

	public void ScrollToBottom() {
		StartCoroutine (SetNormalPositionToBottom ());
	}

	public void ScrollToTop() {
		StartCoroutine (SetNormalPositionToTop ());
	}

	private IEnumerator SetNormalPositionToBottom() {
		yield return new WaitForEndOfFrame ();

		scrollRect.normalizedPosition = new Vector2(0, 0);
	}

	private IEnumerator SetNormalPositionToTop() {
		yield return new WaitForEndOfFrame ();

		scrollRect.normalizedPosition = new Vector2(0, 1);
	}



	// Use this for initialization
	void Start () {
		if (logWindow == null)
			logWindow = GetComponent<RectTransform> ();
		
		if (Height < minHeight) {
			Height = minHeight;
		}
		if (Height > maxHeight) {
			Height = maxHeight;
		}

		current = Height;


		drag.OnOptDragged += Drag;	

		Clear ();
	}

	public int Height {

		get { 
			return (int) logWindow.sizeDelta.y;
		}
		set {
			logWindow.sizeDelta = new Vector2(logWindow.sizeDelta.x, (float) value);

		}
	}

	/// <summary>
	/// TODO: Make this it's own component, like a "expandable" window.
	/// </summary>
	private bool dragging;
	private int startHeight;
	private int startMousePosY;
	void Drag (OptButton button)
	{
		if (dragging) {
			
		} else {
			dragging = true;
			startHeight = Height;
			startMousePosY = (int) Input.mousePosition.y;
		}
		//private 
	}

	void StopDrag() {
		dragging = false;

	}

	public static BattleLog instance;
	void Awake() {
		if (instance == null)
			instance = this;
		else
			Destroy (instance);
	}

	void Update() {
		if (dragging) {
			if (Input.GetMouseButtonUp (0)) {
				
				StopDrag ();	
			} else {
				int offset = (int) Input.mousePosition.y - startMousePosY;
				int desired = startHeight + offset;
				if (desired > maxHeight || desired < minHeight) {
				} else {
					Height = desired;
				}


			}
		}
	}

}
