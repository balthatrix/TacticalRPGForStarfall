using UnityEngine;
using System.Collections.Generic;
using AT.Character;

namespace AT {
	namespace Battle {
		public class Dodge : Action {
			public Dodge(Actor actor=null) : base(actor) {
			}

			public virtual new bool CanBeUsedBy(Actor a) {
				return true;
			}
			public override void DecorateOption(ActionButtonNode n) {
				n.spriteLabel = IconDispenser.instance.SpriteFromIconName (IconName.DODGE);
			}

			public override void Perform() {
				CallOnBegan ();

				AT.Character.Condition.Dodging d = new AT.Character.Condition.Dodging ();
				actor.CharSheet.TakeCondition(d);

				CallOnFinished ();
			}
		}

	}
}