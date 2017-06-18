using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT.Battle;
using AT.Character;

public class CharacterBehaviorLogging : MonoBehaviour {

	// Use this for initialization
	private Actor actor;

	void Start () {
		actor = GetComponent<Actor> ();

		actor.OnWillPerform += (Action a) => {
			LogForCharacter(a);
		};

		actor.CharSheet.OnDamaged += (effect, source) => {
//			Debug.Log(effect.GetTranscript());
			LogForCharacter(effect); 
		};

		actor.CharSheet.OnHealed += (effect, source) => {
			LogForCharacter(effect);
		};

		actor.CharSheet.OnDidAttack += (AT.Character.Situation.AttackSituation attSit) => {
			LogForCharacter(attSit);
		};
	}

	void LogForCharacter(ProvidesBattleTranscript pbt) {
		if (actor.IsOnPlayerSide || actor.IsSeenByAPlayer) {
			if (pbt == null) {
				Debug.LogError ("null battle trn");
				return;
			}
		
			if (actor == null) {
				Debug.LogError ("null actor!");
				return;
			}
			BattleLog.instance.Log (actor.CharSheet.Name + ": " + pbt.GetTranscript ());
		}
	}

}
