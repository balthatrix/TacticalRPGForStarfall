using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AT.Character;
using System.Linq;
using AT.Battle;

/// <summary>
/// Acts as a broker between inventory/equipment manipulation and a group of characters.
/// Various display uis will call refresh on this when drag/drops happen.
/// </summary>
public class InventoryView : MonoBehaviour {
	

	//in the case of selecting multiple characters in a team,
	//this would be the current character selected.
	public Sheet currentCharacter;
	public PaperDollDisplay paperDollDisplay;
	public InventoryDisplay inventoryDisplay;

	public CharacterInventoryTabs characterTabs;

	public static InventoryView instance;

	public bool doStubCharacters;

	public CombatDisplay combatStatsDisplay;
	public AbilitiesDisplay abilitiesStatsDisplay;
	public SavesDisplay saveStatsDisplay;
	public ProficienciesDisplay proficienciesDisplay;
	public FeaturesDisplay featuresDisplay;

	private List<Actor> cachedActors;

	public Text headerLabel;

	public bool interactivityDisabled = false;

	public bool IsBattleMode {
		get { 
			return (BattleManager.instance != null);
		}
	}


	public void AccomadateCustomizationStepChoice(RectTransform window) {
		window.anchorMin = new Vector2 (0f, 0f);
		window.anchorMax = new Vector2 (0f, 1f);
		window.anchoredPosition = new Vector2 (-160f, 0f);
		window.sizeDelta = new Vector2 (300f, 0f);
		window.SetParent(transform.GetChild (0), false);
		(transform.GetChild (0) as RectTransform).anchoredPosition = new Vector2 (150f, 0f);
	}

	void Awake() {
		//There only should be one inventory view per scene.
		if (instance == null) {
			instance = this;
		} else {
			Destroy (this);
		}

		//When testing calls for it
		//Show ();
	}

	public Actor CurrentActor {
		get { 
			return instance.ActorFromSheet (currentCharacter);
		}
	}


	/// <summary>
	/// The hidden y value.  This needs to put the window offscreen, so tiles are still interactible.
	/// </summary>
	private int hiddenY = 1000;
	public void Show() {
//		Debug.Log ("HOW!");
		GetComponent<CanvasGroup> ().alpha = 1f;
		GetComponent<CanvasGroup> ().interactable = true;
		transform.GetChild (0).localPosition = new Vector3 (0f, 0f, 0f);
		if (IsBattleMode) {
			cachedActors = BattleManager.instance.PlayerActors;
			Sheet[] chars = BattleManager.instance.PlayerActors.Select((p)=> p.CharSheet).ToArray();



			characterTabs.characters = chars;

			SetCurrentCharacter (BattleManager.instance.CurrentlyTakingTurn.CharSheet);

			characterTabs.PopulateCharacterTabs ();

		}
	}

	public Actor ActorFromSheet(Sheet character) {
		if (cachedActors == null) {
			Debug.LogError ("Can't get actor from sheet.  Are you in battle mode where actors are present?");
			return null;
		}

		Actor ret = cachedActors.Where ((a) => a.CharSheet == character).Last ();
		return ret; 
	}

	public void Hide() {
		transform.GetChild (0).localPosition = new Vector3 (0f, (float) hiddenY, 0f);
		GetComponent<CanvasGroup> ().alpha = 0f;
		GetComponent<CanvasGroup> ().interactable = false;
	}
	public bool IsShown {
		get { 
			return GetComponent<CanvasGroup> ().interactable;
		}
	}

