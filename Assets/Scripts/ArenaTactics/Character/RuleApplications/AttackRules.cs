using UnityEngine;
using System;
using System.Collections.Generic;
using AT.Character.Situation;
using AT.Character;
using AT.Character.Effect;
using System.Linq;
/// <summary>
/// Rules are a way of organizing and applying callbacks that are appied to a character sheet in various situations
/// </summary>
namespace AT.CharacterRules {
	public class Combat {
		Sheet character;
		public Combat(AT.Character.Sheet character) {
			this.character = character;
			Init ();
		}



		public void Init() {
			character.OnToHitRoll += ApplyAttackBonusToWeaponAttackContestAndSetWeaponUsed;

			character.OnToHitRoll += FlagDisadvantageIfRangedAndNearEnemyOrOutNormRange;
			character.OnAttackDamageProduced += ApplyDamageBonusToWeaponAttack;
			character.OnACProduced += ApplyBaseAC;
			character.OnACProduced += ApplyDexterityToAC;
			character.OnProduceInitiative += ApplyDexterityToInitiative;
		}

		public void FlagDisadvantageIfRangedAndNearEnemyOrOutNormRange(AttackSituation sit, bool offhand) {
			Battle.Attack attack = sit.action as Battle.Attack;
			TileMovement tmv;
			List<Battle.Actor> actorsInRange;
			if (attack == null) {
//				Debug.LogError ("hypoth");
				//probably hypothetical
				Battle.Cast cast = sit.action as Battle.Cast;
				if(cast == null) 
					return;
				
				if (cast.IsRangedSpell) {
					Battle.Actor caster = cast.actor;
					tmv = caster.TileMovement;
					actorsInRange = tmv.TilesWithinRange (1, true)
						.Select((t) => t.FirstOccupant)
						.Where((tm)=> tm != null)
						.Select((tm)=> tm.ActorComponent)
						.ToList();

					if(actorsInRange.Select((a)=>a.EnemiesWith(sit.action.actor)).Contains(true)) {
						sit.FlagDisadvantage(); 
						//				Debug.LogError ("dissing in range!");
					}
				}

				return;
			}

			//Too near logic
			if (!attack.TypeChoice.IsRanged())
				return;
			
			Battle.Actor attacker = attack.actor;
			tmv = attacker.TileMovement;
			actorsInRange = tmv.TilesWithinRange (1, true)
				.Select((t) => t.FirstOccupant)
				.Where((tm)=> tm != null)
				.Select((tm)=> tm.ActorComponent)
				.ToList();

//			Debug.LogError ("Ctors: " + actorsInRange.Count);
		
			
			if(actorsInRange.Select((a)=>a.EnemiesWith(sit.action.actor)).Contains(true)) {
				sit.FlagDisadvantage(); 
//				Debug.LogError ("dissing in range!");
			}


			if (attack.TargetActor ().TileMovement.occupying.HCostTo (attack.actor.TileMovement.occupying) > sit.WeaponUsed.NormRng) {
//				Debug.LogError ("extended range.  You sufer");
				sit.FlagDisadvantage ();
			}

		}

		public void DeInit() {
			character.OnToHitRoll -= ApplyAttackBonusToWeaponAttackContestAndSetWeaponUsed;
			character.OnAttackDamageProduced -= ApplyDamageBonusToWeaponAttack;
			character.OnACProduced -= ApplyBaseAC;
			character.OnACProduced -= ApplyDexterityToAC;
			character.OnProduceInitiative += ApplyDexterityToInitiative;
		}

		private void ApplyDexterityToInitiative(Gauge initiative) {
			int value = Sheet.AbilityScoreModifierValue (character.GaugeByName("dexterity"));
			initiative.Modify(new Modifier(value, "Dexterity"));
		}


		void ApplyAttackBonusToWeaponAttackContestAndSetWeaponUsed (AttackSituation c, bool offhand) {

			if (c.action is AT.Battle.Cast) {

				var cast = c.action as AT.Battle.Cast;
				ApplySpellAbilityAttackAndProficiencyBonus (c.ToHitGauge, cast.Spell.classType);
				return;
			}

			GenericWeapon w;

			if (offhand) {
				w = character.OffHand () as GenericWeapon;
			} else {
				w = character.MainHand ();
			}

			c.SetWeaponUsed (w);


			ApplyAttackAbilityBonus (c.ToHitGauge, offhand);
			ApplyAttackProficiencyBonus (c.ToHitGauge, offhand);
		}

