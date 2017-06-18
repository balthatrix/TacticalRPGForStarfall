using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using AT.Serialization;

namespace AT.Character {

	[System.Serializable]
	public  class GaugeModWrapper : Wrapper {
		public int amount;
		public string sourceName;
		public string gaugeName;
		public bool isBase;

		public GaugeModWrapper(string gaugeName, int amount, string sourceName, bool isBase) {
			this.amount = amount;
			this.sourceName = sourceName;
			this.gaugeName = gaugeName;
			this.isBase = isBase;
		}

		public override SerializedObject GetInstance ()
		{
			return new GaugeMod(gaugeName, amount, sourceName, isBase);
		}
	}

	public class GaugeMod : GenericFeature {
		public int amount;
		public string gaugeName;
		public string sourceName;
		public bool isBase;


		public override Wrapper GetSerializableWrapper() {
			return new GaugeModWrapper (gaugeName, amount, sourceName, isBase);
		}

		public GaugeMod(string gaugeName, int amount, string sourceName, bool consideredBase=false, FeatureBundle parent=null) : base(parent) {
			this.isBase = consideredBase;
			this.amount = amount;
			this.gaugeName = gaugeName;
			this.sourceName = sourceName;
		}


		public override string Name() {
			string sign = "";
			if (amount > 0) {
				sign = "+";
			}

			string ret = "";
//				if (isBase) {
//					ret += "Base ";
//				}
			ret += sign+amount + " " + gaugeName;
			return ret;
		}

		public override string Description ()
		{
			string sign = "";
			if (amount > 0) {
				sign = "+";
			}

			return sign.ToString() + amount + " to " + Util.UtilString.Capitalize(gaugeName) + " ("+sourceName+")";
		}
		public override bool IsMisc {
			get { return false; }
		}

		Modifier last;

		public override void WhenActivatedOn(Sheet c) {
			last = new Modifier (amount, sourceName, isBase);

			c.GaugeByName(gaugeName).Modify(last);
		}

		public override void WhenDeactivatedOn(Sheet c) {
			c.GaugeByName (gaugeName).UnModify (last);
		}
	}

}