	private void StubCharacters() {
		Sheet c = new Sheet ();
		c.inventory.AddItem(new GenericWeapon (EquipmentSubtype.MARTIAL_LONGSWORD));
		c.inventory.AddItem(new GenericWeapon (EquipmentSubtype.SIMPLE_DAGGER));
		c.inventory.AddItem(new GenericArmour (EquipmentSubtype.ARMOUR_LEATHER));
		c.inventory.AddItem(new GenericArmour (EquipmentSubtype.ARMOUR_METAL_SHIELD));
		c.inventory.AddItem(new GenericArmour (EquipmentSubtype.ARMOUR_HORNED_HELM));
		c.PaperDoll.Equip (EquipmentSlotType.BODY, new GenericArmour (EquipmentSubtype.ARMOUR_CHAINMAIL), c);
		c.PaperDoll.Equip (EquipmentSlotType.HEAD, new GenericArmour (EquipmentSubtype.ARMOUR_HORNED_HELM), c);
		c.race = new Race (RaceName.HALF_ORC);

		Sheet c1 = new Sheet ();
		c1.race = new Race (RaceName.HUMAN);
		//		c1.inventory.AddItem(new GenericWeapon (EquipmentSubtype.SIMPLE_HANDAXE));
		//		c1.inventory.AddItem(new GenericWeapon (EquipmentSubtype.SIMPLE_HANDAXE));
		//		c1.inventory.AddItem(new GenericArmour (EquipmentSubtype.ARMOUR_HORNED_HELM));
		for (int i = 0; i < 40; i++) {
			c1.inventory.AddItem(new GenericArmour (EquipmentSubtype.ARMOUR_HORNED_HELM));
		}
		c1.PaperDoll.Equip (EquipmentSlotType.BODY, new GenericArmour (EquipmentSubtype.ARMOUR_LEATHER), c);

		characterTabs.characters = new Sheet[2] {c, c1};
		SetCurrentCharacter (c);
		characterTabs.PopulateCharacterTabs ();


	}

	// Use this for initialization
	void Start () {
		//Debug.LogError ("Starting!");
		if (!IsBattleMode && doStubCharacters) {
			StartCoroutine (DelayStubCharacters ());
		} else {
			//Comment this if you want dev mode (tinkering on inventory view)
			Hide ();
		}


	}

	IEnumerator DelayStubCharacters() {
		yield return new WaitForSeconds (.1f);
		StubCharacters ();
	}


	public void SetCurrentCharacter(Sheet character) {
		if (character == null) {
//			Debug.LogError ("It's null, ass!");
		}

		string header = character.Name;
		if (character.race != null) {
			header += ", " + Util.UtilString.EnumToReadable<RaceName> (character.race.name);
		} else {
			header += ", (No race)";
		}

		if (character.classLevels.Count > 0) {
			header += ", ";
			Dictionary<ClassType, int> lvls = character.ClassTypesToIntLevels ();
			foreach (ClassType typ in lvls.Keys) {
				int lvl = lvls [typ];
				header += "(" + Util.UtilString.EnumToReadable<ClassType> (typ) + " " + lvl + ")";

			}	
		} else {
			header += ", (No class)";
		}

		
		headerLabel.text = header;
		currentCharacter = character;
		RefreshUi();
	}




	public void RefreshUi() {
		inventoryDisplay.SyncUiWithCharacter (currentCharacter);

		paperDollDisplay.SyncUiToCharacter (currentCharacter);


		combatStatsDisplay.SyncUiWithCharacter (currentCharacter);
		abilitiesStatsDisplay.SyncUiWithCharacter (currentCharacter);
		saveStatsDisplay.SyncUiWithCharacter (currentCharacter);
		proficienciesDisplay.SyncUiWithCharacter (currentCharacter);
		featuresDisplay.SyncUiWithCharacter (currentCharacter);
	}

	public void RefreshCharacter() {
		SyncInventoryWithUi (currentCharacter);
		SyncPaperDollWithUi (currentCharacter);
		RefreshUi ();
	}



	/// <summary>
	/// Syncs the inventory of a character to user interface.
	/// </summary>
	void SyncInventoryWithUi(Sheet character) {
		InventoryItem[] items = new InventoryItem[Inventory.MAX_INVENTORY_SLOTS];
		for (int i = 0; i < items.Length; i++) {
			items [i] = inventoryDisplay.GetInventorySlot (i).CurrentItem;
		}

		character.inventory.items = items;
	}

	/// <summary>
	/// Syncs the paper doll of a character to user interface.
	/// Should be called after paper doll equipment elements change.
	/// </summary>
	/// <param name="character">Character.</param>
	void SyncPaperDollWithUi(Sheet character) {
		for (int i = 0; i < paperDollDisplay.paperDollUi.paperDollElements.Length; i++) {
			PaperDollDisplay.PaperDollUi.PaperDollUiElement elem = paperDollDisplay.paperDollUi.paperDollElements [i];
//			if(elem.uiSlot.CurrentItem != null) 
			character.PaperDoll.Equip (elem.slotType, elem.uiSlot.CurrentItem as Equipment, character);
			
		}



	}



}
