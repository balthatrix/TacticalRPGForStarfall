using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AT.Character;
using AT.Character.Situation;

namespace AT {
	namespace Battle {  
		public class Attack : Action {


			/// <summary>
			/// The resulting effect of the attack.  Null if miss.
			/// </summary>
			public AT.Character.Effect.PhysicalDamage resultingEffect;

			/// <summary>
			/// The attack situation surrounding the action.
			/// </summary>
			public AttackSituation attackSituation;



			public Attack(Actor actor=null) : base(actor) {
				this.ActionOptions.Add (new AttackTypeOption ());
			}
			public AttackTypeOption TypeOption {
				get {
					return (ActionOptions [0] as AttackTypeOption);
				}
			}
			public AttackTypeChoice TypeChoice {
				get { 
					AttackTypeOption opt = ActionOptions [0] as AttackTypeOption;
					AttackTypeChoice ret = opt.chosenChoice as AttackTypeChoice;
					return ret;
				}
			}


			GenericWeapon cachedWeaponUsed;
			public void CacheWeaponUsed() {
				switch (TypeChoice.type) {
				case AttackType.MAINHAND_MELEE:
					cachedWeaponUsed =  actor.CharSheet.MainHand ();
					break;
				case AttackType.MAINHAND_RANGED:
					cachedWeaponUsed = actor.CharSheet.MainHand ();
					break;
				case AttackType.OFFHAND_MELEE:
					cachedWeaponUsed =  actor.CharSheet.OffHand () as GenericWeapon;
					break;
				case AttackType.OFFHAND_RANGED:
					cachedWeaponUsed = actor.CharSheet.OffHand () as GenericWeapon;
					break;
				default:
					Debug.LogError ("weapon used type was invalid");
					cachedWeaponUsed =  actor.CharSheet.MainHand ();
					break;
				}
			}

			public GenericWeapon WeaponUsed {
				get { 
					if (cachedWeaponUsed == null)
						CacheWeaponUsed ();
					return cachedWeaponUsed;
				}
			}

			public override string GetTranscript ()
			{
				return string.Format ("{0} Attack with {1}", Util.UtilString.EnumToReadable<AttackType>(TypeChoice.type), WeaponUsed.Name);
			}




			public override bool IsAttack {
				get { return true && !IsReaction; }
			}



			//TODO: bring this to other side: casting!
			public override void LateSetTargetParameters() {
				AttackTypeOption typeOpt = ActionOptions [0] as AttackTypeOption;
				AttackTypeChoice choice = typeOpt.chosenChoice as AttackTypeChoice;
				ActionTargetTileParameter targetParam;

				if (choice == null)
					Debug.LogError ("isssues bub. can't get target tile params without knowing attack type");

				if (choice.IsRanged ()) {
					targetParam = new ActionTargetTileParameter (this, choice.weapon.MaxRng);
				} else {
	//				Debug.Log ("IS NOR RANGED");
					targetParam = new ActionTargetTileParameter (this);
				}


				targetParam.OnListen += (List<ATTile> potentialTargets) => {
					//color red
					MapManager.instance.ColorTiles (potentialTargets, new Color(1f, 0f, 0f, .3f));
				};


				targetParam.OnStopListen += (List<ATTile> potentialTargets) => {
					
					MapManager.instance.UnColorTiles (potentialTargets);

				};

				targetParam.Prompt = "Choose an Enemy";

				cachedTargetActor = null;
				targetParam.OnTargetTileChosen += CacheTargetActor;
				targetParam.targetTileFilters.Add(ActionTargetTileParameter.HaveEnemies);
				actionTargetParameters = new List<ActionTargetTileParameter> { targetParam };
			}

			public override void DecorateOption(ActionButtonNode n) {
				//TODO: check what kind of weapon user has.
				n.spriteLabel = IconDispenser.instance.SpriteFromIconName (IconName.MELEE_ATTACK);
			}

