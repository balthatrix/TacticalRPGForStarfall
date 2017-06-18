using UnityEngine;
using System.Collections;


namespace AT.Character.Effect {


	public class Damage : GaugeEffect {


		private DamageType type;

		public Damage(DamageType type) : base(type.ToString()) {
			this.type = type;

		}

		public DamageType Type {
			get {return this.type;}
		}

		public override void ApplyTo(AT.Character.Sheet c, AT.Battle.Action source = null) {
			Debug.Log("taking damage, with details: " + gauge.ToString());
			c.TakeDamageEffect(this, source);
		}

		public override void FailedAgainst(AT.Character.Sheet c) {

		}

		public override string GetTranscript() {
			return "takes " + Amount + " " + Util.UtilString.EnumToReadable<DamageType> (Type) + " damage";
		}

	}
}
