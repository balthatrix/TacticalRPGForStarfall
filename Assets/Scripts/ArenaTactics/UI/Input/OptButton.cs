using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class OptButton : MonoBehaviour, IPointerClickHandler {

	public Text optText;

	public void OnPointerClick(PointerEventData eventData)
	{
		if ((GetComponent<Toggle>() != null && GetComponent<Toggle> ().interactable) || 
			(GetComponent<Button> () != null && GetComponent<Button> ().interactable)) { 
//			Debug.LogError("something wint");
			if (eventData.button == PointerEventData.InputButton.Left) {
				//			Debug.Log ("Left click");

				WasLeftClicked (eventData);
			} else if (eventData.button == PointerEventData.InputButton.Middle) {
				WasMiddleClicked (eventData);
				//			Debug.Log ("Middle click");
			} else if (eventData.button == PointerEventData.InputButton.Right) {
				WasRightClicked (eventData);
				//			Debug.Log ("Right click");
			}


			if (OnOptClicked != null) {
				OnOptClicked (this, eventData);
			}
		}

	}

	void Start() {
		OnOptRightClicked += DoShowRightClick;
		OnOptMousedOver += DoShowHoverTooltip;

		OnOptMousedOut += (button) => {
			Tooltip.instance.Hide();
		};
	}

	private string tooltipHoverText = null;
	private string tooltipRightClick = null;
	private int tooltipOffset;
	private Tooltip.TooltipPosition tooltipPosition;
	private void DoShowHoverTooltip(OptButton self) {
		if (tooltipHoverText != null) {
			if (tooltipRightClick != null) {
				Tooltip.instance.SetText (tooltipHoverText + "\n(Right-click for more details)");
			} else {
				Tooltip.instance.SetText (tooltipHoverText);
			}
			Tooltip.instance.Show (self.transform as RectTransform, tooltipPosition, tooltipOffset);
		}

	}

	private void DoShowRightClick(OptButton self) {
		if (tooltipRightClick != null) {
			Tooltip.instance.SetText (tooltipRightClick);
			Tooltip.instance.Show (self.transform as RectTransform, tooltipPosition, tooltipOffset);
		}

	}

	public void SetTooltipInfo(Tooltip.TooltipPosition pos, int offset, string hover, string rightclick) {
		tooltipPosition = pos;
		tooltipOffset = offset;
		tooltipHoverText = hover;
		tooltipRightClick = rightclick;
	}

	public void UnsetTooltipInfo() {
		tooltipHoverText = null;
		tooltipRightClick = null;
	}

	public void WasLeftClicked(PointerEventData eventData=null) {

//		Debug.Log ("left clicked! " + name);
		if (OnOptLeftClicked != null) {

			OnOptLeftClicked (this);
		}	
	//	GetComponent<Button> ();
	}


	public void WasRightClicked(PointerEventData eventData=null) {

//		Debug.Log ("right clicked! " + name);
		if (OnOptRightClicked != null) {

			OnOptRightClicked (this);
		}	
		//	GetComponent<Button> ();
	}


	public void WasMiddleClicked(PointerEventData eventData=null) {

//		Debug.Log ("middle clicked! " + name);
		if (OnOptMiddleClicked != null) {

			OnOptMiddleClicked (this);
		}	
		//	GetComponent<Button> ();
	}


	public void WasPointerUpped() {
		if (OnOptMouseUp != null) {
			OnOptMouseUp (this);
		}	
	}


	public void DidBeginDrag(BaseEventData d) {
//		Debug.Log ("Began Drag: " + this.name + d.ToString());
		if (OnOptBeganDrag != null)
			OnOptBeganDrag (this);
	}

	public void DidEndDrag() {
//		Debug.Log ("Ended Drag: " + this.name);
		if (OnOptEndedDrag != null)
			OnOptEndedDrag (this);
	}
		

	public void WasDragged() {
		if (OnOptDragged != null)
			OnOptDragged (this);
	}


	/// <summary>
	/// Gets called when dropped on something droppable.
	/// </summary>
	public void WasDropped() {
//		Debug.Log ("dropped " + this.name);
		if (OnOptDropped != null)
			OnOptDropped (this);
	}


	public void WasMousedOver() {

		if (OnOptMousedOver != null) {
			OnOptMousedOver (this);
		}	
	}


	public void WasMousedOut() {

		if (OnOptMousedOut != null) {
			OnOptMousedOut (this);
		}	
	}

	public void Disable() {
		Button btn = GetComponent<Button> ();
		Toggle tog = GetComponent<Toggle> ();
		if(btn != null)
		    btn.interactable = false;


		if (tog != null)
			tog.interactable = false;
	}

	public void Enable() {
		Button btn = GetComponent<Button> ();
		Toggle tog = GetComponent<Toggle> ();
		if(btn != null)
			btn.interactable = true;


		if (tog != null)
			tog.interactable = true;
	}

	public delegate void ButtonLeftClickedAction(OptButton button);
	public event ButtonLeftClickedAction OnOptLeftClicked;


	public delegate void ButtonRightClickedAction(OptButton button);
	public event ButtonRightClickedAction OnOptRightClicked;

	public delegate void ButtonMiddleClickedAction(OptButton button);
	public event ButtonLeftClickedAction OnOptMiddleClicked;

	public delegate void ButtonClickedAction(OptButton button, PointerEventData eventData);
	public event ButtonClickedAction OnOptClicked;


	public delegate void ButtonDraggedAction(OptButton button);
	public event ButtonDraggedAction OnOptDragged;
	public delegate void ButtonDroppedAction(OptButton button);
	public event ButtonDroppedAction OnOptDropped;



	public delegate void ButtonBeganDragAction(OptButton button);
	public event ButtonBeganDragAction OnOptBeganDrag;

	public delegate void ButtonEndedDragAction(OptButton button);
	public event ButtonEndedDragAction OnOptEndedDrag;

	public delegate void ButtonMousedOverAction(OptButton button);
	public event ButtonMousedOverAction OnOptMousedOver;

	public delegate void ButtonMousedOutAction(OptButton button);
	public event ButtonMousedOutAction OnOptMousedOut;



	public delegate void ButtonUpAction(OptButton button);
	public event ButtonUpAction OnOptMouseUp;

}
