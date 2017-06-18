using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT.Character;
using AT.Battle;
using System.Linq;

namespace AT.Battle {
	public class EquipmentSlotTypeOption : ActionOption {
		public EquipmentSlotTypeOption () : base() {
		}

		public override List<IActionOptionChoice> GetChoicesUnfiltered(Actor actor, Action action) {
			List<IActionOptionChoice> ret = new List<IActionOptionChoice> ();
			ret.Add (new EquipmentSlotTypeChoice (EquipmentSlotType.HEAD));
			ret.Add (new EquipmentSlotTypeChoice (EquipmentSlotType.BODY));
			ret.Add (new EquipmentSlotTypeChoice (EquipmentSlotType.MAIN_HAND));
			ret.Add (new EquipmentSlotTypeChoice (EquipmentSlotType.OFF_HAND));

			return ret;
		}
	}

	public class EquipmentSlotTypeChoice : IActionOptionChoice {
		public EquipmentSlotType slotType;
		public EquipmentSlotTypeChoice (EquipmentSlotType slotType) {
			this.slotType = slotType;
		}

		public void DecorateOption(ActionButtonNode n) {
//			Debug.LogWarning (GetType () + " didn't override decorate option.");
		}

		public string ValueLabel() {
			return Util.UtilString.EnumToReadable<EquipmentSlotType> (slotType);
		}
	}
}