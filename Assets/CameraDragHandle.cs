using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

[RequireComponent(typeof(AT.ATTile))]
public class CameraDragHandle : MonoBehaviour {
	
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown (1)) {
			if (GetComponent<AT.ATTile>().mousedOver) {
				InitializeDrag ();
			}
		}
		if (Input.GetMouseButtonUp (1)) {
			//				if (mousedOver) {
			if (dragging) {
				EndDrag ();
			}
			//				}
		}

		if (dragging) {
			//				Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
			//				Vector3 move = new Vector3(pos.x, pos.y);
			//				Debug.Log ("now dragging! " + pos);
			//				UIManager.instance.cameraController.transform.Translate(move, Space.World);  
			Vector3 pixelOffset = Input.mousePosition - dragOrigin;
			float upp = CameraUtil.UnitsPerPixel (Camera.main);
			pixelOffset.Scale (new Vector3(upp, upp));
			//
			//				Vector3 offset = pixelOffset.normalized * 
			Vector3 desiredPosOffset = Vector3.zero - new Vector3(pixelOffset.x, pixelOffset.y, 0f);
			Vector3 proposed = cameraStartPosition + desiredPosOffset;

			UIManager.instance.cameraController.transform.position = ClampedToMapBounds(proposed);
		}
	}

	Vector3 ClampedToMapBounds(Vector3 proposed) {
		Vector3 ret = proposed;
		if(proposed.x < MapManager.instance.minX)
			ret.x = (float) MapManager.instance.minX;
		if (proposed.x > MapManager.instance.maxX)
			ret.x = (float) MapManager.instance.maxX;
		if (proposed.y < MapManager.instance.minY)
			ret.y = (float) MapManager.instance.minY;
		if (proposed.y > MapManager.instance.maxY)
			ret.y = (float) MapManager.instance.maxY;

		return ret;
	}


	bool dragging = false;
	Vector3 cameraStartPosition;
	private Vector3 dragOrigin;
	public void InitializeDrag() {
		dragOrigin = Input.mousePosition;
		UIManager.instance.cameraController.GoFreeMode ();
		cameraStartPosition = UIManager.instance.cameraController.transform.position;
		dragging = true;
	}

	public void EndDrag() {
		dragging = false;
	}
}
