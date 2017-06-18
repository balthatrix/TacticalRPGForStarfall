using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using AT.Character.Effect;
using AT.Serialization;

namespace AT.Character {
	
	[System.Serializable]
	public class GenericArmourWrapper:Wrapper {
		
		public EquipmentSubtype subtype;
		public Wrapper inventoryItemWrapper;

		public GenericArmourWrapper() {

		}

		//This should be virtual.  This way, if there are magical
		//armour with special when equipped functions, they can 
		//overried GetInstance, and create a new instance of
		//the subtype/magical armour from it...
		public override SerializedObject GetInstance() {
			GenericArmour ret = new GenericArmour ();
			InventoryItem.DressInstance (ret, inventoryItemWrapper as InventoryItemWrapper);
			GenericArmour.DressInstance (ret, this);
			return ret;
		}
	}

	public class GenericArmour : Equipment {
		public static void DressInstance(GenericArmour armour, GenericArmourWrapper wrap) {
			SetArmourProperties (armour, wrap.subtype);
		}


		public override Wrapper GetSerializableWrapper() {

			GenericArmourWrapper w = new GenericArmourWrapper ();
			w.subtype = Subtype;

			w.inventoryItemWrapper = base.GetSerializableWrapper ();

			return w;
		}

		public int BaseAc { get; set; }
		public int StrengthRecommendation { get; set;}


		public GenericArmour() {
			onEquippedEffects = new List<CharacterEquipmentEffect> ();
			onUnequippedEffects = new List<CharacterEquipmentEffect> ();
			AnimationControllerName = EquipmentAnimationControllerName.NOT_SET;
		}

		public GenericArmour(EquipmentSubtype t) {
			onEquippedEffects = new List<CharacterEquipmentEffect> ();
			onUnequippedEffects = new List<CharacterEquipmentEffect> ();									
			AnimationControllerName = EquipmentAnimationControllerName.NOT_SET;
			SetArmourProperties (this, t);
		}

		public override string ShortDescription() {
			string ret = Name + "\n";

			if (this.Type == EquipmentType.ARMOUR_SHIELD) {
				ret += "+2 AC\n";
			} else {
				ret += BaseAc + " AC\n";
			}

			if (StrengthRecommendation > 0) {
				ret += "Recommended Strength: " + StrengthRecommendation + "\n";
			}

			ret += Worth + "g\n";
			ret += Weight + "lb.";



			return ret;
		}




		//TODO: this should be an on equipped effect.
		//Basically this should be based on strength.
		//It's an implementation of the strength column on
		//the armor specs.
		public virtual bool SpeedPenalty(Sheet c) {
			return false;
		}

		//may be unlimited or capped based on armor
		public  virtual int MaxDexterityACModifier {
			get;
			set;
		}

		//TODO: make this an equipment effect instead.
		//may cause disadvantage on stealth rolls.
		public virtual bool StealthDisadvantage (Sheet c) {
			return false;
		}


