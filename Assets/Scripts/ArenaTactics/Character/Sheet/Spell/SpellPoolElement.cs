using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AT.Character {
	

	[System.Serializable]
	public class SpellPoolElement {

		public ClassType ClassType { get; set; }
		public SpellLibrary.SpellName SpellName { get; set;}
		public bool IsPrepared {
			get;
			set;
		}


	}
}
