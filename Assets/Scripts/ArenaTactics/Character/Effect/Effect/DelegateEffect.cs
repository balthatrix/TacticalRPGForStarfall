namespace AT.Character.Effect {
	public class DelegateEffect : GenericEffect {
		public delegate void EffectRoutine(Sheet cw, AT.Battle.Action source);
		EffectRoutine routine;
		string transcript;
		public DelegateEffect(EffectRoutine routine, string transcript="some effect") {
			this.routine = routine;
			this.transcript = transcript;
		}

		public override void ApplyTo(AT.Character.Sheet c, AT.Battle.Action source) {
			routine.Invoke (c, source);
		}

		public override void FailedAgainst(AT.Character.Sheet c) {
			
		}

		public override string GetTranscript() {
			return transcript;
		} 
	}
}