using UnityEngine;
using System.Collections;
using AT.Battle;
using AT.Character;

namespace AT.Character.Condition {


	public class Necrosis : DurationCondition {
		
		public Necrosis(int duration=1, TickType tickType=TickType.TURN_BEGIN, Sheet tickHost=null) : base(duration, tickType, tickHost) {

		}

		public override void ApplyTo(Sheet c) {
			base.ApplyTo (c);
			Debug.Log ("Applying necrosis sir! " + c.Name);
			c.OnWillBeHealed += CancelHealing;
		}


		public override void RemoveFrom(Sheet c) {
			base.RemoveFrom (c);
			Debug.LogError ("Removing necrosis sir! " + c.Name);
			c.OnWillBeHealed -= CancelHealing;

		}

		public void CancelHealing(AT.Character.Effect.Healing effect) {
			Debug.LogError ("Canceling healing!");
			effect.Nullify (Tag ());
		}


		public override string Tag() {
			return "Necrosis";
		}


		public override Sprite SpriteIcon() {
			return IconDispenser.instance.SpriteFromIconName(IconName.ARROW_DOWN);
		}
	}
}