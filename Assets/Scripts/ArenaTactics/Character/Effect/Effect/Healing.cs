using UnityEngine;
using System.Collections;


namespace AT.Character.Effect {

	public class Healing : GaugeEffect {



		public Healing(int baseAmount=0) : base("Healing") {
			
			this.gauge.ChangeCurrentAndMax (baseAmount);
		}

		public override void ApplyTo(Sheet c, AT.Battle.Action source = null) {
			c.TakeHealingEffect(this, source);
		}

		public override void FailedAgainst(Sheet c) {

		}

		public override string GetTranscript() {
			if (!Nullified)
				return "Healed for " + Amount;
			else
				return "Healing nullified (" + NullifiedReason + ")";
		}

	}
}
