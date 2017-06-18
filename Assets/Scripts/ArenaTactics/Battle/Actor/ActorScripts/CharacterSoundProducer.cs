using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AT.Battle {
	public class CharacterSoundProducer : MonoBehaviour {

		 int currentAudioIndex = 0;
		public AudioSource[] auxiliarySources;
		// Use this for initialization
		void Start () {
			Actor thisIsOn = GetComponent<Actor> ();
			if (thisIsOn != null) {
//				Debug.Log ("not nul!");
				thisIsOn.CharSheet.OnDamaged += MakeDamagedSound;
				thisIsOn.OnActorKilled += TryPlayingDied;	
			}
		}

		void TryPlayingDied(Actor a) {
			StartCoroutine (WaitForNotPlaying (SoundDispenser.instance.humanDied));
		}

		IEnumerator WaitForNotPlaying(AudioClip clip) {
//			while (audio.isPlaying) {
//				yield return null;
//			}
			yield return null;
			auxiliarySources[currentAudioIndex].clip = clip;
			auxiliarySources[currentAudioIndex].Play ();
			RotateCurrent ();

		}

		public void Play(AudioClip clip) {
			StartCoroutine (WaitForNotPlaying (clip));

		}

		void MakeDamagedSound(AT.Character.Effect.Damage effect, Action s) {
//			Debug.LogError ("playering !");
			AudioClip clip = SoundDispenser.instance.DamageFXFromType (effect.Type);
			if (clip != null) {
				auxiliarySources[currentAudioIndex].clip = clip;
//				Debug.LogError ("playing the clip I found!");
				auxiliarySources[currentAudioIndex].Play ();
			}
			RotateCurrent ();

			//TODO: check for armour type, and make sound based on that.
		}

		void RotateCurrent() {
			currentAudioIndex++;
			if (currentAudioIndex >= auxiliarySources.Length)
				currentAudioIndex = 0;
		}
	}

}