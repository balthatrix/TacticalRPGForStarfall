using UnityEngine;
using System.Collections;
using AT.Character;

using System.Collections.Generic;
using System.Linq;




namespace AT.Battle {

	//Should be used by items, equipment, etc...
	public interface IActionOptionChoice {
		string ValueLabel();
		//returns true if it did some decorating, false otherwise.
		void DecorateOption (ActionButtonNode optButtonChoice);
	}

	/// <summary>
	/// Represents a grouping of choices, internal to the entity making an action.  This is distinct from target, which is similar, but external to the entity making the action.
	/// examples include 
	/// AttackType -> attacking with main/offhand melee/thrown ranged (5 different permutations)
	/// TargetItem -> what item to do something with (consume? give? throw?)
	/// </summary>
	public class ActionOption {

		public ActionOption() {
			choiceFilters = new List<FilterChoiceAction> ();
		}
		/// <summary>
		///Gets the choices available to a sheet character
		/// </summary>
		/// <returns>The choices.</returns>
		/// <param name="character">Character who is acting.</param>
		/// /// <param name="action">Running action used to determine potential targets/opts down the line.</param>
		public virtual List<IActionOptionChoice> GetChoices (Actor actor, Action action) {
			List<IActionOptionChoice> ret = GetChoicesUnfiltered(actor, action);
			foreach (FilterChoiceAction filter in choiceFilters) {
				ret = filter (ret, action);
			}
			return ret;
		}

		public virtual List<IActionOptionChoice> GetChoicesUnfiltered (Actor actor, Action action) {
			return new List<IActionOptionChoice> ();
		}

		public delegate List<IActionOptionChoice> FilterChoiceAction(List<IActionOptionChoice> choices, Action action);
		public List<FilterChoiceAction> choiceFilters;

		/// <summary>
		/// Its the implementation of action option's responsibility to set this, based on why there are no choices.
		/// </summary>
		public string lastReasonForNoChoices = "";
		public string LastNoChoiceReason {
			get {return lastReasonForNoChoices;}
		}

		/// <summary>
		/// The choice made by the entity taking the action.
		/// </summary>
		public IActionOptionChoice chosenChoice;

		public bool CanChoose(Actor actor, Action action) {
			return GetChoices(actor, action).Count > 0;
		}
	}

}