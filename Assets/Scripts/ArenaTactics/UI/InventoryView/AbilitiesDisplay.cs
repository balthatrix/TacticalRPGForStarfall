using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT.Character;

using UnityEngine.UI;

public class AbilitiesDisplay : StatsDisplay {

	OptButton str;
	OptButton dex;
	OptButton con;
	OptButton intl;
	OptButton wis;
	OptButton cha;

	public override void SyncUiWithCharacter(Sheet character) {
		Clear ();

		str = AddOptButton ("Str:" + character.Strength 	+ " " + ModifierValue("strength"));
		dex = AddOptButton ("Dex:" + character.Dexterity 	+ " " + ModifierValue("dexterity"));
		con = AddOptButton ("Con:" + character.Constitution + " " + ModifierValue("constitution"));
		intl = AddOptButton("Int:" + character.Intelligence + " " + ModifierValue("intelligence"));
		wis = AddOptButton ("Wis:" + character.Wisdom 	 	+ " " + ModifierValue("wisdom"));
		cha = AddOptButton ("Cha:" + character.Charisma 	+ " " + ModifierValue("charisma"));


		str.SetTooltipInfo (Tooltip.TooltipPosition.LEFT, 15, 
			"Strength\n"+ModifierValue("strength")+" to melee hit and damage roll\nCarry weight:"+character.CarryWeight,
			"Strength\nThe modifier, " + ModifierValue("strength") + 
			", affects all melee weapon hit rolls, and damage rolls (main hand only).\nWeight carried is 15 x Strength score of "+GaugeValue("strength") );

		dex.SetTooltipInfo (Tooltip.TooltipPosition.LEFT, 15, 
			"Dexterity\n"+ModifierValue("dexterity")+" to ranged/finesse hit and damage roll\nAC bonus:" + ModifierValue("dexterity") ,
			"Dexterity\nThe modifier, " + ModifierValue("dexterity") + 
			", affects all ranged weapon hit rolls, and damage rolls.  The same applies to melee weapons that have the finesse property.\nAlso, affects Armour Class by the modifier; this is restricted at +2 when wearing medium armour,  and completely when wearing heavy armour.");

		con.SetTooltipInfo (Tooltip.TooltipPosition.LEFT, 15, 
			"Constitution\n" + ModifierValue ("constitution") + " to hit points per level",
			"The modifier, " + ModifierValue("constitution") + " is multiplied by character level " + character.CharacterLevel);

		intl.SetTooltipInfo (Tooltip.TooltipPosition.LEFT, 15, 
			"Intelligence\nWizards prepare more spells, and increase their effectiveness with them.",
			"Intelligence\nThe modifier, " + ModifierValue("intelligence") + " increases/decreases spells a wizard can have prepared in a day.  The base value is the wizard level of the caster.\nThe modifier also affects spell hit rolls when called for, and make a cleric's spells harder to resist.");
		
		wis.SetTooltipInfo (Tooltip.TooltipPosition.LEFT, 15, 
			"Wisdom\nClerics prepare more spells, and increase their effectiveness with them.",
			"Wisdom\nThe modifier, " + ModifierValue("wisdom") + " increases/decreases spells a cleric can have prepared in a day.  The base value is the cleric level of the caster.\nThe modifier also affects spell hit rolls when called for, and make a wizard's spells harder to resist.");
		cha.SetTooltipInfo(Tooltip.TooltipPosition.LEFT, 15,
			"Charisma\nGain more favor with the crowd.", 
			"Charisma\n(Long description needed)");


	}

	string ModifierValue(string gaugeName) {
		int val = Sheet.AbilityScoreModifierValue(InventoryView.instance.currentCharacter.GaugeByName (gaugeName));
		string sign = val >= 0 ? "+" : "";
		return "(" + sign + val + ")";

	}

	int GaugeValue(string gaugeName) {
		return InventoryView.instance.currentCharacter.GaugeByName (gaugeName).ModifiedCurrent;
	}


}


/// <summary>
/// Stats display.  For displaying any stats like abilities or saves.
/// </summary>
public class StatsDisplay : MonoBehaviour {

	public Transform contentParent;

	void Start() {
		contentParent = transform;
		Template ().SetActive (false);
	}

	public virtual Transform ContentParent {
		get { 
			if (contentParent == null) {
				return transform;
			}
			return contentParent;
		}
	}

	public string CharName {
		get { return InventoryView.instance.currentCharacter.Name; }
	}
	public virtual GameObject Template() {
		return	ContentParent.GetChild (0).gameObject;

	}

	public virtual void Clear() {
		for (int i = 1; i < ContentParent.childCount; i++) {
			Destroy (ContentParent.GetChild (i).gameObject);
		}
	}

	/// <summary>
	/// Adds the text template
	/// </summary>
	/// <returns>The text.</returns>
	/// <param name="text">Text.</param>
	public TextElement AddText(string text) {
		GameObject newOne = Instantiate(Template());
		newOne.SetActive (true);
		newOne.transform.SetParent (ContentParent, false);
		newOne.GetComponent<Text> ().text = text;
		return newOne.GetComponent<TextElement> ();
	}

	/// <summary>
	/// Adds the template and returns object
	/// </summary>
	/// <returns>The template.</returns>
	public GameObject AddTemplate() {
		
		GameObject newOne = Instantiate(Template());
		newOne.SetActive (true);
		newOne.transform.SetParent (ContentParent, false);
		return newOne;
	}

	/// <summary>
	/// Adds the opt button template
	/// </summary>
	/// <returns>The opt button.</returns>
	public OptButton AddOptButton(string text) {
		GameObject newOne = Instantiate(Template());
		newOne.SetActive (true);

		newOne.transform.SetParent (ContentParent, false);
		Debug.Log ("name: " + newOne.name);
		newOne.GetComponent<OptButton> ().optText.text = text;
		return newOne.GetComponent<OptButton>();
	}

	public virtual void SyncUiWithCharacter(Sheet character) {
		Debug.LogError ("Syncing should be overriden!");
	}
}
