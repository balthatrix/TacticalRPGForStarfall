using UnityEngine;
using System.Collections.Generic;
using AT.Character;


namespace AT {

	namespace Battle {  
		public class SecondWind : Action {
			public SecondWind(Actor actor=null) : base(actor) {


			}

			public virtual new bool CanBeUsedBy(Actor a) {
				return true;
			}

			public override void DecorateOption(ActionButtonNode n) {
				n.spriteLabel = IconDispenser.instance.SpriteFromIconName (IconName.HEAL);

			}



			public override void Perform() {
				CallOnBegan ();

				AT.Character.Effect.Healing fx = new AT.Character.Effect.Healing (Sheet.DiceRoll(10) + actor.CharSheet.ClassLevelIn(ClassType.FIGHTER));
				fx.ApplyTo (actor.CharSheet, this);
				//			int max = actor.CharSheet.MovementSpeedGauge.ModifiedMax;
				//			actor.CharSheet.MovementSpeedGauge.Modify (new Modifier (max, "dash"));

				CallOnFinished ();
			}
		}

	}
}