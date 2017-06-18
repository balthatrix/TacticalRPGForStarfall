using UnityEngine;
using System.Collections;
using Util;
using AT.Serialization;


namespace AT.Character {


	[System.Serializable]
	public class WeaponProficiencyFeatureWrapper:Wrapper {
		public EquipmentSubtype weaponType;
		public ProficiencyLevel lvl;

		public WeaponProficiencyFeatureWrapper() {

		}

		public override SerializedObject GetInstance() {
			return new WeaponProficiencyFeature (this);

		}
	}


	public class WeaponProficiencyFeature : GenericFeature, SerializedObject {
		EquipmentSubtype weaponType;
		ProficiencyLevel lvl;
		public override bool IsMisc {
			get { return false; }
		}

		public WeaponProficiencyFeature(EquipmentSubtype weaponType, ProficiencyLevel lvl, FeatureBundle parent=null) : base(parent) {
			this.weaponType = weaponType;	
			this.lvl = lvl;
		}

		public WeaponProficiencyFeature(WeaponProficiencyFeatureWrapper wrap) {
			this.weaponType = wrap.weaponType;
			this.lvl = wrap.lvl;
		}

		public override Wrapper GetSerializableWrapper() {
			WeaponProficiencyFeatureWrapper wrap = new WeaponProficiencyFeatureWrapper ();
			wrap.weaponType = weaponType;
			wrap.lvl = lvl;

			return wrap;
		}

			
		public override string Name() {
			string ret = "";
			//			if (armourType == EquipmentType.ARMOUR_SHIELD) {
			ret += UtilString.EnumToReadable<EquipmentSubtype> (weaponType, 1);
			//			} else {

			//				ret += UtilString.EnumToReadable<EquipmentType> (armourType, 1);
			//			}

			return ret;
		}


		public override string Description ()
		{
			return "Grants " + UtilString.EnumToReadable<ProficiencyLevel> (lvl) + " proficiency bonus to " + Name ();
		}

		private WeaponProficiency last;
		public override void WhenActivatedOn(Sheet c) {
			last = new WeaponProficiency (lvl, weaponType);
			c.weaponProficiencies.Add (last);
		}

		public override void WhenDeactivatedOn(Sheet c) {
			c.weaponProficiencies.Remove (last);
			last = null;
		}
	}




}