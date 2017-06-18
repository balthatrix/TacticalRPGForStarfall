using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT.Character;
using System.Linq;
using AT.Character.Situation;
using AT.Battle;
public class SpellLibrary : Util.Singleton<SpellLibrary> {

	public interface SpellScaler {
		int ScaleValue();
	}

	public class ClassLevelScaler : SpellScaler {
		Sheet character;
		ClassType type;
		public ClassLevelScaler(Sheet character, ClassType type) {
			this.character = character;
			this.type = type;
		}

		public int ScaleValue() {
			int n = character.ClassLevelIn (type, true);
			Debug.Log ("heres n: " + n);
			return n;
		}
	}

	public class CharacterLevelScaler : SpellScaler {
		Sheet character;
		public CharacterLevelScaler(Sheet character) {
			this.character = character;

		}

		public int ScaleValue() {
			int n = character.CharacterLevel;
			Debug.Log ("heres n char lvl: " + n);
			return n;
		}
	}

	public class SlotScaler : SpellScaler {
		SpellSlot slot;
		Spell spell;
		public SlotScaler(Spell spell, SpellSlot slot) {
			this.spell = spell;
			this.slot  = slot;
		}

		public int ScaleValue() {
			return (slot.level - spell.level);
		}
	}

	

	public delegate void HitMoment (AnimationTransform inst);

	public List<Spell> SpellsByClassAndLevel(ClassType classType, int lvl) {
		Debug.Log ("Geting spells by " + classType.ToString () + " and " + lvl);
		return AllSpells.Where ((spell) => spell.classType == classType && spell.level == lvl).ToList();
	}

	public List<Spell> SpellsFromPool(List<SpellPoolElement> pool) {
		return pool.Select ((elem) => SpellByClassAndName (elem.ClassType, elem.SpellName)).ToList();
	}

	public Spell SpellByClassAndName(ClassType classType, SpellName spellName) {
		return AllSpells.Where ((s) => s.classType == classType && s.name == spellName).Last ();
	}

	void GaugeDamageRollAndTally(Gauge dmgGauge, int sides, int times = 1, bool critical = false) {
		for (int i = 0; i < times; i++) {
			dmgGauge.ChangeCurrentAndMax(Sheet.DiceRoll(sides));

			if (critical) {
				dmgGauge.Modify (new Modifier (Sheet.DiceRoll (sides), "Critical"));
			}
		}
	}

	void GaugeDamageRollAndTally(Gauge dmgGauge, string diceNotation, bool critical = false) {

		//should allow passing in 1d
		int times = int.Parse(diceNotation.ToLower().Split ('d') [0]);
		int sides = int.Parse(diceNotation.ToLower().Split ('d') [1]);

		Debug.Log ("dic note: " + diceNotation);
		Debug.Log ("times/sides: " + times.ToString() + "/" + sides.ToString());

		for (int i = 0; i < times; i++) {
			dmgGauge.ChangeCurrentAndMax(Sheet.DiceRoll(sides));

			if (critical) {
				dmgGauge.Modify (new Modifier (Sheet.DiceRoll (sides), "Critical"));
			}
		}
	}


	int NumDiceFromScaleThresholds(List<int> thresholds, SpellScaler scaler) {
		int value = 1;
		if (scaler != null) {
			
			value = scaler.ScaleValue ();
			Debug.Log ("Got a scale valuer: " + value);
		}

		int ret = 1;
		int bonusDice = 0;
		for (int i = thresholds.Count - 1; i >= 0; i--) {
			int threshold = thresholds [i];
			if (value >= threshold) {
				bonusDice = i + 1; 
				break;
			}
		}


		return ret + bonusDice;
	}

