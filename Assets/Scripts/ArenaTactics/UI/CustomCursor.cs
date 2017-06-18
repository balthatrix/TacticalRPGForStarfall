using UnityEngine;
using System.Collections;

public class CustomCursor : MonoBehaviour {

	private Texture2D cursorTexture;
	public Texture2D cursorDownTexture;
	public Texture2D cursorUpTexture;
	public CursorMode cursorMode = CursorMode.Auto;
	public Vector2 hotSpot = Vector2.zero;

	public void OnMouseEnter() {
		Cursor.SetCursor(cursorUpTexture, hotSpot, cursorMode);
	}

	public void OnMouseExit() {
		Cursor.SetCursor(null, Vector2.zero, cursorMode);
	}


	bool PointIsInWindow(Vector3 point) {
		if (Screen.height > point.y && point.y > 0f &&
		    Screen.width > point.x && point.x > 0f) {
			return true;
		} else {
			return false;
		}
	}


	
	// Update is called once per frame
	bool trackingMouse = false;
	bool mouseDown = false;
	void Update () {
		
		if (Input.GetAxis ("Mouse Y") > 0 || Input.GetAxis ("Mouse Y") < 0 || Input.GetAxis ("Mouse X") > 0 || Input.GetAxis ("Mouse X") < 0) {
			if (PointIsInWindow (Input.mousePosition)) {
				if (!trackingMouse && !isPaused) {
					trackingMouse = true;
					OnMouseEnter ();
				}
				//Code for action on mouse moving right
			} else {
				if (trackingMouse) {
					trackingMouse = false;
					OnMouseExit ();
				}
			}
		} 

		if (trackingMouse) {
			if (Input.GetMouseButtonDown (0)) {
				if (!mouseDown) {
					mouseDown = true;
					Cursor.SetCursor (cursorDownTexture, hotSpot, cursorMode);
				}
			} else if (Input.GetMouseButtonUp (0)) {
				if (mouseDown) {
					Cursor.SetCursor (cursorUpTexture, hotSpot, cursorMode);
					mouseDown = false;
				}

			}


			//do raycast to
			if (UIManager.instance.canvas.activeSelf && !AT.ATTile.PointerOverUi()) {
				HighlightTile ();
			} else {
				UIManager.instance.selector.Hide ();
			}
			
		}



	}

	public void HighlightTile() {
		Collider2D intersect = Physics2D.OverlapPoint(
			Camera.main.ScreenToWorldPoint(Input.mousePosition),
			1 << LayerMask.NameToLayer("Tile")
		);


		if (intersect != null) {
			UIManager.instance.selector.ShowOnTile(intersect.GetComponent<AT.ATTile>());
		} else {
			
			UIManager.instance.selector.Hide ();
			//Debug.Log ("intersect t: " + tag);
		}


	}

	bool isPaused;
	void OnApplicationFocus( bool hasFocus )
	{
		isPaused = !hasFocus;
	}

	void OnApplicationPause( bool pauseStatus )
	{
		isPaused = pauseStatus;
	}
}
