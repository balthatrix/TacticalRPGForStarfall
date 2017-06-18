using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AT.Character;
using System.Linq;
using AT.Character.Situation;
using AT.Serialization;

namespace AT.Character {

	public class  WizardLevels {
		public static List<GenericFeature> Level1SingleClass() {

			List<GenericFeature> lvl0 = new List<GenericFeature> ();
			//lvl1.Add (new Features.FightingStyle());

			lvl0.Add(new PerLevelHitPoints(6, ClassType.WIZARD));



			//Add all simple
			lvl0.Add(new WeaponProficiencyFeature(EquipmentSubtype.SIMPLE_DAGGER, ProficiencyLevel.FULL));



			//saves
			lvl0.Add (new SaveProficiencyFeature (AT.Character.Situation.AbilityType.INTELLIGENCE, ProficiencyLevel.FULL));
			lvl0.Add (new SaveProficiencyFeature (AT.Character.Situation.AbilityType.WISDOM, ProficiencyLevel.FULL));

			lvl0.Add (new SpellKnownOfClassLvl (0, ClassType.WIZARD));
			lvl0.Add (new SpellKnownOfClassLvl (0, ClassType.WIZARD));
			lvl0.Add (new SpellKnownOfClassLvl (1, ClassType.WIZARD));


			lvl0.Add (new SpellPreparedOfClass (ClassType.WIZARD));
//			lvl0.Add (new SpellPreparedOfClass (ClassType.WIZARD));


			lvl0.Add (new HitDiceFeature (6));


			return lvl0;
		}

		public static List<GenericFeature> Level2() {

			List<GenericFeature> lvl2 = new List<GenericFeature> ();

			lvl2.Add(new PerLevelHitPoints(4, ClassType.WIZARD));

			lvl2.Add (new HitDiceFeature (10));

			return lvl2;
		}



		public class SpellKnownOfClassLvl : FeaturePointer {
			public SpellKnownOfClassLvl(int lvl, ClassType classType) : base(Util.UtilString.EnumToReadable<ClassType>(classType) + " lvl " + lvl,  new List<GenericFeature>()) {
				//no two-handed weapons, so commenting these until there are bows and 2her animations and assets.
				//					pool.Add(new ArcheryFightingStyle());
				//					pool.Add(new GreatWeaponFightingStyle());
				//
				if(SpellLibrary.Instance == null) {
					throw new UnityEngine.UnityException("spell library is missing!  You need an instance in the scene. ");
				}
				foreach(SpellLibrary.Spell spell in SpellLibrary.Instance.SpellsByClassAndLevel(classType, lvl)) {
					pool.Add(new SpellKnownFeature(spell.name, spell.classType));
				}

				filterPool = (Sheet character) => {
					List<GenericFeature> ret = new List<GenericFeature> ();
					List<SpellLibrary.SpellName> spellsAlreadyGot = character
						.spellPool
						.Select((s)=>s.SpellName).ToList();
					
					foreach (GenericFeature gf in pool) {
						SpellKnownFeature skf = gf as SpellKnownFeature;
						if(spellsAlreadyGot.Contains(skf.spellName)){
							//dont add...
						} else {
							ret.Add(gf);
						}
					}
					return ret;
				};

			}
			public override string Name(){
				return "Spell Known of Level";
			}

			public override string Description ()
			{
				return string.Format ("[SpellKnownOfLvl]");
			}
		}

		public class SpellPreparedOfClass : FeaturePointer {
			public SpellPreparedOfClass(ClassType classType) : base(Util.UtilString.EnumToReadable<ClassType>(classType) + " spell preparation",  new List<GenericFeature>()) {
				//no two-handed weapons, so commenting these until there are bows and 2her animations and assets.
				//					pool.Add(new ArcheryFightingStyle());
				//					pool.Add(new GreatWeaponFightingStyle());
				//
				if(SpellLibrary.Instance == null) {
					throw new UnityEngine.UnityException("spell library is missing!  You need an instance in the scene. ");
				}

				//filter pool here is used to actually just populate options
				filterPool = (Sheet character) => {
					

					List<GenericFeature> ret = character
						.spellPool
						.Where((elem)=>!elem.IsPrepared &&
							elem.ClassType == classType && 
							!SpellLibrary.Instance.SpellByClassAndName(classType, elem.SpellName).isCantrip)
						.Select((elem)=>new SpellPreparedFeature(elem) as GenericFeature).ToList();
					


					return ret;
				};

			}
			public override string Name(){
				return "Spell Preparation";
			}

			public override string Description ()
			{
				return string.Format ("[Preparation]");
			}
		}




	} // Wizard features...

}