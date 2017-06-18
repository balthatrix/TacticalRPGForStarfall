using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AT.Character;
using AT.Character.Situation;
using System.Linq;

namespace AT {
	namespace Battle {  
		public class Cast : Action {
			

			public Cast(Actor actor=null) : base(actor) {
				this.ActionOptions.Add (new SpellClassOption ());
				this.ActionOptions.Add (new SpellOption ());


				this.ActionOptions.Add (new SpellSlotOption ());
				this.ActionOptions.Last ().choiceFilters.Add ((choices, action) => {
					if((action as Cast).Spell.isCantrip) {
						choices.Clear();
						SpellLibrary.SpellSlot cantripSlot = new SpellLibrary.SpellSlot();
						cantripSlot.level = 0;
						cantripSlot.used = false;
						choices.Add(new SpellSlotChoice(cantripSlot));
						return choices;
					}
					return choices;
				});
			}

			public bool IsRangedSpell {
				get { 
					if (Spell == null) {
						return false;
					} else {
						return (Spell.rangeInSquares > 1);
					}
				}
			}

			public int NumTargets {
				get;
				set;
			}

			public SpellClassChoice SpellClassChoice {
				get { 
					return ActionOptions [0].chosenChoice as SpellClassChoice;
				}
			}
			public SpellChoice SpellChoice {
				get {
					return ActionOptions [1].chosenChoice as SpellChoice;
				}
			}
			public SpellSlotChoice SpellSlotChoice {
				get { 
					return ActionOptions [2].chosenChoice as SpellSlotChoice;
				}
			}


			public SpellLibrary.Spell Spell {
				get {
					return SpellChoice.spell;
				}
			}

			public SpellLibrary.SpellSlot Slot {
				get {
					return SpellSlotChoice.slot;
				}
			}



			public override void DecorateOption(ActionButtonNode n) {
				//TODO: check what kind of weapon user has.
				n.spriteLabel = IconDispenser.instance.SpriteFromIconName (IconName.HEART);
			}






			public override void Perform() {

				CallOnBegan ();



				Spell.cast (actor, this, Targets());

				Slot.used = true;


			}

			public List<ATTile> Targets() {
				return actionTargetParameters.Select ((tp) => tp.chosenTile).ToList ();
			}

			public override void LateSetTargetParameters() {
				//This is a generalization that assumes all cantrips scale by class level, and all spells scale by slot lvl.
				if (Spell.isCantrip) {
					Spell.scaler = new SpellLibrary.CharacterLevelScaler (actor.CharSheet);
				} else {
					Spell.scaler = new SpellLibrary.SlotScaler (Spell, SpellSlotChoice.slot);
				}

				if (SpellChoice.spell.alterCastOnSetParams != null) {
					SpellChoice.spell.alterCastOnSetParams.Invoke (this);
				}

				actionTargetParameters = new List<ActionTargetTileParameter>();

				switch (Spell.targetingType) {
				case SpellLibrary.TargetingType.SINGLE_ALLY:
					Debug.Log ("single ally!");
					break;
				case SpellLibrary.TargetingType.MULTIPLE_ALLY:
					Debug.Log ("multiply ally");
					break;
				case SpellLibrary.TargetingType.SINGLE_ENEMY:
					Debug.Log ("an enemy!");
					InitSingleEnemyTargetParam ();
					break;
				case SpellLibrary.TargetingType.MULTIPLE_ENEMY:
					InitMultipleEnemyTargetParams ();
					break;
				}


//				if (choice == null)
//					Debug.LogError ("isssues bub. can't get target tile params without knowing attack type");
//
//				if (choice.IsRanged ()) {
//					targetParam = new ActionTargetTileParameter (this, choice.weapon.MaxRng);
//				} else {
//					//				Debug.Log ("IS NOR RANGED");
//					targetParam = new ActionTargetTileParameter (this);
//				}


			}

			void InitSingleEnemyTargetParam() {
				ActionTargetTileParameter targetParam = new ActionTargetTileParameter(this, Spell.rangeInSquares);
				targetParam.OnListen += (List<ATTile> potentialTargets) => {
					//color red
					MapManager.instance.ColorTiles (potentialTargets, new Color(1f, 0f, 0f, .3f));
				};


				targetParam.OnStopListen += (List<ATTile> potentialTargets) => {
					MapManager.instance.UnColorTiles (potentialTargets);
				};

				targetParam.Prompt = "Choose an Enemy";
				targetParam.targetTileFilters.Add(ActionTargetTileParameter.HaveEnemies);
				actionTargetParameters.Add (targetParam);
			}

			void InitMultipleEnemyTargetParams() {

				for (int i = 0; i < NumTargets; i++) {
					ActionTargetTileParameter targetParam = new ActionTargetTileParameter(this, Spell.rangeInSquares);
					targetParam.OnListen += (List<ATTile> potentialTargets) => {
						//color red
						MapManager.instance.ColorTiles (potentialTargets, new Color(1f, 0f, 0f, .3f));
					};


					targetParam.OnStopListen += (List<ATTile> potentialTargets) => {
						MapManager.instance.UnColorTiles (potentialTargets);
					};

					targetParam.Prompt = "Choose an Enemy";
					targetParam.targetTileFilters.Add(ActionTargetTileParameter.HaveEnemies);
					actionTargetParameters.Add (targetParam);
				}

			}


		}

	}
}