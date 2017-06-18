using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpriteFadeEffect : MonoBehaviour {



	public float duration = 1.3f;
	public float decayMoment = .35f;


	private float running = 0f;

	private SpriteRenderer sprite;
	void Start() {
		sprite = GetComponent<SpriteRenderer> ();
		running = 0f;
		StartCoroutine(FadeAway ());
	}

	IEnumerator FadeAway() {

		float startPoint = duration;
		while (running <= duration) {
			float durationRan = (running / duration);
			float durationLeft = 1f - durationRan;
			if (durationRan < decayMoment) {
				running += Time.deltaTime;
				yield return new WaitForSeconds(Time.deltaTime);
				continue;
			} else {
				float normalDomain = 1f - decayMoment;
				float normalRan = durationRan - decayMoment;
				float normalDone = normalRan / normalDomain;
				float alpha = 1f - normalDone;

				sprite.color = new Color (sprite.color.r, sprite.color.g, sprite.color.b, alpha);

				running += Time.deltaTime;
			}

			yield return new WaitForSeconds(Time.deltaTime);
		}
		Destroy (gameObject);
	}
}
