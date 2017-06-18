using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenericDialog : GenericFloatingWindow {

	[SerializeField]
	private OptButton confirm;
	[SerializeField]
	private OptButton cancel;

	public OptButton ConfirmBtn {
		get { return confirm; }
	}
	public OptButton CancelBtn {
		get { return cancel; }
	}

	[SerializeField]
	private bool showCancel;

	public void ShowCancel() {
		showCancel = true;
		cancel.gameObject.SetActive (true);
	}

	public void HideCancel() {
		showCancel = false;
		if(cancel != null)
			cancel.gameObject.SetActive (false);
	}


	public void EnableConfirm() {

		confirm.GetComponent<Button>().interactable = true;
	}
	public void DisableConfirm() {

		confirm.GetComponent<Button>().interactable = false;
	}

	// Use this for initialization
	public override void Start () {
		base.Start ();

		if (!showCancel) {
			HideCancel ();
		}

		if(confirm != null) 
			confirm.OnOptLeftClicked += Confirm;
		
		if (cancel != null) {
			cancel.OnOptLeftClicked += Cancel;
		}
	}

	void Cancel (OptButton button)
	{
		if (OnCanceled != null) {
			OnCanceled (this);
		}
	}

	void Confirm (OptButton button)
	{
		if (OnConfirmed != null) {
			OnConfirmed (this);
		}
	}


	public delegate void ConfirmedAction(GenericDialog self);
	public event ConfirmedAction OnConfirmed;


	public delegate void CanceledAction(GenericDialog self);
	public event CanceledAction OnCanceled;
}
