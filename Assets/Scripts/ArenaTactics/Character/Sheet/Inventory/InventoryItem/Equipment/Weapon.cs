using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using AT.Character.Effect;
using AT.Serialization;
using AT.Battle;
using System.Linq;

namespace AT.Character {




	[System.Serializable]
	public class GenericWeaponWrapper:Wrapper {
		//since the rest of basic weapon attributes are set by a table lookup, all that needs to be stored is the subtype
		public EquipmentSubtype subtype;
		public Wrapper inventoryItemWrapper;



		public GenericWeaponWrapper() {

		}

		//This should be virtual.  This way, if there are magical
		//weapons with special when equipped functions, they can 
		//overried GetInstance, and create a new instance of
		//the subtype from it...
		public override SerializedObject GetInstance() {
			
			GenericWeapon ret  = new GenericWeapon ();

			InventoryItem.DressInstance (ret, inventoryItemWrapper as InventoryItemWrapper);
			GenericWeapon.DressInstance (ret, this);
			return ret;
		}
	}



	public class GenericWeapon : Equipment {
		public override Wrapper GetSerializableWrapper() {

			GenericWeaponWrapper w = new GenericWeaponWrapper ();

			w.subtype = this.Subtype;

			w.inventoryItemWrapper = base.GetSerializableWrapper();

			return w;
		}

		public static void DressInstance(GenericWeapon weapon, GenericWeaponWrapper wrap) {
			GenericWeapon.SetWeaponProperties (weapon, wrap.subtype);
		}
		
		public DamageType DamageType { get; set; }
		public List<WeaponProperty> Properties { get; set; }
		public int[] Dice{ get; set; }

		public WeaponSwingFXType SwingSoundFX { get; set; }
		public int NormRng { get; set; }
		public int MaxRng { get; set; }

		public override string ShortDescription() {
			string ret = Name + "\n";
			ret += "Damage: " + Dice.Length + "d" + Dice [0] + "\n";

			foreach (WeaponProperty p in Properties) {
				ret += Util.UtilString.EnumToReadable<WeaponProperty> (p) + "\n";
			}

			if (this.IsRanged () || this.IsThrown()) {
				ret += "Normal Range: " + this.NormRng + "\n";
				ret += "Max Range: " + this.MaxRng + "\n";
			}

			ret += Worth + "g\n";
			ret += Weight + "lb.";



			return ret;
		}


		public GenericWeapon() {
			Properties = new List<WeaponProperty> ();
			NormRng = -1;
			MaxRng = -1;
			onEquippedEffects = new List<CharacterEquipmentEffect> ();
			onUnequippedEffects = new List<CharacterEquipmentEffect> ();
			SwingSoundFX = AT.WeaponSwingFXType.NONE;
			AnimationControllerName = EquipmentAnimationControllerName.NOT_SET;
			OffhandAnimationControllerName = EquipmentAnimationControllerName.NOT_SET;
			MissileAnimationName = MissileScript.MissileAnimationName.NOT_SET;
		}

		public GenericWeapon(EquipmentSubtype subtype) {
			Properties = new List<WeaponProperty> ();
			NormRng = -1;
			MaxRng = -1;
			onEquippedEffects = new List<CharacterEquipmentEffect> ();
			onUnequippedEffects = new List<CharacterEquipmentEffect> ();

			AnimationControllerName = EquipmentAnimationControllerName.NOT_SET;
			MissileAnimationName = MissileScript.MissileAnimationName.NOT_SET;

			SetWeaponProperties (this, subtype);
		}


			
		public bool IsRanged() {
			return this.Properties.Contains (WeaponProperty.RANGED);
		}

		public bool IsThrown() {
			return this.Properties.Contains (WeaponProperty.THROWN);
		}

		public override void WhenEquipped(Sheet c) {
			base.WhenEquipped (c);
			if (c.OffHand () == this) {
				c.OnDidAttack += AddDualWieldAttacking;
			}
		}

		public override void WhenUnEquipped(Sheet c) {     
			base.WhenUnEquipped (c);
			if (c.OffHand () == this) {
				c.OnDidAttack -= AddDualWieldAttacking;
			}
		}

		private void AddDualWieldAttacking(AT.Character.Situation.AttackSituation sit) {

			if (sit.action.IsReaction)
				return;
			Sheet character = sit.action.actor.CharSheet;
			if (character.HasTypeOfCondition<AT.Character.Condition.DualWieldAttacking> ()) {
				//Nothing
			} else {
				bool offhand = (sit.action.ActionOptions [0].chosenChoice as AttackTypeChoice).IsOffhand ();
				character.TakeCondition(new AT.Character.Condition.DualWieldAttacking (!offhand));
			}
		}




		public PhysicalDamage ProduceDamage() {
			PhysicalDamage ret = new PhysicalDamage (DamageType);
			foreach (int sidedDice in Dice) {
				int roll = Sheet.DiceRoll(sidedDice);
				ret.Gauge.ChangeCurrentAndMax (roll);
//				ret.Gauge.ChangeCurrentAndMax(sidedDice);
			}

			return ret;
		}

		/// <summary>
		/// Denotes what animation controller this weapon has when held in the offhand. 
		/// </summary>
		/// <value>The name of the animation controller.</value>
		public EquipmentAnimationControllerName OffhandAnimationControllerName {
			get;
			set;
		}

		public MissileScript.MissileAnimationName MissileAnimationName {
			get;
			set;
		}


