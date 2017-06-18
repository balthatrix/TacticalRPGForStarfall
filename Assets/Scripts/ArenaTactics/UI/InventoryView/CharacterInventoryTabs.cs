using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AT.Character;

public class CharacterInventoryTabs : MonoBehaviour {


	public Transform tabsRoot;
	public Sheet[] characters;

	void Start() {
		tabsRoot.transform.GetChild (0).gameObject.SetActive (false);
	}
	public void ClearTabs() {
		for (int i = 1; i < tabsRoot.childCount; i++) {
			Destroy (tabsRoot.GetChild (i).gameObject);
		}

	}
	public void PopulateCharacterTabs() {
		ClearTabs ();
		if (characters.Length == 0) {
			return;
		}


		GameObject original = Instantiate(tabsRoot.transform.GetChild (0).gameObject, tabsRoot.transform);
		original.SetActive (true);

		//This has to be done in a seperate loop, or else you can end up with more than one toggle on

		for (int i = 1; i < characters.Length; i++) {
			GameObject nextT = Instantiate (original, tabsRoot.transform);
			SetTabProps(nextT.GetComponent<CharacterInventoryTab>(), characters[i]);
		}
		SetTabProps(original.GetComponent<CharacterInventoryTab>(), characters[0]);

	}

	private void SetTabProps(CharacterInventoryTab tab, Sheet character) {
		if (character == null) {
			Debug.LogError ("Character is null, asshole");
		}
		tab.label.text = character.Name;
		tab.character = character;
		if (character == InventoryView.instance.currentCharacter) {
			Debug.Log ("this is it! "+ character.Name);
			tab.GetComponent<Toggle> ().isOn = true;
		} else {
			Debug.Log ("this is NOT it! "+ character.Name);
			tab.GetComponent<Toggle> ().isOn = false;
		}
	}

}
