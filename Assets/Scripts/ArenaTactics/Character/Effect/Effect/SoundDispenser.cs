using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AT {


	public enum WeaponSwingFXType
	{
		HEAVY,
		MEDIUM,
		LIGHT,
		NONE
	}

	public class SoundDispenser : MonoBehaviour {


		public static SoundDispenser instance;


		public AudioClip humanDied;
		 
		void Awake() {
			if (instance == null) {
				instance = this;
				//sounds/music should be used accross scenes
				DontDestroyOnLoad (this);
			} else {
				Destroy (this);
			}
		}

		[System.Serializable]
		public class DamageFXElement  {
			public Character.Effect.DamageType key;
			public AudioClip val;
			public DamageFXElement(Character.Effect.DamageType key, AudioClip val) {
				this.key = key;
				this.val = val;
			}	
		}



		[System.Serializable]
		public class WeaponSwingFXElement  {
			public WeaponSwingFXType key;
			public AudioClip val;
			public WeaponSwingFXElement(WeaponSwingFXType key, AudioClip val) {
				this.key = key;
				this.val = val;
			}
		}

		public AudioClip SwingFXFromType(WeaponSwingFXType key) {
			List<WeaponSwingFXElement> matching = swingFX.Where ((fxElem) => {
				return (fxElem.key == key && fxElem.val != null);
			}).ToList();

			if (matching.Count > 0) {
				return matching.Last ().val;
			} else {
				Debug.LogWarning ("Couldn't locate a weapon swing sound effect for " + key);
				return null;
			}

		}

		public AudioClip DamageFXFromType(Character.Effect.DamageType key) {
			List<DamageFXElement> matching = damageFX.Where ((fxElem) => {
				return fxElem.key == key && fxElem.val != null;
			}).ToList();

			if (matching.Count > 0) {
				
				return matching.Last ().val;
			} else {
				Debug.LogWarning ("Couldn't locate a damage sound effect for " + key);
				return null;
			}
		}


		public enum ItemInteractFXType
		{
			HEAVY_ARMOUR, //plates
			MEDIUM_ARMOUR, //chain
			LIGHT_ARMOUR, //leather/padded
			HEAVY_IRON, //two-handed swords hammers
			HEAVY_WOOD, //clubs
			MEDIUM_IRON, //longswords
			MEDIUM_WOOD, //poles
			BOW_STRING_PLUCK, //bows/crossbows
			LIGHT_METAL, //dagger
			GOLD_DROP, //gold interaction
			BUCKET //helmet
		}

		[System.Serializable]
		public class ItemInteractFXElement  {

			public ItemInteractFXType key;
			public AudioClip pickUp;
			public AudioClip drop;
			public ItemInteractFXElement(ItemInteractFXType key, AudioClip up, AudioClip down) {
				this.key = key;
				this.pickUp = up;
				this.drop = down;
			}

		}


		public DamageFXElement[] damageFX;
		public WeaponSwingFXElement[] swingFX;

		public ItemInteractFXElement[] itemFX;
	}
}