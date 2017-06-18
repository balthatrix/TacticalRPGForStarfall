using UnityEngine;
using System.Collections;
using AT.Battle;
using AT.Character;

namespace AT.Character.Condition {


	public class Dodging : DurationCondition {
		Modifier lastMod;
								//false means  tick on turn begin, not end...
		public Dodging(int duration=1, TickType tickType=TickType.TURN_BEGIN, Sheet tickHost=null) : base(duration, tickType, tickHost) {

		}

		public override void ApplyTo(Sheet c) {
			base.ApplyTo (c);
			c.OnAboutToBeAttacked += FlagDisadvantage;

		}

		public void FlagDisadvantage(AT.Character.Situation.AttackSituation sit) {
			sit.FlagDisadvantage ();
		}

		public override Sprite SpriteIcon() {
			return IconDispenser.instance.SpriteFromIconName(IconName.DODGE);
		}

		public override void RemoveFrom(Sheet c) {
			base.RemoveFrom (c);
			c.OnAboutToBeAttacked -= FlagDisadvantage;
		}

		public override string Tag() {
			return "Dodging";
		}
	}
}