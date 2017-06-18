using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util.StateMachine;
namespace AT.Battle {
	public class ActionTargetTileChoice : PhaseState {
		public Action action;
		public ActionTargetTileChoice(Controller controller) : base(controller) {
			OnDidEnter += ActionTargetTileChoice_OnDidEnter;
			OnWillExit += Exiting;
		}

		void ActionTargetTileChoice_OnDidEnter (State s, State fromPrevious)
		{
			Debug.Log ("==== ACTION TARGET TILE ====");
			StartActionUiListen ();

		}

		public void ActionParamsFilled(Action a) {
//			Debug.Log ("filled for " + a.GetType ());
			action.OnParamsFilled -= ActionParamsFilled;

			UIManager.instance.OnKeyPressed -= BackToTurnBeginOnEsc;
			ToCharacterActionPhase ();
		}

		public void ToCharacterActionPhase() {
			phaseController.SwitchState (phaseController.characterAction);
		}

		public void BackToTurnBeginOnEsc(KeyCode code) {
//			Debug.Log ("code: " + code);
			if (code == KeyCode.Escape) {
				Debug.Log ("HERE!");
				phaseController.SwitchState (phaseController.characterTurnBegin);
			}
		}

		public void Exiting(State self, State destination) {
//			Debug.LogError ("removing the excape");
			UIManager.instance.OnKeyPressed -= BackToTurnBeginOnEsc;

			if (destination == phaseController.characterAction) {
				UIManager.instance.ActionBarDisable ();
				UIManager.instance.canvas.SetActive (false);
				phaseController.characterAction.action = action;
			} else if(destination == phaseController.characterTurnBegin){
				action.CancelUiListen ();
				action.OnParamsFilled -= ActionParamsFilled;
				action.OnWillFill -= A_OnWillFill;
				phaseController.characterTurnBegin.actor = action.actor;
			}
		}





		public void StartActionUiListen() {
//			Debug.LogError ("listenting for the exacp!");
			UIManager.instance.OnKeyPressed += BackToTurnBeginOnEsc;
			action.OnWillFill += A_OnWillFill;
			//UIManager.instance.OnKeyPressed += CancelActionListen;
			//cancel ui listen for oneclicks...
			//CancelOneClickActions();
//			Debug.Log("listening for param target for " + action.GetType());
			action.OnParamsFilled += ActionParamsFilled;
			action.FillNextParameter ();
		}

		void A_OnWillFill (Action self, ActionTargetTileParameter ap)
		{
//			Debug.Log ("fillin to the vill: " + ap.Prompt);
			UIManager.instance.waitText.text = ap.Prompt;
		}
	}

}