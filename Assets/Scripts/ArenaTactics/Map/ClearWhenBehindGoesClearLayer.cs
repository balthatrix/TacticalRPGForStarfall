using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreativeSpore.SuperTilemapEditor;

public class ClearWhenBehindGoesClearLayer : MonoBehaviour {

	private Collider2D collider;
	// Use this for initialization
	void Start () {
//		Debug.Log ("INTTE");
		collider = GetComponent<Collider2D> ();
		StartCoroutine (CheckBehind ());
	}
	GoesTransparentTilemapLayer currentlyBehind = null;

	public bool IsCurrentlyBehind {
		get { return currentlyBehind != null; }	
	}

	IEnumerator CheckBehind() {
		while (true) {
			Collider2D intersect = Physics2D.OverlapPoint (transform.localPosition, 1 << LayerMask.NameToLayer ("DisappearsWhenWalkedBehind"));

			if (intersect != null) {
				if (!IsCurrentlyBehind) {
					currentlyBehind = intersect.gameObject.GetComponent<ClearTilemapBehind> ().toClear;
					currentlyBehind.AddBehindLayer (collider);
//					Debug.Log ("HE: " + intersect.name);

				}
			} else {
				if (IsCurrentlyBehind) {
					currentlyBehind.RemoveBehindLayer (collider);
					currentlyBehind = null;
				} 
			}

			yield return new WaitForSeconds (.2f);
		}

	}

	void Update() {
		
	}

	public void OnTriggerEnter2D(Collider2D other) {
		//This should be if it's a character, or if 
		//it's a projectile
//		if (other.GetComponent<TileInhabitant> () != null) {
//			Debug.LogError("othere: " + other.name);
//			toClear.TintColor = new Color (1f, 1f, 1f, .5f);
//		}
		ClearTilemapBehind tm =  other.gameObject.GetComponent<ClearTilemapBehind>();

		if (tm != null) {
			tm.toClear.AddBehindLayer (collider);
			Debug.LogError("ENTER : " + tm.name);
		}

	}
	public void OnTriggerExit2D(Collider2D other) {
//		if (other.GetComponent<TileInhabitant> () != null) {
//			Debug.LogError("otherexit: " + other.name);
//			toClear.TintColor = new Color (1f, 1f, 1f, 1f);
//		}

		ClearTilemapBehind tm =  other.gameObject.GetComponentInParent<ClearTilemapBehind>();

		if (tm != null) {
			tm.toClear.RemoveBehindLayer (collider);
			Debug.LogError("Xit : " + tm.name);
		}
	}

}
