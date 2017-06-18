using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using AT.Character;


/// <summary>
/// MIssiles  get their animation prefabs from this.
/// Pieces of equipment should all have animation tags of type MissileScript.MissileAnimationName
/// this is used to determine what animation controllers to use.
/// </summary>
public class MissileAnimationPrefabDispenser : MonoBehaviour {

	public NamedAnimationController[] animationStore;

	[System.Serializable]
	public class NamedAnimationController {
		public MissileScript.MissileAnimationName name;
		public GameObject animationPrefab;
	}

	public GameObject GetAnimationPrefabByName(MissileScript.MissileAnimationName name) {
		GameObject anim = null;

		//		con.clips[0].overrideClip.SampleAnimation
		foreach (NamedAnimationController mapping in animationStore) {
			if (mapping.name == name) {

				anim = mapping.animationPrefab;
				break;
			}
		}
		return anim;

	}




	public static MissileAnimationPrefabDispenser instance;

	void Awake() {
		if (instance == null) {
			instance = this;
		} else {
			Destroy (this);
		}

	}


}