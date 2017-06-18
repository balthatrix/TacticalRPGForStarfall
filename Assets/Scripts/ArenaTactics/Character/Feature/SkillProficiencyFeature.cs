using UnityEngine;
using System.Collections;
using AT.Character.Situation;
using Util;
using AT.Serialization;


namespace AT.Character {

	[System.Serializable]
	public class SkillProficiencyFeatureWrapper:Wrapper {
		public SkillType skillType;
		public ProficiencyLevel lvl;

		public SkillProficiencyFeatureWrapper() {

		}

		public override SerializedObject GetInstance() {
			return new SkillProficiencyFeature (this);

		}
	}

	public class SkillProficiencyFeature : GenericFeature {
		
		public override Wrapper GetSerializableWrapper() {
			SkillProficiencyFeatureWrapper wrap = new SkillProficiencyFeatureWrapper ();
			wrap.skillType = skillType;
			wrap.lvl = lvl;

			return wrap;
		}

		public SkillProficiencyFeature(SkillProficiencyFeatureWrapper wrap) {
			this.skillType = wrap.skillType;	
			this.lvl = wrap.lvl;
		}


		public SkillType skillType;
		public ProficiencyLevel lvl;

		public override string Name() {
			return UtilString.EnumToReadable<SkillType>(skillType);
		}

		public override string Description ()
		{
			return "Grants " + UtilString.EnumToReadable<ProficiencyLevel> (lvl, 0, false) + " proficiency bonus to " + Name () + " checks";
		}

		public SkillProficiencyFeature(SkillType skillType, ProficiencyLevel lvl, FeatureBundle parent=null) : base(parent) {
			this.skillType = skillType;	
			this.lvl = lvl;
		}
		public override bool IsMisc {
			get { return false; }
		}

		private SkillProficiency last;
		public override void WhenActivatedOn(Sheet c) {
			last = new SkillProficiency (lvl, skillType);

			c.skillProficiencies.Add (last);
		}

		public override void WhenDeactivatedOn(Sheet c) {
			c.skillProficiencies.Remove (last);
			last = null;
		}
	}

}