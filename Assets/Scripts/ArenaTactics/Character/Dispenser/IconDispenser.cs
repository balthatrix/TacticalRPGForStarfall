using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT.Character;

public class IconDispenser : MonoBehaviour {

	[System.Serializable]
	public class InventoryIconMapping
	{
		public IconName iconName;
		public Sprite sprite;
	}

	public Sprite errorSprite;

	public InventoryIconMapping[] inventoryIconMappings;

	public Sprite SpriteFromIconName(IconName name){
		Sprite ret = errorSprite;

		foreach (InventoryIconMapping mapping in inventoryIconMappings) {
			if (mapping.iconName == name) {
				ret = mapping.sprite;
				break;
			}
		}

		return ret;

	}
	public static IconDispenser instance;

	private void Awake() {
		if (instance == null) {
			instance = this;
		} else {
			Destroy (this);
		}
	}

//
//	[System.Serializable]
//	public class SerializeableDictionary<TKey,TVal> {
//		DictElem[] keyvals;
//
//
//		private class DictElem
//		{
//			public TKey key;
//			public TVal val;
//		}
//
//		public SerializeableDictionary () {
//			
//		}
//
//
//	}
}
