using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using AT.Serialization;


namespace AT.Character {
	


	//could be a race, a monster, or a characterClass level
	public class FeatureBundle : SerializedObject {
		

		public List<GenericFeature> features;
		bool active = false;

		// Use this for initialization
		public FeatureBundle(List<GenericFeature> _features=null) {
//			Debug.Log ("hs");
			if (_features == null) {
				features = new List<GenericFeature> ();
			} else {
				features = _features;
			}
		}



		public virtual Wrapper GetSerializableWrapper() {

			Debug.LogError ("FeatureBundle subtype " + GetType() + " must override GetSerialization wrapper to support serialization");
			return null;
		}



		public virtual void ActivateFeatures(Sheet c) {
			if (active)
				return;

			active = true;
			features.ForEach ((f)=>f.WhenActivatedOn(c));
		}

		public virtual void DeactivateFeatures(Sheet c) {
			if (!active)
				return;
			active = false;
			features.ForEach ((f)=>f.WhenDeactivatedOn(c));
		}

	}


}