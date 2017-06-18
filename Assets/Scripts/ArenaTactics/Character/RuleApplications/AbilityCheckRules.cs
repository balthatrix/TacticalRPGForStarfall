using UnityEngine;
using System;
using AT.Character.Situation;
using AT.Character;
using AT.Character.Effect;
/// <summary>
/// Rules are a way of organizing and applying callbacks that are appied to a character sheet in various situations
/// </summary>
namespace AT.CharacterRules {
	public class Abilities {
		Sheet character;
		public Abilities(AT.Character.Sheet character) {
			this.character = character;
			Init ();
		}

		public void Init() {
			character.OnProduceCheck += ApplyProficiency;
			character.OnProduceCheck += ApplyAbilityModifier;
		}

		public void DeInit() {
			character.OnProduceCheck -= ApplyProficiency;
			character.OnProduceCheck -= ApplyAbilityModifier;
		}


		private void ApplyProficiency(CheckSituation sit) {
			if (sit.skillType == SkillType.NULL)
				return;
			
			SkillProficiency best = character.BestSkillProficiencyFromType (sit.skillType);
			if (best != null) {
				int chPro = character.ProficiencyModifier ();
				int bonus = (int) Mathf.Floor (chPro * best.Ratio);
				sit.checkValue.Modify (new Modifier (bonus, PresentableSkillNameFromType (sit.skillType) + " proficiency"));
			}
		}

		public string PresentableSkillNameFromType(SkillType t){
			return t.ToString ().ToLower ();
		}

//
//		public string PresentableName() {
//			return name.ToString ().Replace ("WEAPON_", "").ToLower();
//		}

		private void ApplyAbilityModifier(CheckSituation sit) {
			string targetName = AbilityGaugeNameFromType (sit.abilityType);
			int bonus = Sheet.AbilityScoreModifierValue(character.GaugeByName(targetName));
			sit.checkValue.Modify (new Modifier (bonus, sit.abilityType.ToString ().ToLower ()));
		}



		public static string AbilityGaugeNameFromType(AbilityType type) {
			string ret;
			switch (type) {
			case AbilityType.STRENGTH:
				ret = "strength";
				break;
			case AbilityType.DEXTERITY:
				ret = "dexterity";
				break;
			case AbilityType.CONSTITUTION:
				ret = "constitution";
				break;
			case AbilityType.INTELLIGENCE:
				ret ="intelligence";
				break;
			case AbilityType.WISDOM:
				ret = "wisdom";
				break;
			case AbilityType.CHARISMA:
				ret = "charisma";
				break;
			default:
				throw new UnityException ("No valid type sent");
			}
			if(ret == null)
				throw new UnityException ("No gauge yielded from type : " + type);
			return ret;
		}

		public static AbilityType ParentAbilityFromSkill(SkillType t) {
			AbilityType ret = AbilityType.CHARISMA;
			switch (t) {
			case SkillType.ATHLETICS:
				ret = AbilityType.STRENGTH;
				break;

			case SkillType.ACROBATICS:
				ret = AbilityType.DEXTERITY;
				break;
//			case SkillType.PICKPOCKETING:
//				ret = AbilityType.DEXTERITY;
//				break;
			case SkillType.STEALTH:
				ret = AbilityType.DEXTERITY;
				break;
//
//			case SkillType.ARCANA:
//				ret = AbilityType.INTELLIGENCE;
//				break;
//			case SkillType.RELIGION:
//				ret = AbilityType.INTELLIGENCE;
//				break;
			case SkillType.INVESTIGATION:
				ret = AbilityType.INTELLIGENCE;
				break;
//			case SkillType.HISTORY:
//				ret = AbilityType.INTELLIGENCE;
				break;
			case SkillType.NATURE:
				ret = AbilityType.INTELLIGENCE;
				break;

//			case SkillType.ANIMAL_HANDLING:
//				ret = AbilityType.WISDOM;
//				break;
//			case SkillType.INSIGHT:
//				ret = AbilityType.WISDOM;
				break;
			case SkillType.MEDICINE:
				ret = AbilityType.WISDOM;
				break;
			case SkillType.PERCEPTION:
				ret = AbilityType.WISDOM;
				break;
//			case SkillType.SURVIVAL:
//				ret = AbilityType.WISDOM;
//				break;
			}

			return ret;
		}
	}
}