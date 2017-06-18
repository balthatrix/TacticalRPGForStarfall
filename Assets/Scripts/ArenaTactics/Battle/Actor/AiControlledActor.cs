using UnityEngine;
using System.Collections;


namespace AT.Battle {
	
	public class AiControlledActor : Actor {


		AI.AiController controller;
		public string currentStateName;
		//choose action???  Consults the current ai state?
		// Use this for initialization
		public override void Start () {
			base.Start ();



			BattleManager.instance.ReportInAs (BattleManager.Side.ENEMY, this);
			controller = new AI.MeleeAttacker (this);
			controller.OnStateChanged += (fromS, toS) => {
				currentStateName = toS.GetType().ToString();
			};
			currentStateName = controller.CurrentState.GetType().ToString();
			StartCoroutine (UpdateAi ());
		}

		public void ChooseAction(CharacterTurnBegin turnPhase) {
			StartCoroutine (ActuallyChooseAction (turnPhase));
		}

		public IEnumerator ActuallyChooseAction(CharacterTurnBegin turnPhase) {
			yield return new WaitForSeconds (.5f);
			turnPhase.CpuActionParamsFilled (controller.GetBestAction ());
		}

		private IEnumerator UpdateAi() {
			while (true) {
				this.controller.UpdateCurrentState ();
				yield return new WaitForSeconds (.1f);	
			}
		}

	}

}