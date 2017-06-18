using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT.Character;

namespace Dnd5eTest {
	public class TestGauge :  TestModule {

		public  TestGauge() : base() {
			Test ("that base modifiers are counted as base", () => {
				Gauge g = new Gauge("testing");
				g.ChangeCurrentAndMax(10);
				int before = g.BaseValue;

				BaseModifier m = new BaseModifier(5);

				g.Modify(m);


				Assert((before + 5) == g.BaseValue);
				Assert(5 == g.BaseModifierSum);
				Assert(0 == g.ModifierSum);
			});
		}
	}

}