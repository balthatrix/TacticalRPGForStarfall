using UnityEngine;
using System.Collections;
using AT.Battle;
using System.Collections.Generic;
using System.Linq;

namespace AT.Battle {
	public class ActionTargetTileParameter {
		
		public Action action;
		public int range;

		public string Prompt { get; set; }
		public ATTile chosenTile;

		public delegate List<ATTile> FilterTargetTileAction(List<ATTile> targets, Actor actor);
		public List<FilterTargetTileAction> targetTileFilters;


		public ActionTargetTileParameter(Action action, int range = 1) {
			this.action = action;
			this.range = range;
			targetTileFilters = new List<FilterTargetTileAction> ();
			Prompt = "Choose a target";
		}

		public void Choose(ATTile t) {
			chosenTile = t;
		}

		public List<ATTile> PotentialTargets() {
			List<ATTile> ret = action.actor.TileMovement.TilesWithinRange (range, true);

			foreach (FilterTargetTileAction filter in targetTileFilters) {
				ret = filter (ret, action.actor);
			}



			return ret;
		}

		public bool CanBeFilled() {
			return PotentialTargets ().Count > 0;
		}

		public void Reset() {
			if (listening)
				StopListen ();
			chosenTile = null;
		}

		private bool listening = false;
		private List<ATTile> potentialTargetsCached;
		public void Listen() {
			if (listening) {
				Debug.LogError ("already listening in for tile events ");
				return;
			}

			listening = true;
			potentialTargetsCached = PotentialTargets ();
			if (OnListen != null)
				OnListen (potentialTargetsCached.ToList());
			
			foreach (ATTile t in potentialTargetsCached) {

				t.OnClicked += TargetClicked;
				t.OnMouseOverEvent += PotentialMousedOver;
				t.OnMouseOutEvent += PotentialMousedOut;
			}
		}

		public void StopListen() {
			if (!listening) {
				//Debug.LogError ("not listening in for tile events ");
				return;
			}

			listening = false;
			if (OnStopListen != null)
				OnStopListen (potentialTargetsCached.ToList());
			potentialTargetsCached = null;
			

			foreach (ATTile t in PotentialTargets()) {
				t.OnClicked -= TargetClicked;
				t.OnMouseOverEvent -= PotentialMousedOver;
				t.OnMouseOutEvent -= PotentialMousedOut;
			}
		}

		private void TargetClicked(ATTile t) {


			chosenTile = t;

			if (OnTargetTileChosen != null)
				OnTargetTileChosen (this);
		}


		private void PotentialMousedOver(ATTile t) {
			if (OnPotentialTargetMousedOver != null)
				OnPotentialTargetMousedOver (t);
		}


		private void PotentialMousedOut(ATTile t) {
			if (OnPotentialTargetMousedOut != null)
				OnPotentialTargetMousedOut (t);
		}

		public delegate void TargetTileListenAction(List<ATTile> potentialTargets);
		public event TargetTileListenAction OnListen;

		public delegate void TargetTileStopListenAction(List<ATTile> potentialTargets);
		public event TargetTileStopListenAction OnStopListen;

		public delegate void TargetTileMouseOverAction(ATTile t); //used by move to make a path to the target/etc...
		public event TargetTileMouseOverAction OnPotentialTargetMousedOver;

		public delegate void TargetTileMouseOutAction(ATTile t); //used by move to make a path to the target/etc...
		public event TargetTileMouseOutAction OnPotentialTargetMousedOut;

		public delegate void TargetTileChosenAction(ActionTargetTileParameter self);
		public event TargetTileChosenAction OnTargetTileChosen;



		/// <summary>
		/// filters down all tiles by whether or not they contain a visible enemy...
		/// </summary>
		/// <returns>The enemies.</returns>
		/// <param name="potentials">Potentials.</param>
		/// <param name="actor">Actor.</param>
		public static List<ATTile> HaveEnemies(List<ATTile> potentials, Actor actor) {
			List<ATTile> ret = new List<ATTile> ();
			foreach (ATTile t in potentials) {
				
				if (t.FirstOccupant != null && t.FirstOccupant.ActorComponent.EnemiesWith (actor)) {
					ret.Add (t);
				}
			}
			return ret;
		}

		public static List<ATTile> OnlyMovable(List<ATTile> potentials, Actor actor) {
			List<ATTile> ret = new List<ATTile> ();
			foreach (ATTile t in potentials) {
				if (!t.Occupyable ()) {
					//Debug.Log ("prospect is not occupyable");
					continue;
				}
				if ( t == actor.TileMovement.occupying) {
					//Debug.Log ("prospect is occ tile");
					continue;
				}
				if (MapManager.instance.AStarPathForMover (actor,  t) == null) {
					//Debug.Log ("no path to tile: " + prospect.name + ": " +  prospect.transform.position);
					continue;
				}
				ret.Add (t);
			}
			return ret;
		}

		/// <summary>
		/// filters down all tiles by whether or not they contain a visible friendly...
		/// </summary>
		/// <returns>The enemies.</returns>
		/// <param name="potentials">Potentials.</param>
		/// <param name="actor">Actor.</param>
		public static List<ATTile> HasFriendlyCharacter(List<ATTile> potentials, Actor actor) {
			List<ATTile> ret = new List<ATTile> ();
			foreach (ATTile t in potentials) {
				if (t.FirstOccupant != null && t.FirstOccupant.ActorComponent == actor) {
					continue;
				}

				if (t.FirstOccupant != null && !t.FirstOccupant.ActorComponent.EnemiesWith (actor)) {
					ret.Add (t);
				}
			}
			return ret;
		}

	}
}