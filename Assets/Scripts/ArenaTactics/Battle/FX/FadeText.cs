using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeText : MonoBehaviour {


	public Text textComponent;
	public AnimationCurve curve;
	public float duration = 1.3f;
	public float speed = 1.3f;
	private float running = 0f;
	private float scaleDecay = .98f;

	private float decayMoment = .35f;
	
	// Update is called once per frame
	void Update () {
	}

	void Start() {
		running = 0f;
		StartCoroutine(FadeAway ());
	}


	IEnumerator FadeAway() {

		while (running <= duration) {
			float durationRan = (running / duration);
			float durationLeft = 1f - durationRan;

			float newScale = curve.Evaluate (durationRan);

			transform.localScale = new Vector3 (newScale, newScale, transform.localScale.z);
			running += Time.deltaTime;


			yield return new WaitForSeconds(Time.deltaTime);
		}
		Destroy (gameObject);
	}


//
//	IEnumerator FadeAway() {
//
//		float startPoint = duration;
//		while (running <= duration) {
//			float durationRan = (running / duration);
//			float durationLeft = 1f - durationRan;
//			if (durationRan < decayMoment) {
//				running += Time.deltaTime;
//				yield return new WaitForSeconds(Time.deltaTime);
//				continue;
//			} else {
//				float normalDomain = 1f - decayMoment;
//				float normalRan = durationRan - decayMoment;
//				float normalDone = normalRan / normalDomain;
//				float alpha = 1f - normalDone;
//
//
//				transform.position = new Vector3 (transform.position.x, transform.position.y + Time.deltaTime * speed, transform.position.z);
//				//decay color
//				textComponent.color = new Color (textComponent.color.r, textComponent.color.g, textComponent.color.b, alpha);
//				//decay scale
//				transform.localScale = new Vector3 (transform.localScale.x * scaleDecay, transform.localScale.y * scaleDecay, transform.localScale.z);
//				running += Time.deltaTime;
//			}
//
//			yield return new WaitForSeconds(Time.deltaTime);
//		}
//		Destroy (gameObject);
//	}
}
