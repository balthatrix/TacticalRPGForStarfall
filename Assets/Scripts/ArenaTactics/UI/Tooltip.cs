using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour {
	public static Tooltip instance;
	void Awake() {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad((transform as RectTransform).parent.gameObject); //Don't destroy my canvas
		} else {
			Destroy ((transform as RectTransform).parent.gameObject);
		}
	}

	public Text text;

	public RectTransform pointer;

	public enum TooltipPosition {
		TOP,
		BOTTOM,
		LEFT,
		RIGHT
	}

	public Sprite downPoint, upPoint, leftPoint, rightPoint;

	TooltipPosition currentPositioning;

	
	// Update is called once per frame
	void Update () {
	//	transform.position = new Vector2 (Input.mousePosition.x, Input.mousePosition.y )  + offset;

	}

	Vector2 offscreen = new Vector2(100000f, 100000f);
	public Vector2 Offscreen() {
		return offscreen;
	}

	public void Hide() {
		transform.position = offscreen;
		pointer.anchoredPosition = offscreen;
	}


	public void Show(RectTransform onRect, TooltipPosition position=TooltipPosition.TOP, float usedOffset=10f) {
		

		currentPositioning = position;
//		Debug.Log ("showing on " + onRect.transform.position);
		//Set position to rect

		StartCoroutine (DelaySetPosition (onRect, usedOffset));
	}

	public IEnumerator DelaySetPosition(RectTransform onRect, float usedOffset) {
		yield return new WaitForEndOfFrame ();
		SetPosition(onRect, usedOffset);
		GetComponent<Image> ().color = new Color (1f, 1f, 1f, 1f);
		gameObject.SetActive(true);
	}

	public void SetPosition(RectTransform toRect, float usedOffset=10f) {
		//take position of the rect. 
		Vector2 pos = toRect.position;
		RectTransform self = transform as RectTransform;
		Vector2 offset;
		switch (currentPositioning) {
		case TooltipPosition.TOP:

			pos = pos + new Vector2 (0f, toRect.sizeDelta.y * toRect.pivot.y);
			pos = pos + new Vector2 (0f, (self.sizeDelta.y * self.pivot.y) + usedOffset);

			pointer.anchoredPosition = pos - (new Vector2 (0f, self.sizeDelta.y * self.pivot.y) + 
				new Vector2(0f, pointer.sizeDelta.y * pointer.pivot.y * .5f));

			pointer.GetComponent<Image> ().sprite = downPoint;

			break;

		case TooltipPosition.LEFT:
			pos = pos + new Vector2 (-(self.sizeDelta.x * self.pivot.x), 0f);
			pos = pos + new Vector2 (-((toRect.sizeDelta.x * toRect.pivot.x) + usedOffset), 0f);

			offset = (new Vector2 (self.sizeDelta.x * self.pivot.x, 0f) +
			                 new Vector2 (pointer.sizeDelta.x * pointer.pivot.x * .5f, 0f));
			pointer.anchoredPosition = pos + offset;

			pointer.GetComponent<Image> ().sprite = rightPoint;
			break;

		case TooltipPosition.RIGHT:
			pos  =  pos + new Vector2 (self.sizeDelta.x * self.pivot.x, 0f);
			pos = pos + new Vector2 ((toRect.sizeDelta.x * toRect.pivot.x) + usedOffset, 0f);

			offset = (new Vector2 (self.sizeDelta.x * self.pivot.x, 0f) +
				new Vector2 (pointer.sizeDelta.x * pointer.pivot.x * .5f, 0f));
			pointer.anchoredPosition = pos - offset;

			pointer.GetComponent<Image> ().sprite = leftPoint;
			break;

		case TooltipPosition.BOTTOM:
			pos  =  pos + new Vector2 (0f, -(self.sizeDelta.y * self.pivot.y));
			pos = pos + new Vector2 (0f, -((toRect.sizeDelta.y * toRect.pivot.y) + usedOffset));

			offset = (new Vector2 (0f, self.sizeDelta.y * self.pivot.y) +
				new Vector2 (0f, pointer.sizeDelta.y * pointer.pivot.y * .5f));
			pointer.anchoredPosition = pos + offset;

			pointer.GetComponent<Image> ().sprite = upPoint;
			break;

		}

		self.anchoredPosition = pos;




		AdjustForOffscreen ();

//		Debug.Log ("positioning: " + pos);

		//add half the height of the tooltip...
		//add the pivotY * height of the rect.

	}

	public void AdjustForOffscreen() {
		//check if left side is off:
		RectTransform self = transform as RectTransform;
		float leftSide = (self.sizeDelta.x * self.pivot.x);
		float leftOffscreen = leftSide - self.anchoredPosition.x;
		if (leftOffscreen > 0) {
			self.anchoredPosition = new Vector2 (self.anchoredPosition.x + leftOffscreen, self.anchoredPosition.y);
		}

	}


	public void SetText(string str) {
		
		text.text = Util.UtilString.WordWrap (str, 35);
	}
}
