using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

using AT.Battle;


/// <summary>
/// Action pointer.  Points to some group of actions, for example, bonus actions.
/// </summary>
public class ActionPointer {
	List<ActionButtonNode> actionRoots;
	public Sprite spriteLabel;
	public Sprite cornerSpriteLabel;
	string label;

	public ActionPointer(string label, List<ActionButtonNode> actionRoots) {
		this.label = label;
		this.actionRoots = actionRoots;
	}


	public ActionButtonNode GetRoot() {
		ActionButtonNode n = new ActionButtonNode ();
		n.SetChildren (actionRoots);
		n.label = label;
		n.spriteLabel = spriteLabel;
		n.cornerSpriteLabel = cornerSpriteLabel;
		bool dis = true;
		foreach (ActionButtonNode child in n.children) {
			if (!child.disabled) {
				dis = false;
			}
		}
		if (dis) {
			n.disabled = true;
			n.disabledReason = "There are no options.";
		}

		return n;
	}
}

/// <summary>
/// Represents a node of choices the player can take when using the action bar.
/// </summary>
public class ActionButtonNode {
	
	
	public ActionButtonNode() {
		children = new List<ActionButtonNode>();
	}

	public void AddChild(ActionButtonNode child) {
		children.Add (child);
		child.parent = this;
	}

	public void SetChildren(List<ActionButtonNode> childs) {
		children = new List<ActionButtonNode> ();

		foreach (ActionButtonNode n in childs) {
			AddChild (n);
		}
	}

	public List<ActionButtonNode> Siblings() {
		if (parent == null)
			return null;

		return parent.children;
	}

	public int PathLengthToRoot {
		get { 
			ActionButtonNode current = this;
			int i = 0;
			while (current.parent != null) {
				i++;
				current = current.parent;
			}
			return i;
		}
	}

	//whether the button is disabled or not....
	public bool disabled;
	public string disabledReason;
	public string rightClickDescription;
	
	public IActionOptionChoice choice;
	public ActionButtonNode parent;
	public List<ActionButtonNode> children;

	public Sprite spriteLabel;
	public Sprite cornerSpriteLabel;
	public string cornerText;
	public string label;
//
//	public Action NearestActionParent() {
//		ActionButtonNode curr = null;
//
//	}

}

public class ActionBar : MonoBehaviour {



	[SerializeField]
	private RectTransform actionsContainer;

	[SerializeField]
	private OptButton pageLeft;
	[SerializeField]
	private OptButton pageRight;


	public GameObject optTemplate;

	private ActionButtonNode root;

	private List<ActionButtonNode> toTheLeft;
	private List<ActionButtonNode> currentActionNodesShown;
	private List<ActionButtonNode> toTheRight;

	private List<ActionButtonNode> currentSet;


	/// <summary>
	/// if there are more than max options, pagination position is the first action in the current array that is shown.
	/// </summary>
	int paginationPosition = -1;
	int maxPerPage = 8;
	int adjustedMaxPerPage = 8;

	public float buttonWidth = 64f;





	void UpdateActionBarWidth() {
		int proposed = root.children.Count;

		if (proposed > maxPerPage) {
			proposed = maxPerPage;
		}
		float newW = proposed * buttonWidth;

		actionsContainer.sizeDelta = new Vector2 (newW, actionsContainer.sizeDelta.y);
//		Debug.LogError ("new s : " + newW);
		RectTransform self = (transform as RectTransform);
		self.anchoredPosition = new Vector2(newW / 2f, self.anchoredPosition.y);
//		Debug.LogError ("new spos : " + transform.position.x);
		RectTransform pgLeft = (RectTransform) pageLeft.transform;
		RectTransform pgRight = (RectTransform) pageRight.transform;
		pgLeft.anchoredPosition = new Vector2 (-(pgLeft.sizeDelta.x / 2) - 5f, actionsContainer.sizeDelta.y + pgLeft.sizeDelta.y);
		pgRight.anchoredPosition = new Vector2 (pgRight.sizeDelta.x / 2 + 5f, actionsContainer.sizeDelta.y + pgRight.sizeDelta.y);


	}

