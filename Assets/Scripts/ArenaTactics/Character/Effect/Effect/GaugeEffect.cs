using UnityEngine;
using System.Collections;


namespace AT.Character.Effect {

	//TODO: Make a generic effect at some point.
	public class GaugeEffect : GenericEffect {


		public  AT.Character.Gauge gauge;

		public GaugeEffect(string name="effect amount") {
			
			this.gauge = new AT.Character.Gauge(name);
		}

		public AT.Character.Gauge Gauge {
			get {return this.gauge;}
		}
		public int Amount {
			get { return this.gauge.ModifiedCurrent; }
		}

		public override void ApplyTo(AT.Character.Sheet c, AT.Battle.Action source = null) {
			Debug.LogError("You need to override the GaugeEffect's ApplyTo function");
		}

		public override void FailedAgainst(AT.Character.Sheet c) {

		}

		public override string GetTranscript() {
			
			Debug.LogError ("You should override GetTranscript in GaugeEffect");
			return "ERRPR";
		} 


	}
}
