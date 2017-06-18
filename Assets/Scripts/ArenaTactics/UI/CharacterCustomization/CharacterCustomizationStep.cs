using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AT.Battle;
using AT.Character; 
using UnityEngine.UI;
using Util.StateMachine;

namespace AT.UI
{
	

	/// <summary>
	/// Work that needs to be done:
	/// When a customization step is entered:
	///   -options are created
	///   -each option is listened to as the selection
	///When a selection made
	///   -change the character sheet for preview, and enable the forward button.
	///When selection confirmed:
	///   -Advance the character customization, by telling controller to shift the steps
	///When back button clicked: 
	///   -if selection made, unchange the character sheet feature preview
	///   -tell the controller to unshift the customizationSteps (enter previous state)
	///   -
	/// </summary>
	public class CharacterCustomizationStep : State {
		//generic typed list of options.
		//Allows choices to be of RaceName, 
		//Proficiency type,
		//SkillType
		//Etc...

		//this way, the back button can controll behavior properly....
		public CharacterCustomizationStep previous;
		public CharacterCustomizationStep destination;

		public CharacterCustomizationController characterCustomization;




		public List<GenericFeature> featuresSelected;
		public void LinkDestination(CharacterCustomizationStep dest) {
			destination = dest;
			dest.previous = this;
		}

		public CharacterCustomizationStep(CharacterCustomizationController controller) : base(controller) {
			characterCustomization = controller;
			OnDidEnter += Entered;
			OnWillExit += Exiting;
		}


		/// <summary>
		/// Select Features.  Allows the customization step to preview the features that have been added, and control removing them upon selecting a different option.
		/// </summary>
		/// <param name="features">Features.</param>
		public void SelectFeatures(List<GenericFeature> features) {
			
			if (featuresSelected != null) {
				ResetFeaturesSelected ();
			}

			featuresSelected = features;
			 
			foreach(GenericFeature f  in featuresSelected) {
				f.WhenActivatedOn (characterCustomization.character);
			}

			EnableConfirm ();

			characterCustomization.UpdateSheet ();
		}

		public void AddToSelected(GenericFeature feature) {
			if (featuresSelected == null)
				featuresSelected = new List<GenericFeature> ();
			featuresSelected.Add (feature);
			feature.WhenActivatedOn (characterCustomization.character);
			characterCustomization.UpdateSheet ();
		}


		public void RemoveFromSelected(GenericFeature feature) {
			if (featuresSelected == null)
				return;
			featuresSelected.Remove (feature);
			feature.WhenDeactivatedOn (characterCustomization.character);
			characterCustomization.UpdateSheet ();
		}


		public void ResetFeaturesSelected() {
			if (featuresSelected != null) {
				foreach (GenericFeature f  in featuresSelected) {
					f.WhenDeactivatedOn (characterCustomization.character);
				}
				characterCustomization.UpdateSheet ();
				featuresSelected = null;
			}
		}

		public virtual void SetInitialState() {
			ResetFeaturesSelected ();

			Tooltip.instance.Hide ();
			//@switchin
//			characterCustomization.manager.infoWindow.ClearContent();
//			characterCustomization.manager.infoWindow.SetTitle("");

			ClearOptions ();

			//nothing is selected at first
			DisableConfirm ();
			if (previous == null) {

				DisableBack();
			} else {
				EnableBack ();
			}


		}

		public virtual void Exiting(State s, State toDestination) {
			if (toDestination == previous)
				SetInitialState ();

			characterCustomization.manager.confirmSelection.OnOptLeftClicked -= Confirm;
			characterCustomization.manager.backButton.OnOptLeftClicked -= Back;

		}

		public virtual void Entered(State s, State fromPrevious) {
			SetInitialState ();


			characterCustomization.manager.confirmSelection.OnOptLeftClicked += Confirm;
			characterCustomization.manager.backButton.OnOptLeftClicked += Back;
		}



		public void ClearOptions() {

			characterCustomization.manager.optionsWindow.ClearContent();
		}

		public void EnableConfirm() {
			characterCustomization.manager.confirmSelection.GetComponent<Button> ().interactable = true;
		}

		public void DisableConfirm() {
			characterCustomization.manager.confirmSelection.GetComponent<Button> ().interactable = false;
		}

		public void EnableBack() {
			characterCustomization.manager.backButton.GetComponent<Button> ().interactable = true;
		}

		public void DisableBack() {
			characterCustomization.manager.backButton.GetComponent<Button> ().interactable = false;
		}
	

		public virtual void Back(OptButton opt) {
			if (previous != null) {
				characterCustomization.SwitchState (previous);
			}
		}

		public virtual void Confirm(OptButton tbn=null) {
			if (featuresSelected == null)
				return;
//			Debug.Log("Okay now " + this.GetType().ToString());
//			Debug.Log("-> " + this.destination.GetType().ToString());
			//check features for feature pointers.  wire them into to be the next state, with the last customization state
			//pointing to this destination...
			List<CharacterCustomizationStep> steps = new List<CharacterCustomizationStep>();
			foreach (GenericFeature f in featuresSelected) {
				if (f is FeaturePointer) {
					FeaturePointer pointer = (FeaturePointer) f;
					steps.Add(pointer.GetCustomizationStep (characterCustomization));
				}
			}

			if (steps.Count > 0) {
				CharacterCustomizationStep nextStep = steps [0];
				CharacterCustomizationStep nextPrevious = this;

				for (int i = 1; i < steps.Count; i++) {
					nextStep.destination = steps [i];
					nextStep.previous = nextPrevious;
					nextPrevious = nextStep;
					nextStep = steps [i];
				}

				nextStep.destination = destination;
				nextStep.previous = nextPrevious;

				characterCustomization.SwitchState (steps [0]);
			} else {
				characterCustomization.SwitchState (destination);
			}


		}


	}



}