	// Use this for initialization
	void Start () {
		root = new ActionButtonNode ();
		adjustedMaxPerPage = maxPerPage;
		UpdateActionBarWidth ();


		//test this out.
		List<ActionButtonNode> list = new List<ActionButtonNode>();

		//InitiateActionTree (list);
		ClearCurrentBar();
		DisableLeft ();
		DisableRight ();

		pageLeft.OnOptLeftClicked += PageLeft_OnOptClicked;
		pageRight.OnOptLeftClicked += PageRight_OnOptClicked;

	}

	public void InitiateActionTree(List<ActionButtonNode> nodes) {

		root.SetChildren(nodes);
		SetCurrentActionNodes (nodes);
		UpdatePeripheralActions ();
	}


	private void ClearCurrentBar() {
		for (int i = 0; i < actionsContainer.childCount; i++) {
			Destroy (actionsContainer.GetChild (i).gameObject);
		}

//		DisableLeft ();
//		DisableRight ();
	}

	private void DisableLeft() {
//		Debug.Log ("disable left.");
		pageLeft.gameObject.SetActive (false);
//		pageLeft.GetComponent<Button>().interactable=false;
//		pageLeft.optText.text = "";
	}

	private void EnableLeft() {
		pageLeft.gameObject.SetActive (true);
//		Debug.Log ("enabling left.");
//		pageLeft.GetComponent<Button>().interactable=true;

	}

	private void DisableRight() {
//		Debug.Log ("disable right.");
		pageRight.gameObject.SetActive (false);
//		pageRight.GetComponent<Button>().interactable=false;
//		pageRight.optText.text = "";
	}

	private void EnableRight() {
//		Debug.Log ("enabling right.");
		pageRight.gameObject.SetActive (true);
//		pageLeft.GetComponent<Button>().interactable=true;

	}


	private void UpdatePeripheralActions() {
		if (toTheLeft.Count > 0) {
			EnableLeft ();
//			pageLeft.optText.text = toTheLeft.Count.ToString ();
		} else {
//			Debug.Log ("Disabling left");
			DisableLeft ();
		}

		if (toTheRight.Count > 0) {
			
			EnableRight ();
//			pageRight.optText.text = toTheRight.Count.ToString ();
		} else {
//			Debug.Log ("Disabling right");
			DisableRight ();
		}
	}

	private bool IsRoot {
		get {
			
			return currentSet.First ().parent == root;
		}
	}

	private void SetCurrentActionNodes(List<ActionButtonNode> nodes) {
		currentSet = nodes.ToList ();
		if (currentSet.Count == 0) {
			ClearCurrentBar ();
			return;
		}

		
		if (IsRoot) {
			adjustedMaxPerPage = maxPerPage;
//			Debug.Log ("is max");
		} else {
//			Debug.Log ("adbusting ");
			adjustedMaxPerPage = maxPerPage - 1; //to allow for back button
		}



		ActionButtonNode soleOpt = SoleOption (nodes);
		if (soleOpt != null && !IsRoot) {

			autoSelectStackLength += 1;
			NodeSelected (soleOpt);
			return;
		}

		if (adjustedMaxPerPage >= nodes.Count) {
			currentActionNodesShown = nodes.ToList ();
			toTheRight = new List<ActionButtonNode> ();
			toTheLeft = new List<ActionButtonNode> ();
		} else {
			currentActionNodesShown = nodes.GetRange(0, adjustedMaxPerPage);
			toTheRight = nodes.GetRange (adjustedMaxPerPage, nodes.Count - adjustedMaxPerPage);
			pageRight.gameObject.SetActive (true);
			//pageRight.optText.text = toTheRight.Count.ToString ();
			toTheLeft = new List<ActionButtonNode> ();
		}
		

		Populate (currentActionNodesShown);
	}

