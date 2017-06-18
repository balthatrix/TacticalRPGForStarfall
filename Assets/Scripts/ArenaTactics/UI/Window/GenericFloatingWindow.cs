using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenericFloatingWindow : MonoBehaviour {

	public bool IsDraggable { get; set; }



	[SerializeField]
	private RectTransform rootPanel;
	[SerializeField]
	private RectTransform headerPanel;

	[SerializeField]
	private RectTransform contentPanel;
	public RectTransform Content {
		get { return contentPanel; }
	}

	[SerializeField]
	private RectTransform bodyRect;

	public TextElement AddTextContent(string text){
		GameObject textElement = Instantiate (UIPrefabs.instance.textElementPrefab);
		textElement.GetComponent<TextElement> ().textObject.text = text;
		textElement.transform.SetParent (contentPanel, false);
		return textElement.GetComponent<TextElement> ();
	}

	public void ClearContent() {
		for (int i = 0; i < contentPanel.childCount; i++) {
			Destroy (contentPanel.GetChild (i).gameObject);
		}
	}



	[SerializeField]
	private Text titleLabel;
	public void SetTitle(string str) {
		titleLabel.text = str;
	}
	public Text Title {
		get { return titleLabel; }
	}


	public void SetHeaderHeight(int height) {
		//set the height
		headerPanel.sizeDelta = new Vector2(headerPanel.sizeDelta.x, (float) height);

		//set the position
		//headerPanel.anchoredPosition
//		Debug.Log("current: " + headerPanel.anchoredPosition);
		headerPanel.anchoredPosition = new Vector2(headerPanel.anchoredPosition.x, -((float) height/2));

		bodyRect.offsetMax = new Vector2 (bodyRect.offsetMax.x, -(height + 2));
	}

	public void SetSortOrder(int value) {
		Canvas c = GetComponent<Canvas> ();
		if(c != null)
			GetComponent<Canvas> ().sortingOrder = value;
	}

	public void SetPosition(int offsetX, int offsetY) {
		rootPanel.anchoredPosition = new Vector2 ((float)offsetX, (float)offsetY);
	}

	public void Resize(int width, int height) {
		rootPanel.sizeDelta = new Vector2 ((float) width, (float) height);	
	}



	public OptButton AddButton(string text) {
		GameObject btn = Instantiate (UIPrefabs.instance.optButtonPrefab);
		OptButton opt = btn.GetComponent<OptButton> ();
		opt.optText.text = text;

		btn.transform.SetParent (Content,false);
		return opt;
	}


	public NumberDial AddNumberDial(string text="0") {
		GameObject numberDial = Instantiate (UIPrefabs.instance.numberCounterPrefab);
		NumberDial dial = numberDial.GetComponent<NumberDial> ();

		dial.label.text = text;
		numberDial.transform.SetParent (Content,false);
		return dial;
	}



	// Use this for initialization
	public virtual void  Start () {
//		SetPosition (0, 0);
//		Resize (400, 200);
//		SetTitle ("Enter Name: ");
		SetSortOrder (1);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
