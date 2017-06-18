using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class TitleScreenManager : MonoBehaviour {

	// Use this for initialization
	GenericDialog titleMenu;

	public enum SkirmishTeam
	{
		PLAYER,
		ENEMY
	}


	List <string> playerCharNames;
	List <string> enemyCharNames;

	void Start () {
		playerCharNames = new List<string> ();
		enemyCharNames = new List<string> ();

		GameObject go = Instantiate (UIPrefabs.instance.windowPrefab);
		RectTransform titleScreenWindow =  go.transform.GetChild (0) as RectTransform;
		titleScreenWindow.anchorMin = new Vector2 (.25f, .25f);
		titleScreenWindow.anchorMax = new Vector2 (.75f, .75f);
		titleScreenWindow.offsetMin = new Vector2 (0f, 0f);
		titleScreenWindow.offsetMax = new Vector2 (1f, 1f);

		titleMenu = titleScreenWindow.GetComponent<GenericDialog> ();
		titleMenu.SetTitle ("Arena Tactics");
		titleMenu.SetHeaderHeight (60);
		titleMenu.Title.fontSize = 28;
		titleMenu.Content.GetComponent<VerticalLayoutGroup> ().spacing = 10f;

		OptButton newChar = AddMenuButton ("Create New Character");
		newChar.OnOptLeftClicked += (button) => {
			SceneManager.LoadScene(GameManager.persistentInstance.StringSceneName(GameManager.SceneName.CHARACTER_CREATION));
		};

		OptButton skirmish = AddMenuButton ("Skirmish");
		skirmish.OnOptLeftClicked += (button) => {
			playerCharNames.Clear();
			enemyCharNames.Clear();
			titleScreenWindow.gameObject.SetActive(false);
			GameObject skirmishWindow = Instantiate (UIPrefabs.instance.dialogWindowPrefab);


			GenericDialog skirmDialog = UIPrefabs.instance.GetDialogWindow(skirmishWindow);
			skirmDialog.OnConfirmed += ConfirmAndStartSkirmish;
			skirmDialog.SetHeaderHeight(60);
			skirmDialog.Title.fontSize = 28;
			RectTransform swindow =  skirmDialog.gameObject.transform as RectTransform;
			swindow.anchorMin = new Vector2 (.10f, .10f);
			swindow.anchorMax = new Vector2 (.90f, .90f);
			swindow.offsetMin = new Vector2 (0f, 0f);
			swindow.offsetMax = new Vector2 (1f, 1f);

			skirmDialog.SetTitle("New Skirmish");
			skirmDialog.OnCanceled += (GenericDialog self) => {
				Destroy(skirmishWindow);
				titleScreenWindow.gameObject.SetActive(true);
			};

			skirmDialog.DisableConfirm();


			TextElement teMap = skirmDialog.AddTextContent("Select a map:");
			teMap.textObject.fontSize = 20;
			teMap.textObject.alignment = TextAnchor.MiddleCenter;

			GameObject mapDropdown = Instantiate(UIPrefabs.instance.dropdownMenu);
			DropdownInterface mapDropdownInterface = mapDropdown.GetComponent<DropdownInterface>();
			mapDropdownInterface.ClearOptions();
			foreach(GameManager.MapNameToPrefab mnp in GameManager.persistentInstance.maps) {
				mapDropdownInterface.AddOption(mnp.name);
			}
			//makes the current prefab set to the top option after populating them.
			mapDropdownInterface.DropdownChanged();

			//mapDropdownInterface.AddOption("hihf");
//			Debug.LogError("seetting to null");
			GameManager.persistentInstance.currentBattleMapPrefab = null;
			mapDropdownInterface.OnDropdownChanged += (DropdownInterface self) => {
				Debug.Log("Changed to " + self.StringValue);
				GameManager.persistentInstance.SetCurrentMapPrefabFromName(self.StringValue);
				//Debug.Log("new one: " + GameManager.persistentInstance.currentBattleMapPrefab.name);
			};
			mapDropdown.transform.SetParent(skirmDialog.Content, false);

			GameManager.persistentInstance.SetCurrentMapPrefabFromName(mapDropdownInterface.StringValue);



			GameObject horizontal = Instantiate(UIPrefabs.instance.horizontalLayoutElementPrefab);

			horizontal.GetComponent<LayoutElement>().preferredHeight = 400;
			horizontal.transform.SetParent(skirmDialog.Content);
			horizontal.GetComponent<HorizontalLayoutGroup>().padding = new RectOffset(20, 20, 20, 0);
			horizontal.GetComponent<HorizontalLayoutGroup>().spacing = 20;

			GameObject playerWindow = Instantiate (UIPrefabs.instance.windowPrefab);
			GenericDialog playerDialog = UIPrefabs.instance.GetDialogWindow(playerWindow);
			playerDialog.SetHeaderHeight(0);
			playerDialog.Title.text = "";
			playerDialog.transform.SetParent(horizontal.transform);

			GameObject enemyWindow = Instantiate (UIPrefabs.instance.windowPrefab);
			GenericDialog enemyDialog = UIPrefabs.instance.GetDialogWindow(enemyWindow);
			enemyDialog.SetHeaderHeight(0);
			enemyDialog.Title.text = "";
			enemyDialog.transform.SetParent(horizontal.transform);

			AddTeamInterface("Player", playerCharNames, playerDialog, skirmDialog);
			AddTeamInterface("Enemy", enemyCharNames, enemyDialog, skirmDialog );
		};

		if (GameManager.persistentInstance.ExportedCharacterNames ().Count == 0) {
			skirmish.Disable ();
			skirmish.OnOptMousedOver += (button) => {
				UIManager.instance.Tooltip.Show(skirmish.transform as RectTransform);
				UIManager.instance.Tooltip.SetText("Before you can skirmish, you need to create at least one character.");
			};
			skirmish.OnOptMousedOut += (button) => {
				UIManager.instance.Tooltip.Hide();

			};
		}

	}

	public void UpdatedSkirmishSides(GenericDialog lastDialog) {
		if (playerCharNames.Count > 0 && enemyCharNames.Count > 0) {
			lastDialog.EnableConfirm ();
		} else {
			lastDialog.DisableConfirm ();
		}
	}

	public void ConfirmAndStartSkirmish(GenericDialog lastSkirmish) {

		GameManager.persistentInstance.playerBattleCharacters.Clear ();
		foreach (string charName in playerCharNames) {
			AT.Character.Sheet player = AT.Serialization
				.Manager
				.Deserialize<AT.Character.Sheet> (GameManager.persistentInstance.CharacterPathAndDirectory(charName));

			GameManager.persistentInstance.playerBattleCharacters.Add (player);
		}

		GameManager.persistentInstance.enemyBattleCharacters.Clear ();
		foreach (string charName in enemyCharNames) {
			AT.Character.Sheet en = AT.Serialization
				.Manager
				.Deserialize<AT.Character.Sheet> (GameManager.persistentInstance.CharacterPathAndDirectory(charName));

			GameManager.persistentInstance.enemyBattleCharacters.Add (en);
		}

		GameManager.persistentInstance.LoadSceneWithName (GameManager.SceneName.BATTLE);

	}





	OptButton AddMenuButton(string title) {
		GameObject newButton = Instantiate (UIPrefabs.instance.optButtonPrefab);
		newButton.GetComponent<LayoutElement> ().minHeight = 50;
		newButton.GetComponent<OptButton> ().optText.text = title;
		newButton.GetComponent<OptButton> ().optText.fontSize = 20;
		newButton.transform.SetParent (titleMenu.Content, false);
		return newButton.GetComponent<OptButton> ();
	}


	public void AddTeamInterface(string name, List<string> runningCharNames, GenericDialog dialog, GenericDialog lastDialog) {
		TextElement tePlayerTeam = dialog.AddTextContent("Add to "+name+" team:");
		tePlayerTeam.textObject.fontSize = 20;
		tePlayerTeam.textObject.alignment = TextAnchor.MiddleCenter;
		GameObject playerTeamDropdown = Instantiate(UIPrefabs.instance.dropdownMenu);
		DropdownInterface playerTeamDropdownInterface = playerTeamDropdown.GetComponent<DropdownInterface>();

		playerTeamDropdownInterface.ClearOptions();


		foreach(string nma in GameManager.persistentInstance.ExportedCharacterNames()) {
			playerTeamDropdownInterface.AddOption(nma);
		}

		playerTeamDropdownInterface.OnDropdownChanged += (DropdownInterface self) => {
//			Debug.Log("chak to " + self.StringValue);
		};


		playerTeamDropdownInterface.transform.SetParent(dialog.Content, false);//.transform.SetParent(skirmDialog, false);

		//add a couple buttons
		GameObject horizontal = Instantiate(UIPrefabs.instance.horizontalLayoutElementPrefab);
		horizontal.GetComponent<LayoutElement>().preferredHeight = 30;
		horizontal.transform.SetParent(dialog.Content, false);
		TextElement tePlayerTeamDebug = dialog.AddTextContent("");
		tePlayerTeamDebug.textObject.alignment = TextAnchor.MiddleCenter;

		GameObject addPlayerBtn = Instantiate(UIPrefabs.instance.optButtonPrefab);
		OptButton addPlayerBtnOpt = addPlayerBtn.GetComponent<OptButton>();
		addPlayerBtnOpt.optText.text = "+";
		addPlayerBtnOpt.OnOptLeftClicked += (plsBtn) => {

			runningCharNames.Add(playerTeamDropdownInterface.StringValue);


			//refrech
			tePlayerTeamDebug.textObject.text = name + " Roster: \n";
			foreach(string teachChar in runningCharNames) {
				tePlayerTeamDebug.textObject.text += teachChar + "\n";
			}
			UpdatedSkirmishSides(lastDialog);
		};
		//			LayoutElement addPlayerLayout = addPlayerBtn.AddComponent<LayoutElement>();


		GameObject minusPlayerBtn = Instantiate(UIPrefabs.instance.optButtonPrefab);
		OptButton minusPlayerBtnOpt = minusPlayerBtn.GetComponent<OptButton>();
		minusPlayerBtnOpt.OnOptLeftClicked += (plsBtn) => {
//			Debug.Log("Subbing 1!");
			runningCharNames.Remove(playerTeamDropdownInterface.StringValue);


			//refrech
			tePlayerTeamDebug.textObject.text = name+" Roster: \n";
			foreach(string teachChar in runningCharNames) {
				tePlayerTeamDebug.textObject.text += teachChar + "\n";
			}
			UpdatedSkirmishSides(lastDialog);
		};
		minusPlayerBtnOpt.optText.text = "-";


		addPlayerBtn.transform.SetParent(horizontal.transform, false);
		minusPlayerBtnOpt.transform.SetParent(horizontal.transform, false);
	}
}
