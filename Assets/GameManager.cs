using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using AT.Character;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	public static GameManager persistentInstance;

	[System.Serializable]
	public class MapNameToPrefab {
		public string name;
		public GameObject mapPrefab;
	}

	public MapNameToPrefab[] maps;

	public void SetCurrentMapPrefabFromName(string name){
		foreach (MapNameToPrefab mnp in maps) {
			if (mnp.name == name) {
				persistentInstance.currentBattleMapPrefab = mnp.mapPrefab;
				break;
			}
		}




	}

	public enum SceneName
	{
		TITLE_SCREEN,
		BATTLE,
		CHARACTER_CREATION,
		ERROR
	}

	private Dictionary<SceneName, string> SceneNames = new Dictionary<SceneName,string> {
		{SceneName.TITLE_SCREEN, "Title" },
		{SceneName.BATTLE, "Battle" },
		{SceneName.CHARACTER_CREATION, "CharacterCreation" },
		{SceneName.ERROR, "ERROR" },
	};

	public void LoadSceneWithName(SceneName n) {
		string str;
		if(SceneNames.TryGetValue(n, out  str)) {
			SceneManager.LoadScene(str);
		}
	}

	public string StringSceneName(SceneName n) {
		return SceneNames [n];
	}

	public SceneName SceneNameFromString(string n) {
		foreach (SceneName name in SceneNames.Keys) {
			if (SceneNames [name] == n)
				return name;
		}
		return SceneName.ERROR;
	}


	/// <summary>
	/// The current battle map.  Should get set by either an overmap/story selection, 
	/// or by a skirmish selection.
	/// The map manager instantiates whatever this is when it comes online.
	/// </summary>
	public GameObject currentBattleMapPrefab; 

	//new generic characters are instantiated, and
	public List<AT.Character.Sheet> playerBattleCharacters;
	public List<AT.Character.Sheet> enemyBattleCharacters;


	private void StubSetSheets() {
//		Debug.LogError ("stubs " + playerBattleCharacters.Count + " " + playerBattleCharacters.Count + "." );

		playerBattleCharacters.Add (new Sheet ());
		enemyBattleCharacters.Add (new Sheet ());
	}

	public string CharacterExportDirectory {
		get { 
			return  Application.persistentDataPath + Path.DirectorySeparatorChar + "CharacterExports";
		}
	}
	public string CharacterPathAndDirectory(string name) {
		return CharacterExportDirectory + Path.DirectorySeparatorChar + name;
	}

	public List<string> ExportedCharacterNames() {
		if (!Directory.Exists (CharacterExportDirectory)) {
			Directory.CreateDirectory (CharacterExportDirectory);
		}

		List<string> ret =  Directory.GetFiles (CharacterExportDirectory).Select((str)=>{
			return str.Split(Path.DirectorySeparatorChar).Last();
		}).Where((str)=>{
			return !(str[0] == null || str[0] == '.');
		}).ToList ();
		return ret;
	}

	public void SaveCharacter(Sheet character) {
		if (!Directory.Exists (CharacterExportDirectory)) {
			Directory.CreateDirectory (CharacterExportDirectory);
		}

		string name = character.Name;
		int num = 1;
		while(File.Exists(CharacterPathAndDirectory(name))) {
			name = name + "_" + num.ToString ();
			num++;
		}
		AT.Serialization.Manager.Serialize (character, CharacterPathAndDirectory(name));
	}

	void Awake() {
		//This makes it so the game manager can be initialized from any scene, and will only happen once.
		if (persistentInstance == null) {
			playerBattleCharacters = new List<AT.Character.Sheet> ();
			enemyBattleCharacters = new List<AT.Character.Sheet> ();
			persistentInstance = this;
			StubSetSheets();
			SceneManager.sceneLoaded += SceneChanged;
			DontDestroyOnLoad (this);
			alreadyDid = false;
		} else {

			Destroy (this);
		}
	}

	void SceneChanged(Scene scene, LoadSceneMode sceneMode) {
		
		if (scene.name == "Battle")
			SetupBattle ();
		//StartCoroutine (SceneChangeTest ());'
		if (OnSceneChanged != null)
			OnSceneChanged (scene, sceneMode);
	}


	bool alreadyDid;
	IEnumerator SceneChangeTest() {
		yield return new WaitForSeconds (6f);
		if (!alreadyDid) {
			alreadyDid = true;
			SceneManager.LoadScene ("CharacterCreation");
		}
	}

	public delegate void SceneChangeAction(Scene scene, LoadSceneMode sceneMode);
	public event SceneChangeAction OnSceneChanged;


	// Use this for initialization
	void Start () {
		
	}

	void SetupBattle() {
		MapManager.instance.OnMapInitialized += (AT.ATMap map) => {
			BattleManager.instance.InitializeActorEntities();
			BattleManager.instance.OnActorsInitialized += () => {
				BattleManager.instance.InitBattle();
			};
		};



		MapManager.instance.InstantiateMapPrefab ();
	}

}
