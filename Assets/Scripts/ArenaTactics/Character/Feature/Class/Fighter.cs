using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AT.Character;
using System.Linq;
using AT.Character.Situation;
using AT.Serialization;

namespace AT.Character {
		
		public class  FighterLevels {
			public static List<GenericFeature> Level1SingleClass() {

				List<GenericFeature> lvl0 = new List<GenericFeature> ();
				//lvl1.Add (new Features.FightingStyle());

				//This should be conditionally awarded only if the character is take
				//ing fighter as a first class level
				lvl0.Add(new PerLevelHitPoints(10, ClassType.FIGHTER));


				//add all armour prof (really they are just boolean, prof or not for armour
				lvl0.Add(new ArmourProficiencyFeature(EquipmentType.ARMOUR_HEAVY, ProficiencyLevel.FULL));
				lvl0.Add(new ArmourProficiencyFeature(EquipmentType.ARMOUR_MEDIUM, ProficiencyLevel.FULL));
				lvl0.Add(new ArmourProficiencyFeature(EquipmentType.ARMOUR_LIGHT, ProficiencyLevel.FULL));
				lvl0.Add(new ArmourProficiencyFeature(EquipmentType.ARMOUR_SHIELD, ProficiencyLevel.FULL));

				//Add all simple
				lvl0.Add(new WeaponProficiencyFeature(EquipmentSubtype.SIMPLE_CLUB, ProficiencyLevel.FULL));
				lvl0.Add(new WeaponProficiencyFeature(EquipmentSubtype.SIMPLE_DAGGER, ProficiencyLevel.FULL));
				lvl0.Add(new WeaponProficiencyFeature(EquipmentSubtype.SIMPLE_HANDAXE, ProficiencyLevel.FULL));

				//Add all martial
				lvl0.Add(new WeaponProficiencyFeature(EquipmentSubtype.MARTIAL_LONGSWORD, ProficiencyLevel.FULL));


				//saves
				lvl0.Add (new SaveProficiencyFeature (AT.Character.Situation.AbilityType.STRENGTH, ProficiencyLevel.FULL));
				lvl0.Add (new SaveProficiencyFeature (AT.Character.Situation.AbilityType.CONSTITUTION, ProficiencyLevel.FULL));


				lvl0.Add (new HitDiceFeature (10));

				lvl0.Add (new FightingStyle ());

				//You get to choose 2
			//Removing skills as a thing.  Only makes sense as a main game thing.
//				lvl0.Add (new Level1ClassSkills ());
				//lvl0.Add (new Level1ClassSkills ());



				lvl0.Add (new KnightKit ());

				lvl0.Add (new StatelessFeature (SpecialFeatureType.SECOND_WIND));


	//			List<Feature> skillsChoices = new List<Feature
				return lvl0;
			}

			public static List<GenericFeature> Level2() {

				List<GenericFeature> lvl2 = new List<GenericFeature> ();
				    
				lvl2.Add(new PerLevelHitPoints(6, ClassType.FIGHTER));

				lvl2.Add (new HitDiceFeature (10));

				return lvl2;
			}

