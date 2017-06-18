using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AT.Battle;
using Util.StateMachine;

public class TurnCue : MonoBehaviour {
	public GameObject playerCueElementTemplate;
	public GameObject enemyCueElementTemplate;
	public GameObject roundPanel;

	private Dictionary<Actor, GameObject> actorsToCuePanels = new Dictionary<Actor, GameObject>();

	// Use this for initialization
	void Start () {
		if (BattleManager.instance.battleInitialized) {
			BattleManager.instance.battlePhases.OnStateChanged += Init;	
		} else {
			BattleManager.instance.OnBattleInitialized += () => {
				BattleManager.instance.battlePhases.OnStateChanged += Init;	
			};
		}


		playerCueElementTemplate.SetActive (false);
		enemyCueElementTemplate.SetActive (false);
	}

	void Init(State fromS, State toS) {
		if (toS is CharacterTurnBegin) {
			foreach(Actor actor in BattleManager.instance.battlePhases.roundBegin.AllSortedActors()) {
				GameObject cueElem = null;
				switch (BattleManager.instance.SideFor (actor)) {
				case BattleManager.Side.PLAYER:
					cueElem = Instantiate (playerCueElementTemplate);
					cueElem.transform.SetParent(playerCueElementTemplate.transform.parent);
					cueElem.SetActive (true);
					break;
				case BattleManager.Side.ENEMY:
					cueElem = Instantiate (enemyCueElementTemplate);
					cueElem.transform.SetParent (enemyCueElementTemplate.transform.parent);
					actor.GetComponent<Vision> ().OnDiscoveredByEnemy += (Actor player) => {
						cueElem.SetActive (true);
					};
					actor.GetComponent<Vision> ().OnUndiscoveredByEnemy += (Actor player) => {
						if (!actor.IsSeenByAPlayer) {
							cueElem.SetActive (false);
						}
					};
					if (actor.IsSeenByAPlayer) {
						cueElem.SetActive (true);
					}
					break;
				}

				actor.OnActorKilled += (Actor s) => {
					Destroy(cueElem.gameObject);
				};
				actorsToCuePanels.Add (actor, cueElem);
				cueElem.GetComponent<OptButton> ().optText.text = actor.CharSheet.Name;
				cueElem.GetComponent<OptButton>().OnOptMousedOver += (OptButton button) => {
					UIManager.instance.cameraController.GoLockMode();
					UIManager.instance.cameraController.LockOn(actor.transform);
				};
			}
			ToTheBack (roundPanel.transform);
			SetRound (1);
			BattleManager.instance.battlePhases.OnStateChanged -= Init;
			BattleManager.instance.battlePhases.OnStateChanged += ShiftCue;
		}

	}

	void ShiftCue(State prv, State toS) {
		if (toS is CharacterTurnEnd) {
			CharacterTurnEnd turnEnd = toS as CharacterTurnEnd;
			GameObject cuePane = null;
			if (actorsToCuePanels.TryGetValue (turnEnd.actor, out cuePane)) {
				ToTheBack (cuePane.transform);
			}
		} else if (toS is CharacterTurnBegin && prv is RoundBegin) {
			NextRound ();
		}
	}

	void NextRound() {
		ToTheBack (roundPanel.transform);
		SetRound (BattleManager.instance.battlePhases.roundBegin.currentRoundNum);
	}

	void ToTheBack(Transform t) {
		Transform temp = t.parent;
		t.SetParent (null);
		t.SetParent (temp);
	}

	void SetRound(int round) {
		roundPanel.transform.GetChild (0).GetComponent<Text> ().text = string.Format("Round {0}", round);
	}


}
