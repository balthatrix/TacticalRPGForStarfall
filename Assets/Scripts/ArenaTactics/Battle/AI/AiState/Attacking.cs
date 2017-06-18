using UnityEngine;
using System.Collections;
using AT.Battle;
using System.Collections.Generic;
using System.Linq;

namespace AT.Battle.AI {

	//TODO:
	//This should reall not care about finding target
	//The finding should be it's own ai state?
	public class Attacking : AiState {

		public  Actor target;
		public Attacking(AiControlledActor actor, AiController aic) : base(actor, aic) {
			
			actor.GetComponent<Vision>().OnVisionOfEnemyLost += (Actor enemy) => {
				if(enemy == target)
					lostEnemySight = true;
			};

			OnDidEnter += (s, fromPrevious) => {
				lostEnemySight = false;
				targetMoved = false;
				targetDied = false;
				target.OnActorKilled += SetEnemyKilled;
				target.OnDidPerform += SetTargetMoved;
			};
			OnWillExit += (s, toDestination) => {
				target.OnActorKilled -= SetEnemyKilled;
				target.OnDidPerform -= SetTargetMoved;
			};
		}

		bool lostEnemySight;
		bool targetMoved;
		bool targetDied;

		public bool LostEnemySight() {
			return lostEnemySight;
		}

		/// <summary>
		/// This might cause an issue down the line, like if a character has a teleport action.
		/// The target will move without moving....
		/// </summary>
		/// <returns><c>true</c>, if moved was targeted, <c>false</c> otherwise.</returns>
		public bool TargetMoved() {
			return targetMoved;
		}
		public bool TargetDied() {
			return targetDied;
		}


		private void SetTargetMoved(Action action) {
			if (action is Wait)
				return;
			targetMoved = true;
		}
		private void SetEnemyKilled(Actor act) {
			targetDied = true;
		}

		public override Action DecideOnAction ()
		{
			if (!actor.CanAttack()) {
//				Debug.Log ("Deciing on waiting");
				return new Wait (actor);
			} else {
//				Debug.Log ("Deciing on attacke");
				//All sorts of potential gotchas here....
				Attack attack = new Attack (actor);
				attack.ActionOptions [0].chosenChoice = new AttackTypeChoice(AttackType.MAINHAND_MELEE, actor.CharSheet.MainHand());
				attack.LateSetTargetParameters ();
				attack.SetTarget (target);



				return attack;
			}
		}

	}

}