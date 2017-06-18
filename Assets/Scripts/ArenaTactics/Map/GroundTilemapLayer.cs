using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreativeSpore.SuperTilemapEditor;


/// <summary>
/// Ground tilemap layer.  Special in that it's the only layer that superimposes DnD tiles onto the tilemap.
/// </summary>
public class GroundTilemapLayer : TileMapLayer {


	
	public override void Initialize() {
		ForEachTile ((t, x, y) => {
			GameObject newT = Instantiate (MapManager.instance.tilePrefab);
			newT.transform.SetParent (MapManager.instance.transform, false);
			newT.transform.localPosition = new Vector2 (x, y);
		});
		//Initialize should happen after, since it relies on there being tile prefabs.	
		base.Initialize ();
	}
}