	/// <summary>
	/// Populate the specified options, isRoot and offset.
	/// </summary>
	/// <param name="options">Options.</param>
	/// <param name="isRoot">If set to <c>true</c> is root.</param>
	/// <param name="offset">Offset. how many iterations through list to wait until action buttons populate</param>
	private void Populate(List<ActionButtonNode> options) {
		
		ClearCurrentBar ();

		//in this case, all the options should have the same parent.

		if (!IsRoot) {
			InstantiateAndSetOnClickForBack (options.First ());
		}

		//1 option is all that's there, or is all that's enabled
		   //if the option is an action with no parameters, do nothing,
		   //else set nodes to the child of that one...



		if (options.Count > 0) {
			foreach (ActionButtonNode n in options) {
//				Debug.Log ("ahisdfl1");
				GameObject button = InstantiateButtonFromNode (n);
				SetOnClickForOpt (button.GetComponent<OptButton> (), n);
				actionsContainer.GetComponent<GridLayoutGroup> ().SetLayoutVertical ();
				actionsContainer.GetComponent<GridLayoutGroup> ().SetLayoutHorizontal();
			}

		}

		UpdateActionBarWidth ();


	}

	private void NodeSelected(ActionButtonNode n) {
		if (n.children.Count > 0) {
			SetCurrentActionNodes (n.children);
		} else {
			Debug.Log ("resolving!!!!");
			Resolve (n);
		}
	}

	private ActionButtonNode SoleOption(List<ActionButtonNode> options) {
		ActionButtonNode soleOption = null;
		foreach (ActionButtonNode n in options) {
			if (n.disabled) {
				continue;
			}

			if (!(n.choice is Action)) { //don't want to autoselect actions....
				
				if (soleOption != null) { //found another enabled option... 
					soleOption = null;
					break;
				} else {
					soleOption = n;
				}
			}
		}

		return soleOption;
	}





	private void Resolve(ActionButtonNode leaf) {
		autoSelectStackLength = 1;
		List<int> path = new List<int> ();
		ActionButtonNode current = leaf;
		while (current != root) {
			path.Add (current.Siblings ().ToList().IndexOf (current));
			current = current.parent;
		}
		path.Reverse ();

		foreach (int i in path) {

		}

		if (OnPathResolved != null) {
			OnPathResolved (this, path);
		}
	}

	public delegate void ActionPathResolvedAction(ActionBar self, List<int> path);
	public event ActionPathResolvedAction OnPathResolved;

	private void SetOnClickForOpt(OptButton opt, ActionButtonNode n) {
		if (n.disabled) {
			opt.OnOptMousedOver += (button) => {

				UIManager.instance.Tooltip.SetText(n.label + ": " + n.disabledReason);
				UIManager.instance.Tooltip.Show(opt.transform as RectTransform);

			};

			opt.OnOptMousedOut += (button) => {

				UIManager.instance.Tooltip.Hide();
//				UIManager.instance.tooltip.SetText(n.disabledReason);

			};

			opt.GetComponent<Button> ().interactable = false;
		} else {
			opt.OnOptLeftClicked += (button) => {
				NodeSelected(n);
			};

			opt.SetTooltipInfo (Tooltip.TooltipPosition.TOP, 0, n.label, n.rightClickDescription);

		}
	}

