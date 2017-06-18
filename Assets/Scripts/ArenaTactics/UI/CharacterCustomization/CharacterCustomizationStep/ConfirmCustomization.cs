using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AT.Battle;
using AT.Character; 
using UnityEngine.UI;
using Util.StateMachine;

namespace AT.UI
{



	public class ConfirmCustomization : CharacterCustomizationStep {

		public ConfirmCustomization(CharacterCustomizationController controller) : base(controller) {

		}

		bool doFinish;
		public override void Entered(State s, State fromPrevious) {
			
			base.Entered (s,fromPrevious );
			Debug.Log ("prev: " + fromPrevious.GetType ().ToString ());
			Debug.Log ("me  prevo: " + previous.GetType ().ToString ());

			characterCustomization.manager.optionsWindow.SetTitle("Confirm");


//			OptButton done = characterCustomization.manager.optionsWindow.AddButton ("Done");
//			OptButton startOver = characterCustomization.manager.optionsWindow.AddButton ("Start Over");

			characterCustomization.manager.optionsWindow.AddTextContent ("Review changes and press confirm below to finish creation.");
			doFinish = true;
			characterCustomization.manager.confirmSelection.GetComponent<Button> ().interactable = true;
//
//			done.OnOptLeftClicked += SelectFinish;
//			startOver.OnOptLeftClicked += SelectStartOver;
//
//			done.WasLeftClicked ();
		}

		void SelectFinish(OptButton opt) {
			doFinish = true;
//			characterCustomization.manager.infoWindow.ClearContent ();
//			characterCustomization.manager.infoWindow.AddTextContent("\n Press confirm to finalize your choices.");
			characterCustomization.manager.confirmSelection.GetComponent<Button> ().interactable = true;
		}

		void SelectStartOver(OptButton opt) {
//			characterCustomization.manager.infoWindow.ClearContent ();
//			characterCustomization.manager.infoWindow.AddTextContent("\n Press confirm to start all over.");
			doFinish = false;
			characterCustomization.manager.confirmSelection.GetComponent<Button> ().interactable = true;
		}

		public override void Confirm(OptButton opt) {
			if (doFinish)
				Finish ();
			else
				StartOver ();
		}

		void Finish() {
			characterCustomization.ConfirmCustomization ();
		}

		void StartOver() {
			Exiting (this, characterCustomization.head);
			CharacterCustomizationStep step = previous;
			while (step != characterCustomization.head) {				
				step.ResetFeaturesSelected ();
				//step.Entered (step);
				//step.Exited (step);
				if(step is ChooseClass) {
					ChooseClass cc = (ChooseClass)step;
					cc.CleanLastChosen ();
				}
				step = step.previous;
			}
			characterCustomization.UpdateSheet ();
			characterCustomization.HeadStateEntered (characterCustomization.head, this);

		}






	}

}