	void Start() {
		Spell s = new Spell ();

		s.cast = SpellRangedAttack ((Action cast, AT.ATTile target)=>{
			AT.Character.Effect.Damage dmg = new AT.Character.Effect.Damage(AT.Character.Effect.DamageType.NECROTIC);

			int numDice = NumDiceFromScaleThresholds(new List<int> {5,11,17}, (cast as Cast).Spell.scaler);
			GaugeDamageRollAndTally(dmg.gauge, numDice.ToString()+"d8");

			AT.Character.Condition.ICondition condition  =	new AT.Character.Condition.Necrosis(1, AT.Character.Condition.TickType.TURN_BEGIN, cast.actor.CharSheet);
			AT.Character.Effect.ConditionEffect necrosis = 	new AT.Character.Effect.ConditionEffect(condition);

			return new List<AT.Character.Effect.GenericEffect> { dmg, necrosis };
		}, MissileScript.MissileAnimationName.PLACEHOLDER,
		 (Action cast, AT.ATTile target)=>  {
			AT.Character.Effect.Damage dmg = new AT.Character.Effect.Damage(AT.Character.Effect.DamageType.NECROTIC);

				int numDice = NumDiceFromScaleThresholds(new List<int> {5,11,17}, (cast as Cast).Spell.scaler);
				GaugeDamageRollAndTally(dmg.gauge, numDice.ToString()+"d8", true);
				AT.Character.Condition.ICondition condition =	new AT.Character.Condition.Necrosis(1, AT.Character.Condition.TickType.TURN_BEGIN, cast.actor.CharSheet)  ;
				AT.Character.Effect.ConditionEffect necrosis = 	new AT.Character.Effect.ConditionEffect(condition);

				return new List<AT.Character.Effect.GenericEffect> { dmg, necrosis };
		});
		
		s.school = School.NECROMANCY;
		s.name = SpellName.CHILL_TOUCH;
		s.classType = ClassType.WIZARD;
		s.targetingType = TargetingType.SINGLE_ENEMY;
		s.rangeInSquares = 20;
		s.isCantrip = true;
	
		s.description = "<i>Chill Touch\nMake a ranged spell attack against an enemy within 120ft.  The enemy suffers 1d8 necrotic damage, and cannot be healed until the start of the caster's next turn.</i>";
		RegisterSpell (s);


		CastRoutine sgCast = SpellTouchAttack ((Action cast, AT.ATTile target)=>{
			AT.Character.Effect.Damage dmg = new AT.Character.Effect.Damage(AT.Character.Effect.DamageType.LIGHTNING);

			int numDice = NumDiceFromScaleThresholds(new List<int> {5,11,17}, (cast as Cast).Spell.scaler);
			GaugeDamageRollAndTally(dmg.gauge, numDice.ToString()+"d8");

			AT.Character.Effect.DelegateEffect shocked = new AT.Character.Effect.DelegateEffect((character, spellCast)=>{
				target.FirstOccupant.ActorComponent.PreventReactions(spellCast);
			});

			return new List<AT.Character.Effect.GenericEffect> { dmg, shocked };
		}, (Action cast, AT.ATTile target)=>{
			AT.Character.Effect.Damage dmg = new AT.Character.Effect.Damage(AT.Character.Effect.DamageType.LIGHTNING);

			int numDice = NumDiceFromScaleThresholds(new List<int> {5,11,17}, (cast as Cast).Spell.scaler);
			GaugeDamageRollAndTally(dmg.gauge, numDice.ToString()+"d8", true);

			AT.Character.Effect.DelegateEffect shocked = new AT.Character.Effect.DelegateEffect((character, spellCast)=>{
				target.FirstOccupant.ActorComponent.PreventReactions(spellCast);
			});
			return new List<AT.Character.Effect.GenericEffect> { dmg, shocked };
		});
		Spell shockingGrasp = new Spell ();
		shockingGrasp.cast = sgCast;
		shockingGrasp.school = School.EVOCATION;
		shockingGrasp.name = SpellName.SHOCKING_GRASP;
		shockingGrasp.classType = ClassType.WIZARD;
		shockingGrasp.targetingType = TargetingType.SINGLE_ENEMY;
		shockingGrasp.rangeInSquares = 1;
		shockingGrasp.isCantrip = true;
		RegisterSpell (shockingGrasp);

		CastRoutine fbCast =  SpellRangedAttack((Action cast, AT.ATTile target)=>{
			AT.Character.Effect.Damage dmg = new AT.Character.Effect.Damage(AT.Character.Effect.DamageType.FIRE);


			int numDice = NumDiceFromScaleThresholds(new List<int> {5,11,17}, (cast as Cast).Spell.scaler);
			GaugeDamageRollAndTally(dmg.gauge, numDice.ToString()+"d10");

			return new List<AT.Character.Effect.GenericEffect> { dmg };
		}, MissileScript.MissileAnimationName.PLACEHOLDER, 
			(Action cast, AT.ATTile target)=>{
			AT.Character.Effect.Damage dmg = new AT.Character.Effect.Damage(AT.Character.Effect.DamageType.FIRE);

			int numDice = NumDiceFromScaleThresholds(new List<int> {5,11,17}, (cast as Cast).Spell.scaler);
			GaugeDamageRollAndTally(dmg.gauge, numDice.ToString()+"d10", true);

			return new List<AT.Character.Effect.GenericEffect> { dmg };
		});
		Spell firebolt = new Spell ();
		firebolt.cast = fbCast;
		firebolt.school = School.EVOCATION;
		firebolt.name = SpellName.FIREBOLT;
		firebolt.classType = ClassType.WIZARD;
		firebolt.targetingType = TargetingType.SINGLE_ENEMY;
		firebolt.rangeInSquares = 12;
		firebolt.isCantrip = true;
		RegisterSpell (firebolt);

		CastRoutine poisonSprayCast =  SpellSavingThrow((Action cast, AT.ATTile target)=>{
			AT.Character.Effect.Damage dmg = new AT.Character.Effect.Damage(AT.Character.Effect.DamageType.POISON);
			dmg.Gauge.ChangeCurrentAndMax(Sheet.DiceRoll(12));

			return new List<AT.Character.Effect.GenericEffect> { dmg };
		},
		(Action cast, AT.ATTile target) => {
			//miss effect does nothing
				return new List<AT.Character.Effect.GenericEffect>();
			}, AbilityType.CONSTITUTION);
	
		Spell poisonSpray = new Spell ();
		poisonSpray.cast = poisonSprayCast;
		poisonSpray.school = School.CONJURATION;
		poisonSpray.name = SpellName.POISON_SPRAY;
		poisonSpray.classType = ClassType.WIZARD;
		poisonSpray.targetingType = TargetingType.SINGLE_ENEMY;
		poisonSpray.rangeInSquares = 2;
		poisonSpray.isCantrip = true;
		RegisterSpell (poisonSpray);

		CastRoutine magicMissileCast = ((caster, cast, targets) => {
			float delay = 0f; //this doesn't work...  All missils have the same delay
			foreach(AT.ATTile target in targets) {
				CharacterCastingAnimation castingAnims = caster.GetComponent<CharacterCastingAnimation>();
				GameObject prefab = MissileAnimationPrefabDispenser.instance.GetAnimationPrefabByName(MissileScript.MissileAnimationName.PLACEHOLDER);
				if(castingAnims != null) {
					castingAnims.SetupAndDoCastAnimation(cast);
					castingAnims.OneShotSpellRelease((animationInst) => {

						GameObject copy = Instantiate(prefab);
						copy.transform.position = caster.transform.position;
						MissileScript missile = copy.GetComponent<MissileScript>();

						missile.DelayLaunchAt(target, delay, true);
						//Safe to not worry about unsubbing, thanks to garbage cleanup
						missile.OnConnectedWithTarget += (MissileScript self) => {
							AT.Character.Effect.Damage d = new AT.Character.Effect.Damage(AT.Character.Effect.DamageType.FORCE);
							d.Gauge.ChangeCurrentAndMax(Sheet.DiceRoll(4) + 1);
							if(target.FirstOccupant != null) {
								d.ApplyTo(target.FirstOccupant.ActorComponent.CharSheet);
							}
							//This assumes only one missile.   Needs a "Checkin of sorts, to conditionally check on finished"
							cast.CallOnFinished();
						};




					}); //ToBe ATOCIJL

				} else {

					GameObject copy = Instantiate(prefab);
					copy.transform.position = caster.transform.position;
					MissileScript missile = copy.GetComponent<MissileScript>();
					missile.DelayLaunchAt(target, delay, true);
					//Safe to not worry about unsubbing, thanks to garbage cleanup
					missile.OnConnectedWithTarget += (MissileScript self) => {
						AT.Character.Effect.Damage d = new AT.Character.Effect.Damage(AT.Character.Effect.DamageType.FORCE);
						d.Gauge.ChangeCurrentAndMax(Sheet.DiceRoll(4) + 1);
						if(target.FirstOccupant != null) {
							d.ApplyTo(target.FirstOccupant.ActorComponent.CharSheet);
						}
						//This assumes only one missile.   Needs a "Checkin of sorts, to conditionally check on finished"
						cast.CallOnFinished();
					};
				}	
				delay += .2f;
			}
		});

		Spell magicMissile = new Spell ();
		magicMissile.cast = magicMissileCast;
		magicMissile.level = 1;
		magicMissile.school = School.EVOCATION;
		magicMissile.name = SpellName.MAGIC_MISSILE;
		magicMissile.classType = ClassType.WIZARD;
		magicMissile.targetingType = TargetingType.MULTIPLE_ENEMY;
		magicMissile.alterCastOnSetParams = (cast) => { 
			cast.NumTargets = 3;
			cast.NumTargets += magicMissile.scaler.ScaleValue();
		};
		magicMissile.rangeInSquares = 60;

		RegisterSpell (magicMissile);


	}



