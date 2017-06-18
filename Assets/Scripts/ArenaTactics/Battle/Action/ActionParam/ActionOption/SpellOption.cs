using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT.Character;
using AT.Battle;

namespace AT.Battle {
	/// <summary>
	/// Represents the choices an actor can make about what type of attack will be made....
	/// </summary>
	public class SpellOption : ActionOption {

		public SpellOption () : base() {
		}

		public override List<IActionOptionChoice> GetChoicesUnfiltered(Actor actor, Action cast) {
			//return attack type choices....
			List<IActionOptionChoice> ret = new List<IActionOptionChoice>();

			foreach (SpellPoolElement elem in actor.CharSheet.spellPool) {
				
				SpellLibrary.Spell spell = SpellLibrary.Instance.SpellByClassAndName(elem.ClassType, elem.SpellName);
				if (!elem.IsPrepared && !spell.isCantrip)
					continue;
				
				if(spell.classType == (cast as Cast).SpellClassChoice.classType) {
					SpellChoice sc = new SpellChoice (spell);

					ret.Add(sc); //if the spell 
				}
			}

			if (ret.Count == 0) {
				lastReasonForNoChoices = "No spells available";
			}

			return ret;
		}
	}




	public class SpellChoice : IActionOptionChoice {


		public SpellLibrary.Spell spell;


		public SpellChoice(SpellLibrary.Spell spell) {

			this.spell = spell;

		}

		public string ValueLabel() {
			return Util.UtilString.EnumToReadable<SpellLibrary.SpellName> (spell.name);
		}


		public void DecorateOption(ActionButtonNode n) {
			n.rightClickDescription = spell.description;
		}
	}

}