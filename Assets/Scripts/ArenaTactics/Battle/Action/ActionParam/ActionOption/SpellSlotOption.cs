using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT.Character;
using AT.Battle;
using System.Linq;

namespace AT.Battle {
	/// <summary>
	/// Represents the choices an actor can make about what type of attack will be made....
	/// </summary>
	public class SpellSlotOption : ActionOption {
		public SpellSlotOption () : base() {
		}

		public override List<IActionOptionChoice> GetChoicesUnfiltered(Actor actor, Action cast) {
			Cast cst = cast as Cast;

			Dictionary<int, SpellSlotChoice> lvlsToChoices = new Dictionary<int,SpellSlotChoice> ();

			List<SpellLibrary.SpellSlot> allSlots = actor
				.CharSheet.spellSlots
				.Where ((slot) => slot.level >= cst.Spell.level && !slot.used).ToList();



			foreach(SpellLibrary.SpellSlot slot in allSlots) {
				SpellSlotChoice alreadyThere;
				if (lvlsToChoices.TryGetValue (slot.level, out alreadyThere)) {
					alreadyThere.numSiblings += 1;
				} else {
					SpellSlotChoice newOne = new SpellSlotChoice (slot);
					lvlsToChoices.Add (slot.level, newOne);
				}
			}
		


			if (lvlsToChoices.Count == 0) {
				lastReasonForNoChoices = "No slots available for casting";
			}

			return lvlsToChoices.Values.Select((choice)=> choice as IActionOptionChoice).ToList();
		}
	}

	/// <summary>
	///  Attack type choice. Represents the choice that an entity can, or has made about the type of attack that will be made
	/// </summary>
	public class SpellSlotChoice : IActionOptionChoice {


		public SpellLibrary.SpellSlot slot;
		public int numSiblings = 1; //for self


		public SpellSlotChoice(SpellLibrary.SpellSlot slot) {
			
			this.slot = slot;

		}

		public string ValueLabel() {
			return "Lv" + slot.level;

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
			n.cornerText = numSiblings.ToString();
//			Debug.LogWarning (GetType () + " didn't override decorate option.");
		}
	}


}