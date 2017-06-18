using UnityEngine;
using System.Collections;
using AT.Battle;
using Util.StateMachine;

namespace AT.Battle.AI
{
	public class AiState : State {
		public AT.Battle.AiControlledActor actor;


		public AiState(AiControlledActor c, AiController aic) : base(aic) {
			this.actor = c;
		}


		public virtual Action DecideOnAction() {
			Debug.LogError (GetType ().ToString () + " should override decide on action!");
			return null;
		}
	}

}