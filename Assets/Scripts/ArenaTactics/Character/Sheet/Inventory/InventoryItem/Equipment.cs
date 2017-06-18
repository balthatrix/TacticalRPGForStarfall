using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using AT.Character.Effect;
using AT.Serialization;

namespace AT.Character {
	
	public abstract class Equipment : InventoryItem {
		
		/// <summary>
		/// These should make it easier to reuse effects, like light weapons adding offhand bonus hit, any equipment adding weight, etc...
		/// </summary>
		public delegate void CharacterEquipmentEffect(Equipment e, Sheet c);
		[System.NonSerialized]
		public List<CharacterEquipmentEffect> onEquippedEffects;
		[System.NonSerialized]
		public List<CharacterEquipmentEffect> onUnequippedEffects;



		/// <summary>
		/// Applies effects to a sheet character by listening to stat production events on the character, and altering them.
		/// TODO:  use EquippedEffectActions instead.  It should make it much easier to serialize
		/// </summary>
		/// <param name="c">C.</param>
		public virtual void WhenEquipped(Sheet c) {
			foreach (CharacterEquipmentEffect effect in onEquippedEffects) {
				effect (this, c);
			}
		}

		public virtual void WhenUnEquipped(Sheet c) {
			foreach (CharacterEquipmentEffect effect in onUnequippedEffects) {
				effect (this, c);
			}
		}

		/// <summary>
		/// Equipment always has to "fit" into a slot type or another.
		/// used by equipment/inventory interface to tell whether to allow drops.
		/// NOTE: most equipment only fits into one slot.  Exceptions are light weapons, which can fit into 
		/// either off or on hand.
		/// </summary>
		/// <value>The equipment slot types.</value>
		public List<EquipmentSlotType> FittingSlotTypes { 
			get;
			set;
		}

		public EquipmentSlotType DefaultFitSlotType {
			get {
				if (FittingSlotTypes.Count <= 0) {
					return EquipmentSlotType.ERROR;
				}
				return FittingSlotTypes [0];
			}
		}

		/// <summary>
		/// The subtype of equipment, like plate or leather in the case of armour, or longsword or dagger in the case of weapons.
		/// It's used to determine proficiencies.
		/// </summary>
		public EquipmentSubtype Subtype {get; set;}

		/// <summary>                                             
		/// Type over-arching type of equipment
		/// martial/simple in the case of weapons
		/// heavy/light/medium/shield in the case of armour
		/// </summary>
		/// <value>The type.</value>
		public EquipmentType Type { get; set; }  //Martial/Simple weapons, or Heavy, Light, Medium

		/// <summary>
		/// Denotes what animation controller this piece of equipment has. 
		/// </summary>
		/// <value>The name of the animation controller.</value>
		public EquipmentAnimationControllerName AnimationControllerName {
			get;
			set;
		}


		/// <summary>
		/// Equipment names are the subtype by default....
		/// </summary>
		/// <value>The name.</value>
		public override string Name {
			get {
				if (base.Name == null || base.Name == "") {

					return Util.UtilString.EnumToReadable<EquipmentSubtype> (Subtype, 1);
				} else {
					return base.Name;
				}
			}
		}

	}


	/// <summary>
	/// Used in proficiencies to check bonuses to attacks, and in the name of the weapon.
	/// </summary>
	public enum EquipmentSubtype {
		//Weapons
		SIMPLE_CLUB,
		SIMPLE_DAGGER,
		SIMPLE_HANDAXE,
		MARTIAL_LONGSWORD,

		SIMPLE_FIST,

		//Armour Body
		ARMOUR_PADDED,
		ARMOUR_LEATHER,
		ARMOUR_CHAINMAIL,
		ARMOUR_PLATE,
		ARMOUR_HORNED_HELM,

		//Armour Shield
		ARMOUR_METAL_SHIELD,


		HELMET_HORNED

	}

	public enum EquipmentType {
		//Weapons
		WEAPON_SIMPLE,
		WEAPON_MARTIAL,
		WEAPON_UNARMED,

		//Armour
		ARMOUR_LIGHT,
		ARMOUR_MEDIUM,
		ARMOUR_HEAVY,
		ARMOUR_SHIELD,

		//Gear
		GEAR_HELMET,
		GEAR_BOOTS,
		GEAR_GLOVES
	}

	public enum WeaponProperty {
		LIGHT,
		HEAVY,
		FINESSE,
		REACH,
		THROWN,
		VERSATILE,
		RANGED
	}
}
