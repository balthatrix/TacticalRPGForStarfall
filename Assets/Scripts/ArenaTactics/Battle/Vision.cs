using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace AT { 
	namespace Battle {


		/// <summary>
		/// Vision. represents the methods and data to do with what a character can see.
		/// This is different from collective vision, where characters are able to know about
		/// what the rest of the collective can cumulatively see.
		/// Characters still cannot target things they cannot see themselves however.
		/// </summary>
		public class Vision : MonoBehaviour {


			  //Not really used yet.  Not until fog of war exists.
			List<Actor> charactersInVision;
			static Dictionary<int, Vector3> preCalculatedUnitCircle;
			public Vector3 eyesOffset = new Vector3 (0f, 0f, 0f);

			public static Vector3 DirFromAngle(int angleInDegrees) {
				return new Vector3 (Mathf.Sin (angleInDegrees * Mathf.Deg2Rad), Mathf.Cos (angleInDegrees * Mathf.Deg2Rad));
			}
			public static void PreCalcUnitCircle() {
				preCalculatedUnitCircle = new Dictionary<int,Vector3> ();
				int currentAngle = 0;
				float incrementer = 360f / VISIBLE_ANGLES_OF_CASTING;
				for (int i = 0; i < VISIBLE_ANGLES_OF_CASTING; i++) {
					preCalculatedUnitCircle.Add (i, DirFromAngle (currentAngle));
					currentAngle = (int) Mathf.Round((i + 1) * incrementer);
				}
			}


			public static int VISION_DISTANCE {
				get { 
					return 10;
				}	
			}

			public static int VISIBLE_ANGLES_OF_CASTING {
				get {
					return (int) ((float) VISION_DISTANCE * 10f);
				}
			}

			public Actor Actor {
				get { return GetComponent<Actor> (); }
			}

			void Awake() {
				if (preCalculatedUnitCircle == null)
					PreCalcUnitCircle ();
				
				OnTileVisionGained += (ATTile tile) => {
					tile.AddViewer(Actor);
					if(tile.FirstOccupant != null) {
						if(tile.FirstOccupant.ActorComponent.EnemiesWith(Actor)) {
							DiscoverEnemyActor(tile.FirstOccupant.ActorComponent);
						}
					}
				};

				OnTileVisionLost += (ATTile tile) => {
					tile.RemoveViewer(Actor);
					if(tile.FirstOccupant != null) {
						if(tile.FirstOccupant.ActorComponent.EnemiesWith(Actor)) {
							UndiscoverEnemyActor(tile.FirstOccupant.ActorComponent);
						}
					}
				};


				//This should be on right click instead!
//				OnVisionOfEnemyGained += (enemy) => {
//					GetComponent<TileMovement>().Stop();
//				};



			}

			void Start() {
				previousInVision = new List<ATTile> ();
				GetComponent<TileMovement> ().OnTileOccupancySet += RadiateVision;

				if (BattleManager.instance.actorsInitialized) {
					RadiateVision (GetComponent<TileMovement> ().occupying);

				} else {
					BattleManager.instance.OnActorsInitialized += () => {
						RadiateVision(GetComponent<TileMovement> ().occupying);
					};
				}
				Actor.OnActorKilled += (a) => {
					foreach(ATTile tile in previousInVision) {
						tile.RemoveViewer(Actor);
					}
					RemoveVisionFrom(previousInVision);
				};

			}

			private List<ATTile> previousInVision;
			void RadiateVision(ATTile t) {
	//			Debug.Log ("Radiating vision for " + GetComponent<Actor> ().GetType ());
				List<ATTile> tiles = new List<ATTile>();//GetComponent<TileMovement> ().TilesWithinRange (VISION_DISTANCE, true);
				tiles.Add (t);

	//			Debug.Log ("from out to  " + t.transform.position + " out to "  + preCalculatedUnitCircle [0]);
				for (int i = 0; i < VISIBLE_ANGLES_OF_CASTING; i++) {
					RaycastHit2D[] hits = Physics2D.RaycastAll (
						t.transform.position + eyesOffset, preCalculatedUnitCircle [i], 
						VISION_DISTANCE,
						MapManager.instance.TileLayerMask
					);	

					foreach(RaycastHit2D hit in hits) {
						ATTile tileHit = hit.collider.gameObject.GetComponent<ATTile>();
						if (!tiles.Contains (tileHit))						
							tiles.Add(tileHit);
						if(tileHit.BlocksVision) { // Then stop cursing over the tiles.
							break;
						}		
					}
				}

				RemoveVisionFrom (previousInVision, tiles);

				foreach (ATTile tile in tiles) {
					if (previousInVision.Contains (tile)) //if the previous in vision has the tile, continue
						continue;
					if (OnTileVisionGained != null) {
						OnTileVisionGained (tile);
					}
				}

				previousInVision = tiles;
			}

			public void RemoveVisionFrom(List<ATTile> oldTiles, List<ATTile> addingTo=null) {
				if (oldTiles == null)
					return;
				foreach (ATTile tile in oldTiles) {
					if (addingTo != null && addingTo.Contains (tile)) //if the new in vision has this tile, continue.  
						continue;
					if (OnTileVisionLost != null) {
						OnTileVisionLost (tile);
					}
				}
			}



			public List<Actor> EnemiesInVision {
				get {
					return enemiesInSight;
				}
			}

			public List<Actor> enemiesInSight = new List<Actor>();
			public void DiscoverEnemyActor(Actor enemy) {
				enemiesInSight.Add (enemy);
				enemy.GetComponent<Vision> ().CallGotDiscovered (Actor);
				if (OnVisionOfEnemyGained != null) {
					OnVisionOfEnemyGained (enemy);
				}
//				Debug.Log (Actor.CharSheet.Name + ", a " + Actor.GetType().ToString() +  " is discovering this enemy: " + enemy.CharSheet.Name);

			}
			public void UndiscoverEnemyActor(Actor enemy) {
				enemiesInSight.Remove(enemy);
				enemy.GetComponent<Vision> ().CallGotUndiscovered (Actor);
				if (OnVisionOfEnemyLost!= null) {
					OnVisionOfEnemyLost (enemy);
				}
//				Debug.Log(Actor.CharSheet.Name + ", a " + Actor.GetType().ToString()  + " is undiscovering this enemy: " + enemy.CharSheet.Name);
			}

			public bool CanSee (Actor a) {
				return EnemiesInVision.Contains (a);
			}

			public delegate void DiscoveredByEnemyAcion(Actor enemy);
			public event DiscoveredByEnemyAcion OnDiscoveredByEnemy;
			public void CallGotDiscovered (Actor discoverer) {
				if (OnDiscoveredByEnemy != null) {
					OnDiscoveredByEnemy (discoverer);
				}
			}

			public delegate void UndiscoveredByEnemyAcion(Actor enemy);
			public event UndiscoveredByEnemyAcion OnUndiscoveredByEnemy;
			public void CallGotUndiscovered (Actor discoverer) {
				if (OnUndiscoveredByEnemy != null) {
					OnUndiscoveredByEnemy (discoverer);
				}
			}

			public delegate void VisionOfEnemyGained(Actor enemy);
			public event VisionOfEnemyGained OnVisionOfEnemyGained;


			public delegate void VisionOfEnemyLost(Actor enemy);
			public event VisionOfEnemyLost OnVisionOfEnemyLost;

			public delegate void TileVisionGainedAction(ATTile tile);
			public event TileVisionGainedAction OnTileVisionGained;

			public delegate void TileVisionLostAction(ATTile tile);
			public event TileVisionLostAction OnTileVisionLost;

		}
	}
}