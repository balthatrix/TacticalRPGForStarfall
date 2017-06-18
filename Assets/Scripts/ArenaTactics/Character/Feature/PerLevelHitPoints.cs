using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Util;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using AT.Serialization;


namespace AT.Character {
		
	[Serializable]
	public  class PerLevelHitPointsWrapper : Wrapper {
		public int amount;
		public  ClassType type;

		public PerLevelHitPointsWrapper(int amount, ClassType type) {
			this.amount = amount;
			this.type = type;
		}

		public override SerializedObject GetInstance ()
		{
			return new PerLevelHitPoints(amount, type);
		}
	}


	public class PerLevelHitPoints : GenericFeature {
		public override Wrapper GetSerializableWrapper() {
			return new PerLevelHitPointsWrapper(amount, classType);
		}
		public override bool IsMisc {
			get { return false; }
		}

		public  int amount;
		ClassType classType;

		private string featureName;

		public PerLevelHitPoints(int amount, ClassType type, FeatureBundle parent=null) : base(parent) {
			this.classType = type;


			this.amount = amount;

		}

		public PerLevelHitPoints() : base() {
		}

		public override string Name() {
			return amount + "HP";

//			return UtilString.EnumToReadable<ClassType>(classType) + "HP";
		}

		public override string Description ()
		{
			return "Grants first level hitpoints (" + Name () + ": " + amount;
		}


		private BaseModifier lastMod;
		public override void WhenActivatedOn(Sheet c) {
			lastMod = new BaseModifier (amount, "Class Level");
			c.HitPointsGauge.Modify (lastMod);
		}

		public override void WhenDeactivatedOn(Sheet c) {

			c.HitPointsGauge.UnModify (lastMod);

		}
	}

}