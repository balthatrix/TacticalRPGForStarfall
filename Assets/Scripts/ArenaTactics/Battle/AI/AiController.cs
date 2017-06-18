using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Util.StateMachine;


namespace AT.Battle.AI {
	public class AiController : Controller {
		public AiControlledActor actor;
		public AiController(AiControlledActor actor) {
			this.actor = actor;
			actor.OnActorKilled += Cleanup;
		}

		public void Cleanup(Actor a) {
			CurrentState = null;	
		}

		public Action GetBestAction() {
			AiState current = (AiState)CurrentState;
			return current.DecideOnAction ();
		}


		public  static List<ATTile> BestPathFromTo(Actor fromActor, Actor toActor) {
			return BestPath (PathsFromTiles (OccupyableTilesAdjacentTo(toActor), fromActor), fromActor);
		}


		public static List<ATTile> BestPath(List<List<ATTile>>  options, Actor act) {
			if (options.Count == 0)
				return null;
			int bestCost = PathCost (options[0], act);
			List<ATTile> ret = options [0];

			for (int i = 1; i < options.Count; i++) {
				int cost = PathCost (options [i], act);
				if (cost < bestCost) {
					ret = options [i];
					bestCost = cost;
				}
			}
			return ret;
		}

		//path cost for the ai entity.
		public  static int PathCost(List<ATTile> option, Actor act) {
			int cost = 0;
			foreach (ATTile t in option) {
				cost += t.MoveCostFor (act);
			}
			return cost;
		}



		public static List<List<ATTile>> PathsFromTiles (List<ATTile> tiles, Actor act){
			List<List<ATTile>> ret = new List<List<ATTile>> ();
			for (int i = 0; i < tiles.Count; i++) {
				List<ATTile> option = MapManager.instance.AStarPathForMover (act, tiles[i], true);

				if (option != null) {
					ret.Add (option);
				}
			}

			return ret;
		}

		public static List<ATTile> OccupyableTilesAdjacentTo (Actor act) {
			TileMovement tm = act.GetComponent<TileMovement> ();
			List<ATTile> ret = new List<ATTile> ();

			foreach (ATTile n in tm.occupying.Neighbors()) {
				if(n.Occupyable()) 
					ret.Add (n);
			}


			return ret;
		}

		public static List<ATTile> DoablePath(List<ATTile> best, Actor actor) {
			List<ATTile> ret = new List<ATTile> ();

			int left = actor.MovesLeft ();

			foreach (ATTile t in best) {
				int nextCost = t.MoveCostFor (actor);
				if (left - nextCost < 0)
					break;

				ret.Add (t);
				left -= nextCost;
			}

			while (ret.Count > 0 && !ret.Last ().Occupyable ()) {
				ret.Remove (ret.Last ());
			}


			return ret;
		}

		public static bool TwoActorsAreAdjacent(Actor a1, Actor a2) {
			
			return (a1.TileMovement.occupying.HCostTo(a2.TileMovement.occupying) == 1);
			
		}
	}

}