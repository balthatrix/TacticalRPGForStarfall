using UnityEngine;
using System.Collections;
using AT.Battle;
using System.Collections.Generic;
using AT.Character;

namespace AT.Character.Condition {


	public class DualWieldAttacking : DurationCondition {


		bool addOffhand;
		public DualWieldAttacking(bool offhand=true) : base() {
			this.addOffhand = offhand;
		}

		public override void ApplyTo(Sheet c) {
			base.ApplyTo (c);

			if (addOffhand) {
				c.OnProduceBonusActions += AddOffhandAttack;
			} else { 
				c.OnProduceBonusActions += AddMainhandAttack;
			}
		}


		public override void RemoveFrom(Sheet c) {
			base.RemoveFrom (c);

			c.OnProduceBonusActions -= AddOffhandAttack;
			c.OnProduceBonusActions -= AddMainhandAttack;
		}


		private void AddOffhandAttack(Actor actor, List<AT.Battle.Action> actions) {
			
			AT.Battle.Action a = new AT.Battle.Attack ();
			a.IsBonus = true;
			a.ActionOptions [0].choiceFilters.Add((List<IActionOptionChoice> choices, Action action) => {
				List<IActionOptionChoice> ret = new List<IActionOptionChoice>();
				foreach(IActionOptionChoice choice in choices) {
					if((choice as AttackTypeChoice).IsOffhand())
						ret.Add(choice);
				}
				return ret;
			});
			actions.Add (a);
		}


		private void AddMainhandAttack(Actor actor, List<AT.Battle.Action> actions) {
			AT.Battle.Action a = new AT.Battle.Attack ();
			a.IsBonus = true;
			a.ActionOptions [0].choiceFilters.Add((List<IActionOptionChoice> choices, Action action) => {
				List<IActionOptionChoice> ret = new List<IActionOptionChoice>();
				foreach(IActionOptionChoice choice in choices) {
					if(!(choice as AttackTypeChoice).IsOffhand())
						ret.Add(choice);
				}
				return ret;
			});
			actions.Add (a);
		}

		public override string Tag() {
			return "";
		}
	}
}