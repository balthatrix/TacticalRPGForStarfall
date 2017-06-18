using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Util.StateMachine;

namespace AT.Battle {
	public class CharacterTurnBegin : PhaseState {
		
		public CharacterTurnBegin(PhaseController controller) : base(controller) {
			OnDidEnter += Entered;
			OnWillExit += CharacterTurnBegin_OnWillExit;
		}


		private Action oneClickMove;
		private Action oneClickAttack;
		private Action spaceBarWait;
		private Action characterChoice;

		public Actor actor;


		public  void Entered (State self, State previous) {
			Debug.Log ("==ACTOR TURN BEGIN PHASE: " + actor.CharSheet.Name + "==");
			ReallyStart ();
//			if (previous == phaseController.roundBegin) {
//				ReallyStart ();
//			}else {
//				phaseController.battleManager.StartCoroutine (DelayNextAction ());
//			}

		}

		public IEnumerator DelayNextAction() {
			yield return null;
			ReallyStart ();
		}
		void ReallyStart() {
			if (!actor.CanStillAct ()) {

				UIManager.instance.ActionBarDisable();
				ToCharacterTurnEndPhase ();
			} else {

				actor.ActorTurnBegan ();

				//TODO: make this current=ly following =
				if (actor.IsOnPlayerSide) {
					CameraAction ();
				} else {
					if (actor.IsSeenByAPlayer) {
						CameraAction ();
					} else {
						SetupCpuControlledTurn ();
					}
				}
			}
		}

		public void CameraAction() {
			if (UIManager.instance.cameraController.freeCameraMode && !(actor is PlayerControlledActor)) {
				CameraIsThereNow (UIManager.instance.cameraController);
			} else {
				UIManager.instance.cameraController.GoLockMode ();
				UIManager.instance.cameraController.MoveTo (actor.transform.position);
				UIManager.instance.cameraController.OnMoveComplete += CameraIsThereNow;
			}
		}


		public void CameraIsThereNow(CameraController inst) {

			UIManager.instance.cameraController.OnMoveComplete -= CameraIsThereNow;
			if (actor is PlayerControlledActor) {
				UIManager.instance.ActionBarEnable ();
				SetupPlayerControlledTurn ();
			} else {
				SetupCpuControlledTurn ();
			}
		}

		public void SetupCpuControlledTurn() {
			

			AiControlledActor aiActor = (AiControlledActor)actor;
			aiActor.ChooseAction (this);
		}

		public void SetupPlayerControlledTurn() {
			SetupOneClickActions ();
			SetupActionBar ();

			actor.OnReadyToFillParams += ToTargetTileChoice;

			UIManager.instance.canvas.SetActive (true);

			UIManager.instance.OnKeyPressed += OpenInventoryOnI;

			characterChoice = null;
		}

		public void SetupActionBar() {
			UIManager.instance.actionBar.gameObject.SetActive (true);
			UIManager.instance.actionBar.InitiateActionTree (actor.ProduceRootActionButtonNode ().children);
			UIManager.instance.actionBar.OnPathResolved += actor.ActionFromResolutionPath;

			actor.OnWillPerform += StopListenForPathResolve;
		}

		void StopListenForPathResolve(Action a){
			UIManager.instance.actionBar.OnPathResolved -= actor.ActionFromResolutionPath;
			actor.OnWillPerform -= StopListenForPathResolve;
		}


		void OpenInventoryOnI(KeyCode kc) {
			if (kc == KeyCode.I) {
				ToInventoryViewPhase ();
			}
		}
		void ToInventoryViewPhase() {
			phaseController.SwitchState(phaseController.inventoryInteraction);
		}



		public void SetupOneClickActions() {
			//SET UP ONE CLICKS

			string moveAndAttacks = "";
			oneClickMove = new Move (actor);
			moveAndAttacks += "Moves: " + actor.MovesLeft ().ToString ();
			if (actor.MovesLeft () > 0) {
				oneClickMove.OnParamsFilled += OneClickActionParamsFilled;
				oneClickMove.FillNextParameter ();
			}
	
			oneClickAttack = new Attack (actor);
			if (actor.CanAttack()) {
				oneClickAttack.ActionOptions [0].chosenChoice = oneClickAttack.ActionOptions [0].GetChoices(actor, oneClickAttack)[0];
				oneClickAttack.OnParamsFilled += OneClickActionParamsFilled;
				oneClickAttack.LateSetTargetParameters ();
				oneClickAttack.FillNextParameter ();
			}

			UIManager.instance.upperRightText.text = moveAndAttacks + "\n\n" + actor.ActionsLeftDescription();
	//		////////////////////
			spaceBarWait = new Wait (actor, true);
			spaceBarWait.OnParamsFilled += OneClickActionParamsFilled;
			spaceBarWait.FillNextParameter ();
			UIManager.instance.waitText.text = "--Press space to wait--";
			UIManager.instance.waitText.gameObject.SetActive (true);

		}

		public void CancelOneClickActions() {
			if (oneClickMove != null) {
				oneClickMove.CancelUiListen ();
				oneClickMove.OnParamsFilled -= OneClickActionParamsFilled;
			}

			if (oneClickAttack != null) {
				oneClickAttack.CancelUiListen ();
				oneClickAttack.OnParamsFilled -= OneClickActionParamsFilled;
			}

			if (spaceBarWait != null) {

				spaceBarWait.CancelUiListen ();
				spaceBarWait.OnParamsFilled -= OneClickActionParamsFilled;
			}
		}



		public void OneClickActionParamsFilled(Action a) {
//			Debug.Log ("filled for " + a.GetType ());
			a.OnParamsFilled -= OneClickActionParamsFilled;

			//cancel any listening one click actions
			
			characterChoice = a;


			ToCharacterActionPhase ();
		}


		public void CpuActionParamsFilled(Action a) {
			characterChoice = a;

			ToCharacterActionPhase ();
		}

		void CharacterTurnBegin_OnWillExit (State s, State toDestination)
		{
			if (toDestination != phaseController.actionTargetTileChoice) {
				UIManager.instance.waitText.gameObject.SetActive (false);
				UIManager.instance.actionBar.gameObject.SetActive (false);
				UIManager.instance.canvas.SetActive (false);
				ResetText ();
			}

			UIManager.instance.Tooltip.Hide ();

			CancelOneClickActions();
			UIManager.instance.ActionBarDisable();
			actor.OnReadyToFillParams -= ToTargetTileChoice;

			UIManager.instance.OnKeyPressed -= OpenInventoryOnI;

			oneClickMove = null;
			oneClickAttack = null;
			spaceBarWait = null;
			characterChoice = null;


//			UIManager.instance.waitText.gameObject.SetActive (false);
//			UIManager.instance.canvas.SetActive (false);

		}

		public void ToTargetTileChoice(Actor entity, Action a) {
			Debug.Log("going to listening for param target for " + a.GetType());
			phaseController.actionTargetTileChoice.action = a;
			phaseController.SwitchState (phaseController.actionTargetTileChoice);
		}

		public  void ToCharacterTurnEndPhase() {
			phaseController.characterTurnEnd.actor = actor;
			phaseController.SwitchState (phaseController.characterTurnEnd);
		}

		public  void ToCharacterActionPhase() {
			phaseController.characterAction.action = characterChoice;
			phaseController.SwitchState (phaseController.characterAction);
		}

		private void ResetText() {
			UIManager.instance.upperRightText.text = "";
			UIManager.instance.upperLeftText.text = "";
		}

	}

}