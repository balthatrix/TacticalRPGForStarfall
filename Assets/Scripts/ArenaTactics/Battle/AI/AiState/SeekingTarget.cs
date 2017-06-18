using UnityEngine;
using System.Collections;
using AT.Battle;
using System.Collections.Generic;
using System.Linq;

namespace AT.Battle.AI {

	/// <summary>
	/// Seeking target finds best position, and begins character on it's path.  
	/// TODO: parameterize this based on the distant kept from the character
	/// ...this way characters with reach will keep proper distance.
	/// </summary>
	public class SeekingTarget : AiState {

		public Actor target;
		public ATTile bestPosition;

		public SeekingTarget(AiControlledActor actor, AiController aic) : base(actor, aic) {
			OnDidEnter += (s, fromPrevious) => {
				bestPosition = null;
			};
		}

		public ATTile BestPosition() {
			if (RightByTarget ()) {
				return null;
			}
			List<ATTile> bp = BestPathTo (target);
			if (bp == null) {
				return null;
			}
			return bp.Last ();
		}

		public bool NoPathToTarget(){
			if (target.TileMovement.TilesWithinRange (1).Where((t) => t.Occupyable()).Count() == 0) {
				Debug.LogError ("no path to target sonny!");
				return true;
			}

			List<ATTile> bp = BestPathTo (target);
			if (bp == null) {
				return true;
			}

			return false;
		}

		public bool RightByTarget() {
			return (actor.TileMovement.occupying.HCostTo(target.TileMovement.occupying) == 1);
		}


		private List<ATTile> BestPathTo(Actor enemy) {
//			return BestPath (PathsFromTiles (TilesAdjacentTo (enemy)));
			return AiController.BestPath(
				AiController.PathsFromTiles(
					AiController.OccupyableTilesAdjacentTo(enemy)
					, actor)
				, actor);
		}

		public override Action DecideOnAction ()
		{

			return new Wait (actor);
		}

	}

}