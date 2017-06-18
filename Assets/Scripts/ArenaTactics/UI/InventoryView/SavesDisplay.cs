using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT.Character;

using UnityEngine.UI;

public class SavesDisplay : StatsDisplay {


	public void SyncUiWithCharacter(Sheet character) {
		Clear ();

		string savingThrowText = 
			"A saving throw represents a situation in which a character attempts " +
			"to resist some negative repercussion or another, whether from a spell, environment hazard, " +
			"or social situation.  The negative effect will have a difficulty class somewhere between " +
			"5 and 25.  The defending character makes a 1d20 roll, and adds the saving throw bonus." +
			" If the result is equal or greater than the DC, the save was successful and the character takes a reduced or nullified effect. \n\nFor example, suppose an arrow has missed it's target and is " +
			"whizzes toward a character.  This unfortunate character would make a dexterity saving throw to avoid it.  ";
		

		AddOptButton ("Str: " + character.HypotheticalSaveBonus (AT.Character.Situation.AbilityType.STRENGTH)).SetTooltipInfo (
			Tooltip.TooltipPosition.TOP,
			5, 
			"Strength saving throw bonus",
			savingThrowText
		);
		AddOptButton ("Dex: " + character.HypotheticalSaveBonus (AT.Character.Situation.AbilityType.DEXTERITY)).SetTooltipInfo (
			Tooltip.TooltipPosition.TOP,
			5, 
			"Charisma saving throw bonus",
			savingThrowText
		);
		AddOptButton ("Con: " + character.HypotheticalSaveBonus (AT.Character.Situation.AbilityType.CONSTITUTION)).SetTooltipInfo (
			Tooltip.TooltipPosition.TOP,
			5, 
			"Charisma saving throw bonus",
			savingThrowText
		);
		AddOptButton ("Int: " + character.HypotheticalSaveBonus (AT.Character.Situation.AbilityType.INTELLIGENCE)).SetTooltipInfo (
			Tooltip.TooltipPosition.TOP,
			5, 
			"Charisma saving throw bonus",
			savingThrowText
		);
		AddOptButton ("Wis: " + character.HypotheticalSaveBonus (AT.Character.Situation.AbilityType.WISDOM)).SetTooltipInfo (
			Tooltip.TooltipPosition.TOP,
			5, 
			"Charisma saving throw bonus",
			savingThrowText
		);

		AddOptButton ("Cha: " + character.HypotheticalSaveBonus (AT.Character.Situation.AbilityType.CHARISMA)).SetTooltipInfo (
			Tooltip.TooltipPosition.TOP,
			5, 
			"Charisma saving throw bonus",
			savingThrowText
		);
	}
}