using UnityEngine;
using System.Collections;
using AT.Battle;

namespace AT.Character.Condition {
	public interface ICondition {
		void ApplyTo(Sheet c);
		void RemoveFrom(Sheet c);
		string Tag();

		Sprite SpriteIcon ();
	}

	public enum TickType {
		TURN_END,
		TURN_BEGIN,
		ROUND_END
	}



	public class DurationCondition : ICondition {
		int roundsDuration;
		int timeOn;
		TickType tickType;
		Sheet charCached;
		Sheet tickHost;
		public DurationCondition(int roundsDuration = 1, TickType tickType=TickType.TURN_BEGIN, Sheet tickHost=null) {
			this.roundsDuration = roundsDuration;
			this.tickType = tickType;
			this.tickHost = tickHost;
			timeOn = 0;
		}

		public virtual void ApplyTo(Sheet c) {
			Sheet th = (tickHost != null) ? tickHost : c;


			charCached = c;
			switch (tickType) {
			case TickType.TURN_END:
				th.OnTurnEnded += Tick;
				break;
			case TickType.TURN_BEGIN: 
				th.OnTurnBegan += Tick;
				break;
			case TickType.ROUND_END:
				if (BattleManager.instance != null) {
					
					BattleManager.instance.OnRoundEnded += TickRoundEnd;
				} else {
					Debug.LogError ("No battle manager instance to tick " + this.Tag());
				}
				break;
			}
		}

		void Tick(Actor c) {
			timeOn++;
			if (timeOn >= roundsDuration) {
				RemoveFrom (charCached);
			}
		}

		void TickRoundEnd() {
			timeOn++;
			if (timeOn >= roundsDuration) {
				RemoveFrom (charCached);
			}
		}




		Sprite icon;
		public virtual Sprite SpriteIcon() {
			return null;
		}


		public virtual  void RemoveFrom(Sheet c) {
			
			c.OnTurnEnded -= Tick;
			c.DidRemoveCondition (this);
		}

		public virtual string Tag() {
			return "Generic Duration Condition";
		}
	}




}