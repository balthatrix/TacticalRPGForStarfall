using UnityEngine;
using System;
using AT.Character.Situation;
using AT.Character;
using AT.Character.Effect;
/// <summary>
/// Rules are a way of organizing and applying callbacks that are appied to a character sheet in various situations
/// </summary>
namespace AT.CharacterRules {
	public class Saves {
		Sheet character;
		public Saves(AT.Character.Sheet character) {
			this.character = character;
			Init ();
		}

		public void Init() {
			character.OnProduceSave += ApplyProficiency;
			character.OnProduceSave += ApplyAbilityModifier;
			character.OnProduceDcInSave += ApplySpellDc;
		}

		public void DeInit() {
			character.OnProduceSave -= ApplyProficiency;
			character.OnProduceSave -= ApplyAbilityModifier;
		}

		private void ApplySpellDc(SaveSituation sit) {
			if (sit.context == SaveContext.SPELL && sit.spell != null) {
				sit.DC.ChangeCurrentAndMax (8 + character.ProficiencyModifier() + SpellDCAbilityModifier(sit.spell));
			}
		}

		private int SpellDCAbilityModifier(SpellLibrary.Spell spell) {
			switch (spell.classType) {
			case ClassType.WIZARD:
				return Sheet.AbilityScoreModifierValue (character.GaugeByName ("intelligence"));
				break;
			default:
				Debug.LogError ("You need to make ability modifier for spells DC: " + spell.classType);
				return 0;
				break;
			}
		}



		private void ApplyProficiency(SaveSituation sit) {
			SaveProficiency best = character.BestSaveProficiencyFromType (sit.abilityType);
			if (best != null) {
				int chPro = character.ProficiencyModifier ();
				int bonus = (int) Mathf.Floor (chPro * best.Ratio);
				sit.saveValue.Modify (new Modifier (bonus, "save proficiency"));
			}
		}

		public string PresentableAbilityNameFromType(AbilityType t){
			return t.ToString ().ToLower ();
		}



		private void ApplyAbilityModifier(SaveSituation sit) {
			string gaugeName = Abilities.AbilityGaugeNameFromType (sit.abilityType);
			int bonus = Sheet.AbilityScoreModifierValue(character.GaugeByName(gaugeName));
			sit.saveValue.Modify (new Modifier (bonus, sit.abilityType.ToString ().ToLower ()));
		}



	}
}