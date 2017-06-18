using UnityEngine;
using System.Collections;
using AT.Character.Situation;

namespace AT.Character {

	public enum ProficiencyLevel {
		HALF,
		FULL,
		DOUBLE
	}



	public class Proficiency {
		
		ProficiencyLevel level;

		public float Ratio {
			get { return Proficiency.GetRatio(level); }
		} 
		public Proficiency(ProficiencyLevel lvl) {
			level = lvl;
		}



		public static int Bonus(int lvl) {
			int ret = 1;
			float lvlBonus = Mathf.Ceil ((float)lvl/4f);
			if (lvlBonus == 0)
				lvlBonus = 1f;
			return (int) (ret + lvlBonus);
		}

		public static float GetRatio(ProficiencyLevel lvl) {
			switch (lvl) {
			case ProficiencyLevel.HALF:
				return 0.5f;
				break;
			case ProficiencyLevel.FULL:
				return 1f;
				break;
			case ProficiencyLevel.DOUBLE:
				return 2f;
				break;
			default:
				return 0f;
			}
		}
	}


	/// <summary>
	/// WeaponProficiency: Having them on character sheets allows a character to add his/her proficiency bonus to attacks made with the weapon type
	/// </summary>
	public class WeaponProficiency : Proficiency {
		EquipmentSubtype type;

		public EquipmentSubtype Name {
			get { return type; }
		}
		public WeaponProficiency(ProficiencyLevel lvl,  EquipmentSubtype name) : base(lvl) {
			this.type = name;
		}


        public string PresentableName
        {
            get
            {
				return Util.UtilString.EnumToReadable<EquipmentSubtype>(type, 1);
            }
        }


    }

	public class ArmourProficiency : Proficiency {
		EquipmentType type;

		public EquipmentType Name {
			get { return type; }
		}

        public string PresentableName
        {
            get
            {
				if(type == EquipmentType.ARMOUR_SHIELD)
                	return Util.UtilString.EnumToReadable<EquipmentType>(type);
				else
					return Util.UtilString.EnumToReadable<EquipmentType>(type,1);
            }
        }


        public ArmourProficiency(ProficiencyLevel lvl,  EquipmentType name) : base(lvl) {
			this.type = name;
		}
	}



	/// <summary>
	/// SkillProficiency: Having them on character sheets allows a character to add his/her proficiency bonus to skill checks.
	/// </summary>
	public class SkillProficiency : Proficiency {
		AT.Character.Situation.SkillType type;

		public SkillType Type {
			get { return type; }
		}

		public SkillProficiency(ProficiencyLevel lvl,  SkillType type) : base(lvl) {
			this.type = type;
		}



        public string PresentableName
        {
            get
            {
                return Util.UtilString.EnumToReadable<SkillType>(type);
            }
        }
    }


	/// <summary>
	/// SaveProficiency: Having them on character sheets allows a character to add his/her proficiency bonus to saves forced upon them.
	/// </summary>
	public class SaveProficiency : Proficiency {
		AbilityType type;

		public AbilityType Type {
			get { return type; }
		}

		public SaveProficiency(ProficiencyLevel lvl,  AbilityType type) : base(lvl) {
			this.type = type;
		}


		public string PresentableName {
            get {
                return Util.UtilString.EnumToReadable<AbilityType>(type);
            }
        }

	}

}