using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AT.Battle;
using AT.Character; 
using AT.Character;
using System.Linq;
using UnityEngine.UI;
using Util.StateMachine;
using Util;

namespace AT.UI
{
	public class PointBuy : CharacterCustomizationStep {
		int pointsAllowed;
		int pointsAlotted;

		const int MIN_VALUE_ALLOWED_PER_ABILITY = 8;
		const int MAX_VALUE_ALLOWED_PER_ABILITY = 15;


		private Dictionary<string, NumberDial> abilityNamesToDials;
		private string AbilityNameFromButton(NumberDial dial) {
			string ret = abilityNamesToDials.FirstOrDefault(x => x.Value == dial).Key;
			return ret;
		}
		private Dictionary<string, List<GaugeMod>> abilityNamesToAbilityMods;



		//point buy always goes on the last added class.  can't think of a reason to stick it on it's own feature bundle
		ClassLevel5e lastAddedClass;

		public PointBuy(CharacterCustomizationController controller) : base(controller) {
			
		}

		void CleanLast() {
			if (lastAddedClass != null) {
				if (abilityNamesToAbilityMods != null) {
					foreach (string abName in abilityNamesToAbilityMods.Keys) {
						foreach (GaugeMod gm in abilityNamesToAbilityMods[abName]) {
							lastAddedClass.features.Remove (gm);
						}
					}
				}
			}
			lastAddedClass = characterCustomization.character.classLevels [characterCustomization.character.classLevels.Count - 1];
			abilityNamesToAbilityMods = new Dictionary<string, List<GaugeMod>>();
			abilityNamesToAbilityMods.Add ("strength", new List<GaugeMod> ());
			abilityNamesToAbilityMods.Add ("dexterity", new List<GaugeMod> ());
			abilityNamesToAbilityMods.Add ("constitution", new List<GaugeMod> ());
			abilityNamesToAbilityMods.Add ("intelligence", new List<GaugeMod> ());
			abilityNamesToAbilityMods.Add ("wisdom", new List<GaugeMod> ());
			abilityNamesToAbilityMods.Add ("charisma", new List<GaugeMod> ());

			pointsAllowed = 27;
			pointsAlotted = 0;
		}

		void UpdateHeader() {
			characterCustomization.manager.optionsWindow.SetTitle("Choose Abilities (" + PointsLeft + ")");
		}

		void InitUi() {
			UpdateHeader ();

			abilityNamesToDials = new Dictionary<string, NumberDial> ();

			abilityNamesToDials.Add("strength",  characterCustomization.manager.optionsWindow.AddNumberDial ());
			abilityNamesToDials.Add("dexterity", characterCustomization.manager.optionsWindow.AddNumberDial ());
			abilityNamesToDials.Add("constitution", characterCustomization.manager.optionsWindow.AddNumberDial ());
			abilityNamesToDials.Add("intelligence", characterCustomization.manager.optionsWindow.AddNumberDial ());
			abilityNamesToDials.Add("wisdom", characterCustomization.manager.optionsWindow.AddNumberDial ());
			abilityNamesToDials.Add("charisma", characterCustomization.manager.optionsWindow.AddNumberDial ());

			foreach (NumberDial dial in abilityNamesToDials.Values) {
				dial.OnValueWillChange += UpdateDialsAndAbilities;
			}

			foreach (string abilityName in abilityNamesToDials.Keys) {
				NumberDial dial = abilityNamesToDials [abilityName];
				dial.generateLabel = (int value) => {
					return UtilString.Capitalize (abilityName) + ": " + value +", Cost: " + NextPointCost (value);
				};
				dial.Min = MIN_VALUE_ALLOWED_PER_ABILITY;
				dial.Max = MAX_VALUE_ALLOWED_PER_ABILITY;
			}
		}

		private int NextPointCost(int currentValue) {
			int ret = 1;
			if (currentValue >= 13) {
				ret = 2;
			}

			return ret;
		}

		private int CurrentPointRefund(int currentValue) {
			int ret = 1;
			if (currentValue > 13) {
				ret = 2;
			}

			return ret;
		}

		//ClassLevel5e lastClassChosen;
		public override void Entered(State s, State fromPrevious) {
			base.Entered (s, fromPrevious);
			CleanLast();
			InitUi ();
			Debug.Log ("Entered the point bbuy, from!!! " + fromPrevious.GetType().ToString());
			//OptButton fighter = characterCustomization.manager.optionsPanel.AddButton ("Fighter");


			//fighter.OnOptClicked += SelectedFighter;
			//fighter.WasClicked ();

		}

		private void RemoveAbilityGaugeModUnit(string abilityName) {
			if (abilityNamesToAbilityMods [abilityName].Count <= 0) {
				return;
			}
			GaugeMod gm = abilityNamesToAbilityMods [abilityName] [0];
			abilityNamesToAbilityMods [abilityName].RemoveAt (0);
			lastAddedClass.features.Remove (gm);
			RemoveFromSelected (gm);
		}

		private void AddAbilityGaugeModUnit(string abilityName) {
			GaugeMod gm = new GaugeMod (abilityName, 1, "", true, lastAddedClass);
			abilityNamesToAbilityMods [abilityName].Add(gm);
			AddToSelected (gm);
			lastAddedClass.features.Add (gm);
		}

		private void UpdateDialsAndAbilities(NumberDial nd, bool willDecrease) {
			if (willDecrease) {
				int refund = CurrentPointRefund (nd.Current);
				pointsAlotted -= refund;
				RemoveAbilityGaugeModUnit (AbilityNameFromButton (nd));
				//Take away ability increase feature....
			} else {
				int cost = NextPointCost (nd.Current);

				pointsAlotted += cost;
				AddAbilityGaugeModUnit (AbilityNameFromButton (nd));
				//Give ability increase feature...
				//Set max of all gauges to whatever is smaller: 15? or the current amount + (pointsAllowed-pointsAlotted....)
			}
			SetMaxes ();
			UpdateHeader ();
			characterCustomization.UpdateSheet ();
			if (PointsLeft == 0) {
				EnableConfirm ();
			} else {
				DisableConfirm ();
			}
		}

		private  int PointsLeft {
			get { 
				return pointsAllowed - pointsAlotted;
			}
		}

		private void SetMaxes() {
			foreach (string abilityName in abilityNamesToDials.Keys) {
				NumberDial dial = abilityNamesToDials [abilityName];
				if (NextPointCost (dial.Current) > PointsLeft) {
					dial.Max = dial.Current;
				} else {
					dial.Max = MAX_VALUE_ALLOWED_PER_ABILITY;
				}
			}
		}


		public override void Exiting(State s, State toDestination) {
			base.Exiting (s, toDestination);
			if (toDestination == previous) {//the player want back....
				
				CleanLast ();
			} //otherwise the player went forward
		}

	}
}