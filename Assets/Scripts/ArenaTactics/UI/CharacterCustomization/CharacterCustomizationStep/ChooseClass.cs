using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AT.Battle;
using AT.Character; 
using System.Linq;
using UnityEngine.UI;
using Util.StateMachine;

namespace AT.UI
{
	public class ChooseClass : CharacterCustomizationStep {

		public ChooseClass(CharacterCustomizationController controller) : base(controller) {

		}

		ClassLevel5e lastClassChosen;
		public override void Entered(State s, State fromPrevious) {
			

			base.Entered (s, fromPrevious);
			characterCustomization.manager.optionsWindow.SetTitle("Choose Class");


			OptButton fighter = characterCustomization.manager.optionsWindow.AddButton ("Fighter");
			fighter.OnOptMousedOver += (button) => {	
				int lvl;
				if(lastClassChosen != null && lastClassChosen.classType == ClassType.FIGHTER) {
					lvl = characterCustomization.character.ClassLevelIn(ClassType.FIGHTER) - 1;
				} else {
					lvl = characterCustomization.character.ClassLevelIn(ClassType.FIGHTER);
				}
				ClassLevel5e fgterStub = new ClassLevel5e(ClassType.FIGHTER, lvl);
				fgterStub.InitDefaultFeatures();
				Tooltip.instance.SetText(fgterStub.TooltipHoverText());
				Tooltip.instance.Show(fighter.transform as RectTransform, Tooltip.TooltipPosition.RIGHT);
			};
			fighter.OnOptMousedOut += (button) => {
				Tooltip.instance.Hide();
			};

			fighter.OnOptLeftClicked += SelectedFighter;


			OptButton wizard = characterCustomization.manager.optionsWindow.AddButton ("Wizard");
			wizard.OnOptMousedOver += (button) => {	
				int lvl;
				if(lastClassChosen != null && lastClassChosen.classType == ClassType.WIZARD) {
					lvl = characterCustomization.character.ClassLevelIn(ClassType.WIZARD) - 1;
				} else {
					lvl = characterCustomization.character.ClassLevelIn(ClassType.WIZARD);
				}
				ClassLevel5e wizardStub = new ClassLevel5e(ClassType.WIZARD, lvl);
				wizardStub.InitDefaultFeatures();
				Tooltip.instance.SetText(wizardStub.TooltipHoverText());
				Tooltip.instance.Show(wizard.transform as RectTransform, Tooltip.TooltipPosition.RIGHT);
			};
			wizard.OnOptMousedOut += (button) => {
				Tooltip.instance.Hide();
			};

			wizard.OnOptLeftClicked += SelectedWizard;

			fighter.WasLeftClicked ();
		}


		public override void SetInitialState() {

			CleanLastChosen ();
			base.SetInitialState ();
		}

		public void CleanLastChosen() {
			if (lastClassChosen != null) {
				lastClassChosen.DeactivateFeatures (characterCustomization.character);
				characterCustomization.character.RemoveClassLevel (lastClassChosen);
				lastClassChosen = null;
			}
		}
		
		ClassLevel5e ClassLvlFromType(ClassType type) {
			if (characterCustomization.character.CharacterLevel == 0) {
				//0 represents a starting character class level

				return new ClassLevel5e (type, 0);
			} else {
				Debug.Log("gettin nth level: " + (characterCustomization.character.ClassLevelIn(ClassType.FIGHTER) + 1).ToString());
				//1 + represents a multiclassing, or continuing level fighter.
				return new ClassLevel5e (type, characterCustomization.character.ClassLevelIn(ClassType.FIGHTER) + 1);
			}
		}
		

		void SelectedFighter(OptButton opt) {
			CleanLastChosen ();

			//@switchin
//			Tooltip.instance.SetText (ClassLevel5e.Description (ClassType.FIGHTER));
//			characterCustomization.manager.infoWindow.ClearContent ();
//			characterCustomization.manager.infoWindow.AddTextContent(ClassLevel5e.Description (ClassType.FIGHTER));
			lastClassChosen = ClassLvlFromType(ClassType.FIGHTER);
//
//			if (characterCustomization.character.CharacterLevel == 0) {
//                //0 represents a starting character class level
//
//				lastClassChosen = new ClassLevel5e (ClassType.FIGHTER, 0);
//			} else {
//                Debug.Log("gettin nth level: " + (characterCustomization.character.ClassLevelIn(ClassType.FIGHTER) + 1).ToString());
//                //1 + represents a multiclassing, or continuing level fighter.
//                lastClassChosen = new ClassLevel5e (ClassType.FIGHTER, characterCustomization.character.ClassLevelIn(ClassType.FIGHTER) + 1);
//			}



//			characterCustomization.manager.infoWindow.SetTitle(Util.UtilString.Capitalize(lastClassChosen.PresentableType ()));
			lastClassChosen.InitDefaultFeatures ();



			characterCustomization.character.AddClassLevel(lastClassChosen);
			SelectFeatures (lastClassChosen.features);
		}



		void SelectedWizard(OptButton opt) {
			CleanLastChosen ();

			//@switchin
			//			Tooltip.instance.SetText (ClassLevel5e.Description (ClassType.FIGHTER));
			//			characterCustomization.manager.infoWindow.ClearContent ();
			//			characterCustomization.manager.infoWindow.AddTextContent(ClassLevel5e.Description (ClassType.FIGHTER));
			lastClassChosen = ClassLvlFromType(ClassType.WIZARD);
			//
			//			if (characterCustomization.character.CharacterLevel == 0) {
			//                //0 represents a starting character class level
			//
			//				lastClassChosen = new ClassLevel5e (ClassType.FIGHTER, 0);
			//			} else {
			//                Debug.Log("gettin nth level: " + (characterCustomization.character.ClassLevelIn(ClassType.FIGHTER) + 1).ToString());
			//                //1 + represents a multiclassing, or continuing level fighter.
			//                lastClassChosen = new ClassLevel5e (ClassType.FIGHTER, characterCustomization.character.ClassLevelIn(ClassType.FIGHTER) + 1);
			//			}



			//			characterCustomization.manager.infoWindow.SetTitle(Util.UtilString.Capitalize(lastClassChosen.PresentableType ()));
			lastClassChosen.InitDefaultFeatures ();



			characterCustomization.character.AddClassLevel(lastClassChosen);
			SelectFeatures (lastClassChosen.features);
		}
	}
}