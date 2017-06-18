using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreativeSpore.SuperTilemapEditor;
using AT;

/// <summary>
/// Walkable tilemap layer.  This layer instantiates clear elements, whose job is to set a 
/// layer to clear when a clearing thing like an actor goes behind it.
/// Should be designated for things that should never be rendered in front of a dynanic renderer like an actor or missile
/// </summary>
public class GoesTransparentTilemapLayer : TileMapLayer {

	public List<Collider2D> collidersBehindLayer;


	public override void Initialize() {

		//tm.TintColor = new Color (1f,1f,1f,.75f);

		ForEachTile ((t, x, y) => {
			


			GameObject newT = Instantiate (MapManager.instance.clearElement);
			newT.transform.SetParent (MapManager.instance.transform, false);
			newT.transform.localPosition = new Vector3(x, y, 10000f);
			newT.GetComponent<ClearTilemapBehind>().toClear = this;
		
		});


	}

	public void AddBehindLayer(Collider2D col) {
		collidersBehindLayer.Add(col);
		if (collidersBehindLayer.Count == 1)
			transform.GetComponent<Tilemap> ().TintColor = new Color (1f, 1f, 1f, .45f);
	}

	public void RemoveBehindLayer(Collider2D col) {
		collidersBehindLayer.Remove(col);
		if (collidersBehindLayer.Count == 0)
			transform.GetComponent<Tilemap> ().TintColor = new Color (1f, 1f, 1f, 1f);
	}

}
