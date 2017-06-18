using System.Collections.Generic;
using System.Collections;
using UnityEngine;

using AT.Serialization;


namespace AT.Character {

	public enum EquipmentSlotType {
		BODY_OVERRIDE,  //this is just for overriding body animations at this point
		MAIN_HAND,
		BODY,
		OFF_HAND,
		HEAD,
		HANDS,
		FEET,
		FINGER,
		NECK, //necklace
		BACK, //cloak
		WAIST,
		ERROR
	}

	//manages equipment and inventory
	public class PaperDoll :  SerializedObject {

		[System.Serializable]
		private class PaperDollWrapper:Wrapper {
			public Wrapper[] equipment;
			public EquipmentSlotType[] slots;
			public EquipmentType[] equipmentTypes;

			public PaperDollWrapper() {

			}

			public override SerializedObject GetInstance() {
				PaperDoll ret = new PaperDoll ();

				List<EquipmentType> armours = new List<EquipmentType> () {
					EquipmentType.ARMOUR_HEAVY,
					EquipmentType.ARMOUR_LIGHT,
					EquipmentType.ARMOUR_MEDIUM,
					EquipmentType.ARMOUR_SHIELD
				};

				List<EquipmentType> weapons = new List<EquipmentType> () {
					EquipmentType.WEAPON_SIMPLE,
					EquipmentType.WEAPON_MARTIAL,
					EquipmentType.WEAPON_UNARMED
				};

//				List<EquipmentType> shields = new List<EquipmentType> () {
//					EquipmentType.ARMOUR_SHIELD
//				};

				for (int i = 0; i <  equipment.Length; i ++ ) {

					//notice the equipment doesn't get equipped, which means
					//the parent sheet will need to worry about that for itself.
					if (armours.Contains (equipmentTypes [i])) {
						GenericArmourWrapper wrap = (GenericArmourWrapper)equipment [i];
						GenericArmour e = (GenericArmour)wrap.GetInstance ();
						ret.slots.Add (slots [i], e);
					} else if (weapons.Contains (equipmentTypes [i])) {
						GenericWeaponWrapper wrap = (GenericWeaponWrapper)equipment [i];
						GenericWeapon e = (GenericWeapon)wrap.GetInstance ();
						ret.slots.Add (slots [i], e);
					} 
					//this should not be nessecary....
//					else if (shields.Contains(equipmentTypes [i])) {
//						//throw new UnityException ("No generic subclass for shield yet.  Cannot support deserialization");
//						GenericShield e = (GenericShield)equipment [i].GetInstance ();
//						ret.slots.Add (slots [i], e);
//					} 
					else {
						throw new UnityException ("There is no case for deserializing this equipment type: " + equipmentTypes [i]);
					}

//					Equipment e = (Equipment) equipment [i].GetInstance ();

				}

				return ret;
			}
		}

		public Wrapper GetSerializableWrapper() {
			PaperDollWrapper wrap = new PaperDollWrapper();
			wrap.equipment = new Wrapper[slots.Count];
			wrap.slots = new EquipmentSlotType[slots.Count];
			wrap.equipmentTypes = new EquipmentType[slots.Count];

			List<EquipmentSlotType> types = new List<EquipmentSlotType> ();

			int i=0;
			foreach (EquipmentSlotType s in slots.Keys) {
				wrap.slots [i] = s;
				Equipment equipped = slots [s];

				if (equipped is GenericArmour) {
					GenericArmour arm = (GenericArmour)equipped; 
					wrap.equipment [i] = arm.GetSerializableWrapper ();
				} else if (equipped is GenericWeapon) {
					GenericWeapon weap = (GenericWeapon)equipped; 
					wrap.equipment [i] = weap.GetSerializableWrapper ();
				} else {
					Debug.LogError ("Not yet supporting serialization for " + equipped.GetType ());
				}

				wrap.equipmentTypes [i] = equipped.Type;
				i++;
			}

			return wrap;
		}




		private GenericWeapon fistWeapon;


		public  Dictionary<EquipmentSlotType, Equipment> slots;


		public Equipment EquippedOn(EquipmentSlotType eqSlot) {
			Equipment e = null;
			if (slots.TryGetValue (eqSlot, out e)) {
				return e;
			}
			return null;
		}

		public PaperDoll() {
			slots = new Dictionary<EquipmentSlotType, Equipment> ();
			fistWeapon = new Unarmed ();
		}

		public GenericWeapon MainHand() {
			Equipment w = null;
			slots.TryGetValue (EquipmentSlotType.MAIN_HAND, out w);
			try {
				if(w != null)
					return (GenericWeapon) w;		
			} catch {
				Debug.LogError("failed to cast weapon from equipment slot!");
			}
			return fistWeapon;
		}

		public Equipment OffHand() {
			Equipment w = null;
			slots.TryGetValue (EquipmentSlotType.OFF_HAND, out w);
			return w;
		}

		public int Reach() {
			GenericWeapon w = MainHand ();
			if (w != null && w.Properties.Contains (WeaponProperty.REACH))
				return 2;
			else
				return 1;
		}

		public GenericArmour Armour() {
			Equipment a = null;
			slots.TryGetValue (EquipmentSlotType.BODY, out a);
			try {
				return (GenericArmour) a;		
			} catch {
				Debug.Log ("failed to cast armour from equipment slot!");
			}
			return null;

		}

		public Equipment Equip(EquipmentSlotType slot, Equipment e, Sheet character) {
			
			Equipment ret = null;
			Unequip (slot, character);
			if (e == null) {
//				Debug.LogError ("equipping null don't work, boy");
				return null;
			}
			slots.Add (slot, e);

			if (e != null) {
				Debug.Log ("equipping! on " + slot + " - " + e.Name + " - " + character.Name);
				e.WhenEquipped (character);
				character.CallEquipped (slot, e);
			}

			return ret;
		}


		public EquipmentSlotType SlotFor(Equipment equipped) {
			foreach (EquipmentSlotType type in slots.Keys) {
				if (slots [type] == equipped)
					return type;
			}
			return EquipmentSlotType.ERROR;
		}

		public Equipment Unequip(EquipmentSlotType slot, Sheet character) {
			Equipment ret = null;
			if (EquippedOn (slot) != null) {
				ret = EquippedOn (slot);
				//eq.ToInventory ();
//				if(ret != null)
//					Debug.Log ("UN equipping on " + slot + " - " + ret.Name + " - " + character.Name);
//				else
//					Debug.Log ("UN equipping on " + slot + " - " + character.Name);
				ret.WhenUnEquipped(character);
				character.CallUnequipped (slot, ret);
			} 
			slots.Remove(slot);

			return ret;
		}

		public Equipment Unequip(Equipment e, Sheet character) {
			Equipment ret = null;
			foreach (EquipmentSlotType slotType in slots.Keys) {
				if (slots [slotType] == e) {
					ret = Unequip (slotType, character);
					break;
				}
			}

			return ret;
		}




	}
}