using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util.StateMachine;

namespace AT.Battle {
	public class InventoryInteractionPhase : PhaseState {
		
		public InventoryInteractionPhase(Controller controller) : base(controller) {
			OnDidEnter += ShowInventoryAndListenForClose;
			OnWillExit += HideInventory;

		}

		void ShowInventoryAndListenForClose (State s, State fromPrevious)
		{
			Debug.Log ("==== VIEWING INVENTORY ====");
			InventoryView.instance.Show ();
			UIManager.instance.OnKeyPressed += TriggerExit;
		}

		private void TriggerExit(KeyCode code) {
			if (code == KeyCode.I || code == KeyCode.Escape) {
				phaseController.SwitchState (phaseController.characterTurnBegin);
			}
		}

		public void HideInventory(State self, State destination) {
			//			Debug.LogError ("removing the excape");
			UIManager.instance.OnKeyPressed -= TriggerExit;
			InventoryView.instance.Hide ();
		}

	}

}