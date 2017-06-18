using UnityEngine;

using System.Collections;
using System.Collections.Generic;


namespace AT.Character.Situation {

	public enum SaveContext {
		SPELL,
		TRAP
	}



	/// <summary>
	/// Abstracts a situation, in which a saving throw
	/// is forced on a character.  Identical to checks, 
	/// but there is not skill involved, only raw ability.
	/// </summary>
	public class SaveSituation : CheckLikeSituation {
		private Sheet saveProducer;
		private DcProducer dcProducer;
		public SpellLibrary.Spell spell;
		public  Gauge DC;
		public Gauge saveValue;
		public AbilityType abilityType;
		public SaveContext context;


		public List<int> rolls = new List<int>();

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


		public SaveSituation(Sheet _char, DcProducer cp, AbilityType abilityType, SaveContext context, SpellLibrary.Spell spell=null) {  
			this.saveProducer = _char;
			this.dcProducer = cp;
			this.abilityType = abilityType;
			this.spell = spell;
			this.context = context;

			saveValue = new Gauge (abilityType.ToString().ToLower() + " save");
			DC = new Gauge ("DC");
		}

		public ResultType CalculateResult (Gauge save, Gauge dc) {
			Debug.Log ("Comparing this shich: " + save.ModifiedCurrent);
			Debug.Log ("with this shich: " + dc.ModifiedCurrent);
			if (save.ModifiedCurrent >= dc.ModifiedCurrent) {
				
				return ResultType.SUCCESS;
			} else {
				return ResultType.FAILURE;
			}
		}

		public ResultType GetResult(bool hypothetical=false) {
			saveProducer.ProduceSave (this);
			dcProducer.ProduceDcInSave (this);
			ResultType ret = ResultType.FAILURE;

			//TODO: This is not DRY.  See attack situations....  This should be refactored.
			if (!hypothetical) {
				saveProducer.AboutToResolveSave (this);
				dcProducer.AboutToResolveDcInSave (this);

				int rollAccepted = 0;
				if (DisadvantageFlagged () && AdvantageFlagged()) {
					//roll normally
					int rll = Sheet.DiceRoll(20);
					rolls.Add (rll);
					rollAccepted = rll; 
				} else if(AdvantageFlagged()){
					//roll adv
					int[] rll = Sheet.MultipleDiceRoll(20, 2);
					rolls.Add (rll[0]);
					rolls.Add (rll[1]);

					if (rll[0] >= rll[1]) {
						rollAccepted = rll[0]; 
					} else {
						rollAccepted = rll[1];
					}
				} else if(DisadvantageFlagged()) {
					//roll dis
					int[] rll = Sheet.MultipleDiceRoll(20, 2);
					rolls.Add (rll[0]);
					rolls.Add (rll[1]);

					if (rll[0] <= rll[1]) {
						rollAccepted = rll[0]; 
					} else {
						rollAccepted = rll[1];
					}
				} else {
					//roll normally
					int rll = Sheet.DiceRoll(20);
					rolls.Add (rll);
					rollAccepted = rll; 
				}
				saveValue.ChangeCurrentAndMax(rollAccepted);

				ret = CalculateResult (saveValue, DC);

				saveProducer.DidResolveSave (this);
				dcProducer.DidResolveDcInSave (this);
			}

			return ret;
		}

		public string GetTranscript() {
			return GetType ().ToString ();
		}
	}

}
