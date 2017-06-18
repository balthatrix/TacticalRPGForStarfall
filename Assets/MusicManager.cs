using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour {

	public static MusicManager instance;
	public AudioClip[] songs;

	void Awake() {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (instance);
		} else {
			Destroy (this);
		}
	}

	void Init() {
		Debug.Log ("Initing !");

		GameManager.persistentInstance.OnSceneChanged += (scene, sceneMode) => {
			if(scene.name != lastScenePlay) {

				lastScenePlay = scene.name;
				if(SongForScene(scene) == lastPlay) {
					//nothing
				} else {
					PlaySong(SongForScene(scene));
				}
			}
		};

		PlaySong(SongForScene (SceneManager.GetActiveScene ()));
	}

	public AudioSource AudioSource {
		get { return GetComponent<AudioSource> (); }
	}

	void PlaySong(AudioClip song) {
		AudioSource.clip = song;
		AudioSource.Play ();
		lastPlay = song;
	}

	string lastScenePlay = "";
	AudioClip lastPlay = null;
	AudioClip SongForScene(Scene scene) {
		Debug.Log("scene has changed my friend: " + scene.name);


		switch (scene.name) {
		case "Battle":
			Debug.Log ("battle!");

			return songs [0];
			break;
		case "Title":
			Debug.Log ("title!");
			return songs[1];
			break;
		default:
			return lastPlay;

		}
	}



	// Use this for initialization
	void Start () {
		Init ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
