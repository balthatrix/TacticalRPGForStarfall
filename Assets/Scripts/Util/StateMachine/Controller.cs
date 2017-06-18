using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Util.StateMachine
{
	public class Controller {


		State currentState;

		public State CurrentState {	get; set; }

		public virtual void SwitchState(State destination) {
			if(CurrentState != null)
				CurrentState.WillExit (destination);
			State prev = CurrentState;
			CurrentState = destination;
//			string prevnam = "nul";
//			if (prev != null) {
//				prevnam = prev.GetType ().ToString ();
//			}
			//Debug.Log ("Switthing from " + prevnam + " to " + destination.GetType ().ToString ());
			destination.DidEnter (prev);
			if(OnStateChanged != null)
				OnStateChanged (prev, destination);
		}

		public void UpdateCurrentState() {
			if (CurrentState != null) {
				State dest = CurrentState.DestinationState ();
				CurrentState.DidTryTransition ();
				if (dest != null) {
					SwitchState (dest);
				}
			}
		}

		public delegate void StateChangedAction(State fromS, State toS);
		public event StateChangedAction OnStateChanged;
	}

}