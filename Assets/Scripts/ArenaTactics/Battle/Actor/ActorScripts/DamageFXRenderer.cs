using UnityEngine;
using System.Collections;
using AT.Battle;
using AT.Character;
using System.Collections.Generic;

public class DamageFXRenderer : MonoBehaviour {
	
	Dictionary<AttackingAnimation, int> animationsToAmounts;

	List<int> damageStack;
	float delayBetweenDamages = 0.3f;


	// Use this for initialization
	public Sheet CharSheet {
		get { return GetComponent<Actor>().CharSheet; }
	}

	void Start () {
		damageStack = new List<int> ();
		animationsToAmounts = new Dictionary<AttackingAnimation, int> ();
		CharSheet.OnDamaged += CallPushToDamageStack;
	}


	void OnDestroy() {
		CharSheet.OnDamaged -= CallPushToDamageStack;
	}

	public void  CallPushToDamageStack(AT.Character.Effect.Damage effect, Action source) {

		PushToDamageStack(effect.Amount);
	}

	bool processingDamageStack = false;
	public void PushToDamageStack(int amount) {

		StartCoroutine (Shudder ());
		damageStack.Add (amount);
		if (!processingDamageStack) {
			StartCoroutine(ShiftDamageStack ());
		}
	}

	IEnumerator ShiftDamageStack() {
		processingDamageStack = true;
		while(damageStack.Count > 0) {
			if (!this.enabled) {//the character died....
				break;
			}
			int nextAmount = damageStack [0];
			damageStack.RemoveAt (0);
//			InstantiateDmgText (nextAmount);
			yield return new WaitForSeconds (delayBetweenDamages);
		}

		processingDamageStack = false;

	}


	void DoStackDmgText(AttackingAnimation anim) {

		int amount;
		if(animationsToAmounts.TryGetValue(anim, out amount)) {

			anim.OnHitMoment -= DoStackDmgText;
			animationsToAmounts.Remove(anim);
			PushToDamageStack(amount);
		}
	}

	void InstantiateDmgText(int amount) {
		GameObject damageText = Object.Instantiate (UIManager.instance.physicalDamageText);

		damageText.GetComponent<FadeText> ().textComponent.text = amount.ToString ();
		damageText.transform.position = TileMovement.avatar.position;

	}

	private Vector3 originalPosition;
	private bool shuddering = false;
	IEnumerator Shudder() {
		if (!shuddering) {
			shuddering = true;
			float timeGoneBy = 0f;
			float shudderTime = .15f;
			float rangeDivisor = 8.5f; //means the character will shudder in the range of 1/10 of a map tile 

			originalPosition = transform.position;

			while (true) {
				timeGoneBy += Time.deltaTime;

				if (timeGoneBy >= shudderTime) {
					transform.position = originalPosition;
					break;
				}

				transform.position = new Vector3 (
					originalPosition.x + (Random.value - .5f) / rangeDivisor,
					originalPosition.y + (Random.value - .5f) / rangeDivisor,
					originalPosition.z
				);

				yield return null;
			}
			shuddering = false;
		}
	}

	public TileMovement TileMovement {
		get { return GetComponent<TileMovement> (); }
	}
}
