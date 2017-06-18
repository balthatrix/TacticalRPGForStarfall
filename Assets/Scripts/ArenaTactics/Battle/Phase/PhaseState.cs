using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Util.StateMachine;

namespace AT.Battle {
	public class PhaseState : State {
		public PhaseController phaseController;

		public PhaseState(Controller controller) : base(controller) {
			this.phaseController = controller as PhaseController;
		}

	}
}