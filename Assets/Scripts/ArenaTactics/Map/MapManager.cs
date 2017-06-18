using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AT.Battle;
using System.Linq;

using UnityEngine.SceneManagement;
using AT;

public class MapManager : MonoBehaviour {

	//Don't really do anything with this yet
	public GameObject currentBattleMapPrefab;

	public AT.ATMap currentMapInstance;
	public GameObject tileMask;
	public GameObject tilePrefab;
	public GameObject clearElement;
	public GameObject fogOfWarMask;
	public GameObject onTheGroundItemPrefab;


	public GameObject clearPointer;
//	public ClearOnWalkedBehind clearPointerInstance;
//
//	private IEnumerator UpdateClearPointerPos() {
//
//		while (true) {
//			clearPointerInstance.transform.localPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
//			yield return new WaitForSeconds (.2f);
//		}
//	}
	void Update() {
			
	}

	const int NOT_SET = -2000000;

	[SerializeField]
	public int maxX = NOT_SET;

	[SerializeField]
	public int minX = NOT_SET;

	[SerializeField]
	public int maxY = NOT_SET;

	[SerializeField]
	public int minY = NOT_SET;

	public int CurrentHeight {
		get { return (maxY - minY) + 1; }
	}

	public int CurrentWidth {
		get { return (maxX - minX) + 1; }
	}


	public void SetRenderLayer(SpriteRenderer sr, Vector2 basePosition) {

		sr.sortingOrder = (int) ((CurrentHeight - basePosition.y) * 5); //*5 leaves room for multiple inhabitants on a given tile.
	}


	public void TryCheckInTile(ATTile t) {
		
		StartCoroutine (DelayCheckIn (t));
		//CheckInNow (t);
	} 

	public IEnumerator TransitionToCreation() {
		yield return new WaitForSeconds (10f);
		for (int i = 0; i < instance.transform.childCount; i++)
			Destroy (instance.transform.GetChild (i).gameObject);
		SceneManager.LoadScene ("CharacterCreation");
	}

	private IEnumerator DelayCheckIn(ATTile t) {
		yield return new WaitForSeconds(.2f);
		CheckInNow (t);

	}

	private void CheckInNow(ATTile t) {

		allTilesCached.Add (t);
		int tX = (int) t.transform.position.x;
		int tY = (int) t.transform.position.y;
		//		Debug.Log ("fhiL: : " + tX + " " + tY);
		if (tX < minX || minX == NOT_SET) {
			minX = tX;
		}

		if (tX > maxX || maxX == NOT_SET) {
			maxX = tX;
		}

		if (tY > maxY || maxY == NOT_SET) {
			maxY = tY;
		}

		if (tY < minY || minY == NOT_SET) {
			minY = tY;
		}
	}

	public static MapManager instance;

	private List<ATTile> allTilesCached;

	void Awake() {
		instance = this;


		allTilesCached = new List<ATTile> ();
//		clearPointerInstance = Instantiate (clearPointer).GetComponent<ClearOnWalkedBehind> ();
//		StartCoroutine (UpdateClearPointerPos ());
		//instance.StartCoroutine (TransitionToCreation ());
	}



	public void InstantiateMapPrefab() {
		if(currentBattleMapPrefab != null) {
			GameObject map = Instantiate (GameManager.persistentInstance.currentBattleMapPrefab);
			currentMapInstance = map.GetComponent<AT.ATMap> ();
			currentMapInstance.Initialize ();
		}
	}



	//TODO: invalidate cache when adding a new tile if it ever comes to that.
	public List<ATTile> AllTiles() {
		

		return allTilesCached;
	}

	public LayerMask TileLayerMask {
		get { 
			return 1 << LayerMask.NameToLayer ("Tile");
		}
	}


