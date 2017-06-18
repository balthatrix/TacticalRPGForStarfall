using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using AT.Serialization;

namespace AT.Character {
	public class ClassLevel5e : FeatureBundle, TooltipDescriptable {
		public ClassType classType;
		int level;

		public static ClassFeatureTable table = new ClassFeatureTable();
		public ClassLevel5e(ClassType t, int level, List<GenericFeature> features=null) : base(features) {
			classType = t;

			this.level = level;
		}

		public string DebugFeatures() {
			string j = "";
			foreach (GenericFeature f in features) {
				j += f.Name () + ", ";
			}
			return j;
		}

		public void InitDefaultFeatures() {
			features = ClassLevel5e.table.GetFeatures (classType, level);

			foreach (GenericFeature f in features) {
				SetSelfAsParentTo (f);
			}
		}

		public void SetSelfAsParentTo(GenericFeature f) {
			f.parent = this;
//			if (f is FeaturePointer) {
//				FeaturePointer ptr = (FeaturePointer)f;
//				foreach (GenericFeature child in ptr.pool) {
//					SetSelfAsParentTo (child);
//				}
//			} 
		}

		public string PresentableType() {
			return classType.ToString ().ToLower();
		}



		public static string Description(ClassType t) {
			string ret = "ERROR";
			switch (t) {
			case ClassType.FIGHTER:
				ret = FighterDescription ();
				break;
			}
			return ret;
		}


		public string TooltipHoverText() {
			int lvl = level;
			if(lvl == 0) lvl++;
			string hover = (Util.UtilString.EnumToReadable<ClassType>(classType)+  " lvl " + lvl + "\n");
			foreach(GenericFeature feat in features) {
//				if (!feat.IsMisc)
//					continue;
				hover += "+" + feat.Name () + "\n";

			}

//			hover.Remove (hover.Count() - 1);
			return hover;
		}

		public string TooltipMoreDetails() {
			return ""; //this returns the fighter description
		}



		//Should represent the entire fighter.
		public static string FighterDescription() {
			string ret =  "Fighters are unmatched in raw, physical combat, and can choose to specialize in a variety of weapon styles.  They live and die by the edge of their weapons, and the strength of their arms.";
			ret += "\n\n-----Hit Points-----\n";
			ret += "1st Level: \n10 + Constitution Modifier\n";
			ret += "2nd+ Level:\n6 + Constitution Modifier\n";

			ret += "-----Hit Dice-----\n";
			ret += "1d10 per level\n";


			ret += "\n----Proficiencies----\n";
			ret += "Saves: ";
			ret += "Strength, Constitution\n\n";
			ret += "Weapons: ";
			ret += "All Simple and Martial\n\n";
			ret += "Armour: ";
			ret += "All Armour, Shields\n\n";
//			ret += "Skills: ";
//			ret += "Choose two skills from Acrobatics, Animal Handling, Athletics, History, Insight, Intimidation, Perception, and Survival\n\n";
			return ret;
		}


		//SERIALIZATION
		public override Wrapper GetSerializableWrapper ()
		{
			
			ClassLevel5eWrapper wrap = new ClassLevel5eWrapper (classType, level, features);


			List<GenericFeature> serialized = new List<GenericFeature> ();

			foreach (GenericFeature f in wrap.features) {
				if (GenericFeature.ShouldNotSerialize(f)) 
					continue;


				serialized.Add (f);
			}

			wrap.featureWraps = serialized.Select((f)=>f.GetSerializableWrapper()).ToArray();

			return wrap;
		}



	}

	[System.Serializable]
	public  class ClassLevel5eWrapper : Wrapper {

		public Wrapper[] featureWraps;
		public int level;
		public ClassType classType;

		[System.NonSerialized]
		public  List<GenericFeature> features;



		public ClassLevel5eWrapper(ClassType classType, int level, List<GenericFeature> features) {
			this.level = level;
			this.classType = classType;
			this.features = features;
		}



		public override SerializedObject GetInstance ()
		{
			features = new List<GenericFeature> ();
			foreach (Wrapper f in featureWraps) {

				GenericFeature fe = (GenericFeature)f.GetInstance ();
				features.Add (fe);


			}


			//each feature needs to de serialize....
			return new ClassLevel5e(classType, level, features.ToList());
		}
	}


	public class ClassFeatureTable {
		public Dictionary<ClassType, Dictionary<int, List<GenericFeature>>> table;

		public ClassFeatureTable() {
			table = new Dictionary<ClassType, Dictionary<int, List<GenericFeature>>>();
			PopulateTable ();
		}

		public virtual void  PopulateTable() {
			table.Clear ();




			table.Add (ClassType.FIGHTER, FighterColumn());
			table.Add (ClassType.WIZARD, WizardColumn());
		}

		public Dictionary<int, List<GenericFeature>> FighterColumn() {
		
			Dictionary<int, List<GenericFeature>> fighterColumn = new Dictionary<int, List<GenericFeature>> ();


			//this leaves 1 for multi-classing fighters
			fighterColumn.Add (0,FighterLevels.Level1SingleClass());
			fighterColumn.Add (2,FighterLevels.Level2());
			return fighterColumn;

		}

		public Dictionary<int, List<GenericFeature>> WizardColumn() {

			Dictionary<int, List<GenericFeature>> wizardColumn = new Dictionary<int, List<GenericFeature>> ();


			//this leaves 1 for multi-classing wizards
			wizardColumn.Add (0,WizardLevels.Level1SingleClass());
			wizardColumn.Add (2,WizardLevels.Level2());
			return wizardColumn;

		}






		//TODO: Make this add in spell slot features depending on level....
		public virtual List<GenericFeature> GetFeatures(ClassType type, int level) {
			Dictionary<int, List<GenericFeature>> classFeatures = null;
			if (table.TryGetValue (type, out classFeatures)) {

				List<GenericFeature> features = null;
				if (classFeatures.TryGetValue (level, out features)) {
					List<GenericFeature> cp = features.ToList ();
					if (level == 0 && type == ClassType.WIZARD) {
						cp.Add (new SpellSlotFeature (1));
						cp.Add (new SpellSlotFeature (1));
						cp.Add (new SpellSlotFeature (3));
						cp.Add (new SpellSlotFeature (2));
					}
					return cp;
				}
			}
			Debug.LogError ("Failed to get the features!");
			return null;
		}


	}

	public enum ClassType {
		FIGHTER,
		ROGUE,
		WIZARD,
		CLERIC
	}



}