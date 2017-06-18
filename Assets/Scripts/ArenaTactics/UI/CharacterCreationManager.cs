using UnityEngine;
using System.Collections;

using AT.Character;
using UnityEngine.SceneManagement;
using AT.UI;

public class CharacterCreationManager : MonoBehaviour {

	public GameObject dialogWindowPrefab;
	public GameObject descriptionWindowPrefab;
	public GameObject textInputDialogPrefab;

	public GameObject inventoryViewPrefab;

	public GenericDialog optionsWindow;
	public GenericDialog infoWindow;

	public OptButton confirmSelection;
	public OptButton backButton;

	public RectTransform ui;

	public TextInputDialog CreateNameInput() {
		GameObject root = Instantiate (textInputDialogPrefab);
		TextInputDialog ret = root.GetComponentInChildren<TextInputDialog> ();
		root.transform.GetChild (0).SetParent (InventoryView.instance.transform);
		return ret;
	}



	CharacterCreationController customizationController;
	Sheet character;

	void Awake() {
		
	}

	public void BackToTitle() {
		SceneManager.LoadScene (GameManager.persistentInstance.StringSceneName (GameManager.SceneName.TITLE_SCREEN));
	}

	// Use this for initialization
	void Start () {
		
		StartCoroutine (SetWindowRefs ());


	}

	IEnumerator SetWindowRefs() {
		yield return new WaitForEndOfFrame ();

		GameObject options = Instantiate (dialogWindowPrefab);
		//get the window out of it's own canvas
		Transform optionsTransform = options.transform.GetChild (0); 
		//add it to the creation ui transform
//		optionsTransform.SetParent (ui,false);
		//finally set the ref to it's window component
		optionsWindow = optionsTransform.GetComponent<GenericDialog>();
		optionsWindow.gameObject.SetActive (false);
		confirmSelection = optionsWindow.ConfirmBtn;
		backButton = optionsWindow.CancelBtn;




		//This should be a tooltip on each option that is created....
//		GameObject  descriptionWindow = Instantiate (descriptionWindowPrefab);
		//get the window out of it's own canvas
//		Transform descsTransform = descriptionWindow.transform.GetChild (0); 
		//add it to the creation ui transform
//		descsTransform.SetParent (ui,false);
		//finally set the ref to it's window component
//		Utilities.UtilUnity.PasteValuesFromComponentAToB<GenericDialog>(
//			descriptionWindow.GetComponent<GenericDialog>(),
//		infoWindow = descsTransform.GetComponent<GenericDialog>();


		GameObject  runningSheet = Instantiate (inventoryViewPrefab);
		//get the window out of it's own canvas
//		Transform runningSheetTransform = runningSheet.transform.GetChild (0); 
		//add it to the creation ui transform
//		runningSheetTransform.SetParent (ui,false);
		//finally set the ref to it's window component
		//finally set the ref to it's window component
//		Utilities.UtilUnity.PasteValuesFromComponentAToB<GenericDialog>(
//			runningSheet.GetComponent<GenericDialog>(),
//		);


		InventoryView.instance.doStubCharacters = false;
		InventoryView.instance.interactivityDisabled = true;


		character = new Sheet ();
		character.Name = "New Character";
		customizationController = new CharacterCreationController (this, character);
		customizationController.BeginCustomization ();

		StartCoroutine (ShowInv ());
	}


	IEnumerator ShowInv() {
		yield return new WaitForSeconds (0f);
		optionsWindow.gameObject.SetActive (true);
		InventoryView.instance.Show ();
		InventoryView.instance.AccomadateCustomizationStepChoice (optionsWindow.transform as RectTransform);
//		RectTransform invRect = InventoryView.instance.transform.GetChild (0) as RectTransform;
//		invRect.anchoredPosition = new Vector3 (150f, 0f, 0f);
		InventoryView.instance.characterTabs.gameObject.SetActive (false);
		InventoryView.instance.SetCurrentCharacter (character);
	}

}
