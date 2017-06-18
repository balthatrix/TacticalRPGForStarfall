using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;


namespace AT {
	namespace Battle {

		public interface ProvidesBattleTranscript {
			/// <summary>
			/// This should return the value that should output to the battle log at whatever point
			/// examples, are attack situations, damage effects, and actions.
			/// It should not assume that the character name can be gotten internally.  
			/// </summary>
			/// <returns>The transcript.</returns>
			string GetTranscript();
		}

		public interface Descriptable {
			string GetDescription();
		}
		
		public class Action : IActionOptionChoice, ProvidesBattleTranscript, Descriptable {
			private bool isAsReaction;
			private bool isAsBonus;
			private bool isAWait;
			private bool isAnInteraction;

			public bool skipAnimation = false;
			public Actor actor;



			public List<ActionTargetTileParameter> actionTargetParameters;

			public Action(Actor actor=null) {
				this.actor = actor;
				//opts create the split between real world and local opts.
				//such as: hitting with main hand or off if there is offhand ability...
				actionOpts = new List<ActionOption> ();
				actionTargetParameters = new List<ActionTargetTileParameter> ();
				UseLimitReachedUntilLongRest = false;
				UseLimitReachedUntilShortRest = false;
			}

			public virtual string GetDescription() {
				Debug.LogWarning (GetType () + " didn't override description.");
				return "[make description]";
			}

			public virtual void DecorateOption(ActionButtonNode n) {
//				n.spriteLabel = IconDispenser.instance.SpriteFromIconName (AT.Character.InventoryIconName.CHAIN_MAIL);
//				n.cornerSpriteLabel = IconDispenser.instance.SpriteFromIconName (AT.Character.InventoryIconName.DAGGER);
				Debug.LogWarning (GetType () + " didn't override decorate option.");
			}

			//Most actions will just have one target tile parameter, but will support multiple.  
			/// <summary>
			///  calculates the action target tile parameters.  Allows dynamic target tile parameter calculation
			/// use case story: to calculate action bar tree
			/// -calculate tree of action options. 
			/// -action target tiles are created, using the options sometimes to filter, or create ranges.
			/// 
			/// -Used for attacks, where the target parameter is not really known until the options are set... which will determine parameter range.
			/// </summary>
			/// <returns>The action target tile parameters.</returns>
			public virtual void LateSetTargetParameters () {
				
			
			}

			public virtual string GetTranscript() {
				return this.GetType ().ToString();
			}



			private List<ActionOption> actionOpts;
			public List<ActionOption> ActionOptions {
				get { return actionOpts; }
			}

			public virtual void Perform() {
				
			}


			public virtual bool IsReaction {
				get { return isAsReaction; }
				set { isAsReaction = value; }
			}

			public virtual bool IsWait {
				get { return isAWait; }
				set { isAWait = value; }
			}

			//attack overrides this....
			public virtual bool IsAttack {
				get { return false; }
			}

			//move overrides this....
			public virtual bool IsMove {
				get { return false; }
			}

			public virtual bool IsBonus {
				get { return isAsBonus; }
				set { isAsBonus = value; }
			}

			public virtual bool IsInteraction {
				get { return isAnInteraction; }
				set { isAnInteraction = value; }
			}

			/// <summary>
			/// Flagged by features or spell actions to 
			/// </summary>
			/// <value><c>true</c> if use limit reached; otherwise, <c>false</c>.</value>
			public bool UseLimitReachedUntilShortRest {
				get;
				set; 
			}
			public bool UseLimitReachedUntilLongRest {
				get;
				set; 
			}

			public string ValueLabel() {
				//gets rid of the battle namespace.
				char[] delim = new char[] {'.'};
				string[] split = GetType ().ToString ().Split(delim); 
				return split[split.Length - 1];
			}



			public ActionButtonNode GetOptionTreeRoot() {
				return GetOptionTreeRoot (actor);
			}

			public ActionButtonNode GetOptionTreeRoot(Actor actor) {
				ActionButtonNode root = new ActionButtonNode ();
				root.label = ValueLabel ();
				DecorateOption (root);
				root.choice = this;
				PopulatePathChildren(root, ActionOptions.ToList(), actor);


	            if (UseLimitReachedUntilLongRest) {
	                root.disabled = true;
	                root.disabledReason = "You have to wait until a long rest to use this again.";
	            }

				return root;
			}

