using UnityEngine;
using System.Collections;
using AT;

[RequireComponent (typeof(SpriteRenderer))]

/// <summary>
/// Used mainly by doodads and characters to keep their sorting order correct on the map.  
/// Sprites lower on the map should have a higher sorting order than sprites higher on the map.
/// </summary>
public class TileInhabitant : MonoBehaviour {

	public Transform avatar;

	void Awake() {

		if(avatar == null)
			avatar = transform;

		if (avatar == null) {
			throw new UnityException ("Tile inhabitant needs an avatar!  If this is a doodad, avatar should be a tile, if it's an entity, it should be a character");
		}

		GetComponent<SpriteRenderer>().sortingLayerName = "TileInhabitant";

		TileMovement tm = GetComponent<TileMovement> ();
		if(tm != null) 
			tm.OnWalkedOutOfTile += AvatarMovedInto;
	}

	void AvatarMovedInto(ATTile from, ATTile toDestination) {
		ResetRenderLayer (MapManager.instance, new Vector2(toDestination.transform.position.x, toDestination.transform.position.y ));

		SetChildrenOffsets (GetComponent<SpriteRenderer>().sortingOrder);

	}


	public void InitialOrderInhabitantLayer(MapManager inst) {
		ResetRenderLayer (inst, avatar.transform.position);
		SetChildrenOffsets (GetComponent<SpriteRenderer>().sortingOrder);
	}

	void ResetRenderLayer(MapManager inst, Vector2 position) {
		inst.SetRenderLayer (this.GetComponent<SpriteRenderer>(),position);
	}

	void SetChildrenOffsets(int rootRenderOrder, Transform nextT=null, int depth=0) {
		if (nextT == null)
			nextT = transform;
		bool aChildHadSr = false;

		for (int i = 0; i < nextT.childCount; i++) {
			Transform child = nextT.GetChild (i);

			SpriteRenderer sr = child.GetComponent<SpriteRenderer> ();
			if (sr != null) {

				sr.sortingOrder = rootRenderOrder + depth + 1;
			}
			SetChildrenOffsets (rootRenderOrder, child, depth+1);

		}
	}


}
