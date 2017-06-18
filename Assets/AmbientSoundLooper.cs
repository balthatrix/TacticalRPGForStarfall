using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSoundLooper : MonoBehaviour {

	public float delay;
	public AudioSource source;
	public AudioClip clip;

	// Use this for initialization
	void Start () {
		StartCoroutine (DelayBegin ());
	}

	IEnumerator DelayBegin() {
		yield return new WaitForSeconds (delay);
		source.loop = true;
		source.Play ();
	}

}
