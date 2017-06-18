using UnityEngine;
using System.Collections;
using AT.Battle;
using System.Collections.Generic;
using System.Linq;


namespace AT.Battle.AI {

	//TODO:
	//This should reall not care about finding target
	//The finding should be it's own ai state?
	public class Camping : AiState {

		public Camping(AiControlledActor actor, AiController aic) : base(actor, aic) {
			
		}

		public bool TargetSighted() {
			return (actor.GetComponent<Vision>().EnemiesInVision.Count > 0);
		}

		public override Action DecideOnAction ()
		{
			return new Wait (actor);
		}

	}

}