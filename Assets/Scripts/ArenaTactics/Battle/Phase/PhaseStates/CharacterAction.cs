using UnityEngine;
using System.Collections;
using Util.StateMachine;

namespace AT.Battle {
	public class CharacterAction : PhaseState {
		public CharacterAction(Controller controller) : base(controller) {
			OnDidEnter += Entered;
		}

		public Action action;
//		bool transitioning = false;


		public void Entered (State self, State previous) {
			Debug.Log ("==ACTOR ACTION "+action.actor.CharSheet.Name + " PHASE with " + action.GetType().ToString() + "==");

//			transitioning = false;

			action.OnFinished += Completed;
			action.Perform ();
		}

		public void Completed(Action a){

			BattleManager.instance.EmitActorPerformedEvent (action.actor, action);
			action.OnFinished -= Completed;

			if (action.actor != null && !action.actor.Dying && action.actor.CanStillAct ()) {
				action.actor.StartCoroutine (DelayToCharacterTurnBeginPhase ());
			} else {

				ToCharacterTurnEndPhase ();
			}
		}

		IEnumerator DelayToCharacterTurnBeginPhase() {
//			transitioning = true;
			yield return new WaitForEndOfFrame ();
			ToCharacterTurnBeginPhase ();
		}



		public void ToCharacterTurnBeginPhase() {
			phaseController.characterTurnBegin.actor = action.actor;
			phaseController.SwitchState (phaseController.characterTurnBegin);
		}

		public  void ToCharacterTurnEndPhase() {
			phaseController.characterTurnEnd.actor = action.actor;
			phaseController.SwitchState (phaseController.characterTurnEnd);
		}
	}

}