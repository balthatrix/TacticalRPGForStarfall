using UnityEngine;
using System.Collections;
using Util;
using AT.Serialization;



namespace AT.Character {

	[System.Serializable]
	public class ArmourProficiencyFeatureWrapper:Wrapper {
		public EquipmentType armourType;
		public ProficiencyLevel lvl;

		public ArmourProficiencyFeatureWrapper() {
			
		}

		public override SerializedObject GetInstance() {
			return new ArmourProficiencyFeature (this);

		}
	}

	public class ArmourProficiencyFeature : GenericFeature, SerializedObject {
		EquipmentType armourType;
		ProficiencyLevel lvl;

		public override bool IsMisc {
			get { return false; }
		}

		public override Wrapper GetSerializableWrapper() {
			ArmourProficiencyFeatureWrapper wrap = new ArmourProficiencyFeatureWrapper ();
			wrap.armourType = armourType;
			wrap.lvl = lvl;

			return wrap;
		}

		public override string Name() {
			string ret = "";
//			if (armourType == EquipmentType.ARMOUR_SHIELD) {
			ret += UtilString.EnumToReadable<EquipmentType> (armourType, 1);
//			} else {

//				ret += UtilString.EnumToReadable<EquipmentType> (armourType, 1);
//			}

			return ret + " Armour";
		}

		public override string Description ()
		{
			string ret = "Grants proficiency in any " + Name ();
			return ret;
		}

		public ArmourProficiencyFeature(EquipmentType armourType, ProficiencyLevel lvl, FeatureBundle parent=null) : base(parent) {
			this.armourType = armourType;	
			this.lvl = lvl;
		}

		public ArmourProficiencyFeature(ArmourProficiencyFeatureWrapper wrap) {
			this.armourType = wrap.armourType;	
			this.lvl = wrap.lvl;
		}

		private ArmourProficiency last;
		public  override void WhenActivatedOn(Sheet c) {
			last = new ArmourProficiency (lvl, armourType);
			c.armourProficiencies.Add (last);
		}

		public  override void WhenDeactivatedOn(Sheet c) {
			c.armourProficiencies.Remove (last);
			last = null;
		}
	}
}
