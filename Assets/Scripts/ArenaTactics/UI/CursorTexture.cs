using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class CursorTexture : MonoBehaviour {
	public Texture2D cursorTexture;
	public CursorMode cursorMode = CursorMode.Auto;
	public Vector2 hotSpot = Vector2.zero;
	public void OnMouseEnter() {
		Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
	}
	public void OnMouseExit() {
		Cursor.SetCursor(null, Vector2.zero, cursorMode);

	}


		
}