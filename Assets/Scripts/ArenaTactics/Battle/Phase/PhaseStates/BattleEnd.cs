using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Util.StateMachine;

namespace AT.Battle {
	public class BattleEnd : PhaseState {
		
		public BattleEnd(Controller controller) : base(controller) {
			this.OnDidEnter += Entered;
		}

		public void Entered (State self, State previous) {

			Debug.Log ("==BATTLE END PHASE==");

			Debug.Log ("winning side: " + BattleManager.instance.WinningSide);
			UIManager.instance.roundPanel.gameObject.SetActive (true);
			if (BattleManager.instance.WinningSide == BattleManager.Side.ENEMY) {
				UIManager.instance.roundText.text = "Mission Failed";
			} else {
				UIManager.instance.roundText.text = "Mission Success";
			}


			phaseController.battleManager.StartCoroutine (DelayBackToMainMenu ());
		}

		IEnumerator DelayBackToMainMenu() {
			yield return new WaitForSeconds (4f);
			GameManager.persistentInstance.LoadSceneWithName (GameManager.SceneName.TITLE_SCREEN);
		}
	}
}