		public void ApplyAttackProficiencyBonus(Gauge toHit, bool offhand) {
			float proficiencyRatio;
			if(offhand)
				proficiencyRatio = character.WeaponProficiencyRatio(character.OffHand() as GenericWeapon);
			else
				proficiencyRatio = character.WeaponProficiencyRatio(character.MainHand());


			int proficiencyMod = (int) Mathf.Ceil(proficiencyRatio * character.ProficiencyModifier ());
			toHit.Modify (new Modifier(proficiencyMod, " Proficiency"));
		}


		public void ApplyAttackDamageBonus(Gauge toDamage) {
			ApplyAttackDamageAbilityBonus (toDamage);
		}

		private void ApplyAttackDamageAbilityBonus(Gauge damage, bool offhand=false) {

			string abilityBonusType = "Strength";

			GenericWeapon w;
			if (offhand) {
				w = character.OffHand () as GenericWeapon;
			} else { 
				w = character.MainHand ();
			}

			if ( w.IsRanged ()) {
				abilityBonusType = "Dexterity";
			} else if ( w.Properties.Contains (WeaponProperty.FINESSE)) {
				int strMod = AttackAbilityModifier ("Strength");
				int dexMod = AttackAbilityModifier ("Dexterity");


				if (dexMod > strMod) {
					abilityBonusType = "Dexterity";
				}
			} 

			int mod = AttackAbilityModifier (abilityBonusType);
			//Rules say that offhand can modify damage if negative
			if(!offhand || mod < 0)
				damage.Modify (new Modifier(mod, abilityBonusType+" Bonus"));
		}

		private void ApplySpellAbilityAttackAndProficiencyBonus(Gauge toHit, ClassType classType) {

			string name = "intelligence";
			switch (classType) {
			case ClassType.CLERIC:
				name = "wisdom";
				break;
			default:
				break;

			}

			int mod = Sheet.AbilityScoreModifierValue (character.GaugeByName(name));

			toHit.Modify(new Modifier(mod, name + " Bonus"));

			int proficiencyMod = character.ProficiencyModifier ();
			toHit.Modify (new Modifier(proficiencyMod, " Proficiency"));
		}

		private void ApplyAttackAbilityBonus(Gauge toHit, bool offhand=false) {
			string abilityBonusType = "Strength";

			GenericWeapon w;
			if (offhand) {
				w = character.OffHand () as GenericWeapon;
				if(w == null)//there is no weapon
					return;
			} else { 
				w = character.MainHand ();
			}
		
			if ( w.IsRanged ()) {
				abilityBonusType = "Dexterity";
			} else if ( w.Properties.Contains (WeaponProperty.FINESSE)) {
				int strMod = AttackAbilityModifier ("Strength");
				int dexMod = AttackAbilityModifier ("Dexterity");


				if (dexMod > strMod) {
					abilityBonusType = "Dexterity";
				}
			} 

			int mod = AttackAbilityModifier (abilityBonusType);

			toHit.Modify (new Modifier(mod, abilityBonusType+" Bonus"));
		}

		void ApplyDamageBonusToWeaponAttack (PhysicalDamage e, AttackSituation c, bool offhand) {
			//will apply damage bonus with 0 proficiency bonus (you never add proficiency to damage)
			if(c.Attacker.OffHand() != c.WeaponUsed) {
				ApplyAttackDamageBonus( e.Gauge);	
			}

		}


		private void ApplyBaseAC(AttackSituation sit) {
			sit.ArmorClassGauge.SetCurrent(character.ArmorClass);
		}


		private void ApplyDexterityToAC(AttackSituation sit) {
			int value = Sheet.AbilityScoreModifierValue (character.GaugeByName("dexterity"));
			GenericArmour armour = character.PaperDoll.Armour ();
			if (armour != null && value > armour.MaxDexterityACModifier) {
				value = armour.MaxDexterityACModifier;
			}
			sit.ArmorClassGauge.Modify(new Modifier(value, "Dexterity"));
		}

		public int AttackDamageAbilityModifier(string type) {
			//at this point it's just the attack ability modifier
			return AttackAbilityModifier (type);
		}

		public int AttackAbilityModifier(string type) {
			int attackModifier = 0;
			if (type == "Dexterity") {
				attackModifier = Sheet.AbilityScoreModifierValue (character.GaugeByName("dexterity"));
			} else {
				attackModifier = Sheet.AbilityScoreModifierValue (character.GaugeByName("strength"));
			}
			return attackModifier;
		}



	}

}