	public ATTile TileAt(Vector3 pos) {
		Collider2D intersect = Physics2D.OverlapPoint(pos, 1 << LayerMask.NameToLayer("Tile"));

		if (intersect == null) {
			//Debug.Log ("intersect t: " + tag);
			return null;
		} else {
			//Debug.Log ("intersect t: " + tag);
		}
		return intersect.GetComponent<ATTile> ();
	}

	public ATTile TileAt(int x, int y) {
		return TileAt (new Vector3(x, y, transform.position.z));
	}


	public ATTile TileUnderMouse() {
		return TileAt (WorldMousePosition());
	}

	public Vector3 WorldMousePosition() {
		return Camera.main.ScreenToWorldPoint (Input.mousePosition);
	}
		


	//uses a* algorithm to find the shortest path from start to finish
	public List<ATTile> AStarPath(ATTile start, ATTile finish) {
		ATTile startNode = start;
		ATTile targetNode = finish;

		List<ATTile> openSet = new List<ATTile> ();
		List<ATTile> closedSet = new List<ATTile> ();

		openSet.Add (startNode);
		while (openSet.Count > 0) {
			ATTile currentNode = openSet [0];
			foreach (ATTile t in openSet) {
				if (t.FCost() < currentNode.FCost() || 
					(t.FCost() == currentNode.FCost() && t.DistanceSquared(targetNode) < currentNode.DistanceSquared(targetNode))) {
					currentNode = t;
				}
			}

			openSet.Remove (currentNode);
			closedSet.Add (currentNode);

			if (currentNode == targetNode) {
				return startNode.TracePathTo (targetNode);
			}


			foreach (ATTile n in currentNode.Neighbors()) {
				if (!n.Occupyable() || closedSet.Contains (n)) {
					continue;
				}

				//TODO: I think this is where a* needs to incorporate movecost of the target tile.
				int newMoveCostToNeighbor = (int) currentNode.gCost + 1;
				if (newMoveCostToNeighbor < n.gCost || !openSet.Contains (n)) {
					n.gCost = newMoveCostToNeighbor;
					n.hCost = n.HCostTo (targetNode);
					n.pathParent = currentNode;

					if (!openSet.Contains (n))
						openSet.Add (n);
				}
			}
		}
		return null;
	}

	//uses a* algorithm to find the shortest path from start to finish
	public List<ATTile> AStarPathForMover(Actor actor, ATTile finish, bool ignoreMovesLeft = false) {
		if (actor == null) {
			Debug.Log ("actor in null!!!??: " + actor.CharSheet.Name);
			
		}
		ATTile startNode = actor.GetComponent<TileMovement>().occupying;
		ATTile targetNode = finish;

		List<ATTile> openSet = new List<ATTile> ();
		List<ATTile> closedSet = new List<ATTile> ();

		startNode.gCost = 0;
		openSet.Add (startNode);
		while (openSet.Count > 0) {
			ATTile currentNode = openSet [0];
			foreach (ATTile t in openSet) {
				if (t.FCost() < currentNode.FCost() || 
					(t.FCost() == currentNode.FCost() && t.DistanceSquared(targetNode) < currentNode.DistanceSquared(targetNode))) {
					currentNode = t;
				}
			}

			openSet.Remove (currentNode);
			closedSet.Add (currentNode);

			if (currentNode == targetNode) {
				return startNode.TracePathTo (targetNode);
			}


			foreach (ATTile n in currentNode.Neighbors()) {
				if (!n.Travellable(actor) || closedSet.Contains (n)) {
					continue;
				}

				int newMoveCostToNeighbor = (int) currentNode.gCost + n.MoveCostFor(actor);
				if (newMoveCostToNeighbor > actor.MovesLeft () && !ignoreMovesLeft) {
					continue;
				}

				if (ignoreMovesLeft) {
					if (newMoveCostToNeighbor < n.gCost || !openSet.Contains (n)) {
						n.gCost = newMoveCostToNeighbor;
						n.hCost = n.HCostTo (targetNode);
						n.pathParent = currentNode;

						if (!openSet.Contains (n))
							openSet.Add (n);
					}
				} else {
					if ((newMoveCostToNeighbor < n.gCost || !openSet.Contains (n)) &&
						actor.MovesLeft () >= newMoveCostToNeighbor) {
						n.gCost = newMoveCostToNeighbor;
						n.hCost = n.HCostTo (targetNode);
						n.pathParent = currentNode;

						if (!openSet.Contains (n))
							openSet.Add (n);
					}
				}

			}
		}
		return null;
	}


