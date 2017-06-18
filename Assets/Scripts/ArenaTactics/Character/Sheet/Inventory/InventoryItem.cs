using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT.Serialization;

namespace AT.Character {
	

	/// <summary>
	/// Inventory icon type is for mapping inventory items to icons shown in inventory or what not.
	/// </summary>
	public enum IconName {
		//inventory icons
		NONE,
		SWORD_BLACK,
		DAGGER,
		CHAIN_MAIL,
		LEATHER_ARMOUR,
		HORNED_HELM,
		SHIELD_METAL,
		SHIELD_WOOD,
		HANDAXE,

		//actions
		MELEE_ATTACK,
		RANGED_ATTACK,
		MOVE,
		DODGE,
		DASH,
		PICK_UP,
		DROP,
		GIVE,
		EQUIP,
		HEAL,
		BONUS_ACTION,
		INVENTORY,
		ARROW_UP,
		ARROW_DOWN,
		ARROW_LEFT,
		ARROW_RIGHT,
		HEART,
		HOURGLASS

	}

	/// <summary>
	/// Inventory item wrapper which gets derived by wrappers meant for subclasses derived from inv item.
	/// </summary>
	[System.Serializable]
	public class InventoryItemWrapper : Wrapper {
		public IconName iconType;
		public int weight;
		public string name;
		public int worth;

		public override SerializedObject GetInstance ()
		{
			return null;
		}

	}

	public abstract class InventoryItem : SerializedObject, Tooltipable {

		public virtual Wrapper GetSerializableWrapper() {

			InventoryItemWrapper inv = new InventoryItemWrapper ();
			inv.weight = Weight;
			inv.name = Name;
			inv.worth = Worth;
			inv.iconType = IconType;
			return inv;
		}

		public static void DressInstance(InventoryItem item, InventoryItemWrapper wrap) {
			if (item == null)
				Debug.LogError ("item is null.  need something to dress.");
			if (wrap == null)
				Debug.LogError ("dressing wrapper is null.  dressing null?");
			item.Weight = wrap.weight;
			item.Name = wrap.name;
			item.Worth = wrap.worth;
			item.IconType = wrap.iconType;
		}

		public virtual string ShortDescription() {
			string ret = Name + "\n";
			ret += Worth + "g\n";
			ret += Weight + "lb.";



			return ret;
		}

		public virtual void DressOptButtonForTooltip (OptButton opt, Tooltip.TooltipPosition pos, int offset) {
			opt.SetTooltipInfo (pos, offset, ShortDescription(), null);
		}

		/// <summary>
		/// The icon type, which maps to an icon for displaying in shops/inventory
		/// </summary>
		/// <returns>The type.</returns>
		public IconName IconType {
			get;
			set;
		}

		/// <summary>
		/// The weight of the item in lb.
		/// </summary>
		public int Weight{ get; set; }

		/// <summary>
		/// The name/label of the item displayed on hovering, or when browsing in a shop
		/// </summary>
		public virtual string Name {get; set;}

		/// <summary>
		/// The worth of the item in GP
		/// </summary>
		public int Worth {get; set;}
	}
}
