using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
public class TileInfoPanel : MonoBehaviour {
	public GameObject textValueTemplate;
	public GameObject imageValueTemplate;

	// Use this for initialization
	void Start () {
		textValueTemplate.SetActive (false);
		imageValueTemplate.SetActive (false);
//		AddImageValue (IconDispenser.instance.SpriteFromIconName (AT.Character.IconName.DROP),
//			IconDispenser.instance.SpriteFromIconName (AT.Character.IconName.DASH));
//		DestroyLastCollection ();


	}

	public GameObject AddImageValue(Sprite icon, Sprite cornerImage=null) {

		GameObject newImage = Instantiate (imageValueTemplate);
		newImage.transform.SetParent (transform, false);

		newImage.GetComponent<Image> ().sprite = icon;
		newImage.transform.GetChild (0).GetComponent<Image> ().sprite = cornerImage;
		newImage.SetActive (true);
		lastCollection.Add (newImage);
		return newImage;
	}

	public GameObject AddTextValue(string val, Sprite cornerImage=null) {

		GameObject newgo = Instantiate (textValueTemplate);
		newgo.transform.SetParent (transform, false);

		newgo.GetComponent<Text> ().text = val;
		newgo.transform.GetChild (0).GetComponent<Image> ().sprite = cornerImage;
		newgo.SetActive (true);
		lastCollection.Add (newgo);
		return newgo;
	}

	List<GameObject> lastCollection = new List<GameObject>();
	public void DestroyLastCollection() {
//		Debug.Log ("Got Here!");
		foreach (GameObject go in lastCollection) {
			Destroy (go);
		}
		lastCollection.Clear ();
		destroyedSinceWait = true;
		gameObject.SetActive (false);
	}

	bool destroyedSinceWait = false;
	AT.ATTile lastTile;

	public void ActuallyShow(AT.ATTile tile) {
		
//			destroyedSinceWait = false;

			
//			yield return new WaitForSeconds (1f);


//			if (!destroyedSinceWait && tile != lastTile) {

		AddTextValue (tile.baseMoveCost.ToString (),
			IconDispenser.instance.SpriteFromIconName (AT.Character.IconName.MOVE));

		if (tile.IsDifficultTerrain) {

			AddImageValue (IconDispenser.instance.SpriteFromIconName (AT.Character.IconName.ARROW_DOWN),
				IconDispenser.instance.SpriteFromIconName (AT.Character.IconName.MELEE_ATTACK)).GetComponent<Image> ().color = new Color (1f, 0.1f, 0f, .7f);
			AddImageValue (IconDispenser.instance.SpriteFromIconName (AT.Character.IconName.ARROW_DOWN),
				IconDispenser.instance.SpriteFromIconName (AT.Character.IconName.SHIELD_METAL));
		}
//			}
//			lastTile = tile;




	}
	public void ShowTileProps(AT.ATTile tile) {
//		if (tile == lastTile)
//			return;
		//do movement

		DestroyLastCollection ();
		gameObject.SetActive (true);
		ActuallyShow (tile);
		//StartCoroutine (ActuallyShow (tile));

	}


}
