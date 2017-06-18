using UnityEngine;
using System.Collections;
using AT.Character.Situation;
using AT.Serialization;
namespace AT.Character {

		[System.Serializable]
		public class HitDiceFeatureWrapper : Wrapper {
			public int sides;

			public HitDiceFeatureWrapper() {
			}

			public override SerializedObject GetInstance() {
				return new HitDiceFeature (this);
			}

		}


		public class HitDiceFeature : GenericFeature, SerializedObject {
			int sides;
			private HitDice last;
			public override bool IsMisc {
				get { return false; }
			}

			public override Wrapper GetSerializableWrapper ()
			{
				HitDiceFeatureWrapper wrap = new HitDiceFeatureWrapper ();
				wrap.sides = this.sides;
				return wrap;
			}

			public override string Name() {
				return "HitDice(" + sides + ")";
			}

			public override string Description() {
				return "Grants " + Name ();
			}

			public HitDiceFeature(int sides, FeatureBundle parent=null) : base(parent) {
				this.sides = sides;
			}

			public HitDiceFeature(HitDiceFeatureWrapper wrap) {
				this.sides = wrap.sides;
			}

			public override void WhenActivatedOn(Sheet c) {
				last = new HitDice(sides);

				c.hitDice.Add (last);
			}

			public override void WhenDeactivatedOn(Sheet c) {
				c.hitDice.Remove (last);
				last = null;
			}
		}

}