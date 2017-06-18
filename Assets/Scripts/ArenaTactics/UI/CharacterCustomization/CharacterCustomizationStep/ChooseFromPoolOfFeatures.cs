using AT.Character;
using AT.Character;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Util.StateMachine;

namespace AT.UI {
public class ChooseFromPoolOfFeatures : CharacterCustomizationStep {

		FeaturePointer featurePointer;
		Dictionary<OptButton, GenericFeature> buttonsToFeatureOpts;
		public ChooseFromPoolOfFeatures(FeaturePointer ptr, CharacterCustomizationController controller) : base(controller) {
			this.featurePointer = ptr;
			buttonsToFeatureOpts = new Dictionary<OptButton, GenericFeature> ();
		}


		public override void SetInitialState() {
			base.SetInitialState ();

			buttonsToFeatureOpts.Clear ();
			CleanLastChosen ();

		}



		private void CleanLastChosen() {
			if (lastChosen != null) {

				ResetFeaturesSelected ();
				featurePointer.parent.features.Remove (lastChosen);
				lastChosen = null;
			}
		}


		public override void Entered(State s, State fromPrevious) {
			base.Entered (s, fromPrevious);



			characterCustomization.manager.optionsWindow.SetTitle(featurePointer.headerText);
			//filtration should happen here...
			List<GenericFeature> filtered = featurePointer.filterPool(characterCustomization.character);
			if (filtered.Count == 0) {
				Debug.LogWarning (this.GetType ().ToString () + " had 0 options after being filtered");
				characterCustomization.SwitchState (destination);
				return;
			}
			foreach (GenericFeature f in filtered) {
				OptButton opt = characterCustomization.manager.optionsWindow.AddButton (f.Name ());
				f.DressOptButtonForTooltip (opt, Tooltip.TooltipPosition.RIGHT);
				opt.OnOptLeftClicked += SelectedOption;
				buttonsToFeatureOpts.Add (opt, f);
			}


			buttonsToFeatureOpts.Keys.ToList().First().WasLeftClicked();
		}


		private GenericFeature lastChosen;

		void SelectedOption(OptButton opt) {
			GenericFeature f;
			if(buttonsToFeatureOpts.TryGetValue(opt, out f)) {
				featurePointer.parent.features.Remove (lastChosen);
				lastChosen = f;
				featurePointer.parent.features.Add (lastChosen);

//				Tooltip.instance.SetText (f.Description ());
				//@switchin
//				characterCustomization.manager.infoWindow.ClearContent ();
//				characterCustomization.manager.infoWindow.AddTextContent (f.Description());
//				characterCustomization.manager.infoWindow.SetTitle(f.Name());

				//this adds the features to the character.  apply features shouldn't be called 
				SelectFeatures (new List<GenericFeature> { lastChosen }); 
			}

		}
	}


}