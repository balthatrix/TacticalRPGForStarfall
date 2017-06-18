using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Util.StateMachine;
namespace AT.Battle {
	public class CharacterTurnEnd : PhaseState {
		
		public CharacterTurnEnd(Controller controller) : base(controller) {
			OnDidEnter += Entered;
		}

		public Actor actor;
		public void Entered (State self, State previous) {
			Debug.Log ("==ACTOR TURN END PHASE==");


			phaseController.roundBegin.actorsLeftThisRound.Remove (actor);

			actor.TurnInProgress = false;

			if (phaseController.roundBegin.actorsLeftThisRound.Count > 0) {
				ToCharacterTurnBeginPhase ();
			} else {
				BattleManager.instance.StartCoroutine (DelayRoundBegin ());
			}
		}

		public IEnumerator DelayRoundBegin() {
			yield return new WaitForEndOfFrame ();
			phaseController.battleManager.RoundEnded ();
			ToRoundBeginPhase ();
		}


		public void ToRoundBeginPhase() {
			phaseController.SwitchState (phaseController.roundBegin);
		}


		public void ToCharacterTurnBeginPhase() {
			Actor p = phaseController.roundBegin.AllSortedActors()[0];
			phaseController.characterTurnBegin.actor = p;
			actor.CharSheet.TurnEnded (actor);
			phaseController.SwitchState (phaseController.characterTurnBegin);
		}

	}

}