			public class Level1ClassSkills : FeaturePointer {
				public Level1ClassSkills() : base("Skills", new List<GenericFeature>()) {
					pool.Add(new SkillProficiencyFeature(AT.Character.Situation.SkillType
					.ACROBATICS,
					ProficiencyLevel.FULL));
					//pool.Add(new SkillProficiencyFeature(Situation.SkillType.ANIMAL_HANDLING, ProficiencyLevel.FULL));
					pool.Add(new SkillProficiencyFeature(AT.Character.Situation.SkillType.ATHLETICS, ProficiencyLevel.FULL));
					//pool.Add(new SkillProficiencyFeature(Situation.SkillType.HISTORY, ProficiencyLevel.FULL));
					//pool.Add(new SkillProficiencyFeature(Situation.SkillType.INSIGHT, ProficiencyLevel.FULL));
					//pool.Add(new SkillProficiencyFeature(Situation.SkillType.INTIMIDATION, ProficiencyLevel.FULL));
					pool.Add(new SkillProficiencyFeature(AT.Character.Situation.SkillType.PERCEPTION, ProficiencyLevel.FULL));
					//pool.Add(new SkillProficiencyFeature(Situation.SkillType.SURVIVAL, ProficiencyLevel.FULL));

					filterPool = (Sheet character) => {
						List<GenericFeature> ret = new List<GenericFeature> ();
						foreach (GenericFeature gf in pool) {
							SkillProficiencyFeature f = (SkillProficiencyFeature)gf;
							SkillProficiency alreadyGot = character.BestSkillProficiencyFromType (f.skillType);
							if (alreadyGot != null) {
								
								if(alreadyGot.Ratio < Proficiency.GetRatio (f.lvl) - 0.0001f) {
									ret.Add (gf);
								}
							} else {
								
								ret.Add (gf);
							}
						}
						return ret;
					};

				}
				public override string Name(){
					return "Fighter Level 1 Skill";
				}
			}


			public class FightingStyle : FeaturePointer {
				public FightingStyle() : base("Fighting Style",  new List<GenericFeature>()) {
				//no two-handed weapons, so commenting these until there are bows and 2her animations and assets.
//					pool.Add(new ArcheryFightingStyle());
//					pool.Add(new GreatWeaponFightingStyle());
				//
					pool.Add(new DuelingFightingStyle());
					pool.Add(new DefenseFightingStyle());
					pool.Add(new TwoWeaponFightingStyle());

					filterPool = (Sheet character) => {
						List<GenericFeature> ret = new List<GenericFeature> ();
						List<System.Type> typesInMisc = character.AllMiscFeats().Select((gf)=>gf.GetType()).ToList();
						foreach (GenericFeature gf in pool) {
							if(typesInMisc.Contains(gf.GetType())){
								//dont add...
							} else {
								ret.Add(gf);
							}
						}
						return ret;
					};

				}
				public override string Name(){
					return "Fighting Style";
				}

				public override string Description ()
				{
					return string.Format ("[FightingStyle]");
				}
			}

			[System.Serializable]
			public class ArcheryFightingStyleWrapper:Wrapper {
				public ArcheryFightingStyleWrapper() {}

				public override SerializedObject GetInstance() {
					return new ArcheryFightingStyle ();
				}
			}
			public class ArcheryFightingStyle : GenericFeature, SerializedObject {

				public override Wrapper GetSerializableWrapper ()
				{
					return new ArcheryFightingStyleWrapper ();
				}
	
				public ArcheryFightingStyle() : base() {
				}


	
				public override string Name() {
					return "Fighting Style (Archery)";
				}

				public override string Description ()
				{
					return "You gain a +2 bonus to attack rolls you make with  ranged weapons. ";
				}
					
				public override void WhenActivatedOn(Sheet c) {
					c.OnToHitRoll += AddPlus2IfRanged;
				}
	
	
				public override void WhenDeactivatedOn(Sheet c) {
					c.OnToHitRoll -= AddPlus2IfRanged;
						
				}
	
				private void AddPlus2IfRanged(AttackSituation sit, bool offhand) {
				
					
				if (sit.WeaponUsed != null && sit.WeaponUsed.IsRanged()) {
						sit.ToHitGauge.Modify (new Modifier (2, "Archery Style"));
					}
				}	
			}

			[System.Serializable]
			public class GreatWeaponFightingStyleWrapper:Wrapper {
				public GreatWeaponFightingStyleWrapper() {}

				public override SerializedObject GetInstance() {
					return new GreatWeaponFightingStyle ();
				}
			}

			public class GreatWeaponFightingStyle : GenericFeature, SerializedObject {
				public override Wrapper GetSerializableWrapper ()
				{
					return new GreatWeaponFightingStyleWrapper ();
				}

				public GreatWeaponFightingStyle() : base() {
				}

				public override string Name() {
					return "Fighting Style (Great Weapons)";
				}

