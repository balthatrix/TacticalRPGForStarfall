using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT.Character;

using UnityEngine.UI;

public class CombatDisplay : StatsDisplay {
	

	public override void SyncUiWithCharacter(Sheet character) {
		Clear ();
		int mainHand = character.HypotheticalToHitBonus ();

		AddOptWithText("M/H Att: " + mainHand).SetTooltipInfo(
			Tooltip.TooltipPosition.BOTTOM,
			10,
			"Main hand attack bonus\nA measure of accuracy when attacking with main hand.",
			"Main hand attack bonus\n During a main hand attack, " +CharName+ 
			" will roll a 1d20, and add this value (" + character.HypotheticalToHitBonus() + "). If "+
			"this value is equal or greater than the defender's AC, the hit connects with the target.  Otherwise, it's assumed as if the attack missed or was deflected."
		);


		AddOptWithText ("HP: " + character.HitPoints + "/" + character.HitPointsGauge.ModifiedMax).SetTooltipInfo (
			Tooltip.TooltipPosition.BOTTOM,
			10,
			"Once this value reaches 0, " + CharName + " is dead.",
			null
		);

		if (character.PaperDoll.EquippedOn (EquipmentSlotType.OFF_HAND) is GenericWeapon) {
			int offHand = character.HypotheticalOffhandToHitBonus ();
			AddOptWithText("O/H Att: " + offHand).SetTooltipInfo(
				Tooltip.TooltipPosition.BOTTOM,
				10,
				"Offhand attack bonus\nA measure of accuracy when attacking with off hand.",
				"Offhand attack bonus\n During an offhand hand attack, " +CharName+ 
				" will roll a 1d20, and add this value (" + character.HypotheticalOffhandToHitBonus() + "). If "+
				"this value is equal or greater than the defender's AC, the hit connects with the target.  Otherwise, it's assumed as if the attack missed or was deflected."
			);
		} else {
			AddOptWithText("O/H Att: N/a").SetTooltipInfo(
				Tooltip.TooltipPosition.BOTTOM,
				10,
				"There is no offhand weapon " + CharName + " is wielding.",
				null
			);

		}

		AddOptWithText ("AC: " + character.HypotheticalAC ()).SetTooltipInfo(
			Tooltip.TooltipPosition.BOTTOM,
			10,
			"Armour Class\nUsed to resist incoming attacks.",
			"Armour Class\nAttacks come in the form of spells, melee swings, and ranged shots/throws.  AC is used to resist them all.  During an attack, the attacker rolls 1d20, and adds to hit bonus.  If the armour class of the defender is better, the attack has no effect."
		);


		AddOptWithText ("Mv: " + character.MovementSpeedAsFeet).SetTooltipInfo(
			Tooltip.TooltipPosition.BOTTOM,
			10,
			"Movement Speed\nHow far in feet " + CharName + " can move on their turn.",
			"Movement Speed\nEach turn, " +CharName+ " is allowed to move "+character.MovementSpeedAsFeet+"ft. Each square of the battlefield is 5x5 feet."
		);

		AddOptWithText ("Vis: " + AT.Battle.Vision.VISION_DISTANCE).SetTooltipInfo(
			Tooltip.TooltipPosition.BOTTOM,10,"How far "+CharName+" can see on the battlefield.",
			"Vision\nFor every 5 feet a character travels on the field, the character will cast their vision in every direction a distance of 5ft x Vision.");
	}

	OptButton AddOptWithText(string text) {
		GameObject temp = AddTemplate ();
		temp.GetComponent<OptButton>().optText.text = text;
		return temp.GetComponent<OptButton> ();
	}
}
