using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class DropdownInterface : MonoBehaviour {

	Dropdown dropdown;


	void Awake() {
		dropdown = GetComponent<Dropdown> ();

	}
	public void ClearOptions() {
		dropdown.ClearOptions ();
	}

	public void AddOption(string option) {
		Dropdown.OptionData opt = new Dropdown.OptionData ();
		opt.text = option;

		dropdown.options.Add (opt);

		dropdown.RefreshShownValue ();

	}

	public string StringValue {
		get { return dropdown.captionText.text; }
	}

	public int Value {
		get { return dropdown.value; }
	}

	public void DropdownChanged() {

		if (OnDropdownChanged != null)
			OnDropdownChanged (this);
	}
	public delegate void DropdownChangeAction(DropdownInterface self);
	public event DropdownChangeAction OnDropdownChanged;
}
