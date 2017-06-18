using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AT.Character {
	public class  KnightKit : EquipmentKitFeature {

		public KnightKit() : base() {
			//add stuff to apply
			ups.Add((Sheet c) => {
				Longsword sword = new Longsword();
				c.PaperDoll.Equip(EquipmentSlotType.MAIN_HAND, sword, c);	
			});

			downs.Add((Sheet c) => {
				c.PaperDoll.Unequip(EquipmentSlotType.MAIN_HAND, c);	
				//c.PaperDoll.Unquip(EquipmentSlotType.MAIN_HAND, sword, c);	
			});

			//add stuff to apply
			ups.Add((Sheet c) => {
				GenericArmour chain = new GenericArmour(EquipmentSubtype.ARMOUR_CHAINMAIL);
				c.PaperDoll.Equip(EquipmentSlotType.BODY, chain, c);	
			});

			downs.Add((Sheet c) => {
				c.PaperDoll.Unequip(EquipmentSlotType.BODY, c);	
				//c.PaperDoll.Unquip(EquipmentSlotType.MAIN_HAND, sword, c);	
			});



			ups.Add((Sheet c) => {
				GenericShield shield = new GenericShield();
				c.PaperDoll.Equip(EquipmentSlotType.OFF_HAND, shield, c);	
			});

			downs.Add((Sheet c) => {
				c.PaperDoll.Unequip(EquipmentSlotType.OFF_HAND, c);	
				//c.PaperDoll.Unquip(EquipmentSlotType.MAIN_HAND, sword, c);	
			});
		}

		public override string Name()
		{
			return "Knight Kit";
		}


	}

}