	public List<Spell> AllSpells {
		get { 
			return allSpells.ToList ();
		}
	}

	public enum TargetingType {
		SINGLE_CHARACTER,
		SINGLE_ENEMY,
		SINGLE_ALLY,
		MULTIPLE_CHARACTER,
		MULTIPLE_ENEMY,
		MULTIPLE_ALLY,
		SINGLE_GROUND,
		MULTIPLE_GROUND
	}

	public enum School {
		NECROMANCY,
		ILLUSION,
		EVOCATION,
		ABJURATION,
		ECHANTMENT,
		DIVINATION,
		CONJURATION,
		TRANSMUTATION
	}

	public enum SpellName {
		CHILL_TOUCH,
		SHOCKING_GRASP,
		FIREBOLT,
		POISON_SPRAY,

		MAGIC_MISSILE
	}

	public enum ScaleType{
		CLASS_LVL,
		SLOT,
	}

	public delegate void CastRoutine(AT.Battle.Actor caster, AT.Battle.Action cast, List<AT.ATTile> targets);

	public delegate void AlterCastActionOnSetParams(Cast cast);

	public class Spell
	{
		public CastRoutine cast;
		public AlterCastActionOnSetParams alterCastOnSetParams;
		public SpellName name;
		public School school;
		public TargetingType targetingType;
		public IconName icon;
		public ClassType classType;
		public string description;
		public bool isCantrip;
		public int rangeInSquares;
		public int level;
		public SpellScaler scaler; //scaler is set late by the Cast Action, based on the scale type.  They have to be set late
		public ScaleType scaleType;