			public Actor cachedTargetActor;
			private void CacheTargetActor(ActionTargetTileParameter param) {
				param.OnTargetTileChosen -= CacheTargetActor;
				cachedTargetActor = param.chosenTile.FirstOccupant.ActorComponent;
			}

			public void SetTarget(Actor a) {
				if (actionTargetParameters.Count == 0)
					Debug.LogError ("cant set target unless parameter type has been set.  did you set the attack type option?");
				cachedTargetActor = a;
				actionTargetParameters [0].chosenTile = a.TileMovement.occupying;
			}
	//
	//		public virtual new bool CanBeUsedBy(Actor a) {
	//			//AttackTypeOption opt = ActionOptions [0] as AttackTypeOption;
	//			foreach(ActionOption opt in ActionOptions) {
	//				
	//				foreach(AttackTypeChoice c in opt.GetChoices(a.CharSheet, this)) {
	//					opt.chosenChoice = c;
	//					foreach (ActionTargetTile potentialParam in GetActionTargetTileParameters()) {
	//						if (potentialParam.CanBeFilled ())
	//							return true;
	//					}
	//				}
	//			}
	//
	//			return false;
	////			
	////			List<Actor> list;
	////				list = a.AllMainHandAttackOptions ();
	////
	////			if (list.Count > 0)
	////				return true;
	////			return false;
	//		}





			public Actor TargetActor() {
				if (cachedTargetActor != null) {
					return cachedTargetActor;
				}
				ActionTargetTileParameter param =  actionTargetParameters [0];
				//you can assume that the tile has an occupant....
				return param.chosenTile.FirstOccupant.ActorComponent;
			}

			public bool IsOffhand {
				get { 
					return TypeChoice.IsOffhand();
				}
			}

			public void ForceFlagOffhand() {
				TypeOption.chosenChoice = new AttackTypeChoice (AttackType.OFFHAND_MELEE, null);
//				TypeOption.chosenChoice = new AttackTypeChoice (ActionOptions [0].chosenChoice as AttackTypeChoice);
			}

			public override void Perform() {

				CallOnBegan ();

				//TargetEnemyTile param = (TargetEnemyTile) parameters [0];
				//IsOffhand opt = (IsOffhand) ActionOptions [0].OptVal;

				AT.Character.Effect.PhysicalDamage hit;
				AT.Character.Effect.PhysicalDamage crit;

				Actor target = TargetActor ();

				attackSituation  = new AttackSituation (actor.CharSheet, target.CharSheet, this);

				ResultType result = attackSituation.GetResult ();
				resultingEffect = null;


				if (IsOffhand) {
					hit = actor.CharSheet.OffhandAttackDamageEffect (attackSituation);
					crit = actor.CharSheet.OffhandAttackCritDamageEffect (attackSituation);
					//IsOffhand = true;
				} else {
					hit = actor.CharSheet.MainHandAttackDamageEffect (attackSituation);
					crit = actor.CharSheet.MainHandAttackCritDamageEffect (attackSituation);
				}

				switch (result) {
				case ResultType.CRITICAL_HIT:
					resultingEffect = crit;
					break;
				case ResultType.HIT:
					resultingEffect =  hit;
					break;
				case ResultType.MISS:
					//	actor.OnMissedAttack ();
					break;
				case ResultType.CRITICAL_MISS:
					//	actor.OnMissedAttack ();
					break;
				}
	//			Debug.Log("Attack was made: " + attackSituation.VerboseToString());



				if (actor.GetComponent<CharacterAttacker> () != null) {
					actor.GetComponent<CharacterAttacker> ().OnAttackAnimationEnded += WaitForAnimationFinish;
				} else {
					if (resultingEffect != null) {
						resultingEffect.ApplyTo (target.CharSheet, this);
					} else {
						//target.InstantiateMissText ();
					}
					CallOnFinished ();
				}

			
			}

			void WaitForAnimationFinish(CharacterAttacker anim) {

				anim.OnAttackAnimationEnded -= WaitForAnimationFinish; 
				CallOnFinished ();
			}
		}

	}
}