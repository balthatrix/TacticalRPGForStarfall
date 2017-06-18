using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AT.Serialization;

namespace AT.Character {
	public interface TooltipDescriptable {
//		string Show(RectTransform elem);

		//option moused over shows this text
		string TooltipHoverText();
		//right click the thing for more details
		string TooltipMoreDetails();
	}

	public enum RaceName {
		TIEFLING,
		HUMAN,
		HALF_ORC,
		ELF,
		HALF_ELF,
		GNOME,
		HAFLING,
		DRAGONBORN,
		DWARF
	}

	[System.Serializable]
	public class RaceWrapper : Wrapper {
		//speed is set with speed table
		//public Wrapper speed;
		public RaceName name;

		public RaceWrapper() {
			
		}

		public override SerializedObject GetInstance() {
			Race ret = new Race(name);

			return ret;
		}

	}

	/// <summary>
	/// Represents an aggregation of traits under a particular race.
	/// </summary>
	public class Race : FeatureBundle, Tooltipable {

		public Wrapper GetSerializableWrapper() {
			RaceWrapper wrap = new RaceWrapper ();

			//speed is set with speed table
			//wrap.speed = speed.GetSerializableWrapper ();

			wrap.name = name;


			return wrap;
		}

		public virtual void DressOptButtonForTooltip (OptButton opt, Tooltip.TooltipPosition pos, int offset = 10) {
			opt.SetTooltipInfo (pos, offset, ShortDescription(this.name), Description (this.name));
		}


		public RaceName name;
		public Gauge speed;

		public Race(RaceName name) : base(null) {
			ApplyRaceTables (this, name);
		}

		public EquipmentAnimationControllerName BodyAnimationOverride {
			get;
			set;
		}

		public string PresentableName() {
			return name.ToString ().ToLower();
		}



		public static void ApplyRaceTables(Race race, RaceName name) {
			race.name = name;
			race.speed = SpeedTable (name);
			race.features = FeaturesFromName (name);
			race.BodyAnimationOverride = BodyOverrideController (name);
		}

		public static EquipmentAnimationControllerName BodyOverrideController(RaceName name) {
			switch (name) {
			case RaceName.HALF_ORC:
//				Debug.LogError ("seting");
				return EquipmentAnimationControllerName.ORC_OVERRIDE;
				break;
			default:
//				Debug.LogError ("NOT seting");
				return EquipmentAnimationControllerName.NOT_SET;
			}

		}




		public static List<GenericFeature> FeaturesFromName(RaceName race) {
			List<GenericFeature> ret = new List<GenericFeature>();
			switch (race) {
			case RaceName.TIEFLING:
				ret =  TieflingFeatures ();
				break;
			case RaceName.HUMAN:
				ret =  HumanFeatures ();
				break;
			case RaceName.HALF_ORC:
				ret =  HalfOrcFeatures ();
				break;
			default: 
				break;
			} 

			return ret;
		}

		public static List<GenericFeature> TieflingFeatures() {
			List<GenericFeature> ret = new List<GenericFeature> ();
			//Initial creation


			string modName = "Tiefling";

			ret.Add(new GaugeMod("intelligence", 1, modName,true));
			ret.Add(new GaugeMod("charisma", 2, modName,true));

			return ret;
		}

		public static List<GenericFeature> HumanFeatures() {
			List<GenericFeature> ret = new List<GenericFeature> ();
			//Initial creation

			string modName = "Human Balance";
			ret.Add(new GaugeMod("strength", 1, modName,true));
			ret.Add(new GaugeMod("dexterity", 1, modName,true));
			ret.Add(new GaugeMod("constitution", 1, modName,true));
			ret.Add(new GaugeMod("intelligence", 1, modName,true));
			ret.Add(new GaugeMod("wisdom", 1, modName,true));
			ret.Add(new GaugeMod("charisma", 1, modName,true));

			return ret;
		}
		public static List<GenericFeature> HalfOrcFeatures() {
			List<GenericFeature> ret = new List<GenericFeature> ();
			//Initial creation
			string modName = "Half-Orc Brawn";
			ret.Add(new GaugeMod("strength", 2, modName,true));
			ret.Add(new GaugeMod("constitution", 1, modName,true));

			return ret;
		}

		public static string Description(RaceName name) {
			string ret = "";
			switch (name) {
			case RaceName.HUMAN:
				ret = "Humans are the most common race in [some realm].  They get along pretty well with other races, and span a wide variety of alignments.  They are quite balanced, excelling in any profession with proper planning.";
				ret += "\n\n";
				ret += "Traits:\n";
				ret += "+1 All Abilities\n";
				ret += "Speed: 30ft\n";
				break;
			case RaceName.HALF_ORC:
				ret = "Half-orcs inherit a tendency toward  chaos from their orc parents and are not strongly inclined toward good. Half-orcs that are raised among orcs are usually evil. ";
				ret += "\n\n";
				ret += "Traits:\n";
				ret += "+2 Strength\n";
				ret += "+1 Constitution\n";
				ret += "Speed: 30ft\n";
				break;

			}


			return ret;
		}

		public static string ShortDescription(RaceName name) {
			string ret = "";
			switch (name) {
			case RaceName.HUMAN:
				ret += "+1 All Abilities\n";
				ret += "Speed: 30ft";
				break;
			case RaceName.HALF_ORC:
				ret += "+2 Strength\n";
				ret += "+1 Constitution\n";
				ret += "Speed: 30ft";
				break;

			}


			return ret;
		}


		public static Gauge SpeedTable(RaceName rn) {
			Gauge ret = new Gauge ("movement speed");
			switch (rn) {
			case RaceName.TIEFLING:
				ret.ChangeCurrentAndMax(6);
				break;
			case RaceName.HUMAN:
				ret.ChangeCurrentAndMax (6);
				break;
			case RaceName.HALF_ORC:
				ret.ChangeCurrentAndMax (6);
				break;
			case RaceName.ELF:
				ret.ChangeCurrentAndMax (6);
				break;
			case RaceName.HALF_ELF:
				ret.ChangeCurrentAndMax (6);
				break;
			case RaceName.GNOME:
				ret.ChangeCurrentAndMax (5);
				break;
			case RaceName.HAFLING:
				ret.ChangeCurrentAndMax (5);
				break;
			case RaceName.DWARF:
				ret.ChangeCurrentAndMax (5);
				break;
			case RaceName.DRAGONBORN:
				ret.ChangeCurrentAndMax (6);
				break;
			}

			return ret;


		}

	}

}