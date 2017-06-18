using UnityEngine;
using System.Collections;
using AT.Battle;
using AT.Character;

namespace AT.Character.Condition {
	

	public class Dashing : DurationCondition {
		Modifier lastMod;

		public Dashing() : base() {
			
		}

		public override void ApplyTo(Sheet c) {
			base.ApplyTo (c);
			int max = c.MovementSpeedGauge.ModifiedMax;
//			Debug.Log ("Ma: " + c.MovementSpeedGauge.ToString());
			lastMod = new Modifier (max, "Dashing");
			c.MovementSpeedGauge.Modify (lastMod);
//			Debug.Log ("c thing: " + c.Name + ", " + c.MovementSpeed);
		}


		public override void RemoveFrom(Sheet c) {
			base.RemoveFrom (c);
			c.MovementSpeedGauge.UnModify (lastMod);
		}


		public override string Tag() {
			return "Dashing";
		}


		public override Sprite SpriteIcon() {
			return IconDispenser.instance.SpriteFromIconName(IconName.DASH);
		}
	}
}