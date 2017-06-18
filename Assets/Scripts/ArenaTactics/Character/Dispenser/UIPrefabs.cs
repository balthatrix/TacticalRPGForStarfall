using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is just a dispenser for ui prefabs, which whould be in every scene.
/// </summary>
public class UIPrefabs : MonoBehaviour {

	//Windows
	public GameObject dialogWindowPrefab;
	public GameObject windowPrefab;
	public GameObject textInputDialogPrefab;


	///Inputs
	public GameObject dropdownMenu;
	public GameObject optButtonPrefab;
	public GameObject numberCounterPrefab;


	//Text
	public GameObject textElementPrefab;

	//Layout
	public GameObject horizontalLayoutElementPrefab;


	public static UIPrefabs instance;
	void Awake() {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (gameObject);
		} else {
			Destroy (this);
			return;
		}
	}

	public GenericDialog GetDialogWindow(GameObject instancedDialog) {
		return instancedDialog.transform.GetChild (0).GetComponent<GenericDialog> ();
	}

}
