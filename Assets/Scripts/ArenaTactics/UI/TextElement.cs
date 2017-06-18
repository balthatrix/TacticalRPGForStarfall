using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class TextElement : MonoBehaviour {

	void Awake() {
		textObject = GetComponent<Text> ();
	}

	public Text textObject;

}
