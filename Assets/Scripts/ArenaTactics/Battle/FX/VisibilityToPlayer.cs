using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AT.Battle {

	/// <summary>
	/// Visibility to player. Used by enemy actors, and by items on the ground, ground fx, and by dynamic doodads.
	/// </summary>
	public class VisibilityToPlayer : MonoBehaviour {
		void Awake() {
			//assume not visible
			GoInvis();


		} 

		void Start() {
			Vision vis = GetComponent<Vision> ();
			if (vis != null) {
				vis.OnDiscoveredByEnemy += GoVisibleForDiscovery;
				vis.OnUndiscoveredByEnemy += GoInvisibleForUndiscovery;
			} else {

				ATTile til = MapManager.instance.TileAt (transform.position);
				foreach (Actor a in til.actorsThatSeeThisTile) {
					if (a is PlayerControlledActor) {
						GoVis ();
						break;
					} 
				}

				til.OnViewerAdded += (Actor actor) => {
					if(actor.IsOnPlayerSide) {
						GoVis();
					}
				};

				til.OnViewerRemoved += (Actor actor) => {
					if(actor.IsOnPlayerSide && !til.SeenByAlliesOf(actor)) {
						GoInvis();
					}
				};
			}

		}

		void GoVisibleForDiscovery(Actor a) {
			if (BattleManager.instance.SideFor(a) == BattleManager.Side.PLAYER) {
				GoVis ();
			}

		}

		void GoInvisibleForUndiscovery(Actor a) {
			if (a.IsOnPlayerSide) {
				TileMovement tm = GetComponent<TileMovement> ();
				ATTile tile;
				if (tm != null) {
					tile = tm.occupying;
				} else {
					//the visible thing is a weapon or ground effect.
					tile = MapManager.instance.TileAt(transform.position);
				}
				 
//				Debug.LogError ("undiscovered by an enemy player.  checking if I should go invisible");
//				bool goingInvis = true;
				if(!tile.SeenByAlliesOf(a)) 
					GoInvis ();	
			}


		}

		void GoInvis() {
			SpriteRenderer sr = GetComponent<SpriteRenderer> ();
			if (sr != null) {
				sr.enabled = false;
			}

			SpriteRenderer[] childSrs = GetComponentsInChildren<SpriteRenderer> ();
			foreach (SpriteRenderer Sr in childSrs) {
				
					Sr.enabled = false;
			}
		}

		void GoVis() {
			SpriteRenderer sr = GetComponent<SpriteRenderer> ();
			if (sr != null) {
				sr.enabled = true;
			}

			SpriteRenderer[] childSrs = GetComponentsInChildren<SpriteRenderer> ();
			foreach (SpriteRenderer Sr in childSrs) {
				if (GetComponent<CharacterIndicator> () != null && Sr != GetComponent<CharacterIndicator> ().indicator) {
//					Debug.Log ("enable: " + sr.name);
					Sr.enabled = true;
				}
			}
		}

	}
}
