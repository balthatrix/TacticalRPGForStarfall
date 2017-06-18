using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using AT.Character;

namespace AT {
	
	namespace Battle {  
		public class Move : Action {
			public Move(Actor actor) : base(actor) {

				cachedAoopLines = new List<LineRenderer> ();
				ActionTargetTileParameter placeToMove = new ActionTargetTileParameter(this, actor.MovesLeft());

				placeToMove.OnListen += (List<ATTile> potentialTargets) => {
					
	//				MapManager.instance.ColorTiles (potentialTargets, new Color(0f, 0f, 1f, .3f));
					foreach(ATTile t in potentialTargets) {
						//color blue
						if (actor.GetComponent<TileMovement> ().TileHasEnemyReach (t)) {
							t.PushShade(new Color (1f, 1f, .5f, .3f));

						} else {
							t.PushShade(new Color(0f, 0f, 1f, .30f));
						}
					}

	//				if(MapManager.instance.TileUnderMouse() != null && !UIManager.instance.OverUI) {
	//					MapManager.instance.StartCoroutine(DelayColorOverTiles());
	//				}
				};


				placeToMove.OnStopListen += (List<ATTile> potentialTargets) => {
					
					if(cachedPath != null) {
						MapManager.instance.UnColorTiles (cachedPath);
					}
						
					MapManager.instance.UnColorTiles (potentialTargets);
					DestroyAOOPLines ();
	//				foreach(Tile t in potentialTargets) {
	//					Debug.Log("stopped listening: " + t.name);
	//				}	
				};


				//color path grey on mouse over
				placeToMove.OnPotentialTargetMousedOver += ColorTilesMousedOver;

				placeToMove.OnPotentialTargetMousedOut += (t) => {
					if (cachedPath != null) {
						MapManager.instance.UnColorTiles (cachedPath);
					}
					DestroyAOOPLines ();
				};


				placeToMove.targetTileFilters.Add (ActionTargetTileParameter.OnlyMovable);


				actionTargetParameters.Add(placeToMove);
			}


			public override void DecorateOption(ActionButtonNode n) {
				n.spriteLabel = IconDispenser.instance.SpriteFromIconName (IconName.MOVE);
			}

			private void ColorTilesMousedOver (ATTile t) {
				cachedPath = MapManager.instance.AStarPathForMoverPreferNoReachers (actor, t);
				if (cachedPath != null) {
					MapManager.instance.ColorTiles (cachedPath, new Color (1f, 1f, 1f, .5f));
					List<ATTile> fromP = cachedPath.ToList ();
					fromP.Insert (0, actor.GetComponent<TileMovement> ().occupying);
					DrawAOOTLines (fromP);
				}
			}


			public List<ATTile> cachedPath;

			private List<LineRenderer> cachedAoopLines;


			public void FinishedMove() {
				
				actor.GetComponent<CharacterWalker> ().OnEndedWalking -= FinishedMove;

				UIManager.instance.cameraController.CancelLock ();
				CallOnFinished ();
			}

			public override bool IsMove {
				get  { return true; }
			}

			public override void Perform() {
				CallOnBegan ();
	//			Debug.Log ("performing the action!");
				UIManager.instance.cameraController.LockOn (actor.transform);
				if(cachedPath == null) //theres an odd edge case where this can happen.
					cachedPath = MapManager.instance.AStarPathForMoverPreferNoReachers (actor, MapManager.instance.TileUnderMouse ());

				if (!actor.IsOnPlayerSide) {
					if (actor.IsSeenByAPlayer) {
//						Debug.LogError ("going anim");
						AnimatedMovePerform (cachedPath);
					} else {
//						Debug.LogError ("going non-animmmmmmmmmmmmmmmmmmmmm");
						NonAnimatedMovePerform (cachedPath);
					}
				} else {
//					Debug.LogError ("going anim");
					AnimatedMovePerform (cachedPath);
				}
			}

			private void AnimatedMovePerform(List<ATTile> thePath) {

				TileMovement tm = actor.GetComponent<TileMovement> ();

				List<string> path = DirectionsFrom (cachedPath);
				if (path.Count == 0) {
					FinishedMove ();
				} else {
					tm.Walk (path);


					actor.GetComponent<CharacterWalker> ().OnEndedWalking += FinishedMove;
				}
			}


			private bool discoveryFlagged;
			private void NonAnimatedMovePerform(List<ATTile> thePath) {
				actor.GetComponent<Vision> ().OnDiscoveredByEnemy += DiscoveredByEnemyDuringNonAnimatedMove;
				List<ATTile> runningPath = thePath.ToList ();
				while (runningPath.Count > 0 && !discoveryFlagged) {
					ATTile nextTile = runningPath.First ();

					actor.TileMovement.CallOnWalkedOutOfTile (actor.TileMovement.occupying, nextTile);
					actor.TileMovement.SetOccupyingTile (actor.TileMovement.occupying, nextTile);
					actor.transform.position = nextTile.transform.position;
					runningPath.Remove (nextTile);

//					Debug.Log ("movement left walking 1 step: " + actor.MovesLeft ());

				}

				actor.GetComponent<Vision> ().OnDiscoveredByEnemy -= DiscoveredByEnemyDuringNonAnimatedMove;
				FinishedMove ();
			}

			private void DiscoveredByEnemyDuringNonAnimatedMove(Actor enemy) {
				discoveryFlagged = true;
			}








			private List<string> DirectionsFrom(List<ATTile> aStarPath) {
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

			public void DrawAOOTLines(List<ATTile> path) {
				DestroyAOOPLines ();
				for(int i = 0; i < path.Count - 1; i++) {
					ATTile t = path [i];
		
		
					if (t.FirstOccupant != actor.TileMovement) {
						continue;
					}
		
					ATTile n = path [i+1];
		
					foreach(Actor a in t.Reachers) {
						if (a.EnemiesWith (actor) && 
							!n.Reachers.Contains(a) &&
							!a.UsedReaction()) {
							DrawAOOPLineFrom (a.transform.position, actor.transform.position);
							//Draw line/sword from a to char....
						}
					}
		
				}
			}
		
			public void DrawAOOPLineFrom(Vector3 pos1,Vector3 pos2){
				LineRenderer l = GameObject.Instantiate (UIManager.instance.aOOLine).GetComponent<LineRenderer>();
				Vector3[] vs = new Vector3[2];
				vs [0] = pos1;
				vs [1] = pos2;
		
				vs [0].z = -1;
				vs [1].z = -1;
		
				l.SetPositions(vs);
				cachedAoopLines.Add (l);
			}
		
			public void DestroyAOOPLines() {
				
				foreach(LineRenderer l in cachedAoopLines)
				{
					GameObject.Destroy(l.gameObject);
				}
				cachedAoopLines.Clear ();
			}

			public IEnumerator DelayColorOverTiles() {
				yield return new WaitForSeconds (.1f);
				ColorTilesMousedOver(MapManager.instance.TileUnderMouse());	
			}




		}
	}
}