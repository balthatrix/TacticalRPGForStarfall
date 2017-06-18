using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AT.Battle;
using AT.Character; 
using UnityEngine.UI;

using Util.StateMachine;

namespace AT.UI
{



	public class ChooseName : CharacterCustomizationStep {

		public ChooseName(CharacterCustomizationController controller) : base(controller) {

		}

		TextInputDialog textInputDialog;



		public override void Exiting(State s, State toDestination) {
			Debug.LogError ("Exits!Z");

			textInputDialog.OnChanged -= ChangedName;
			textInputDialog.OnConfirmed -= NameConfirmed;
			GameObject.Destroy (textInputDialog.gameObject);
			base.Exiting (this, toDestination);
		}

		public override void Entered(State s, State fromPrevious) {
			base.Entered (s, fromPrevious);

			textInputDialog = characterCustomization.manager.CreateNameInput ();
			textInputDialog.OnChanged += ChangedName;
			textInputDialog.OnConfirmed += NameConfirmed;
			textInputDialog.InputField.characterLimit = 21;
//
//			Debug.Log ("rep: " + previous.GetType ().ToString ());
//			Debug.Log ("next: " + destination.GetType ().ToString ());
			//DisableBack ();
			DisableConfirm ();
		}


		void ChangedName(TextInputDialog dlg) {
//			Debug.LogError ("HIF");
			if (dlg.value != "") {
				dlg.EnableConfirm ();
			} else {
				dlg.DisableConfirm ();
			}

			characterCustomization.character.Name = dlg.value;
			characterCustomization.UpdateSheet ();
		}

		void NameConfirmed(GenericDialog dlg) {
			
			characterCustomization.SwitchState (destination);
		}


	}

}