using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AT.Character {
	/// <summary>
	/// NonSerializedFeature.  Special kind of feature only applied one time at character creation.
	/// usually used for things that handle their own serialization on the char sheet.
	/// the weapons/items will all be serialized on their own.
	/// </summary>
	public class NonSerializedFeature : GenericFeature {
		public delegate void ApplicationAction(Sheet c);


		public NonSerializedFeature(List<ApplicationAction> ups=null, List<ApplicationAction> downs=null) : base() {
			if (ups == null)
				this.ups = new List<ApplicationAction> ();
			else
				this.ups = ups;

			if(downs == null)
				this.downs = new List<ApplicationAction> ();
			else
				this.downs = downs;
		}

		public List<ApplicationAction> ups;
		public List<ApplicationAction> downs;

		public override bool IsMisc {
			get { return false; }
		}

		public override void WhenActivatedOn(Sheet c) {
			foreach (ApplicationAction up in ups) {
				up (c);
			}
		}

		public override void WhenDeactivatedOn(Sheet c) {
			foreach (ApplicationAction down in downs) {
				down (c);
			}
		}
	}

}