				public override string Description ()
				{
					return "When you roll a 1 or 2 on a damage die for an attack  you make with a melee weapon that you are wielding with two hands, you can reroll the die and  must use the new roll, even if the new roll is a 1 or a  2. The weapon must have the two handed or  versatile property for you to gain this benefit. ";
				}
				

				public override void WhenActivatedOn(Sheet c) {
					//c.OnToHitRoll += AddPlus2IfRanged;
					c.OnAboutToAttack += RerollIf1Or2;
				}


				public override void WhenDeactivatedOn(Sheet c) {
					//c.OnToHitRoll -= AddPlus2IfRanged;
					c.OnAboutToAttack -= RerollIf1Or2;
				}

				private void RerollIf1Or2(AttackSituation sit) {

//					bool ranged = sit.Attacker.MainHand ().IsRanged ();
//					if (ranged) {
//						sit.ToHitGauge.Modify (new Modifier (2, "Archery Style"));
//					}
				}	
			}


			[System.Serializable]
			public class DuelingFightingStyleWrapper:Wrapper {
				public DuelingFightingStyleWrapper() {}

				public override SerializedObject GetInstance() {
					return new DuelingFightingStyle ();
				}
			}

			public class DuelingFightingStyle : GenericFeature, SerializedObject {
				public override Wrapper GetSerializableWrapper ()
				{
					return new DuelingFightingStyleWrapper ();
				}

				public DuelingFightingStyle() : base() {
				}

				public override string Name() {
					return "Fighting Style (Dueling)";
				}

			public override void DressOptButtonForTooltip(OptButton opt, Tooltip.TooltipPosition pos= Tooltip.TooltipPosition.TOP, int offset=2) {
				opt.SetTooltipInfo (pos, offset, "Deal +2 damage with single-handed weapons, if holding no other weapons.", "When you are wielding a melee weapon in one hand  and no other weapons, you gain a +2 bonus to  damage rolls with that weapon. ");
			}

			public override void WhenActivatedOn(Sheet c) {
				//c.OnToHitRoll += AddPlus2IfRanged;
				Debug.LogError("Activating " +GetType());
				c.OnAttackDamageProduced += Add2IfOneHandedNoOtherWeapons;

			}


			public override void WhenDeactivatedOn(Sheet c) {
				//c.OnToHitRoll -= AddPlus2IfRanged;
				c.OnAttackDamageProduced -= Add2IfOneHandedNoOtherWeapons;
			}

			private void Add2IfOneHandedNoOtherWeapons( Character.Effect.PhysicalDamage effect, AttackSituation sit, bool offhand) {
				if (sit.WeaponUsed == null) {
//					Debug.LogError("thing again");
				}
				if (sit.WeaponUsed.Properties.Contains (WeaponProperty.HEAVY))
					return;

				GenericWeapon mainhand = sit.Attacker.MainHand ();
				Equipment off_hand = sit.Attacker.OffHand();
//				Debug.LogError ("hfosdoih attack ?");
				if (mainhand == sit.WeaponUsed) {
					if (off_hand is GenericWeapon || mainhand.Subtype == EquipmentSubtype.SIMPLE_FIST) {
						return;
					} else {
//						Debug.LogError ("yes!");
						effect.Gauge.Modify (new Modifier (2, "Dueling"));
					}
				} else {
					if(sit.Attacker.MainHand().Subtype == EquipmentSubtype.SIMPLE_FIST) { //no other weapon is present.
//						Debug.LogError ("yes!");
						
						effect.Gauge.Modify (new Modifier (2, "Dueling"));
					}
				}


			}

					//					bool ranged = sit.Attacker.MainHand ().IsRanged ();
					//					if (ranged) {
					//						sit.ToHitGauge.Modify (new Modifier (2, "Archery Style"));
					//					}
					
			}

			[System.Serializable]
			public class DefenseFightingStyleWrapper:Wrapper {
				public DefenseFightingStyleWrapper() {}

