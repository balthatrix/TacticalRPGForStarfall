using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Linq;
using System.Collections.Generic;
using AT.Character.Situation;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization;
using System.IO;
using AT.UI;
using AT.Serialization;
using AT.Battle;

namespace AT.Character {
	

	//class features, or feat features, or race features can all use this...
	public interface Feature {
		void WhenActivatedOn(Sheet c);
		void WhenDeactivatedOn(Sheet c);


		string Description();
		string Name();
	}




	public class FeaturePointer : GenericFeature {
		public delegate List<GenericFeature> FilterPool (Sheet character);

		public FilterPool filterPool;
		public override void WhenActivatedOn(Sheet c) {}
		public override void WhenDeactivatedOn(Sheet c) {}
		public override bool IsMisc {
			get { return false; }
		}

		public override string Name() {
			return "Pointer to some features.";
		}

		public override string Description() {
			return "Pointer to some features.";
		}

		public List<GenericFeature> pool;
		public string headerText;


		public FeaturePointer(string headerText, List<GenericFeature> pool, FeatureBundle parent=null) : base(parent) {
			this.pool = pool;
			this.headerText = headerText;
			filterPool = (Sheet c) => pool;
		}


		public CharacterCustomizationStep GetCustomizationStep(CharacterCustomizationController cont) {
			return new ChooseFromPoolOfFeatures (this, cont);
		}
	}





	public class GenericFeature : Feature, SerializedObject, Tooltipable {


		public virtual void DressOptButtonForTooltip(OptButton opt, Tooltip.TooltipPosition pos= Tooltip.TooltipPosition.TOP, int offset=2) {
			opt.SetTooltipInfo (pos, offset, "Just some feature", null);
		}

		public virtual bool IsMisc {
			get { return true; }
		}

		public static List<System.Type> nonSerializedTypes = new List<System.Type> () {
			typeof(FeaturePointer),
			typeof(NonSerializedFeature)
		};


		public static bool ShouldNotSerialize(GenericFeature f) {
			bool ret = false;
			foreach (System.Type t in nonSerializedTypes) {
				if (f.GetType () == t || f.GetType ().IsSubclassOf (t)) {
					ret = true;
					break;
				}
			}
			return ret;
		}


		public FeatureBundle parent;

		public string nameOverride;


		public virtual Wrapper GetSerializableWrapper() {
			Debug.LogError (this.GetType().ToString() + " must override GetSerialization wrapper to support serialization");
			return null;
		}


		public GenericFeature(FeatureBundle parent=null) {
			this.parent = parent;
		}

		public GenericFeature() {
			
		}

		public virtual void WhenActivatedOn(Sheet c) {
			if(!(this is FeaturePointer)) 
				Debug.LogError ("Features should override activation routine");
		}


		public virtual void WhenDeactivatedOn(Sheet c) {
			if(!(this is FeaturePointer)) 
				Debug.LogError ("Features should override deactivation routine");
		}




		public virtual string Name() {
			Debug.LogError (GetType().ToString() + " MUST HAVE FEATURE NAME");
			return "ERROR";
		}

		public virtual string Description() {
			Debug.LogError (GetType().ToString() + " MUST HAVE FEATURE DESCRIPTION");
			return "ERROR";
		}


	}

	public enum SpecialFeatureType {
		SECOND_WIND,
	}
	public delegate void FeatureAppAction(Sheet CharacterInfo);

	[System.Serializable]
	public class SpecialFeatureWrapper : Wrapper {
		SpecialFeatureType type;
		public SpecialFeatureWrapper(SpecialFeatureType type) {
			this.type = type;
		}

		public override SerializedObject GetInstance ()
		{
			StatelessFeature ret = new StatelessFeature (type);
			return ret;
		}
	}

	/// <summary>
	/// Other than type, special features should remain stateless.
	/// They can however, access state on the character sheet they are being applied to.
	/// </summary>
	public class StatelessFeature : GenericFeature, SerializedObject  {
		SpecialFeatureType type;
		public  FeatureAppAction applied;
		public FeatureAppAction unapplied;
		public string description = "";
		public string shortDescription = "";

		public string encyclopediaPage = "some/path/to/page";

		public StatelessFeature(SpecialFeatureType type, FeatureBundle parent=null) : base(parent) {
			this.type  = type;
			SetEffectsAndDescription (this);
		}

		public override void WhenActivatedOn(Sheet character) {
			applied(character);
		}

		public override void WhenDeactivatedOn(Sheet character) {
			unapplied(character);
		}

		public override Wrapper GetSerializableWrapper() {
			SpecialFeatureWrapper wrap = new SpecialFeatureWrapper (this.type);
			return wrap;
		}
		public override string Name() {
			return Util.UtilString.EnumToReadable<SpecialFeatureType> (type);
		}

		public override string Description() {
			return description;
		}

		public override void DressOptButtonForTooltip (OptButton opt, Tooltip.TooltipPosition pos = Tooltip.TooltipPosition.TOP, int offset = 2)
		{
			opt.SetTooltipInfo (pos, offset, shortDescription, description);
		}

		/// <summary>
		///Sets the effects of a feature.  Doing this will allow us to freely change mechanics without needing to
		/// make migrations
		/// </summary>
		/// <param name="instance">Instance of special feature to set.</param>
		public static void SetEffectsAndDescription(StatelessFeature instance) {
			switch (instance.type) {
			case SpecialFeatureType.SECOND_WIND:
				instance.applied = (Sheet character) => {
					character.OnProduceBonusActions += AddSecondWindAction;
					character.OnDidPerform += (Action act) => {
						if (act is SecondWind) {
							character.metaData.SetMetaValue ("used_second_wind", true);
						}
					};

					//Todo: Add something to reset this value on rest...
				};
				instance.unapplied = (Sheet character) => {
					character.OnProduceBonusActions -= AddSecondWindAction;
				};
				instance.shortDescription = "The figher can recover lost hit points once per battle.";
				instance.description = "Once per battle, the fighter can spend a bonus action to regain hit points equal to 1d10 + its level in the fighter class.";
				break;
			default:
				Debug.LogError (instance.type + " is not set up on the special feature effects table");
				break;
			}
		}
        
		public static void AddSecondWindAction(Actor actor, List<Action> actions) {
            SecondWind act = new SecondWind();
            act.IsBonus = true;

            actions.Add(act);
			if (actor.CharSheet.metaData.GetMetaValue<bool>("used_second_wind"))
            {
                act.UseLimitReachedUntilLongRest = true;
//                Debug.LogError("you already used!!!!");
            }
			
		}
	}


}//character
