using UnityEngine;
using System.Collections;
using AT.Battle;

namespace AT.Character.Effect {

	//Responsible for applying damage, and conditions
	public interface IEffect : ProvidesBattleTranscript {
		void ApplyTo(AT.Character.Sheet c, AT.Battle.Action source);
		void FailedAgainst(AT.Character.Sheet c);
//		string GetTranscript();
	}

	public abstract class GenericEffect : IEffect {
		private  bool nullified = false;
		private string nullifiedReason;
		public string NullifiedReason {
			get { return nullifiedReason; }
		}
		public bool Nullified {
			get { return nullified; }
		}
		public void Nullify(string reason) {
			nullifiedReason = reason;
			nullified = true;
		}


		public virtual void ApplyTo(AT.Character.Sheet c, AT.Battle.Action source) {
			Debug.LogError ("You should override ApplyTo GenericEffect");
		}

		public virtual void FailedAgainst(AT.Character.Sheet c) {
			Debug.LogError ("You should override FailedAgainst GenericEffect");
		}

		public virtual string GetTranscript() {
			Debug.LogError ("You should override GetTranscript GenericEffect");
			return "ERROR";
		} 
	}
}
