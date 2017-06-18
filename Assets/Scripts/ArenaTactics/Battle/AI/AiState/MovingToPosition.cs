using UnityEngine;
using System.Collections;
using AT.Battle;
using System.Collections.Generic;
using System.Linq;


namespace AT.Battle.AI {


	public class MovingToPosition : AiState {
		
		public ATTile position;
		public MovingToPosition(AiControlledActor actor, AiController aic) : base(actor, aic) {
			OnDidEnter += (s, fromPrevious) => {
				canceled = false;
			};
		}

		public bool canceled;
		public bool asFastAsPossible = true;
		public bool ReachedPosition() {
			return (Canceled() || actor.GetComponent<TileMovement>().occupying == position);
		}

		private bool Canceled() {
			return (canceled || !position.Occupyable ());
		}

		public override Action DecideOnAction ()
		{
			Move move = new Move (actor);
			move.cachedPath = AiController.DoablePath (MapManager.instance.AStarPathForMover(actor, position, true), actor);
			if (move.cachedPath.Count == 0) {
				if (!actor.UsedAction () && asFastAsPossible) {
					return new Dash (actor);
				} else {
					return new Wait (actor);
				}
			}

			return move;
		}



	}

}