using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Util.StateMachine;
using AT.Battle;
using AT.Character;

namespace AT.UI {
	
	public class CharacterCustomizationController : Controller {
		public AT.Character.Sheet character;
		public CharacterCreationManager manager;
		private List<CharacterCustomizationStep> staticSteps;
		//should have a stack of character customization blahs

		public CharacterCustomizationStep head;
		public CharacterCustomizationStep tail;


		public CharacterCustomizationController(
			CharacterCreationManager manager, 
			Sheet character,
			List<CharacterCustomizationStep> staticSteps=null
		) : base() {
			if (staticSteps == null) {
				this.staticSteps = new List<CharacterCustomizationStep> ();

			} else {
				this.staticSteps = staticSteps;
			}

			this.head = new CharacterCustomizationStep (this);
			this.head.OnDidEnter += HeadStateEntered;

			this.tail = new ConfirmCustomization (this);
			this.tail.OnDidEnter += TailStateEntered;

			this.head.LinkDestination (tail);

			this.manager = manager;
			this.character = character;
		}

		public virtual void HeadStateEntered(State s, State prev) {
//			Debug.Log ("yo head entered!");
			SwitchState (head.destination);
		}

		public virtual void TailStateEntered(State s, State prev) {
//			Debug.Log ("yo tail entered!");

			//SwitchState (tail.previous);
		}

		public void BeginCustomization() {			
			manager.ui.gameObject.SetActive(true);
			SwitchState (head);
		}

		public void EndCustomization() {			
			manager.ui.gameObject.SetActive (false);
		}

        public void ConfirmCustomization()
        {
            if (OnWillConfirmCustomization != null)
            {
                OnWillConfirmCustomization();
            }

            EndCustomization();

            if (OnDidConfirmCustomization != null) {
                OnDidConfirmCustomization();
            }
        }


        public delegate void WillConfirmCustomizationAction();
        public event WillConfirmCustomizationAction OnWillConfirmCustomization;


        public delegate void DidConfirmCustomizationAction();
        public event DidConfirmCustomizationAction OnDidConfirmCustomization;

        public void AddStaticStep(CharacterCustomizationStep step) {

			tail.previous.LinkDestination (step);
			step.LinkDestination (tail);

		}

		public void UpdateSheet() {
			InventoryView.instance.SetCurrentCharacter (character);

//			manager.avatarWindow.ClearContent();
//
//			manager.avatarWindow.AddTextContent("\n"+character.ToString ());
		}
	}


}