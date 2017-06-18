using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AT.Character;

namespace AT {
	namespace Battle {  
		public class Wait : Action {
			bool spaceWait;
			public Wait(Actor actor, bool spaceWait=true) : base(actor) {
				
				this.spaceWait = spaceWait;
				IsWait = true;
			}


			public override void FillNextParameter(ActionTargetTileParameter ap = null) {
				if (spaceWait) {

					UIManager.instance.OnKeyPressed += KeyGotPressed;
				} else {

					CallOnParamsFilled ();
				}
				
			}

			public override void DecorateOption(ActionButtonNode n) {
				n.spriteLabel = IconDispenser.instance.SpriteFromIconName (IconName.HOURGLASS);
			}


			public void KeyGotPressed(KeyCode kc) {
	//			Debug.Log ("wait noticed a kp: " + kc);
				if (kc == KeyCode.Space) {

					UIManager.instance.OnKeyPressed -= KeyGotPressed;
					CallOnParamsFilled ();
				}
			}

			public override void CancelUiListen() {
				UIManager.instance.OnKeyPressed -= KeyGotPressed;
			}


			public override void Perform() {
				CallOnBegan ();
				Debug.LogError ("Waiting you sob!");
				CallOnFinished ();
			}



		}
	}
}