				public override SerializedObject GetInstance() {
					return new DefenseFightingStyle ();
				}
			}

			public class DefenseFightingStyle : GenericFeature, SerializedObject {
				public override Wrapper GetSerializableWrapper ()
				{
					return new DefenseFightingStyleWrapper ();
				}

				public DefenseFightingStyle() : base() {
				}

				public override string Name() {
				Debug.LogError ("hi");
					return "Fighting Style (Defense)";
				}

			public override void DressOptButtonForTooltip(OptButton opt, Tooltip.TooltipPosition pos= Tooltip.TooltipPosition.TOP, int offset=2) {
				opt.SetTooltipInfo (pos, offset,  "The character is well versed in armour of all kinds.",  "Gain +1 to armour class while wearing any armour.");
			}


				public override void WhenActivatedOn(Sheet c) {
					//c.OnToHitRoll += AddPlus2IfRanged;
				Debug.LogError("Activating " +GetType());
					c.OnACProduced += Add1ToAcIfArmour;


				}


				public override void WhenDeactivatedOn(Sheet c) {
					//c.OnToHitRoll -= AddPlus2IfRanged;
					c.OnACProduced -= Add1ToAcIfArmour;
				}

			private void Add1ToAcIfArmour(AttackSituation sit) {
				bool adding = false;
				if (sit.Defender.PaperDoll.Armour () != null) {
					adding = true;
				} else if (sit.Defender.OffHand () is GenericArmour) {
					adding = true;
				}
				if (adding) {
//					Debug.LogError ("defense!");
					sit.ArmorClassGauge.Modify (new Modifier (1, "Defensive"));
				}

			}	
		}


			[System.Serializable]
			public class TwoWeaponFightingStyleWrapper:Wrapper {
				public TwoWeaponFightingStyleWrapper() {}

				public override SerializedObject GetInstance() {
					return new TwoWeaponFightingStyle ();
				}
			}


			public class TwoWeaponFightingStyle : GenericFeature, SerializedObject {
				
				public override Wrapper GetSerializableWrapper ()
				{
					return new TwoWeaponFightingStyleWrapper ();
				}

				public TwoWeaponFightingStyle() : base() {
				}

				public override string Name() {
					return "Fighting Style (Two-Weapon Fighting)";
				}

				public override string Description ()
				{
					return "When you engage in two-weapon fighting, you can  add your ability modifier to the damage of the second attack. ";
				}


			public override void DressOptButtonForTooltip(OptButton opt, Tooltip.TooltipPosition pos= Tooltip.TooltipPosition.TOP, int offset=2) {
				opt.SetTooltipInfo (pos, offset,  "You deal more damage (add ability modifier) to the bonus attack offered by two-weapon fighting",  "When you engage in two-weapon fighting, you can  add your ability modifier to the damage of the second attack. ");
			}

			public override void WhenActivatedOn(Sheet c) {
					//c.OnToHitRoll += AddPlus2IfRanged;
				c.OnAttackDamageProduced += AddAbilityModifierIfOffhand;


			}


			public override void WhenDeactivatedOn(Sheet c) {
					//c.OnToHitRoll -= AddPlus2IfRanged;
				c.OnAttackDamageProduced -= AddAbilityModifierIfOffhand;
			}

			private void AddAbilityModifierIfOffhand(Effect.PhysicalDamage effect, AttackSituation sit, bool offhand) {
				if (sit.WeaponUsed == sit.Attacker.OffHand () && sit.action.IsBonus ) {
					effect.Gauge.Modify(
						new Modifier(sit.Attacker.ModifierValueFromWeapon(sit.WeaponUsed), "Two-weapon fighting")
					);

//					Debug.LogError ("Offhand.  Adding my thing!");
				}
					//					bool ranged = sit.Attacker.MainHand ().IsRanged ();
					//					if (ranged) {
					//						sit.ToHitGauge.Modify (new Modifier (2, "Archery Style"));
					//					}
			}	
		}

            





		} // Fighter features...

}