		/// <summary>
		/// Weapon property table.  Sets the basic properties for the equipment subtype.
		/// To see how enchantments are stored and set, see the Set Weapon enchantment types.
		/// </summary>
		/// <param name="weapon">Weapon.</param>
		/// <param name="t">T.</param>
		public static void SetWeaponProperties(GenericWeapon weapon, EquipmentSubtype t) {

			weapon.FittingSlotTypes = new List<EquipmentSlotType> (){ EquipmentSlotType.MAIN_HAND };
			weapon.OffhandAnimationControllerName = EquipmentAnimationControllerName.NOT_SET;
			weapon.SwingSoundFX = AT.WeaponSwingFXType.NONE;

			switch (t) {
			case EquipmentSubtype.SIMPLE_CLUB:
				weapon.Subtype = EquipmentSubtype.SIMPLE_CLUB;
				weapon.Type = EquipmentType.WEAPON_SIMPLE;
				weapon.Dice = new int[]{ 6 };
				weapon.DamageType = DamageType.BLUDGEONING;
				weapon.Worth = 5;
				weapon.Properties.Add (WeaponProperty.LIGHT);
				weapon.FittingSlotTypes.Add (EquipmentSlotType.OFF_HAND);
				weapon.SwingSoundFX = AT.WeaponSwingFXType.LIGHT;
				break;

			case EquipmentSubtype.SIMPLE_DAGGER:
				weapon.Type = EquipmentType.WEAPON_SIMPLE;
				weapon.Subtype = EquipmentSubtype.SIMPLE_DAGGER;
				weapon.Dice = new int[]{ 4 };
				weapon.DamageType = DamageType.PIERCING;
				weapon.NormRng = 4;
				weapon.MaxRng = 12;
				weapon.Properties.Add (WeaponProperty.THROWN);
				weapon.Properties.Add (WeaponProperty.LIGHT);
				weapon.Properties.Add (WeaponProperty.FINESSE);
				weapon.AnimationControllerName = EquipmentAnimationControllerName.MAINHAND_DAGGER;
				weapon.OffhandAnimationControllerName = EquipmentAnimationControllerName.OFFHAND_DAGGER;
				weapon.IconType = IconName.DAGGER;
				weapon.MissileAnimationName = MissileScript.MissileAnimationName.DAGGER;
				weapon.FittingSlotTypes.Add (EquipmentSlotType.OFF_HAND);
				weapon.SwingSoundFX = AT.WeaponSwingFXType.LIGHT;
				break;


			case EquipmentSubtype.SIMPLE_HANDAXE:
				weapon.Type = EquipmentType.WEAPON_SIMPLE;
				weapon.Subtype = EquipmentSubtype.SIMPLE_HANDAXE;
				weapon.Dice = new int[]{ 6 };
				weapon.DamageType = DamageType.SLASHING;
				weapon.NormRng = 4;
				weapon.MaxRng = 12;
				weapon.AnimationControllerName = EquipmentAnimationControllerName.MAINHAND_HANDAXE;
				weapon.OffhandAnimationControllerName = EquipmentAnimationControllerName.OFFHAND_HANDAXE;
				weapon.MissileAnimationName = MissileScript.MissileAnimationName.HANDAXE;
				weapon.Properties.Add (WeaponProperty.THROWN);
				weapon.Properties.Add (WeaponProperty.LIGHT);
				weapon.IconType = IconName.HANDAXE;
				weapon.FittingSlotTypes.Add (EquipmentSlotType.OFF_HAND);
				weapon.SwingSoundFX = AT.WeaponSwingFXType.LIGHT;
				break;

			case EquipmentSubtype.MARTIAL_LONGSWORD:
				weapon.Subtype = EquipmentSubtype.MARTIAL_LONGSWORD;
				weapon.Type = EquipmentType.WEAPON_MARTIAL;
				weapon.Dice = new int[]{ 8 };
				weapon.DamageType = DamageType.SLASHING;
				weapon.Properties.Add (WeaponProperty.VERSATILE);
				weapon.AnimationControllerName = EquipmentAnimationControllerName.MAINHAND_LONGSWORD;
				weapon.IconType = IconName.SWORD_BLACK;
				weapon.SwingSoundFX = AT.WeaponSwingFXType.MEDIUM;
				break;

			case EquipmentSubtype.SIMPLE_FIST:
				weapon.Subtype = EquipmentSubtype.SIMPLE_FIST;
				weapon.Type = EquipmentType.WEAPON_UNARMED;
				weapon.Dice = new int[]{ 1 };
				weapon.DamageType = DamageType.BLUDGEONING;

				weapon.SwingSoundFX = AT.WeaponSwingFXType.LIGHT;
				break;

			default:
				Debug.LogError ("issue. type was not valid: " + t);
				break;
			}
		}
	}




	public class Club : GenericWeapon {
		public Club() : base()  {
			GenericWeapon.SetWeaponProperties (this, EquipmentSubtype.SIMPLE_CLUB);
		}
	}

	public class Longsword : GenericWeapon {
		public Longsword() : base() {
			GenericWeapon.SetWeaponProperties (this, EquipmentSubtype.MARTIAL_LONGSWORD);
		}

	}

	public class Unarmed : GenericWeapon {
		public Unarmed() : base()  {
			GenericWeapon.SetWeaponProperties (this, EquipmentSubtype.SIMPLE_FIST);
		}
	}



	public class Handaxe : GenericWeapon {
		public Handaxe() : base()  {
			GenericWeapon.SetWeaponProperties (this, EquipmentSubtype.SIMPLE_HANDAXE);

		}
	}


	public class Dagger : GenericWeapon {
		public Dagger() : base()  {
			GenericWeapon.SetWeaponProperties (this, EquipmentSubtype.SIMPLE_DAGGER);
		}
	}





}