		public Spell() {
			
		}
		public Spell(CastRoutine cast,
			 AlterCastActionOnSetParams alterCastOnSetParams,
			 SpellName name,
			 School school,
			 TargetingType targetingType,
			 IconName icon,
			 ClassType classType,
			 string description,
			 bool isCantrip,
			 int rangeInSquares,
			 int level,
			 SpellScaler scaler) {

			this.cast = cast;
			this.alterCastOnSetParams = alterCastOnSetParams;
			this.name = name;
			this.school = school;
			this.targetingType = targetingType;
			this.icon = icon;
			this.classType = classType;
			this.description = description;
			this.isCantrip = isCantrip;
			this.rangeInSquares = rangeInSquares;
			this.level = level;
			this.scaler = scaler;

		}
	}

	[System.Serializable]
	public class SpellSlot  
	{
		public int level;
		public bool used;
	}

	private List<Spell> allSpells = new List<Spell>();


	public void RegisterSpell(Spell s) {
		allSpells.Add (s);

	}

	/// <summary>
	/// Spell attack effector.  Used to delegate hit/miss/success/failure effects.
	/// </summary>
	public delegate List<AT.Character.Effect.GenericEffect> SpellAttackEffector(AT.Battle.Action cast, AT.ATTile target);

	public CastRoutine SpellSavingThrow (SpellAttackEffector failure, SpellAttackEffector success, AbilityType abilityType) {
		return (AT.Battle.Actor caster, AT.Battle.Action cast, List<AT.ATTile> targets) => {
			AT.Battle.Cast c = cast as AT.Battle.Cast;
			foreach(AT.ATTile target in targets) {
				SaveSituation sit = new SaveSituation(target.FirstOccupant.ActorComponent.CharSheet,
					cast.actor.CharSheet,
					abilityType,
					SaveContext.SPELL,
					c.Spell);
				List<AT.Character.Effect.GenericEffect> resultingEffects = null;

				ResultType result = sit.GetResult();
				switch(result) {
				case ResultType.SUCCESS:
					resultingEffects = success.Invoke(cast, target);
					break;
				case ResultType.FAILURE:
					resultingEffects = failure.Invoke(cast, target);
					break;
				}


				CharacterCastingAnimation castingAnims = caster.GetComponent<CharacterCastingAnimation>();
				if(castingAnims != null) {
					castingAnims.SetupAndDoCastAnimation(cast);
					castingAnims.OneShotSpellRelease((animationInst) => {
						if(resultingEffects != null) {
							foreach(AT.Character.Effect.GenericEffect effect in resultingEffects) {
								effect.ApplyTo(target.FirstOccupant.CharSheet, cast);
							}
						}
					});

					castingAnims.OneShotAnimEnded((animationInst) => {
						cast.CallOnFinished();
					});

				} else {
					if(resultingEffects != null) {
						foreach(AT.Character.Effect.GenericEffect effect in resultingEffects) {
							effect.ApplyTo(target.FirstOccupant.CharSheet, cast);
						}
					}
					cast.CallOnFinished();
				}

			}
		};
	}

