using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Util.StateMachine;

namespace AT.Battle {
	public class BattleBegin : PhaseState {
		public BattleBegin(Controller controller) : base(controller) {
			this.OnDidEnter += Entered;
		}

		public void Entered (State self, State previous) {

			Debug.Log ("==BATTLE BEGIN PHASE==");


			foreach (Actor p in phaseController.battleManager.AllActors()) {
				p.RollInitiative ();
			}

			phaseController.SwitchState (phaseController.roundBegin);

		}
	}
}