using UnityEngine;
using System.Collections;
using AT.Character.Situation;
using Util;
using AT.Serialization;

namespace AT.Character {

	[System.Serializable]
	public class SaveProficiencyFeatureWrapper:Wrapper {
		public AbilityType abilityType;
		public ProficiencyLevel lvl;

		public SaveProficiencyFeatureWrapper() {

		}

		public override SerializedObject GetInstance() {
			return new SaveProficiencyFeature (this);

		}
	}

	public class SaveProficiencyFeature : GenericFeature {
		AbilityType abilityType;
		ProficiencyLevel lvl;

		public override Wrapper GetSerializableWrapper() {
			SaveProficiencyFeatureWrapper wrap = new SaveProficiencyFeatureWrapper ();
			wrap.abilityType = abilityType;
			wrap.lvl = lvl;

			return wrap;

		}

		public override bool IsMisc {
			get { return false; }
		}

		public SaveProficiencyFeature(SaveProficiencyFeatureWrapper wrap) {
			this.abilityType = wrap.abilityType;	
			this.lvl = wrap.lvl;
		}

		public override string Name() {
			return UtilString.EnumToReadable<AbilityType> (abilityType) + " Saves";
		}

		public override string Description() {
			return "Grants proficiency bonus to " + Name () + " saves";
		}

		public SaveProficiencyFeature(AbilityType abilityType, ProficiencyLevel lvl, FeatureBundle parent=null) : base(parent) {
			this.abilityType = abilityType;	
			this.lvl = lvl;
		}

		private SaveProficiency last;
		public override void WhenActivatedOn(Sheet c) {
			last = new SaveProficiency (lvl, abilityType);

			c.saveProficiencies.Add (last);
		}

		public override void WhenDeactivatedOn(Sheet c) {
			c.saveProficiencies.Remove (last);
			last = null;
		}
	}

}