	//uses a* algorithm to find the shortest path from start to finish, counting reacher tiles as unwalkable...
	public List<ATTile> AStarPathForMoverCountReachersUnwalkable(Actor actor, ATTile finish) {
		ATTile startNode = actor.GetComponent<TileMovement>().occupying;
		ATTile targetNode = finish;

		List<ATTile> openSet = new List<ATTile> ();
		List<ATTile> closedSet = new List<ATTile> ();

		startNode.gCost = 0;
		openSet.Add (startNode);
		while (openSet.Count > 0) {
			ATTile currentNode = openSet [0];
			foreach (ATTile t in openSet) {
				if (t.FCost() < currentNode.FCost() || 
					(t.FCost() == currentNode.FCost() && t.DistanceSquared(targetNode) < currentNode.DistanceSquared(targetNode))) {
					currentNode = t;
				}
			}

			openSet.Remove (currentNode);
			closedSet.Add (currentNode);

			if (currentNode == targetNode) {
				return startNode.TracePathTo (targetNode);
			}


			foreach (ATTile n in currentNode.Neighbors()) {
				if (!n.Travellable(actor) || (n.HasEnemyReachers(actor) && n != targetNode) || closedSet.Contains (n)) {
					continue;
				}

				//TODO: I think this is where a* needs to incorporate movecost of the target tile.
				int newMoveCostToNeighbor = (int) currentNode.gCost + n.MoveCostFor(actor);
				if (newMoveCostToNeighbor > actor.MovesLeft ()) {
					continue;
				}

				if ((newMoveCostToNeighbor < n.gCost || !openSet.Contains (n)) &&
					actor.MovesLeft () >= newMoveCostToNeighbor) {
					n.gCost = newMoveCostToNeighbor;
					n.hCost = n.HCostTo (targetNode);
					n.pathParent = currentNode;

					if (!openSet.Contains (n))
						openSet.Add (n);
				}
			}
		}
		return null;
	}

	public List<ATTile> AStarPathForMoverPreferNoReachers(Actor actor, ATTile finish) {
		List<ATTile> path = AStarPathForMover (actor, finish);
		if (path == null || path.Count == 0) {
			return null;
		}

		bool TrySecond = actor.HasEnemyReachersIn(path.GetRange(0,path.Count-1)) ;
		if (TrySecond) {
			List<ATTile> path2 = AStarPathForMoverCountReachersUnwalkable (actor, finish);
			if (path2 != null && path2.Count <= path.Count) {
				path = path2;
			}

		}

			

		return path;
	}



	public void ColorTiles(List<ATTile> ts, Color c) {

		foreach (ATTile t in ts) {
			t.PushShade (c);
		}
	}

	public void UnColorTiles(List<ATTile> ts) {
		if (ts == null)
			return;
		foreach (ATTile t in ts) {
			t.PopShadeStack();
		}
	}

	public void DebugTiles(List<ATTile> ts) {
		foreach (ATTile t in ts) {
			Debug.Log (t.ToString ());
		}
	}

	public delegate void MapInitializedAction(AT.ATMap map);
	public event MapInitializedAction OnMapInitialized;

	/// <summary>
	/// A battle map was initialized in the scene.  This may break if multiple maps are created at some point?
	/// </summary>
	/// <param name="map">Map.</param>
	public void MapInitialized(AT.ATMap map) {
		currentMapInstance = map;
		if (OnMapInitialized != null) {
			OnMapInitialized (map);
		}
	}
}
