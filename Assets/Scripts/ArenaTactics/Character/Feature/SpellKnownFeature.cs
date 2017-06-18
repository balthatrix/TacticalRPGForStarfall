using UnityEngine;
using System.Collections;
using AT.Character.Situation;
using Util;
using AT.Serialization;


namespace AT.Character {

	/// <summary>
	/// Spell known feature.  Adds a spell to the character's spell pool....
	/// </summary>
	public class  SpellKnownFeature : NonSerializedFeature {



		public override string Name() {
			return UtilString.EnumToReadable<SpellLibrary.SpellName>(spellName);
		}

		public override string Description ()
		{
			return "You can prepare the " + Name() + " spell";
		}

		SpellPoolElement cached = null;
		public SpellLibrary.SpellName spellName;
		public ClassType classType;
		public SpellKnownFeature(SpellLibrary.SpellName spellName, ClassType classType, FeatureBundle parent=null) : base() {
			this.spellName = spellName;
			this.classType = classType;
			ups.Add ((character) => {
				SpellPoolElement elem = cached = new SpellPoolElement();
				elem.ClassType  = classType;
				elem.SpellName  = spellName;
				elem.IsPrepared = false;
				character.spellPool.Add(elem);
			});
			downs.Add ((character) => {
				character.spellPool.Remove(cached);
			});
		}

		public override bool IsMisc {
			get { return false; }
		}

	}

}