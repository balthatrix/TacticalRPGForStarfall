using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSelector : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

	Vector3 offscreen = new Vector3(10000f, 10000f, 0f);
	public void Hide() {
		transform.position = offscreen;

		UIManager.instance.tileInfoPanel.DestroyLastCollection();
	}

	public void ShowOnTile(AT.ATTile tile) {
		transform.position = tile.transform.position;
		UIManager.instance.tileInfoPanel.ShowTileProps (tile);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
