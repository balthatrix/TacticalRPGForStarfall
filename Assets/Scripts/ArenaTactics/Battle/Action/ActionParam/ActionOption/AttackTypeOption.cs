using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT.Character;
using AT.Battle;

namespace AT.Battle {
	/// <summary>
	/// Represents the choices an actor can make about what type of attack will be made....
	/// </summary>
	public class AttackTypeOption : ActionOption {
		public AttackTypeOption () : base() {
		}

		public override List<IActionOptionChoice> GetChoicesUnfiltered(Actor actor, Action attack) {
			//return attack type choices....
			List<IActionOptionChoice> ret = new List<IActionOptionChoice>();
			GenericWeapon mainhand = actor.CharSheet.MainHand ();
			Equipment offhand = actor.CharSheet.OffHand ();
			if (!mainhand.IsRanged ()) {
				ret.Add (new AttackTypeChoice (AttackType.MAINHAND_MELEE, actor.CharSheet.MainHand ()));
			}


			if (mainhand.IsThrown () || mainhand.IsRanged()) {
				ret.Add (new AttackTypeChoice (AttackType.MAINHAND_RANGED, actor.CharSheet.MainHand()));
			}


			if (offhand != null && offhand is GenericWeapon && !(offhand as GenericWeapon).IsRanged()) {
				ret.Add(new AttackTypeChoice(AttackType.OFFHAND_MELEE, actor.CharSheet.OffHand() as GenericWeapon));
			}

			if (offhand is GenericWeapon) {
				GenericWeapon wep = offhand  as GenericWeapon;
				if (wep.IsThrown () || wep.IsRanged()) {
					ret.Add (new AttackTypeChoice (AttackType.OFFHAND_RANGED, offhand as GenericWeapon));
				}
			}

			if (ret.Count == 0) {
				lastReasonForNoChoices = "No weapons available for attacking.  Really there should always be an option for fist?";
			}

			return ret;
		}
	}



	public enum AttackType {
		MAINHAND_MELEE,
		MAINHAND_RANGED, //could be a ranged weapon, like darts, a thrown, or a bow
		OFFHAND_MELEE,  //could be 
		OFFHAND_RANGED, //could be a thrown, or darts
		SPELL_MELEE, 
		SPELL_RANGED
	}

	/// <summary>
	///  Attack type choice. Represents the choice that an entity can, or has made about the type of attack that will be made
	/// </summary>
	public class AttackTypeChoice : IActionOptionChoice {


		public AttackType type;
		public GenericWeapon weapon;

		public AttackTypeChoice(AttackType type, GenericWeapon w) {
			this.type = type;
			this.weapon = w;
		}

		public string ValueLabel() {
			return Util.UtilString.EnumToReadable<AttackType> (type, 0);
		}

		public bool IsRanged() {

			List<AttackType> rangedTypes = new List<AttackType> () {
				AttackType.MAINHAND_RANGED,
				AttackType.OFFHAND_RANGED,
				AttackType.SPELL_RANGED
			};

			return rangedTypes.Contains (type);
		}

		public bool IsOffhand() {

			List<AttackType> offhandTypes = new List<AttackType> () {
				AttackType.OFFHAND_MELEE,
				AttackType.OFFHAND_RANGED
			};

			return offhandTypes.Contains (type);
		}

		public void DecorateOption(ActionButtonNode n) {
//			Debug.LogError ("HERE!");
			n.spriteLabel = IconDispenser.instance.SpriteFromIconName (weapon.IconType);
			n.label = weapon.Name;
			if (IsRanged ()) {
				if (weapon.IsThrown()) {
					n.label += " Throw"; 
				} else {
					n.label += " Shot";
				}
			} else {
				n.label += " Swing";
			}


			if (IsRanged ()) {
				n.cornerSpriteLabel = IconDispenser.instance.SpriteFromIconName (IconName.RANGED_ATTACK);
			} else {
				n.cornerSpriteLabel = IconDispenser.instance.SpriteFromIconName (IconName.MELEE_ATTACK);
			}

//			Debug.LogWarning (GetType () + " didn't override decorate option.");
		}
	}

}