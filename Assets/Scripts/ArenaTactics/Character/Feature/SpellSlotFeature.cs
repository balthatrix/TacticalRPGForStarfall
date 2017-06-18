using UnityEngine;
using System.Collections;
using AT.Character.Situation;
using Util;
using AT.Serialization;


namespace AT.Character {

	/// <summary>
	/// Spell known feature.  Adds a spell to the character's spell pool....
	/// </summary>
	public class  SpellSlotFeature : NonSerializedFeature {



		public override string Name() {
			return "Lvl " + level + " spell slot";
		}

		public override string Description ()
		{
			return "You can use this spell slot to cast lvl "+ level + "+ spells";
		}

		SpellLibrary.SpellSlot cached = null;
		public int level;

		public SpellSlotFeature(int level, FeatureBundle parent=null) : base() {
			this.level = level;
			ups.Add ((character) => {
				SpellLibrary.SpellSlot slot = new SpellLibrary.SpellSlot();
				slot.level = level;
				slot.used = false;
				character.spellSlots.Add(slot);
			});
			downs.Add ((character) => {
				character.spellSlots.Remove(cached);
			});
		}

		public override bool IsMisc {
			get { return false; }
		}

	}

}