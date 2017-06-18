using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is just a dispenser for ui prefabs, which whould be in every scene.
/// </summary>
public class FXPrefabs : MonoBehaviour {

	//damage
	public GameObject healingTextPrefab;

	//healing
	public GameObject physDamageTextPrefab;

	/// missile for thrown weapons, arrows, or spell bolts.
	public GameObject missile;


	public static FXPrefabs instance;
	void Awake() {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (gameObject);
		} else {
			Destroy (this);
			return;
		}
	}


}