	private void InstantiateAndSetOnClickForBack(ActionButtonNode n) {
		GameObject back = Instantiate (optTemplate);
		back.SetActive (true);
		back.transform.SetParent (actionsContainer, false);
		Text t = back.GetComponentInChildren<Text> ();
		t.text = "<";

		back.GetComponent<OptButton> ().OnOptMousedOver += (button) => {

			UIManager.instance.Tooltip.SetText ("Back");
			UIManager.instance.Tooltip.Show (back.transform as RectTransform);

		};



		back.GetComponent<OptButton> ().OnOptMousedOut += (button) => {

			UIManager.instance.Tooltip.Hide ();
			//				UIManager.instance.tooltip.SetText(n.disabledReason);

		};
		back.GetComponent<OptButton> ().OnOptLeftClicked += (button) => {
			//this needs to call set new action node set

			DisableLeft ();
			DisableRight ();
			ActionButtonNode curr = n;
			Debug.Log("Auto stack backing: " + autoSelectStackLength);
			for (int i = 0; i < autoSelectStackLength; i++) {
				curr = curr.parent;
			}
			autoSelectStackLength = 1;
			SetCurrentActionNodes (curr.Siblings ());
		};
	}
	int autoSelectStackLength = 1;

	public GameObject InstantiateButtonFromNode(ActionButtonNode n) {
		GameObject ret = Instantiate (optTemplate);
		ret.SetActive (true);
		Text text = ret.GetComponent<OptButton>().optText;
		Image cornerImg = ret.transform.GetChild (1).gameObject.GetComponent<Image>();
		Text cornerTxt = ret.transform.GetChild (2).gameObject.GetComponent<Text>();

		if (n.spriteLabel != null) {
			ret.GetComponent<Image> ().sprite = n.spriteLabel;
			//tooltip on hover for the action
		} else {
			text.fontSize = 13;
			text.text = n.label;
		}

		if (n.cornerSpriteLabel != null) {
			cornerImg.sprite = n.cornerSpriteLabel;
			cornerImg.color = new Color (1f, 1f, 1f, 1f);
		} else {
			cornerImg.color = new Color (1f, 1f, 1f, 0f);
		}

		if (n.cornerText != null) {
			cornerTxt.text = n.cornerText;
		} else {
			cornerTxt.text = "";
		}

		ret.transform.SetParent (actionsContainer, false);
		return ret;
	}



	void PageLeft_OnOptClicked (OptButton button)
	{
		if (toTheLeft.Count >= adjustedMaxPerPage) {
			toTheRight = toTheRight.Concat (currentActionNodesShown).ToList();

			currentActionNodesShown = toTheLeft.GetRange (toTheLeft.Count - adjustedMaxPerPage, adjustedMaxPerPage);
			toTheLeft.RemoveRange (toTheLeft.Count - adjustedMaxPerPage, adjustedMaxPerPage);

		} else {
			toTheRight = currentActionNodesShown
				.GetRange (currentActionNodesShown.Count - toTheLeft.Count, toTheLeft.Count)
				.Concat (toTheRight).ToList();

			currentActionNodesShown.RemoveRange (currentActionNodesShown.Count - toTheLeft.Count, toTheLeft.Count);
			currentActionNodesShown = toTheLeft.Concat (currentActionNodesShown).ToList ();
			toTheLeft= new List<ActionButtonNode> ();

		}
		UpdatePeripheralActions ();
		Populate (currentActionNodesShown);
	}

	void PageRight_OnOptClicked (OptButton button)
	{
		if (toTheRight.Count >= adjustedMaxPerPage) {
			toTheLeft = toTheLeft.Concat (currentActionNodesShown).ToList();

			currentActionNodesShown = toTheRight.GetRange (0, adjustedMaxPerPage);
			toTheRight.RemoveRange (0, adjustedMaxPerPage);

		} else {
			toTheLeft = toTheLeft.Concat (currentActionNodesShown.GetRange (0, toTheRight.Count)).ToList();
			currentActionNodesShown.RemoveRange (0, toTheRight.Count);
			currentActionNodesShown = currentActionNodesShown.Concat(toTheRight).ToList();
			toTheRight = new List<ActionButtonNode> ();

		}
		UpdatePeripheralActions ();
		Populate (currentActionNodesShown);
	}
}