		/// <summary>
		/// Armour property table.  Sets the basic properties for the armour subtypes.
		/// To see how enchantments are stored and set, see the Set equipment enchantment types.
		/// </summary>
		/// <param name="weapon">Weapon.</param>
		/// <param name="t">T.</param>
		public static void SetArmourProperties(GenericArmour armour, EquipmentSubtype t) {
			armour.Subtype = t;
			switch (t) {
			case EquipmentSubtype.ARMOUR_PADDED:
				armour.Type = EquipmentType.ARMOUR_LIGHT;
				armour.BaseAc = 11;
				armour.MaxDexterityACModifier = 30; //no max
				armour.onEquippedEffects.Add (ApplyStealthDisArmourEquipped);
				armour.onUnequippedEffects.Add (ApplyStealthDisArmourUnequipped);
				armour.FittingSlotTypes = new List<EquipmentSlotType> () { EquipmentSlotType.BODY };
				break;
			case EquipmentSubtype.ARMOUR_LEATHER:
				armour.Type = EquipmentType.ARMOUR_LIGHT;
				armour.BaseAc = 12;
				armour.MaxDexterityACModifier = 30; //no max
				armour.IconType = IconName.LEATHER_ARMOUR;
				armour.AnimationControllerName = EquipmentAnimationControllerName.BODY_LEATHERARMOUR;
				armour.FittingSlotTypes = new List<EquipmentSlotType> () { EquipmentSlotType.BODY };
				break;

			case EquipmentSubtype.ARMOUR_CHAINMAIL:
				armour.Type = EquipmentType.ARMOUR_MEDIUM;
				armour.BaseAc = 13;
				armour.MaxDexterityACModifier = 2; //medium max
				armour.AnimationControllerName = EquipmentAnimationControllerName.BODY_CHAINARMOUR;
				armour.StrengthRecommendation = 13;
				armour.IconType = IconName.CHAIN_MAIL;
				armour.FittingSlotTypes = new List<EquipmentSlotType> () { EquipmentSlotType.BODY };
				break;

			case EquipmentSubtype.ARMOUR_METAL_SHIELD:
				armour.Type = EquipmentType.ARMOUR_SHIELD;
				armour.onEquippedEffects.Add (ShieldEquipped);
				armour.onUnequippedEffects.Add (ShieldUnequipped);
				armour.AnimationControllerName = EquipmentAnimationControllerName.METAL_SHIELD;
				armour.IconType = IconName.SHIELD_METAL;
				armour.FittingSlotTypes = new List<EquipmentSlotType> () { EquipmentSlotType.OFF_HAND };
				break;
			case EquipmentSubtype.ARMOUR_HORNED_HELM:
				armour.Type = EquipmentType.ARMOUR_LIGHT; //horned helm is not really light armour.... in fact it's not really armour
				armour.AnimationControllerName = EquipmentAnimationControllerName.HEAD_HORNEDHELMET;
				armour.IconType = IconName.HORNED_HELM;
				armour.FittingSlotTypes = new List<EquipmentSlotType> () { EquipmentSlotType.HEAD };
				break;

			default:
				Debug.LogError ("issue. type was not valid: " + t);
				break;
			}
		}


		//Basic armour effects.... Rely on functional routines to alter situations.
		public static void ShieldEquipped(Equipment e, Sheet c) {
			c.OnACProduced += Add2ArmourClass;
		}
		public static void ShieldUnequipped(Equipment e, Sheet c) {

			c.OnACProduced -= Add2ArmourClass;
		}
		private static void Add2ArmourClass(AT.Character.Situation.AttackSituation sit) {
			sit.ArmorClassGauge.Modify (new Modifier (2, "shield"));
		}


		private static void ApplyStealthDisArmourEquipped(Equipment e, Sheet c) {
			c.OnProduceCheck += FlagStealthDisadvantage;
		}
		private static void ApplyStealthDisArmourUnequipped(Equipment e, Sheet c) {
			c.OnProduceCheck -= FlagStealthDisadvantage;
		}
		private static void FlagStealthDisadvantage(AT.Character.Situation.CheckSituation sit) {
			if (sit.skillType == AT.Character.Situation.SkillType.STEALTH) {
				sit.FlagDisadvantage ();
			}
		}


		private static void ApplySpeedPenaltyArmourEquipped(Equipment e, Sheet c) {
			if (c.GaugeByName ("strength").ModifiedCurrent < (e as GenericArmour).StrengthRecommendation) {
				c.GaugeByName("speed").Modify(new Modifier(-10, "armour penalty"));
				//c.GaugeByName("strength").OnChanged  -> this should update when strength is changed.  how should that work?
			}
		}
		private static void ApplySpeedPenaltyArmourUnequipped(Equipment e, Sheet c) {
			c.GaugeByName("strength").UnModify(c.GaugeByName("strength").FindModifierByTag("armour penalty"));
		}





	}



	public class PaddedArmour : GenericArmour {

		public PaddedArmour() : base() {
			SetArmourProperties (this, EquipmentSubtype.ARMOUR_PADDED);
		}

	}





	public class GenericShield : GenericArmour {
		


		public GenericShield() : base() {
			SetArmourProperties (this, EquipmentSubtype.ARMOUR_METAL_SHIELD);
		}
	}



}
