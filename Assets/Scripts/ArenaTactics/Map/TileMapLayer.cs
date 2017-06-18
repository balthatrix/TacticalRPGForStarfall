using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreativeSpore.SuperTilemapEditor;
using AT;

public class TileMapLayer : MonoBehaviour {


	//if this layer overrides properties, any tiles set by this layer take the props here
	public bool overridesProperties;


	public int baseMovementCostOverride;
	public bool isDifficultTerrainOverride;
	public bool blocksVisionOverride;
	public bool unwalkableOverride;
	public bool blocksMissiles;
	public bool hindersMissiles;

	//
	public virtual void Initialize () {
		Tilemap tm = transform.GetComponent<Tilemap> ();

		//tm.TintColor = new Color (1f,1f,1f,.75f);
//		Debug.LogError("initializing " + name);

		ForEachTile ((t, x, y) => {
			ATTile tile = MapManager.instance.TileAt(x, y);
			if(tile != null)  {
				SetATTilePropsFromTile(t, tile);
				if (overridesProperties) {
					ATTile propTile  = MapManager.instance.TileAt(x,y);
					if (baseMovementCostOverride > 0) {
						//set base stuff
						propTile.baseMoveCost = baseMovementCostOverride;
					}

					if (isDifficultTerrainOverride) {
						propTile.IsDifficultTerrain = true;
					}

					if (blocksVisionOverride) {
//						Debug.Log("Setting tile blocking vision! " + x + " , " + y);
						propTile.BlocksVision = true;
					}

					if(blocksMissiles) {
						propTile.BlocksMissiles = true;
					}

					if (unwalkableOverride) {
						propTile.overriddenAsNotWalkable = true;
					}
				}
			}
		});

	}

	public delegate void TileAction(Tile t, int x, int y);
	public void ForEachTile(TileAction action) {
		Tilemap tm = transform.GetComponent<Tilemap> ();
		for (int i = tm.MinGridX; i <= tm.MaxGridX; i++) {
			for (int j = tm.MinGridY; j <= tm.MaxGridY; j++) {
				Tile t = tm.GetTile (new Vector2 (i, j));


				if (t != null) {
					action (t, i, j);
				}
			}
		}
	}

	public void SetATTilePropsFromTile(Tile t, ATTile dndTile) {
		int moveCost = t.paramContainer.GetIntParam("baseMoveCost");
		if(moveCost != 0)
			dndTile.baseMoveCost = moveCost;

		bool isDiff = t.paramContainer.GetBoolParam ("isDifficultTerrain");

		if (isDiff) {

			dndTile.IsDifficultTerrain = true;
		}


	}

}
