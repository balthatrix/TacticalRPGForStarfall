using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Util.StateMachine;
using System.Linq;
namespace AT.Battle  {
	public class RoundBegin : PhaseState {


		public RoundBegin(Controller controler) : base (controler) {
			OnDidEnter += Entered;
		}

		public int currentRoundNum = 0;


		public  List<Actor> actorsLeftThisRound;

		public void Entered (State self, State previous) {
			currentRoundNum++;

			Debug.Log ("==ROUND BEGIN " + currentRoundNum + " PHASE==");
			UIManager.instance.roundText.text = "Round " + currentRoundNum;
			UIManager.instance.roundPanel.gameObject.SetActive(true);



			//Fresh fetch of all actors in the battle....
			phaseController.battleManager.ResetActionsThisRound ();
			actorsLeftThisRound = phaseController.battleManager.AllActors();
//			Debug.Log ("actors left: " + actorsLeftThisRound.Count);
			foreach (Actor a in actorsLeftThisRound) {
				
				a.OnActorKilled += RemoveDeadActorFromLeft;
			}

			phaseController.battleManager.StartCoroutine(startActorTurn());
		}

		IEnumerator startActorTurn() {
			yield return new WaitForSeconds (1.0f);
			UIManager.instance.roundPanel.gameObject.SetActive(false);

			ToCharacterTurnBeginPhase ();
		}


		public void RemoveDeadActorFromLeft(Actor a) {
			if (actorsLeftThisRound.Contains (a)) {
				actorsLeftThisRound.Remove (a);
			}
			a.OnActorKilled -= RemoveDeadActorFromLeft;
		}

		public List<Actor> AllSortedActors() {
			
			List<Actor> ret = actorsLeftThisRound.OrderByDescending ((actor) => {
//				Debug.Log(actor.CharSheet.Name + " here : " + actor.CurrentInitiative.ModifiedCurrent);
				return actor.CurrentInitiative.ModifiedCurrent;
				}).ToList();

//			foreach (Actor act in ret) {
//				Debug.Log(act.CharSheet.Name + " will return  : " + act.CurrentInitiative.ModifiedCurrent);
//			}		
			return ret;
		}


		public void ToCharacterTurnBeginPhase() {
			phaseController.characterTurnBegin.actor = AllSortedActors()[0];
//			Debug.Log ("new one: " + phaseController.characterTurnBegin.actor.CharSheet.Name);
			phaseController.SwitchState (phaseController.characterTurnBegin);
		}

	}

}