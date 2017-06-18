using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AT.Character.Situation {
	public class AttackSituation : CheckLikeSituation {
		//could be Cast, or Attack
		public  AT.Battle.Action action;
		private Sheet attacker;
		private Sheet defender;
		private Gauge hitRoll;
		private Gauge AC;

		public List<int> rolls;
		public ResultType result;


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


		private bool wasRanged = false;
		private bool wasThrown = false;

		public Gauge ArmorClassGauge {
			get { return AC; }
		}
		public Gauge ToHitGauge {
			get { return hitRoll; }
		}

		private GenericWeapon weaponUsed;
		public void SetWeaponUsed(GenericWeapon w){
			weaponUsed = w;
		}

		public GenericWeapon WeaponUsed {
			get { return weaponUsed; }
		}

		public bool IsOffhand {
			get { 
				if (action is Battle.Attack) {
					return (action as Battle.Attack).IsOffhand;
				} else {
					return false;
				}
			}
		}

		public AttackSituation (Sheet attacker, Sheet defender, AT.Battle.Action action=null) {
			this.attacker = attacker;
			this.defender = defender;
			this.action = action;
			hitRoll = new Gauge ("To Hit");
			AC = new Gauge ("AC");
			rolls = new List<int> ();
		}



		public Sheet Defender {get{ return defender;}}
		public Sheet Attacker {get{ return attacker;}}


		//TODO: Generalize this....
		public ResultType GetResult(bool hypothetical = false) {
			
			if (IsOffhand) {
				attacker.ProduceToHit(this, true);
			} else {
				attacker.ProduceToHit(this);
			}
			defender.ProduceAC (this);



			if (!hypothetical) {
				attacker.AboutToAttack (this);
				defender.AboutToBeAttacked (this);

				int rollAccepted = 0;
				//TODO: This is not DRY.  See save situations....  This should be refactored.
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

//				rollAccepted = 20;
				hitRoll.ChangeCurrentAndMax(rollAccepted);
				result = CalculateResult (hitRoll, AC);


				attacker.DidAttack (this);
				defender.WasAttacked (this);


			}

			return this.result;
		}

		public string GetTranscript() {
			string ret = "Attack Roll ";
			if (rolls.Count > 1) {
				ret += rolls [0] + "|" + rolls [1] + " -> " + ToHitGauge.BaseValue + " ";
			} else {
				ret += rolls [0] + " ";
			}

			ret += "+ " + hitRoll.ModifierSum + " = " + hitRoll.ModifiedCurrent + " : ";
			ret += result.ToString ();
			return ret;
		}


		public ResultType CalculateResult(Gauge toHit, Gauge ac) {
			if (toHit.BaseValue == 1) {
				return ResultType.CRITICAL_MISS;
			}	else if (toHit.BaseValue == 20) {
				return ResultType.CRITICAL_HIT;
			}	else if (toHit.ModifiedCurrent >= ac.ModifiedCurrent) {
				return ResultType.HIT;
			}	else {
				return ResultType.MISS;
			}
		}

		public string Interpretation() {
			string interpretation = attacker.Name + " ";

			if (wasThrown) {
				interpretation += " throws ";
			} else if (wasRanged) {
				interpretation += " shoots ";
			}else {
				interpretation += " swings ";
			}

			interpretation += " a " + WeaponUsed.Name + " at " + Defender.Name + "!";
			return interpretation;
		}

		public string VerboseToString(string baseStr = "Attack d20:") {

			string disp = Interpretation ();
			if (rolls.Count > 1) {
				if (DisadvantageFlagged ()) {
					disp += "(Disadvantage => " + rolls [0] + "|" + rolls [1] + " = " + ToHitGauge.BaseValue + ") ";
				} else {
					disp += "(Advantage => " + rolls [0] + "|" + rolls [1] + " = " + ToHitGauge.BaseValue + ") ";
				}
			}
			disp += "  (Hit - AC => ";
			disp += hitRoll.ModifiedCurrent.ToString() + " - " + AC.ModifiedCurrent.ToString() + " = " + (hitRoll.ModifiedCurrent - AC.ModifiedCurrent).ToString () + ")";
			return  disp + "| " + result.ToString()  + "! " + hitRoll.ToString ()  + " --vs--" + AC.ToString ();
		}

		public string ToString() {
			string ret = attacker.Name + ": Attack Roll ";
			if (rolls.Count > 1) {
				ret += rolls [0] + "|" + rolls [1] + " -> " + ToHitGauge.BaseValue + " ";
			} else {
				ret += rolls [0] + " ";
			}

			ret += "+ " + hitRoll.ModifierSum + " = " + hitRoll.ModifiedCurrent + " : ";
			ret += result.ToString ();
			return ret;
		}

		private int Diff() {
			return hitRoll.ModifiedCurrent - AC.ModifiedCurrent;
		}

	}

}
