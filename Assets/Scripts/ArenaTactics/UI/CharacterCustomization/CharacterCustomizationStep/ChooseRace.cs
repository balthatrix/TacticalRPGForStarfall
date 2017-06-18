using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AT.Battle;
using AT.Character; 
using UnityEngine.UI;
using Util.StateMachine;
namespace AT.UI
{



	public class ChooseRace : CharacterCustomizationStep {

		public ChooseRace(CharacterCustomizationController controller) : base(controller) {

		}


		public override void Entered(State s, State fromPrevious) {

			characterCustomization.character.race = null;
			base.Entered (s, fromPrevious);





			characterCustomization.manager.optionsWindow.SetTitle("Choose Race");


			OptButton human = characterCustomization.manager.optionsWindow.AddButton ("Human");
			human.OnOptLeftClicked += SelectedHuman;
			Race race =  new Race (RaceName.HUMAN);
			race.DressOptButtonForTooltip (human, Tooltip.TooltipPosition.RIGHT);

			race =  new Race (RaceName.HALF_ORC);
			OptButton halfOrc = characterCustomization.manager.optionsWindow.AddButton ("Half-Orc");
			race.DressOptButtonForTooltip (halfOrc, Tooltip.TooltipPosition.RIGHT);
			halfOrc.OnOptLeftClicked += SelectedHalfOrc;



			human.WasLeftClicked ();
		}


		void SelectedHuman(OptButton opt) {
			characterCustomization.character.race = new Race (RaceName.HUMAN);
			//@switchin
//			characterCustomization.manager.infoWindow.SetTitle("Human");
//			characterCustomization.manager.infoWindow.ClearContent ();
//			Tooltip.instance.SetText (Race.Description(RaceName.HUMAN));
//			characterCustomization.manager.infoWindow.AddTextContent(Race.Description (RaceName.HUMAN));
			SelectFeatures (characterCustomization.character.race.features);
		}

		void SelectedHalfOrc(OptButton opt) {
			//@switchin
			characterCustomization.character.race = new Race (RaceName.HALF_ORC);
//			characterCustomization.manager.infoWindow.SetTitle("Half-Orc");
//			characterCustomization.manager.infoWindow.ClearContent ();
//			characterCustomization.manager.infoWindow.AddTextContent(Race.Description (RaceName.HALF_ORC));
			SelectFeatures (characterCustomization.character.race.features);
		}


	}

}