	public CastRoutine SpellTouchAttack(SpellAttackEffector hit, SpellAttackEffector crit=null, SpellAttackEffector miss=null) {
		return (AT.Battle.Actor caster, AT.Battle.Action cast, List<AT.ATTile> targets) => {

			foreach(AT.ATTile target in targets) {
				AttackSituation sit = new AttackSituation(caster.CharSheet, target.FirstOccupant.CharSheet, cast);

				List<AT.Character.Effect.GenericEffect> resultingEffects = null;

				ResultType result = sit.GetResult();
				switch(result) {
				case ResultType.HIT:
					resultingEffects = hit.Invoke(cast, target);
					break;
				case ResultType.CRITICAL_HIT:
					if(crit != null) 
						resultingEffects = crit.Invoke(cast, target);
					break;
				case ResultType.MISS:
					if(miss != null) 
						resultingEffects = miss.Invoke(cast, target);
					break;
				case ResultType.CRITICAL_MISS:
					//nothing
					break;
				}


				CharacterCastingAnimation castingAnims = caster.GetComponent<CharacterCastingAnimation>();
				if(castingAnims != null) {
					castingAnims.SetupAndDoCastAnimation(cast);
					castingAnims.OneShotSpellRelease((animationInst) => {
						if(resultingEffects != null) {
							foreach(AT.Character.Effect.GenericEffect effect in resultingEffects) {
								if(target.FirstOccupant != null) 
									effect.ApplyTo(target.FirstOccupant.CharSheet, cast);
							}
						}
					});

					castingAnims.OneShotAnimEnded((animationInst) => {
						cast.CallOnFinished();
					});

				} else {
					if(resultingEffects != null) {
						foreach(AT.Character.Effect.GenericEffect effect in resultingEffects) {
							effect.ApplyTo(target.FirstOccupant.CharSheet, cast);
						}
					}
					cast.CallOnFinished();
				}
			}


		};
	}



