using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT.Battle;

public class CharacterIndicator : MonoBehaviour {

	public SpriteRenderer indicator;
	// Use this for initialization
	void Start () {
		HideIndicator ();
	}

	public void HideIndicator() {
//		Debug.Log ("hiding for " + name + GetComponent<Actor>().GetType());
		indicator.enabled = false;
	}

	public void ShowIndicator() {
//		Debug.Log ("showing for " + name+ GetComponent<Actor>().GetType());
		indicator.enabled = true;

	}

	public void StartBlinking() {
		
		StartCoroutine (Blink ());
	}

	public void StopBlinking() {
		blinking = false;
	}

	bool blinking;
	IEnumerator Blink() {
		bool decreasing = true;
		blinking = true;
		float startA = indicator.color.a;
		while (true && blinking) {
			if (decreasing) {	
				float newA = indicator.color.a - Time.deltaTime;
				indicator.color = new Color (
					indicator.color.r, 
					indicator.color.g,
					indicator.color.b,
					newA
				);
				if (newA <= 0f) {
					decreasing = false;
				}
			} else {
				float newA = indicator.color.a + Time.deltaTime;
				indicator.color = new Color (
					indicator.color.r, 
					indicator.color.g,
					indicator.color.b,
					newA
				);
				if (newA >= startA) {
					decreasing = true;
				}
			}
			yield return null;
		}
		indicator.color = new Color (
			indicator.color.r, 
			indicator.color.g,
			indicator.color.b,
			startA
		);
	}

	public void ColorIndicator(Color c) {
		indicator.color = new Color (c.r, c.g, c.b, indicator.color.a);
	}

}
