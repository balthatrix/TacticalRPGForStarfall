using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TextInputDialog : GenericDialog {
	[SerializeField]
	private InputField inputField;
	public string value;

	public InputField InputField {
		get { 
			return inputField;
		}
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.Return) || Input.GetKeyDown (KeyCode.KeypadEnter)) {
			Debug.Log ("hi");
//			if (InputField.isFocused) {
			ConfirmBtn.WasLeftClicked ();	
//			}
		}
	}

	public override void Start() {
		base.Start ();
		InputField.text = "";

		//TODO: this is really the custom name validation, but instead it should be customizable so 
		//this can be used accross different places.
		InputField.onValidateInput = (string input, int index, char character) => {

			//allows spaces, only if input is not blank, and the last character is not a space.
			//prevents multiple spaces between name.
			if(character == ' ') {
				if(input == "" || input.LastIndexOf(' ') == input.Length - 1)
					return '\0';
			}

			if(char.IsLetterOrDigit(character) || character == ' ') 
				return character;
			else
				return '\0';
		};

		InputField.ActivateInputField ();
	}


	public void TextChanged(string value) {
		StartCoroutine (ShedValueAfterFrameEnd ());
	}

	public void FinishedEdit(string value) {
		if (OnFinishedEdit != null)
			OnFinishedEdit (this);
	}

	//could be used to move focus to another input element....
	public delegate void FinishedEditAction(TextInputDialog self);
	public event FinishedEditAction OnFinishedEdit;

	public delegate void ChangedAction(TextInputDialog self);
	public event ChangedAction OnChanged;


	private IEnumerator ShedValueAfterFrameEnd() {
		yield return new WaitForEndOfFrame ();
		string previous = value;
		value = InputField.text;

		if (OnChanged != null) {
			OnChanged (this);
		}


	}
}
