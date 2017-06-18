using UnityEngine;
using System.Collections;

public class MaskLayer : MonoBehaviour {
	
	public float scaleFactor;
	// Use this for initialization
	void Start () {
		


	}

	public SpriteRenderer SpriteRenderer {
		get { return GetComponent<SpriteRenderer> (); }
	}

	public void SetParent(MaskLayer m) {
		SpriteRenderer.transform.SetParent (m.transform,false);
		SpriteRenderer.transform.localScale = scaleFactor * m.SpriteRenderer.transform.localScale;
		SpriteRenderer.transform.localPosition = Vector3.zero;
	}

	public void Color(Color c) {
		
		GetComponent<SpriteRenderer> ().color = c;
	}
	

}