			public bool OptionsFilled {
				get { 
					foreach(ActionOption opt in ActionOptions) {
						if (opt.chosenChoice == null)
							return false;
					}
					return true;
				}
			}

			/// <summary>
			/// Determines whether this action instance can fill parameters.
			/// This assumes that all options have been filled. 
			/// </summary>
			/// <returns><c>true</c> if this instance can fill parameters; otherwise, <c>false</c>.</returns>
			public bool CanFillParams (Actor actor) {
				if (OptionsFilled) {
					LateSetTargetParameters ();
					foreach (ActionTargetTileParameter param in actionTargetParameters) {
						if (!param.CanBeFilled()) {

							return false;
						}
					}
					return true;
				} else {
					Debug.LogError ("Can check if can fill params when options are not filled.");
					return false;
				}
			}

			private string lastLackOfTargetReason = null;
			public string LastLackOfTargetReason {
				get { 
					if (lastLackOfTargetReason == null) {
						return "No potential targets.";
					}
					return lastLackOfTargetReason; 
				}
			}

			public void PopulatePathChildren(ActionButtonNode parent, List<ActionOption> optionsLeft, Actor actor) {
				//this is a leaf state of the action in the following case. 
				//Now check to see if there are any target leaves it leads to for the purpose of setting the parent disabled
				if (optionsLeft.Count == 0) {
					Actor theActor = null; //need to change our character for an actor instead....
					//dead end, can't fill params at this option leaf.  Set parent disabled, and reason to "cannot fil prams"
					if (!CanFillParams (theActor)) {
						parent.disabled = true;
						parent.disabledReason = LastLackOfTargetReason;
					}
					return;
				}

				ActionOption nextOne = optionsLeft.First ();
				optionsLeft.Remove (nextOne);

				List<IActionOptionChoice> choices = nextOne.GetChoices (actor, this);

				//set parent disabled.  
				if (choices.Count <= 0) {
					parent.disabled = true;

					parent.disabledReason = nextOne.LastNoChoiceReason;
				}

				foreach (IActionOptionChoice choice in choices) {
					ActionButtonNode child = new ActionButtonNode ();
					child.label = choice.ValueLabel ();
					child.choice = choice;
					choice.DecorateOption (child);

					parent.AddChild (child);

					//imitate the next choice being chosen, so GetChoices works correctly...
					nextOne.chosenChoice = choice;
					PopulatePathChildren (child, optionsLeft.ToList(), actor);
					nextOne.chosenChoice = null;
				}
			}

			public void CheckForLeaves(
				ActionButtonNode parent, 
				List<ActionOption> optionsLeft, 
				Actor actor, 
				CheckInOptionLeafAction checkIn, 
				DeadEndOptionLeafAction deadEnd) 
			{
				if (optionsLeft.Count == 0 && parent.PathLengthToRoot == ActionOptions.Count) //this is a leaf state
				{
					checkIn (parent);
					return;
				}

				ActionOption nextOne = optionsLeft.First ();
				optionsLeft.Remove (nextOne);

				//here there could be a check for 0 choices on the options, and a way to "get a reason explanation" for the pat dead end
				if(nextOne.GetChoices(actor, this).Count == 0) {
					deadEnd (parent, nextOne.LastNoChoiceReason);
					return;
				}

				foreach (IActionOptionChoice choice in nextOne.GetChoices (actor, this)) {
					ActionButtonNode child = new ActionButtonNode ();
					child.label = choice.ValueLabel ();

					//imitate the next choice being chosen, so GetChoices works correctly...
					nextOne.chosenChoice = choice;
					CheckForLeaves (child, optionsLeft.ToList(), actor, checkIn, deadEnd);
					nextOne.chosenChoice = null;
				}
			}

			public  delegate void CheckInOptionLeafAction(ActionButtonNode leaf);
			public  delegate void DeadEndOptionLeafAction(ActionButtonNode leaf, string reason);


