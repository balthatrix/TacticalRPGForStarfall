using UnityEngine;
using System.Collections;
using AT.Battle;
using System.Collections.Generic;
using System.Linq;


namespace AT.Battle.AI {

	/// <summary>
	/// Finding best target based on the shortest path.  
	/// TODO: should parameterize this.  Best target may mean different things to different ais
	/// For example: maybe the best target is the weakest target, or the best target is the healer/mage
	/// </summary>
	public class FindingBestTarget : AiState {

		Actor bestTarget;

		public FindingBestTarget(AiControlledActor actor, AiController aic) : base(actor, aic) {
			OnDidEnter += (s, fromPrevious) => {
				CalculateBestTargetAndPath();
				actor.OnTurnBegan += RecalcOnTurnBegan;
			};

			OnWillExit += (s, toDestination) => {
				actor.OnTurnBegan -= RecalcOnTurnBegan;
			};

		}

		public void RecalcOnTurnBegan(Actor s) {
			CalculateBestTargetAndPath();
		}

		public bool NoTargetInSight() {
			return (actor.GetComponent<Vision> ().EnemiesInVision.Count == 0);
		}

		private bool noPathToBestTarget = false;
		public bool NoPathToBestTarget() {
			return noPathToBestTarget;
		}

		public Actor BestTarget() {
			return bestTarget;
		}
			
		//Exaustively checks all enemies in vision and sets a best path based on paths to their neighboring tiles.
		//This is search heavy.  
		private void CalculateBestTargetAndPath() {
			bestTarget = null;
			List<Actor> enemies = actor.GetComponent<Vision>().EnemiesInVision;
//			Debug.Log ("enemies size in calc: " + enemies.Count);
			if (enemies.Count <= 0) {
				return;
			}

			Actor bestT = enemies[0];
			

			//Without this, actors see a path other than when already adjacent to enemy.
			for (int i = 0; i < enemies.Count; i++) {
				if (AiController.TwoActorsAreAdjacent (enemies[i], actor) && !enemies[i].Dying) {
					bestTarget = enemies [i];
					return;
				}
			}

			List<ATTile> bestP = AiController.BestPathFromTo (actor, bestT);
			for (int i = 1; i < enemies.Count; i++) {
				if (enemies [i].Dying)
					continue;
				
				List<ATTile> proposed = AiController.BestPathFromTo (actor, enemies [i]);
				if (bestP == null && proposed != null) {
					bestP = proposed;
					bestT = enemies [i];
				} else 	if (proposed != null && bestP != null && AiController.PathCost (bestP, actor) > AiController.PathCost (proposed, actor)) {
					bestP = proposed;
					bestT = enemies [i];
				}
			}

//			Debug.LogError ("Calculated best target as: " + bestT.CharSheet.Name);
			if (bestP != null) {
				noPathToBestTarget = false;
				bestTarget = bestT;
			} else {
				noPathToBestTarget = true;
//				Debug.LogError ("no path to any targets!");
			}
		}

		public override Action DecideOnAction ()
		{
			return new Wait (actor);
		}
	}
}