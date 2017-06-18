using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT.Character;

using UnityEngine.UI;

public class FeaturesDisplay : StatsDisplay {

	void Awake() {
		contentParent = GetComponent<GenericFloatingWindow> ().Content;
		Debug.Log ("here fo:  " + Template ().name);
	}

	public void SyncUiWithCharacter(Sheet character) {
		Clear ();
//		GetComponent<GenericFloatingWindow> ().ClearContent ();
		List<GenericFeature> feats = character.AllMiscFeats ();
		if (feats.Count > 0) {
			foreach (GenericFeature f in character.AllMiscFeats()) {
				
				f.DressOptButtonForTooltip(AddOptButton (f.Name ()), Tooltip.TooltipPosition.LEFT, 30);

			}
		} else {
			AddFeatureText ("(none)");
		}




		(ContentParent as RectTransform).anchoredPosition = new Vector2 (0f, 0f);

	}

	public override Transform ContentParent {
		get { return GetComponent<GenericFloatingWindow> ().Content; }
	}




	public void AddFeatureText(string text, int fontSize = 14) {
		TextElement elem = GetComponent<GenericFloatingWindow> ().AddTextContent (text);
		elem.GetComponent<Text>().color = new Color(0f,0f,0f,1f);
		elem.GetComponent<Text>().fontSize = fontSize;
		elem.GetComponent<Text> ().alignment = TextAnchor.MiddleCenter;
	}
}
