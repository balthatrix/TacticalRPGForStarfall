using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using AT.Serialization;

namespace AT.Character {
	public delegate int InfluencerModAmount(Gauge influencer);


	[System.Serializable]
	public class GaugeWrapper:Wrapper {
		public int current;
		public int max;
		public string name;

		public  GaugeWrapper() : base() {
			
		}

		public override SerializedObject GetInstance() {
			Gauge ret = new Gauge (name);
			ret.SetMax (max);
//			Debug.Log ("Set " + name + " to max " + max);
			ret.SetCurrent (current);
			return ret;
		}
	} 

	public class Gauge:SerializedObject {

		public Wrapper GetSerializableWrapper() {
			GaugeWrapper gw = new GaugeWrapper ();
			gw.current = current;
			gw.max = max;
			gw.name = name;

			return gw;
		}

		public delegate void ChangedAction(Gauge g);
		public event ChangedAction OnChanged;


		private List<Modifier> modifiers;
	

		//gauge influencers are for gauges that affect this one...
		//Gauge -> The gauge that influences this.  Modifier -> the modifier that influences
		private Dictionary<Gauge, Modifier> influencers;
		private Dictionary<Gauge, InfluencerModAmount> influencerFuncs;

		private int current;
		public int Current { get { return current; } }
		private int max;
		public int Max { get { return max; } }

		public int BaseValue { 
			get	{ 
				return Current + BaseModifierSum; 
			}
		}


		public int BaseModifierSum {
			get { 
				return modifiers.Aggregate (0, (runningSum, nextMod) => {
					if(nextMod.isBase) {
						return 	runningSum + nextMod.Value;
					} else {
						return runningSum;
					}
				});
			}
		}


		public int ModifierSum {
			get { 
				return modifiers.Aggregate (0, (runningSum, nextMod) => {
					if(nextMod.isBase) {
						return runningSum;
					} else {
						return 	runningSum + nextMod.Value;
					}
				});
			}
		}

		private string name;
		public string Name { get { return name; } }

		public Gauge(string name) {
			

			this.name = name;
			this.modifiers = new List<Modifier> ();
			influencers = new Dictionary<Gauge, Modifier> ();
			influencerFuncs = new Dictionary<Gauge, InfluencerModAmount> ();
		}

		public void SetCurrent(int val) {
			int old = current;
			current = val;
			if (current != old) {

				Changed ();
			}
		}

		public void SetMax(int val) {
			int old = max;
			max = val;
			if (max != old) {
				Changed ();
			}
		}

		public void Changed() {
			if (OnChanged != null) {

				OnChanged (this);
			}
		}

		public void ChangeCurrent(int byVal) {
			current += byVal;


			Changed ();
		}


		public void ChangeCurrentAndMax(int byVal) {
			current += byVal;
			max += byVal;


			Changed ();
		}

		public void ChangeMax(int byVal) {
			max += byVal;
		}


		public int ModifiedCurrent {
			get { 

				return current + ModifierSum + BaseModifierSum;
			}
		}

		public int ModifiedMax {
			get { 
				return max + ModifierSum + BaseModifierSum;
			}
		}

		public Modifier FindModifierByTag(string tag) {
			foreach (Modifier m in modifiers) {
				if (m.SourceTag == tag)
					return m;
			}
			return null;
		}



		public new string ToString() {
			IEnumerable<string> strings = modifiers.Select ((m)=>{
				string opsign = "";
				if(m.Value > 0) {
					opsign = "+"; //negative value happens automatically
				} 
				return m.SourceTag + ": " + opsign + m.Value.ToString();
			});

			if (modifiers.Count > 0) {
				return  " {"+name+" total: " + ModifiedCurrent.ToString () + "} " + BaseValue.ToString () + ", " + string.Join (", ", strings.ToArray ());
			} else {

				return name + ": " + BaseValue.ToString() + ", " +  string.Join (", ", strings.ToArray());
			}
		}

		public void AddInfluencerGauge(Gauge g, InfluencerModAmount modAmount) {

			Modifier m = new Modifier (modAmount (g), g.name);
			modifiers.Add (m);

			influencers.Add (g, m);
			influencerFuncs.Add (g, modAmount);
			g.OnChanged += UpdateInfluencerModAmount;

		}

		public void RemoveInfluencerGauge(Gauge g) {
			Modifier m = null;
			if (influencers.TryGetValue (g, out m)) {
				modifiers.Remove(m);
				influencers.Remove(g);
				influencerFuncs.Remove (g);
				g.OnChanged -= UpdateInfluencerModAmount;
			}
		}

		public void UpdateInfluencerModAmount(Gauge g) {
			int initial = ModifiedCurrent;

			Modifier m = null;
			if (influencers.TryGetValue (g, out m)) {
				InfluencerModAmount modAmount = null;
				if(influencerFuncs.TryGetValue(g, out modAmount)) {
					m.SetValue(modAmount(g));
				}
			}
			if (initial != ModifiedCurrent) {
				if(OnChanged != null)
					OnChanged (this);
			}
		}

		public Modifier Modify(Modifier m) {
			int initial = ModifiedCurrent;
			modifiers.Add (m);
			if (initial != ModifiedCurrent) {
				if(OnChanged != null)
					OnChanged (this);
			}
			return m;
		}

		public void UnModify(Modifier m) {
			int initial = ModifiedCurrent;
			modifiers.Remove (m);
			if (initial != ModifiedCurrent) {
				if(OnChanged != null)
					OnChanged (this);
			}
		}
	}

	public class Modifier {
		private int value;
		public bool isBase;

		public int Value { get { return value; } }

		private string sourceTag;
		public string SourceTag { get { return sourceTag; } }

		public void SetValue(int newVal) {
			value = newVal;
		}

		public Modifier(int value, string sourceTag = "none", bool isBase=false) {
			this.value = value;
			this.sourceTag = sourceTag;
			this.isBase = isBase;
		}


	}

	public class BaseModifier : Modifier {
		public BaseModifier(int value, string sourceTag="none") : base(value, sourceTag, true) {}
	}
}


