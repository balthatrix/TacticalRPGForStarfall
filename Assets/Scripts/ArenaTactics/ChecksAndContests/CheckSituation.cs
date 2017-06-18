using UnityEngine;

using System.Collections;


namespace AT.Character.Situation {

	/// <summary>
	/// Check situation.  Represents a situation in which
	/// A character is attempting to perform some action,
	/// scaling a wall, picking a pocket, running a 5-minute mile
	/// </summary>
	public class CheckSituation : CheckLikeSituation {
		private Sheet checkProducer;
		private DcProducer dcProducer;
		public  Gauge DC;
		public Gauge checkValue;
		public SkillType skillType;
		public AbilityType abilityType;

		bool advantageFlagged = false;
		bool disadvantageFlagged = false;


		public void FlagAdvantage() {
			advantageFlagged = true;
		}
		public bool AdvantageFlagged() {
			return advantageFlagged;
		}

		public void FlagDisadvantage() {
			disadvantageFlagged = true;
		}
		public bool DisadvantageFlagged() {
			return disadvantageFlagged;
		}

		//skill type is optional
		public CheckSituation(Sheet _char, DcProducer cp, AbilityType abilityType, SkillType skillType = SkillType.NULL) {//add a check producer for second this.  
			this.checkProducer = _char;
			this.dcProducer = cp;
			this.abilityType = abilityType;
			this.skillType = skillType;

			string name = abilityType.ToString ().ToLower ();
			if (skillType != SkillType.NULL) {
				name += " (" + skillType.ToString ().ToLower () + ")";
			}

			checkValue = new Gauge (name+ " check");
			DC = new Gauge ("DC");
		}

		public ResultType CalculateResult (Gauge check, Gauge dc) {
			if (check.ModifiedCurrent >= dc.ModifiedCurrent) {
				return ResultType.SUCCESS;
			} else {
				return ResultType.FAILURE;
			}
		}

		public ResultType GetResult(bool hypothetical=false) {
			checkProducer.ProduceCheck (this);
			dcProducer.ProduceDcInCheck (this);
			ResultType ret = ResultType.FAILURE;
			if (!hypothetical) {
				checkProducer.AboutToResolveCheck (this);
				dcProducer.AboutToResolveDcInCheck (this);

				ret = CalculateResult (checkValue, DC);

				checkProducer.DidResolveCheck (this);
				dcProducer.DidResolveDcInCheck (this);
			}

			return ret;
		}

		public string GetTranscript() {
			return GetType ().ToString ();
		}
	}

}
