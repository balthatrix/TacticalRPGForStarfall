using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AT.Battle;
using AT;
/*
*  Assumes each animation is one full cycle per tile unit moved.
*/
[RequireComponent (typeof(CharacterWalker))]
public class TileMovement : MonoBehaviour {


	public Transform avatar;


	private List<ATTile> lastReachTiles;


	public ATTile occupying;

	public List<ATTile> TilesWithinAttackReach (){

		return occupying.TilesWithinRange (CharSheet.Reach ());
	}

	public List<ATTile> TilesWithinRange(int range=-1, bool ignoreMovability = false) {
		List<ATTile> tiles = new List<ATTile> ();
		Actor actor = GetComponent<Actor> ();

		if(range == -1)
			range = actor.MovesLeft ();
		
		Vector3 bottomLeftOfRange = new Vector3 (occupying.transform.position.x - range,
			occupying.transform.position.y - range);
		int checks = (range * 2) + 1;
		for(int i = 0; i < checks; i++) {
			for(int j = 0; j < checks; j++) {
				ATTile prospect = MapManager.instance.TileAt (bottomLeftOfRange + new Vector3(i, j, occupying.transform.position.z));
				if (prospect == null) {
					//Debug.Log ("prospect is null");
					continue;
				}
				if (!ignoreMovability) {
					if (!prospect.Occupyable ()) {
						//Debug.Log ("prospect is not occupyable");
						continue;
					}
					if (prospect == occupying) {
						//Debug.Log ("prospect is occ tile");
						continue;
					}
					if (MapManager.instance.AStarPathForMover (actor, prospect) == null) {
						//Debug.Log ("no path to tile: " + prospect.name + ": " +  prospect.transform.position);
						continue;
					}
					tiles.Add (prospect);
				} else {
					if (prospect.HCostTo (occupying) <= range)
						tiles.Add (prospect);
				}

			}
		}
		return tiles;
	}





	public void CallOnWalkedOutOfTile(ATTile current, ATTile dest) {
		if (OnWalkedOutOfTile != null) {
			OnWalkedOutOfTile (current, dest);
		}
	}

	public delegate void MovedOutOfTileAction(ATTile current, ATTile dest);
	public event MovedOutOfTileAction OnWalkedOutOfTile;

	private Actor actor;
	public Actor ActorComponent {
		get { return actor; }
	}

	public CharacterWalker characterWalker;
	// Use this for initialization
	void Start () {
		if (avatar == null) {
			avatar = transform;
		}

		characterWalker = GetComponent<CharacterWalker> ();
		characterWalker.OnBeganWalking += WalkingOutOfTile;
		characterWalker.OnContinuedWalking += WalkingOutOfTile;

		OnWalkedOutOfTile += SetOccupyingTile;
		


		actor = GetComponent<Actor> ();
		ActorComponent.OnActorKilled += OnKilledCleanup;

		SetOccupyingTile (null, MapManager.instance.TileAt(avatar.position));
	}

	void TellBattleManagerToEmitMoved(ATTile fromTile, ATTile toTile) {
		BattleManager.instance.EmitActorMovedEvent (this.ActorComponent, fromTile, toTile);
	}

	void OnKilledCleanup(Actor a) {
		
		occupying.RemoveOccupant (this, null);
//		foreach( Tile t in TilesWithinAttackReach()) {
//			Debug.Log ("Cleaning up reach " + t.name);
//			t.RemoveReacher (ActorComponent);
//		}
	}


	public void SetOccupyingTile(ATTile from, ATTile destination) {
//		Debug.Log ("Setting occupaancy for " + ActorComponent.GetType());
//		Debug.Log ("Actors that sees dest: " + destination.actorsThatSeeThisTile.Count);
		if (from != null) {
			if (from.AllOccupants.Contains(this)) {
				from.RemoveOccupant(this, destination);
			}
		}

		//Fixes an odd bug where this function is called after moving into a tile, 
		//where the moving creature was killed by a reaction.  
		//The new tile would set reachers
		if (!ActorComponent.Dying) {
			occupying = destination;
			CycleReachTo (destination);
			destination.AddOccupant (this, from);
		}

		if (OnTileOccupancySet != null) {
			OnTileOccupancySet (destination);
		} 
	}

	public delegate void TileOccupancySetAction(ATTile tile);
	public event TileOccupancySetAction OnTileOccupancySet;

	private void CycleReachTo(ATTile newOcc) {
		if (lastReachTiles != null) {
			RemoveReach (lastReachTiles);
		}

		List<ATTile> withinReach = TilesWithinAttackReach();
		foreach (ATTile t in withinReach) {
			t.AddReacher (GetComponent<Actor>());
		}

		lastReachTiles = withinReach;
	}

	public void RemoveReach(List<ATTile> tiles) {
		foreach (ATTile t in tiles) {
			t.RemoveReacher (GetComponent<Actor>());
		}
	}



	public void Walk(List<string> directions) {
		foreach (string direction in directions) {
			characterWalker.AddToCue (direction);
		}
	}

	public void Stop() {
//		Debug.Log ("interuping!");
		characterWalker.Interrupt ();
	}




	void WalkingOutOfTile(string inDirection) {
		

		if (OnWalkedOutOfTile != null) {
			ATTile dest = null;
			ATTile now = MapManager.instance.TileAt (avatar.position);

			switch (inDirection) {
				case "WalkUp":
					dest = now.Up ();
					break;
				case "WalkDown":
					dest = now.Down ();
					break;
				case "WalkLeft":
					dest = now.Left ();
					break;
				case "WalkRight":
					dest = now.Right ();
					break;
			}
			OnWalkedOutOfTile (now, dest);
		}
	}





	public bool TileHasEnemyReach(ATTile t) {
		foreach (Actor a in t.Reachers) {
			if (a.EnemiesWith (this.ActorComponent) && !a.UsedReaction())
				return true;
		}
		return false;
	}

	public AT.Character.Sheet CharSheet {
		get { 
			return GetComponent<Actor> ().CharSheet;
		}
	}



	public List<string> DirectionsFrom(List<ATTile> aStarPath) {
		List<string> ret = new List<string> ();
		Vector3 startPos = actor.transform.position;
		foreach (ATTile t in aStarPath) {
			Vector3 diff = t.transform.position - startPos;

			int absDiffX = (int) Mathf.Round(Mathf.Abs (diff.x));

			if (absDiffX > 0) {
				if (diff.x > 0f) {
					ret.Add ("WalkRight");
				} else {
					ret.Add ("WalkLeft");
				}
			} else {
				if (diff.y > 0f) {
					ret.Add ("WalkUp");
				} else {
					ret.Add ("WalkDown");
				}
			}

			startPos = t.transform.position;

		}

		return ret;
	}

}

