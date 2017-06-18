using UnityEngine;
using System.Collections;


namespace AT.Character.Effect {


	public class ConditionEffect : GenericEffect {

		private AT.Character.Condition.ICondition condition;
		private Sheet cachedChar;

		public ConditionEffect(AT.Character.Condition.ICondition condition) {
			this.condition = condition;
		}


		public override void ApplyTo(AT.Character.Sheet c, AT.Battle.Action source = null) {
			
			c.TakeCondition (condition);

			cachedChar = c;
		}

		public override void FailedAgainst(AT.Character.Sheet c) {

		}

		public override string GetTranscript() {
			return "takes the " + condition.Tag() + " condition";
		} 


	}
}
