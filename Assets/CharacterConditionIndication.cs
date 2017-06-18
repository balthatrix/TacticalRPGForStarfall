using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AT.Battle {
	public class CharacterConditionIndication : MonoBehaviour {
		public Text hpText;
		public SpriteRenderer heartIndicator;

		Actor Actor {
			get { return GetComponent<Actor> (); }
		}

		// Use this for initialization
		void Start () {
			Actor.OnTurnBegan += (Actor s) => {
				
				BattleManager.Side side = BattleManager.instance.SideFor(s);
				List<Actor> teamMates = BattleManager.instance.ActorsOnSide(side);
				if(side == BattleManager.Side.PLAYER) {
					foreach(Actor actor in teamMates) {
						actor.GetComponent<CharacterConditionIndication>().Show();
					}
				} else {
					//set hearts...
					foreach(Actor actor in teamMates) {
						actor.GetComponent<CharacterConditionIndication>().ShowJustImage();
					}
				}
				BattleManager.instance.AllActors();
			}; 

			Actor.OnWillPerform += (Action s) => {
				BattleManager.Side side = BattleManager.instance.SideFor(s.actor);
				List<Actor> teamMates = BattleManager.instance.ActorsOnSide(side);
				foreach(Actor actor in teamMates) {
					actor.GetComponent<CharacterConditionIndication>().Hide();
				}
			}; 
			Hide ();
		}

		public void Hide() {
			hpText.text = "";

			heartIndicator.sprite = null;
		}

		public void Show() {
			hpText.text = Actor.CharSheet.HitPoints + "/" + Actor.CharSheet.HitPointsGauge.ModifiedMax;
			if (Actor.CharSheet.HitPointRatio < .4f) {
				hpText.color = new Color (1f, 0f, 0f, 1f);
			} else if (Actor.CharSheet.HitPointRatio < .7f) {
				hpText.color = new Color (1f, 1f, 0f, 1f);
			} else {
				hpText.color = new Color (0f, 1f, 0f, 1f);
			}

			heartIndicator.sprite = IconDispenser.instance.SpriteFromIconName(AT.Character.IconName.HEART);
			heartIndicator.color = new Color (heartIndicator.color.r, heartIndicator.color.g, heartIndicator.color.b, Actor.CharSheet.HitPointRatio);


		}

		public void ShowJustImage() {
			
			heartIndicator.sprite = IconDispenser.instance.SpriteFromIconName(AT.Character.IconName.HEART);
			heartIndicator.color = new Color (heartIndicator.color.r, heartIndicator.color.g, heartIndicator.color.b, Actor.CharSheet.HitPointRatio + .25f);


		}


	}

}