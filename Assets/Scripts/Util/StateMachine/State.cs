using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AT.Character;

namespace Util.StateMachine {
	public delegate bool TransitionAction(State state);

	public class Transition {
		TransitionAction doTransition;
		public State start;
		public State destination;
		public Transition(TransitionAction doTransition, State start, State destination) {
			this.doTransition = doTransition;
			this.start = start;
			this.destination = destination;
		}

		public bool ShouldTransition {
			get {return this.doTransition (start); } 
		}
	}



	public class State  {

		public Controller controller;

		public State(Controller c) {
			this.controller = c;
			transitions = new List<Transition> ();
		}



		private List<Transition> transitions;
		public void AddTransition(TransitionAction action, State destination) {
			if (transitions == null)
				transitions = new List<Transition> ();
			transitions.Add (new Transition(action, this, destination));
		}


		public delegate void EnteredAction(State s, State fromPrevious);
		public event EnteredAction OnDidEnter;
		public void DidEnter(State fromPrevious) {
			if (OnDidEnter != null) {
				OnDidEnter (this, fromPrevious);
			}
		}

		public delegate void WillExitAction(State s, State toDestination);
		public event WillExitAction OnWillExit;
		public void WillExit(State toDestination) {
			if (OnWillExit != null) {
				OnWillExit (this, toDestination);
			}
		}


		public delegate void TransitionAttemptAction (State s);
		public event TransitionAttemptAction OnAttemptTransition;

		public void DidTryTransition() {
			if (OnAttemptTransition != null) {
				OnAttemptTransition (this);
			}
		}



		public State DestinationState() {
			
			foreach (Transition t in transitions) {
				if (t.ShouldTransition)
					return t.destination;
			}
			return null;
		}


	}



}