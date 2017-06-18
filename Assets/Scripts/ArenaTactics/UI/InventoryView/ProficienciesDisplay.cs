using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT.Character;

using UnityEngine.UI;

public class ProficienciesDisplay : MonoBehaviour {
	public void ClearContent() {
		GetComponent<GenericFloatingWindow> ().ClearContent ();
	}

	public void SyncUiWithCharacter(Sheet character) {
		ClearContent ();
		AddProficiencyText ("Proficieny: " + character.ProficiencyModifier());

		AddProficiencyText ("Weapons",20);
		if (character.weaponProficiencies.Count == 0) {
			AddProficiencyText ("(none)");
		} else {
			foreach (WeaponProficiency prof in character.weaponProficiencies) {
				AddProficiencyText (prof.PresentableName);
			}
		}

		AddProficiencyText ("-");

		AddProficiencyText ("Armour", 20);
		if (character.armourProficiencies.Count == 0) {
			AddProficiencyText ("(none)");
		} else {
			foreach (ArmourProficiency prof in character.armourProficiencies) {
				AddProficiencyText (prof.PresentableName);
			}
		}
		AddProficiencyText ("-");
		AddProficiencyText ("Saves",20);
		if (character.saveProficiencies.Count == 0) {
			AddProficiencyText ("(none)");
		} else {
			foreach (SaveProficiency prof in character.saveProficiencies) {
				AddProficiencyText (prof.PresentableName);
			}
		}
		AddProficiencyText ("-");
		AddProficiencyText ("Skills",20);
		if (character.skillProficiencies.Count == 0) {
			AddProficiencyText ("(none)");
		} else {
			foreach (SkillProficiency prof in character.skillProficiencies) {
				AddProficiencyText (prof.PresentableName);
			}
		}



	}

	void Start() {
		
	}

	public void AddProficiencyText(string text, int fontSize=14) {
		TextElement elem = GetComponent<GenericFloatingWindow> ().AddTextContent (text);
		elem.GetComponent<Text>().color = new Color(0f,0f,0f,1f);
		elem.GetComponent<Text>().fontSize = fontSize;
		elem.GetComponent<Text> ().alignment = TextAnchor.MiddleCenter;
	}
}
