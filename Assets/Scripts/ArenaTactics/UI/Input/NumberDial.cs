using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberDial : MonoBehaviour {

	public delegate void ValueChangedAction(NumberDial numberInput, bool decrease);
	public event ValueChangedAction OnValueChanged;

	public delegate void ValueWillChangeAction(NumberDial numberInput, bool decrease);
	public event ValueWillChangeAction OnValueWillChange;

	public delegate string SetLabelAction(int value);
	public SetLabelAction generateLabel = (int val) => {
		return val.ToString ();
	};

	[SerializeField]
	private int min = -9999;
	[SerializeField]
	private int max = 9999;

	private int current = 0;

	public int Current {

		get { 
			return current;
		} 
	}
	public int Min { 
		get { return min; }
		set { 
			if (current < value) {

				current = value;
				Changed ();
			}
			min = value;
		}
	}
	public int Max { 
		get { return max; } 
		set { 
			if (current > value) {
				current = value;
				Changed (true);
			}
			max = value;
		}
	}


	[SerializeField]
	private OptButton addOne;
	[SerializeField]
	private OptButton subtractOne;


	public  Text label;

	void Start() {

		addOne.OnOptLeftClicked += Incr;
		subtractOne.OnOptLeftClicked += Decr;

	}

	void Incr(OptButton op) {
		if (current < Max) {
			WillChange ();
			current++;
			Changed ();
		}
	}

	void Decr(OptButton op) {
		if (current > Min) {

			WillChange (true);
			current--;
			Changed (true);
		}
	}

	void Changed(bool decrease=false) {

		label.text = generateLabel (current);
		if (OnValueChanged != null) {
			OnValueChanged (this, decrease);
		}
	}

	void WillChange(bool decrease=false) {
		if (OnValueWillChange != null) {
			OnValueWillChange (this, decrease);
		}
	}
}
