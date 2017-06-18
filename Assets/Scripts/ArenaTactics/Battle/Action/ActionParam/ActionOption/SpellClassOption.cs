using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT.Character;
using AT.Battle;

namespace AT.Battle {
	/// <summary>
	/// Represents the choices an actor can make about what type of attack will be made....
	/// </summary>
	public class SpellClassOption : ActionOption {

		public SpellClassOption () : base() {
		}

		public override List<IActionOptionChoice> GetChoicesUnfiltered(Actor actor, Action cast) {
			//return attack type choices....
			List<IActionOptionChoice> ret = new List<IActionOptionChoice>();

			ret.Add (new SpellClassChoice(ClassType.WIZARD));
			ret.Add (new SpellClassChoice(ClassType.CLERIC));

			return ret;
		}
	}




	public class SpellClassChoice : IActionOptionChoice {


		public ClassType classType;

		public SpellClassChoice(ClassType classType) {
			this.classType = classType;
		}

		public string ValueLabel() {
			return Util.UtilString.EnumToReadable<ClassType> (classType);
		}


		public void DecorateOption(ActionButtonNode n) {
			//			Debug.LogError ("HERE!");
			//			n.spriteLabel = IconDispenser.instance.SpriteFromIconName (weapon.IconType);
			//			n.label = weapon.Name;
			//			if (IsRanged ()) {
			//				if (weapon.IsThrown()) {
			//					n.label += " Throw"; 
			//				} else {
			//					n.label += " Shot";
			//				}
			//			} else {
			//				n.label += " Swing";
			//			}
			//
			//
			//			if (IsRanged ()) {
			//				n.cornerSpriteLabel = IconDispenser.instance.SpriteFromIconName (IconName.RANGED_ATTACK);
			//			} else {
			//				n.cornerSpriteLabel = IconDispenser.instance.SpriteFromIconName (IconName.MELEE_ATTACK);
			//			}
			//

			Debug.LogWarning ("hsidfhih spell!!");
			//			Debug.LogWarning (GetType () + " didn't override decorate option.");
		}
	}

}