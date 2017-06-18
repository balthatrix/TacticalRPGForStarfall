using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util.StateMachine;
using Util.StateMachine;

namespace AT.Battle {
	public class PhaseController : Controller {
		public  BattleManager battleManager;

		public BattleBegin battleBegin;
		public RoundBegin roundBegin;
		public CharacterAction characterAction;
		public CharacterTurnEnd characterTurnEnd;
		public CharacterTurnBegin characterTurnBegin;
		public ActionTargetTileChoice actionTargetTileChoice;
		public InventoryInteractionPhase inventoryInteraction;
		public BattleEnd battleEnd;

		public PhaseController(BattleManager driver) {
			this.battleManager = driver;
			this.battleBegin = new BattleBegin (this);
			this.roundBegin = new RoundBegin (this);
			this.characterAction = new CharacterAction (this);
			this.characterTurnBegin = new CharacterTurnBegin (this);
			this.characterTurnEnd = new CharacterTurnEnd (this);
			this.actionTargetTileChoice = new ActionTargetTileChoice (this);
			this.inventoryInteraction = new InventoryInteractionPhase (this);
			this.battleEnd = new BattleEnd (this);
		}


		public override void SwitchState(State toDest) {
			if(CurrentState != battleEnd) {
				base.SwitchState (toDest);
			}
		}

	}

}