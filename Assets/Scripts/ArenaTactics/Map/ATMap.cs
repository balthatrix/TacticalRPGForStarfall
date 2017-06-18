using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreativeSpore.SuperTilemapEditor;


namespace AT {
	/// <summary>
	/// Represents a mapping from a supertilemap editor to a group of at tiles.  
	/// Dynamically checking meta info on tiles in tile map and adding them to
	/// new instances of Dnd tiles, such as baseMoveCost.
	/// 
	/// 
	/// IMPORTANT NOTE: Tile map must be positioned in the root of the heirarchy at -.5, -.5,
	///  since tile coordinates are anchored at lower left.
	/// </summary>
	/// 
	public class ATMap : MonoBehaviour {

		public List<Vector2> PlayerSpawnPoints= new List<Vector2> ();
		public List<Vector2> CpuEnemySpawnPoints= new List<Vector2> ();






		void StubSetSpawnPoints() {
			PlayerSpawnPoints.Add (new Vector2 (0f, -1f));
			CpuEnemySpawnPoints.Add (new Vector2 (5f, -3));
		}

		void Awake() {
			StubSetSpawnPoints ();
		}

		public void Initialize() {
			///TODO: This should grab the ground and doodad layers separately....

			for (int i = 0; i < transform.childCount; i++) {
				TileMapLayer tml = transform.GetChild (i).GetComponent<TileMapLayer> ();
				if(tml != null)
					tml.Initialize ();
			}
			//		Debug.Log ("tilemap props: " + tm.CellSize.ToString());
			//		Debug.Log ("tilemap h: " + tm.GridHeight);
			//		Debug.Log ("tilemap w: " + tm.GridWidth);
			//		Debug.Log ("tilemap maxX: " + tm.MaxGridX);
			//		Debug.Log ("tilemap minX: " + tm.MinGridX);
			//		Debug.Log ("tilemap maxY: " + tm.MaxGridY);
			//		Debug.Log ("tilemap minY: " + tm.MinGridY);
			//		Debug.Log ("tilemap w: " + tm.GridWidth);



	//		Debug.LogError ("shits here!");
			MapManager.instance.MapInitialized (this);

			//Debug.Log("==?  " + (newT == MapManager.instance.TileAt(-5, -7)));
			//		Debug.Log ("t null? " + (t == null));
			//		Debug.Log ("to: " + t.prefabData.offset);
			//		Debug.Log ("map bounds: " + bnds.size);
			//		Debug.Log ("map bounds center: " + bnds.center);
			//tm.Erase (new Vector2 (0f, 0f));

		}



	}

}