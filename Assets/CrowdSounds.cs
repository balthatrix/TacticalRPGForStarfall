using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT.Battle;

public class CrowdSounds : MonoBehaviour {
	
	public static CrowdSounds instance;
	public AT.Battle.CharacterSoundProducer sounds;
	public AudioClip gaspClip;
	public AudioClip cheerClip;

	public AudioClip longCheerClip;

	public AudioSource[] ambientSources;

	void Awake() {
		if (instance == null) {
			instance = this;
			//Crowd sounds should be in in different scenes.
			DontDestroyOnLoad (this);
		} else {
			Destroy (this);
		}
	}

	void Start() {
//		StartCoroutine(LoopPlay ());

		TryInitialize ();

		GameManager.persistentInstance.OnSceneChanged += (scene, sceneMode) => {
			if (scene.name == "Battle") {
				TryInitialize();
			} else {
				foreach(AudioSource sou in ambientSources) {
//					sou.Stop();
					Debug.Log("stopping this mus");
					sou.loop = false;
				}
			}
		};
	}

	void TryInitialize() {
		foreach(AudioSource sou in ambientSources) {
			sou.loop = true;

//			Debug.Log("starting this mus");

		}

		if (BattleManager.instance.actorsInitialized) {
			InitializeActorHooks ();
		} else {
			BattleManager.instance.OnActorsInitialized += InitializeActorHooks;
		}
	}

	void InitializeActorHooks ()
	{
		//subscribe to each actor for getting hit, etc...
		foreach(AT.Battle.Actor actor in BattleManager.instance.AllActors()) {
//			Debug.LogError ("hsdfih");
			actor.OnActorKilled += GaspOrCheer;

		}
	}

	public void GaspOrCheer(Actor actor) {
			//maybe add a crowd favor?
//		Debug.Log("thing dies: " + actor.name);
		if(actor.IsOnPlayerSide) {
			Play(gaspClip);
		} else {
			Play(cheerClip);
		}
	}

	public IEnumerator DelayPlay(AudioClip clip) {
		yield return new WaitForSeconds (.3f);

// 		Debug.LogError("her she goes "+ clip.name);
		sounds.Play (clip);
	}

	public void Play(AudioClip clip) {
		StartCoroutine (DelayPlay (clip));
	}

	public IEnumerator LoopPlay() {
		while (true) {
			Play (longCheerClip);
			yield return new WaitForSeconds (2f);
		}
	}

	public void Update() {
		
	}

}