			public bool CanFillOptions(Actor actor) {
				ActionButtonNode root = new ActionButtonNode ();
				root.label = ValueLabel ();
				bool canFill = false;

				CheckInOptionLeafAction checkin = (leaf) => {
					Debug.Log("found a leaf!: " + leaf.label);
					//any leaf will do...
					canFill = true;
				};

				DeadEndOptionLeafAction deadend = (leaf, reason) => {
					Debug.Log("Dead end: " + leaf.label);
					Debug.Log("reason: " + reason);
				};


				CheckForLeaves(root, ActionOptions.ToList(), actor, checkin, deadend);

				return canFill;

			}


			/// <summary>
			/// Gets the reason that this action cannot be used by an actor, if there is one.
			/// -Checks if the actor has already used this type of action (assumes this action has it's type set: isAttack, isBonus, isFree)
			/// -Checks if the action can fill it's options.  returns "no choices for <optionType>" on the first option that cant be filled
			/// -Checks if the action can fill it's target tile parameters.  return "no suitable <targetName>" for the first parameter 
			/// </summary>
			/// <returns>The reason cannot use.</returns>
			/// <param name="a">The alpha component.</param>
	//		public virtual string GetReasonCannotUse(Actor a) {
	//		}



	//		public virtual new bool CanBeUsedBy(Actor a, ) {
	//			AttackTypeOption opt = ActionOptions [0] as AttackTypeOption;
	//			foreach(ActionOption opt in ActionOptions) {
	//
	//				foreach(AttackTypeChoice c in opt.GetChoices(a.CharSheet, this)) {
	//					opt.chosenChoice = c;
	//					foreach (ActionTargetTile potentialParam in GetActionTargetTileParameters()) {
	//						if (potentialParam.CanBeFilled ())
	//							return true;
	//					}
	//				}
	//			}
	//
	//			return false;
	//		}


	//
			public List<ActionTargetTileParameter> CleanListOfParameters() {
				foreach (ActionTargetTileParameter ap in actionTargetParameters) {
					ap.Reset ();
					ap.OnTargetTileChosen -= FillNextParameter;
				}
				return actionTargetParameters.ToList ();
			}

			private List<ActionTargetTileParameter> toFill;
			private ActionTargetTileParameter prev;
			public virtual void FillNextParameter(ActionTargetTileParameter ap = null) {
				if (toFill == null) {
					toFill = CleanListOfParameters();
				}

				//if actor is a cpu controlled actor, do
				//ai fill params....

				if (prev != null) {
					prev.StopListen ();
					prev.OnTargetTileChosen -= FillNextParameter;
				}


				if (toFill.Count > 0) {
					ActionTargetTileParameter next = toFill [0];
					if (OnWillFill != null) {
						OnWillFill (this, next);
					}
					toFill.RemoveAt (0);
					next.OnTargetTileChosen += FillNextParameter;
					prev = next;
					next.Listen ();
				} else {
					CallOnParamsFilled ();
				}
			}

			public delegate void WillFillAction(Action self, ActionTargetTileParameter ap);
			public event WillFillAction OnWillFill;



			public virtual void CancelUiListen() {
				if (prev != null) {
					//prev.Reset ();
					prev.OnTargetTileChosen -= FillNextParameter;
					prev = null;
				}

				foreach (ActionTargetTileParameter ap in actionTargetParameters) {		
					ap.StopListen ();
				}
			}


			public void CallOnParamsFilled() {
				toFill = null;
				if (OnParamsFilled != null) {
					OnParamsFilled (this);
				}
			}

			public delegate void ParamsFilled(Action a);
			public event ParamsFilled OnParamsFilled;
			public void ParamsWereFilled() {
				if (OnParamsFilled != null) {
					OnParamsFilled (this);
				}
			}

			public delegate void BeganOccurred(Action a);
			public event BeganOccurred OnBegan;
			public void CallOnBegan() {

				if (OnBegan != null) {
					OnBegan (this);
				}

				actor.CallOnWillPerform (this);
				actor.CurrentlyPerforming = this;
			}

			public delegate void FinishedOccurred(Action a);
			public event FinishedOccurred OnFinished;
			public void CallOnFinished() {
				if (OnFinished != null) {
					OnFinished (this);
				}

				actor.CurrentlyPerforming = null;
				actor.CallOnDidPerform (this);
			}

		}








	}
}