	public CastRoutine SpellRangedAttack(SpellAttackEffector hit,
		MissileScript.MissileAnimationName missileName,
		SpellAttackEffector crit=null,
		SpellAttackEffector miss=null) {
		return (AT.Battle.Actor caster, AT.Battle.Action cast, List<AT.ATTile> targets) => {

			GameObject prefab = MissileAnimationPrefabDispenser.instance.GetAnimationPrefabByName(missileName);

			foreach(AT.ATTile target in targets) {
				AttackSituation sit = new AttackSituation(caster.CharSheet, target.FirstOccupant.CharSheet, cast);

				List<AT.Character.Effect.GenericEffect> resultingEffects = null;

				ResultType result = sit.GetResult();
				switch(result) {
				case ResultType.HIT:
					resultingEffects = hit.Invoke(cast, target);
					break;
				case ResultType.CRITICAL_HIT:
					if(crit != null) 
						resultingEffects = crit.Invoke(cast, target);
					break;
				case ResultType.MISS:
					if(miss != null) 
						resultingEffects = miss.Invoke(cast, target);
					break;
				case ResultType.CRITICAL_MISS:
					//nothing
					break;
				}


				CharacterCastingAnimation castingAnims = caster.GetComponent<CharacterCastingAnimation>();
				if(castingAnims != null) {
					castingAnims.SetupAndDoCastAnimation(cast);
					castingAnims.OneShotSpellRelease((animationInst) => {
						
						GameObject copy = Instantiate(prefab);
						copy.transform.position = caster.transform.position;
						MissileScript missile = copy.GetComponent<MissileScript>();

						missile.LaunchAt(target, true);
						//Safe to not worry about unsubbing, thanks to garbage cleanup
						missile.OnConnectedWithTarget += (MissileScript self) => {
							if(resultingEffects != null) {
								foreach(AT.Character.Effect.GenericEffect effect in resultingEffects) {
									if(target.FirstOccupant != null) 
										effect.ApplyTo(target.FirstOccupant.CharSheet, cast);
								}
							}
							//This assumes only one missile.   Needs a "Checkin of sorts, to conditionally check on finished"
							cast.CallOnFinished();
						};




					}); //ToBe ATOCIJL

				} else {

					GameObject copy = Instantiate(prefab);
					copy.transform.position = caster.transform.position;
					MissileScript missile = copy.GetComponent<MissileScript>();
					missile.LaunchAt(target.transform);
					//Safe to not worry about unsubbing, thanks to garbage cleanup
					missile.OnConnectedWithTarget += (MissileScript self) => {
						if(resultingEffects != null) {
							foreach(AT.Character.Effect.GenericEffect effect in resultingEffects) {
								effect.ApplyTo(target.FirstOccupant.CharSheet, cast);
							}
						}
						//This assumes only one missile.   Needs a "Checkin of sorts, to conditionally check on finished"
						cast.CallOnFinished();
					};
				}
			}


		};
	}
}
