using UnityEngine;
using System.Collections;
using AT.Character.Situation;
using Util;
using AT.Serialization;


namespace AT.Character {

	/// <summary>
	/// Spell prepared feature.  Makes a spell in a character's pool prepared
	/// </summary>
	public class  SpellPreparedFeature : NonSerializedFeature {

		SpellPoolElement spellElement;

		public override string Name() {
			return UtilString.EnumToReadable<SpellLibrary.SpellName>(spellElement.SpellName);
		}

		public override string Description ()
		{
			return "You prepare the " + Name() + " spell";
		}

		SpellPoolElement cached = null;
		public SpellLibrary.SpellName spellName;
		public ClassType classType;
		public SpellPreparedFeature(SpellPoolElement spellElement, FeatureBundle parent=null) : base() {
			this.spellElement = spellElement;
			ups.Add ((character) => {
				spellElement.IsPrepared = true;
			});
			downs.Add ((character) => {
				spellElement.IsPrepared = false;
			});
		}

		public override bool IsMisc {
			get { return false; }
		}

	}



}