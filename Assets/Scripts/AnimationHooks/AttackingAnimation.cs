using UnityEngine;
using System.Collections;
using AT.Battle;



[RequireComponent (typeof(TileMovement))]
[RequireComponent (typeof(Animator))]
public class AttackingAnimation : MonoBehaviour {
	TileMovement tileMovement;
	Animator animator;

	void Awake() {
		animator = GetComponent<Animator> ();
		tileMovement = GetComponent<TileMovement> ();
	}

	// Use this for initialization
	void Start () {
		GetComponent<Actor>().OnWillPerform += SetupAttackAnimation;
	}

	public void SetupAttackAnimation(Action a) {
		
		if (a is Attack) {
			Debug.LogError("a was an attack!");
			Attack action = (Attack)a;

			Actor target = action.TargetActor ();
			string name = NameFromTarget (target);
			DoAttackAnimation (name);
		}
	}

	string NameFromTarget(Actor t) {
		Vector3 directions = t.transform.position - transform.position;

		if (Mathf.Abs (directions.x) < Mathf.Abs (directions.y)) {
			//Attacking up or down
			if (directions.y > 0f) {
				return "AttackUp";
			} else {
				return "AttackDown";
			}
		} else {
			//Attacking left or right.
			if (directions.x > 0f) {
				return "AttackRight";
			} else {
				return "AttackLeft";
			}
		}
	}

	public void DoAttackAnimation(string name) {
		animator.SetTrigger (name);
	}

	public void AttackAnimationBegan() {
		if (OnAttackAnimationBegan != null) {
			OnAttackAnimationBegan (this);
		}
	}

	public void AttackAnimationEnded() {
		if (OnAttackAnimationEnded != null) {
			OnAttackAnimationEnded (this);
		}

	}

	public void AttackAnimationHitMoment() {


		if (OnHitMoment != null) {
			OnHitMoment (this);
		}
	}


	public delegate void ImpactMadeAction(AttackingAnimation animationInst);
	public event ImpactMadeAction OnHitMoment;


	public delegate void AttackAnimationBeganAction(AttackingAnimation animationInst);
	public event AttackAnimationBeganAction OnAttackAnimationBegan;


	public delegate void AttackAnimationEndedAction(AttackingAnimation animationInst);
	public event AttackAnimationEndedAction OnAttackAnimationEnded;
}
