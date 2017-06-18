using UnityEngine;
using System.Collections.Generic;
using AT.Character;

namespace AT.Battle {  
	public class Dash : Action {
		public Dash(Actor actor=null) : base(actor) {


		}

		public virtual new bool CanBeUsedBy(Actor a) {
			return true;
		}

		public override void DecorateOption(ActionButtonNode n) {
			n.spriteLabel = IconDispenser.instance.SpriteFromIconName (IconName.DASH);
		}


		public override void Perform() {
			CallOnBegan ();

			AT.Character.Condition.Dashing d = new AT.Character.Condition.Dashing ();
			actor.CharSheet.TakeCondition(d);
//			int max = actor.CharSheet.MovementSpeedGauge.ModifiedMax;
//			actor.CharSheet.MovementSpeedGauge.Modify (new Modifier (max, "dash"));

			CallOnFinished ();
		}
	}

}