using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT.Character;
using Util.StateMachine;

namespace AT.UI {
	public class CharacterCreationController  : CharacterCustomizationController {
		public CharacterCreationController(	CharacterCreationManager manager, Sheet character, List<CharacterCustomizationStep> staticSteps=null
		) : base( manager, character, staticSteps ) {
			ChooseRace chooseRace = new ChooseRace (this);
			ChooseClass chooseClass = new ChooseClass (this);
			ChooseName chooseName = new ChooseName (this);
			PointBuy pointBuy = new PointBuy (this);

			AddStaticStep (chooseRace);
			AddStaticStep (chooseClass);
			AddStaticStep (pointBuy);
			AddStaticStep (chooseName);

			OnDidConfirmCustomization += SerializeThisShit;

		}


		public override void HeadStateEntered(State s, State prev) {
			Debug.Log ("yo head entered!");
			if (prev != null) {
				manager.BackToTitle ();
			} else {
				SwitchState (head.destination);
			}
		}

		private void SerializeThisShit() {
			GameManager.persistentInstance.SaveCharacter (character);
			manager.BackToTitle ();
		}

	}

}