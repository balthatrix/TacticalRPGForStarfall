using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using AT.Character;
using AT.Character.Effect;
using UnityEngine.UI;
using AT.Battle;
using System.Linq;
using AT.Character.Situation;
using AT.Serialization;

namespace AT.Character {

	public class HitDice {
		public int sides;
		public bool used;

		public HitDice(int sides, bool used=false) {
			this.sides = sides;
			this.used = used;
		}
	}

	[System.Serializable]
	public class SheetWrapper:Wrapper {

		public string name;

		//feature bundles 
		public Wrapper[] classLevelsWrappers;
		public Wrapper raceWrapper;


		//Dont forget to set the propterties, dex, str, con, etc as well as adding them to the List of gauges.
		public Wrapper strengthWrapper;
		public Wrapper dexterityWrapper;
		public Wrapper constitutionWrapper;
		public Wrapper intelligenceWrapper;
		public Wrapper wisdomWrapper;
		public Wrapper charismaWrapper;

        public Wrapper metaDataWrapper;

        public Wrapper inventoryWrapper;

		public int damageTaken;


		//equipment
		public Wrapper paperDollWrapper;

		//hit dice
		public bool[] hitDiceUseage;

		public Wrapper attacksPerRoundWrapper;


		public SheetWrapper() {
		}

		public override SerializedObject GetInstance() {
			return new Sheet (this);
		}

		public SpellPoolElement[] spellPool;
		public SpellLibrary.SpellSlot[] spellSlots;

	}


	public class Sheet : AT.Character.Situation.DcProducer, SerializedObject {
        public string Name {
            get { return name; }
            set { name = value; }
        }

		public List<SpellPoolElement> spellPool = new List<SpellPoolElement>();
		public List<SpellLibrary.SpellSlot> spellSlots =  new List<SpellLibrary.SpellSlot>();



        /// <summary>
        /// meta data for features and stuff to hook into. 
        /// </summary>
        public SheetKeyValueStore metaData;

		public Wrapper GetSerializableWrapper() {
			SheetWrapper wrap = new SheetWrapper ();
            wrap.metaDataWrapper = metaData.GetSerializableWrapper();

            wrap.inventoryWrapper = inventory.GetSerializableWrapper();

			wrap.name = name;

            if(race != null)
                wrap.raceWrapper = race.GetSerializableWrapper ();

			wrap.classLevelsWrappers = new Wrapper[classLevels.Count];
			int i = 0;
			foreach (ClassLevel5e cl in classLevels) {
				wrap.classLevelsWrappers [i] = cl.GetSerializableWrapper ();
				i++;
			}

			wrap.spellPool = spellPool.ToArray ();
			wrap.spellSlots = spellSlots.ToArray ();

			wrap.strengthWrapper = strength.GetSerializableWrapper ();
			wrap.dexterityWrapper = dexterity.GetSerializableWrapper ();
			wrap.constitutionWrapper = constitution.GetSerializableWrapper ();
			wrap.intelligenceWrapper = intelligence.GetSerializableWrapper ();
			wrap.wisdomWrapper = wisdom.GetSerializableWrapper ();
			wrap.charismaWrapper = charisma.GetSerializableWrapper ();


			wrap.damageTaken = hitPoints.ModifiedMax - hitPoints.ModifiedCurrent;


			wrap.paperDollWrapper = paperDoll.GetSerializableWrapper ();

			wrap.hitDiceUseage = new bool[hitDice.Count];
			i = 0;
			foreach (HitDice hd in hitDice) {
				wrap.hitDiceUseage [i] = hd.used;
				i++;
			}

			wrap.attacksPerRoundWrapper = attacksPerRound.GetSerializableWrapper ();


			return wrap;
		}




		//serialized
        private string name;

		//serialized
        public Race race;


		//serialized
        private List<Gauge> gauges;


        public List<WeaponProficiency> weaponProficiencies;
        public List<ArmourProficiency> armourProficiencies;
        public List<SkillProficiency> skillProficiencies;
        public List<SaveProficiency> saveProficiencies;

		public bool CastsSpells() {
			return SpellCastingClasses ().Count > 0;
		}

		public List<ClassType> SpellCastingClasses() {
			List<ClassType> castingClasses = new List<ClassType> {
				ClassType.CLERIC,
				ClassType.WIZARD,
			};
			List<ClassType> ret = new List<ClassType> ();
			classLevels.Select ((lvl) => {
				if(castingClasses.Contains(lvl.classType) && !ret.Contains(lvl.classType)) {
					ret.Add(lvl.classType);
				}
				return lvl.classType;
			}).ToList();
			return ret;
		}

		public bool HasSpellCastingClass(ClassType type) {
			return SpellCastingClasses ().Contains (type);
		}

		//used state should be serialized
        public List<HitDice> hitDice;


        public static int DiceRoll(int sides) {
            return Random.Range(1, sides + 1);
        }



        public static int[] MultipleDiceRoll(int eachWithSides, int howMany) {
            int[] ret = new int[howMany];
            for (int i = 0; i < ret.Count(); i++) {
                ret[i] = DiceRoll(eachWithSides);
            }
            return ret;
        }


        public static int instCount = 0;
        

        public Sheet() {

            Sheet.instCount++;

            metaData = new SheetKeyValueStore();



            this.inventory = new Inventory();
            
            weaponProficiencies = new List<WeaponProficiency>();
            armourProficiencies = new List<ArmourProficiency>();
            skillProficiencies = new List<SkillProficiency>();
            saveProficiencies = new List<SaveProficiency>();

            hitDice = new List<HitDice>();

			conditions = new List<AT.Character.Condition.ICondition> ();

            InitGauges();
            InitRules();
            InitRace();
            InitClassLevels();
            ActivateFeatures();

//			attacksPerRound.ChangeCurrentAndMax (10);
            Name = "Sheet" + instCount;



			inventory.AddItem (new GenericWeapon (EquipmentSubtype.MARTIAL_LONGSWORD));
			inventory.AddItem (new GenericWeapon (EquipmentSubtype.SIMPLE_HANDAXE));
			inventory.AddItem (new GenericWeapon (EquipmentSubtype.SIMPLE_DAGGER));
			inventory.AddItem (new GenericWeapon (EquipmentSubtype.SIMPLE_DAGGER));
			inventory.AddItem (new GenericWeapon (EquipmentSubtype.SIMPLE_DAGGER));
			inventory.AddItem (new GenericWeapon (EquipmentSubtype.SIMPLE_DAGGER));
			inventory.AddItem (new GenericWeapon (EquipmentSubtype.SIMPLE_DAGGER));
			inventory.AddItem (new GenericWeapon (EquipmentSubtype.SIMPLE_DAGGER));

			inventory.AddItem (new GenericWeapon (EquipmentSubtype.SIMPLE_HANDAXE));
			inventory.AddItem (new GenericArmour (EquipmentSubtype.ARMOUR_LEATHER));
			inventory.AddItem (new GenericArmour (EquipmentSubtype.ARMOUR_METAL_SHIELD));

//			PaperDoll.Equip (EquipmentSlotType.MAIN_HAND, new Longsword (), this);
//			PaperDoll.Equip (EquipmentSlotType.ARMOUR, new GenericArmour(EquipmentSubtype.ARMOUR_CHAIN), this);
//			PaperDoll.Equip (EquipmentSlotType.OFF_HAND, new Handaxe (), this);





        }
        

        private InfluencerModAmount ConstitutionPerLvlHpBonus() {
			return new InfluencerModAmount((Gauge lvl) => {
				int bonus = AbilityScoreModifierValue(constitution) * lvl.ModifiedCurrent;

				return bonus;
			});
		}

		public Dictionary<ClassType, int> ClassTypesToIntLevels() {
			Dictionary<AT.Character.ClassType, int> types = new Dictionary<AT.Character.ClassType, int> ();
			if (classLevels.Count == 0) {
				return null;
			}
			foreach (ClassLevel5e lvl in classLevels) {
				int currLvl;
				if (types.TryGetValue (lvl.classType, out currLvl)) {
					types [lvl.classType]++;

				} else {
					types.Add (lvl.classType, 1);
				}
			}

			return types;
		}

		public Sheet(SheetWrapper wrap) {

			Name = wrap.name;

			if (wrap.metaDataWrapper != null)
				metaData = (SheetKeyValueStore)wrap.metaDataWrapper.GetInstance ();
			else
				metaData = new SheetKeyValueStore ();
			
			if (wrap.inventoryWrapper != null)
				inventory = (Inventory)wrap.inventoryWrapper.GetInstance ();
			else
				inventory = new Inventory ();
			
            weaponProficiencies = new List<WeaponProficiency>();
			armourProficiencies = new List<ArmourProficiency>();
			skillProficiencies = new List<SkillProficiency>();
			saveProficiencies = new List<SaveProficiency>();

			hitDice = new List<HitDice>();
			conditions = new List<AT.Character.Condition.ICondition> ();

            //InitRace();
            if (wrap.raceWrapper != null)  
                race = (Race) wrap.raceWrapper.GetInstance();


			strength = (Gauge)wrap.strengthWrapper.GetInstance();
			dexterity = (Gauge)wrap.dexterityWrapper.GetInstance ();
			constitution = (Gauge) wrap.constitutionWrapper.GetInstance ();
			intelligence = (Gauge) wrap.intelligenceWrapper.GetInstance ();
			wisdom = (Gauge) wrap.wisdomWrapper.GetInstance ();
			charisma = (Gauge) wrap.charismaWrapper.GetInstance ();

			constitution.OnChanged += UpdatedConstitution;	

			hitPoints = new Gauge("hitPoints");


			characterLevelGauge = new Gauge ("Character Level");

			if(wrap.spellPool != null)
				spellPool = wrap.spellPool.ToList ();
			if(wrap.spellSlots != null)
				spellSlots = wrap.spellSlots.ToList ();

            hitPoints.AddInfluencerGauge(characterLevelGauge, ConstitutionPerLvlHpBonus());

			attacksPerRound = (Gauge) wrap.attacksPerRoundWrapper.GetInstance ();

			paperDoll = (PaperDoll) wrap.paperDollWrapper.GetInstance ();

			foreach (EquipmentSlotType eqType  in paperDoll.slots.Keys) {
				
				paperDoll.slots[eqType].WhenEquipped (this);
			}

			inventory.AddItem (new GenericWeapon (EquipmentSubtype.SIMPLE_HANDAXE));
			inventory.AddItem (new GenericWeapon (EquipmentSubtype.SIMPLE_HANDAXE));
			inventory.AddItem (new GenericArmour (EquipmentSubtype.ARMOUR_METAL_SHIELD));

			gauges = new List<Gauge>();
			gauges.Add(strength);
			gauges.Add(constitution);
			gauges.Add(dexterity);
			gauges.Add(intelligence);
			gauges.Add(wisdom);
			gauges.Add(charisma);
			gauges.Add(attacksPerRound);
			gauges.Add(hitPoints);


			OnProduceActions += AddBasicActions;


			InitRules();

			classLevels = new List<ClassLevel5e> ();
			//InitClassLevels();
			foreach(Wrapper clasrap in wrap.classLevelsWrappers) {
				ClassLevel5e cl = (ClassLevel5e) clasrap.GetInstance ();
				AddClassLevel (cl);
			}
            

            ActivateFeatures();


			//Set hit points to be damaged as before:
			hitPoints.ChangeCurrent(-wrap.damageTaken);

			//Now set hit dice
			int i = 0;
			foreach(bool used in wrap.hitDiceUseage) { 
				hitDice [i].used = used;
				i++;
			}

		}

		public void AddClassLevel(ClassLevel5e cl) {
				
			classLevels.Add (cl);
			CharacterLevel = classLevels.Count;

		}

		public void RemoveClassLevel(ClassLevel5e cl) {
			classLevels.Remove (cl);
			CharacterLevel = classLevels.Count;

		}


        
		//serialized
        private PaperDoll paperDoll;
        

        //ability scores:
        private Gauge strength;
        public int Strength { get { return strength.ModifiedCurrent; } }
        private Gauge dexterity;
        public int Dexterity { get { return dexterity.ModifiedCurrent; } }
        private Gauge constitution;
        public int Constitution { get { return constitution.ModifiedCurrent; } }
        private Gauge intelligence;
        public int Intelligence { get { return intelligence.ModifiedCurrent; } }
        private Gauge wisdom;
        public int Wisdom { get { return wisdom.ModifiedCurrent; } }
        private Gauge charisma;
        public int Charisma { get { return charisma.ModifiedCurrent; } }

		private Gauge characterLevelGauge;
		public int CharacterLevel {
			get { 
				return characterLevelGauge.ModifiedCurrent;
			}

			set { 

				characterLevelGauge.SetMax (value);
				characterLevelGauge.SetCurrent (value);



			}
		}

        public static int AbilityScoreModifierValue(Gauge score) {
            int ret = (int)Mathf.Floor((score.ModifiedCurrent - 10) / 2f);
            return ret;
        }

		public static int AbilityScoreModifierValue(int score) {
			int ret = (int)Mathf.Floor((score - 10) / 2f);
			return ret;
		}

		public GenericWeapon MainHand() {
			return paperDoll.MainHand();
		}

		public Equipment OffHand() {
			return paperDoll.OffHand();
		}

		public PaperDoll PaperDoll {
			get { return paperDoll; }
		}

		public int Reach() {
			return PaperDoll.Reach();
		}



        public PhysicalDamage MainHandAttackDamageEffect(AttackSituation sit) {
            PhysicalDamage effect = paperDoll.MainHand().ProduceDamage();

			CallOnAttackDamageProduced (effect, sit);

            return effect;
        }

		public void CallOnAttackDamageProduced(Effect.PhysicalDamage effect, AttackSituation sit, bool offhand=false) {
			if (sit == null) {
				Debug.LogError ("situation is nullify");
			}
			if (OnAttackDamageProduced != null) {
				OnAttackDamageProduced(effect, sit, false);
			}
		}

        public PhysicalDamage MainHandAttackCritDamageEffect(AttackSituation sit) {
            PhysicalDamage effect = paperDoll.MainHand().ProduceDamage();
            PhysicalDamage effect2 = paperDoll.MainHand().ProduceDamage();
            effect.Gauge.Modify(new Modifier(effect2.Amount, "Critical Hit")); //add new weapon roll

            
			CallOnAttackDamageProduced(effect, sit);
            

            return effect;
        }

        public PhysicalDamage OffhandAttackDamageEffect(AttackSituation sit) {
			PhysicalDamage effect = ((GenericWeapon) paperDoll.OffHand()).ProduceDamage();
			CallOnAttackDamageProduced (effect, sit, true);
            return effect;
        }

        public PhysicalDamage OffhandAttackCritDamageEffect(AttackSituation sit) {
			PhysicalDamage effect = ((GenericWeapon) paperDoll.OffHand()).ProduceDamage();
			PhysicalDamage effect2 = ((GenericWeapon) paperDoll.OffHand()).ProduceDamage();
            effect.Gauge.Modify(new Modifier(effect2.Amount, "Critical Hit")); //add new weapon roll

			CallOnAttackDamageProduced (effect, sit, true);
            

            return effect;
        }







        public float WeaponProficiencyRatio(GenericWeapon w) {
			if (w.Subtype == EquipmentSubtype.SIMPLE_FIST)
				return 1f;
			
            WeaponProficiency wp = BestWeaponProficiencyFromType(w.Subtype);
            if (wp != null) {
                return wp.Ratio;
            }
            return 0f;
        }

		public ArmourProficiency BestArmourProficiencyFromType(EquipmentType type) {
			ArmourProficiency ret = null;
			foreach (ArmourProficiency ap in armourProficiencies) {
				if (ap.Name == type) {
					if (ret == null || ret.Ratio < ap.Ratio)
						ret = ap;
				}
			}
			return ret;
		}


		public WeaponProficiency BestWeaponProficiencyFromType(EquipmentSubtype n) {
            WeaponProficiency best = null;
            foreach (WeaponProficiency wp in weaponProficiencies) {
                if (wp.Name == n) {
                    if (best == null || best.Ratio < wp.Ratio)
                        best = wp;
                }
            }
            return best;
        }

        public SkillProficiency BestSkillProficiencyFromType(SkillType t) {
            SkillProficiency best = null;
            foreach (SkillProficiency sk in skillProficiencies) {
                if (sk.Type == t) {
                    if (best == null || best.Ratio < sk.Ratio)
                        best = sk;
                }
            }
            return best;
        }

        public SaveProficiency BestSaveProficiencyFromType(AbilityType t) {
            SaveProficiency best = null;

            foreach (SaveProficiency s in saveProficiencies) {
                if (s.Type == t) {
                    if (best == null || best.Ratio < s.Ratio)
                        best = s;
                }
            }
            return best;
        }






        /// <summary>
        /// Proficiency modifier.  The set bonus based on lvl that is applied to 
        /// skills, attacks, or saves depending on having proficiency or not.
        /// </summary>
        /// <returns>The modifier.</returns>
        public int ProficiencyModifier() {
            int raw = Proficiency.Bonus(CharacterLevel);
            return raw;
        }




        //movement allowed per round.  default is 6, which represents 30 ft
		private Gauge defaultSpeed;
		public Gauge MovementSpeedGauge { 
			get { 
				if (race == null) {
					if (defaultSpeed == null) {
						defaultSpeed = new Gauge ("speed");
						defaultSpeed.ChangeCurrentAndMax (6);

					}
					return defaultSpeed;

				} else {
					return race.speed; 
				}
			} 
		}

		public int MovementSpeed { 
			get { 
				return MovementSpeedGauge.ModifiedCurrent;
			}
		}

		public int MovementSpeedAsFeet { get 
			{ 
				if(race != null)
					return race.speed.ModifiedCurrent * 5; 
				return -1;
			} 
		} //5 feet represent a square

        //movement allowed per round.  default is 1
        private Gauge attacksPerRound;
        public int AttacksPerRound { get { return attacksPerRound.ModifiedCurrent; } }

		private Gauge hitPoints;
		public int HitPoints { get { return hitPoints.ModifiedCurrent; } }
		public float HitPointRatio { 
			get { 
				if (hitPoints.ModifiedMax == 0) {
					return 0f;
				}
				return hitPoints.ModifiedCurrent / (float) hitPoints.ModifiedMax;
			}
		}

        public Gauge HitPointsGauge { get { return hitPoints; } }


        public int ArmorClass {
            get {
                int universalBase = 10;
                GenericArmour a = paperDoll.Armour();
                if (paperDoll == null || a == null) {
                    return universalBase;
                }

				if (a.BaseAc > universalBase) {
					return a.BaseAc;
                } else {
                    return universalBase;
                }
            }
        }






        public void TakeDamageEffect(AT.Character.Effect.Damage damage, Action source = null) {
            if (OnWillBeDamaged != null) {
                OnWillBeDamaged(damage);
            }

            int am = damage.Amount;
            //TODO: implement DR and % Res

            if (am < 0)
                am = 0;

            hitPoints.ChangeCurrent(-am);

             Debug.Log("Damage summary (total :" + damage.Gauge.ModifiedCurrent + ") -> " + damage.Gauge.ToString());

            if (OnDamaged != null) {
                OnDamaged(damage, source);
            }

            if (am > 0 && HitPoints <= 0) {
                Died();
            }
        }

		public void TakeHealingEffect(AT.Character.Effect.Healing healing, Action source = null) {
			if (OnWillBeHealed != null) {
				OnWillBeHealed(healing);
			}

			int am = healing.Amount;

			if (am < 0)
				am = 0;


			if(!healing.Nullified)
				hitPoints.ChangeCurrent(am);



			//Debug.Log("Healing summary (total :" + healing.Gauge.ModifiedCurrent + ") -> " + healing.Gauge.ToString());


			if (OnHealed != null) {
				OnHealed(healing, source);
			}

			if (HitPoints > hitPoints.ModifiedMax) {
				hitPoints.ChangeCurrent (hitPoints.ModifiedMax - HitPoints);
			}

		}


		public List<AT.Character.Condition.ICondition> conditions;

		public bool HasTypeOfCondition<T>(){
			bool ret = false;
			foreach(AT.Character.Condition.ICondition con in conditions) {
				if (con is T) {
					ret = true;
					break;
				}
			}
			return ret;
		}
        public delegate void ConditionTakenAction(Sheet chara, AT.Character.Condition.ICondition c);
        public event ConditionTakenAction OnConditionTaken;
        public void TakeCondition(AT.Character.Condition.ICondition condition) {
			
            //condition = (condition.GetType()) condition;
			condition.ApplyTo(this);
			conditions.Add (condition);
            if (OnConditionTaken != null) {
                OnConditionTaken(this, condition);
            }

        }

        public delegate void ConditionRemovedAction(Sheet chara, AT.Character.Condition.ICondition c);
        public event ConditionRemovedAction OnConditionRemoved;
        public void DidRemoveCondition(AT.Character.Condition.ICondition condition) {
			conditions.Remove (condition);
            if (OnConditionRemoved != null) {
                OnConditionRemoved(this, condition);
            }
        }

		public void UpdatedConstitution(Gauge g) {
			hitPoints.UpdateInfluencerModAmount (characterLevelGauge);
		}

        void InitGauges() {
            
            strength = new Gauge("strength");
            strength.ChangeCurrentAndMax(8);
            constitution = new Gauge("constitution");
            constitution.ChangeCurrentAndMax(8);
            dexterity = new Gauge("dexterity");
            dexterity.ChangeCurrentAndMax(8);
            intelligence = new Gauge("intelligence");
            intelligence.ChangeCurrentAndMax(8);
            wisdom = new Gauge("wisdom");
            wisdom.ChangeCurrentAndMax(8);
            charisma = new Gauge("charisma");
            charisma.ChangeCurrentAndMax(8);

			characterLevelGauge = new Gauge ("Character Level");


			constitution.OnChanged += UpdatedConstitution;		
            
            attacksPerRound = new Gauge("attacksPerRound");
            attacksPerRound.ChangeCurrentAndMax(1);

            //TODO: 10 should be gotten by character class...
            //example here: http://www.5esrd.com/classes/fighter/
            hitPoints = new Gauge("hitPoints");

            //
			//Debug.LogError("Hit points are currently being set like they shouldn't be");
            //hitPoints.ChangeCurrentAndMax (10);



			hitPoints.AddInfluencerGauge(characterLevelGauge, ConstitutionPerLvlHpBonus());

            paperDoll = new PaperDoll();

            gauges = new List<Gauge>();
            gauges.Add(strength);
            gauges.Add(constitution);
            gauges.Add(dexterity);
            gauges.Add(intelligence);
            gauges.Add(wisdom);
            gauges.Add(charisma);
            gauges.Add(attacksPerRound);
            gauges.Add(hitPoints);



            OnProduceActions += AddBasicActions;

        }

		public int CarryWeight {
			get {
				return GaugeByName ("strength").ModifiedCurrent * 15;
			}
		}


        public void AddBasicActions(Actor self, List<Action> actions) {
            Attack attack = new Attack(self);
            //attack.useCondition = (Actor actor) => actor.AttacksLeft() > 0;
            actions.Add(attack);


            Dash dash = new Dash(self);
            actions.Add(dash);


            Dodge dodge = new Dodge(self);
            actions.Add(dodge);


			Wait wait = new Wait(self, false);
			actions.Add(wait);

			if (CastsSpells ()) {
				Cast cast = new Cast (self);

				actions.Add(cast);
			}




//			ActionPointer p = new ActionPointer("Inventory", );
//			

//			PickUp pickup = new PickUp (self);
//			actions.Add(pickup);
//
//			DropItem drop = new DropItem (self);
//			actions.Add(drop);
//
//
//			GiveItem give = new GiveItem (self);
//			actions.Add(give);
//
//			Equip equip = new Equip (self);
//			actions.Add (equip);


        }

		public bool IsProficientInAllWornEquipment() {
			bool ret = true; //assume proficient
			foreach (EquipmentSlotType slotType in PaperDoll.slots.Keys) {
				Equipment eq = PaperDoll.slots [slotType];


				if (eq is GenericWeapon) {
					WeaponProficiency prof = BestWeaponProficiencyFromType (eq.Subtype);
					if (prof == null) {
						ret = false;
						break;
					}
				} else if (eq is GenericArmour) {
					ArmourProficiency prof = BestArmourProficiencyFromType (eq.Type);
					if (prof == null) {
						ret = false;
						break;
					}
				}
			}
			return ret;
		}

        public Gauge GaugeByName(string gaugeName) {
            foreach (Gauge g in gauges) {

                if (g.Name == gaugeName)
                    return g;
            }
            return null;

        }

        public string GaugesDebug() {
            IEnumerable<string> strings = gauges.Select((m) => {
                return m.Name + " -> " + m.ToString();
            });



            return string.Join(" | ", strings.ToArray());

        }



        private void InitRace() {
            //			race = new Race(RaceName.TIEFLING);
        }


        private void InitClassLevels() {
            classLevels = new List<ClassLevel5e>();
//            ClassLevel5e lvl = new ClassLevel5e (ClassType.FIGHTER, 0);
//			lvl.InitDefaultFeatures();
//			AddClassLevel (lvl);
			//lvl.GetSerializableWrapper ();
			//classLevels.Add (lvl);
        }

        private string AbilityValuePlusBonusString(Gauge ability) {
            int mod = AbilityScoreModifierValue(ability);
            string sign = "";

            if (mod > 0) {
                sign = "+";

            }


            return ability.ModifiedCurrent.ToString() + " (" + sign + AbilityScoreModifierValue(ability) + ")";
        }

        public List<HitDice> UsedHitDice() {
            List<HitDice> ret = new List<HitDice>();
            foreach (HitDice hd in hitDice) {
                if (hd.used)
                    ret.Add(hd);
            }
            return ret;
        }

        public List<HitDice> AvailableHitDice() {
            List<HitDice> ret = new List<HitDice>();
            foreach (HitDice hd in hitDice) {
                if (!hd.used)
                    ret.Add(hd);
            }
            return ret;
        }

        public Dictionary<ClassType, int> ClassLevelCounts() {
            List<string> ret = new List<string>();

            Dictionary<ClassType, int> uniqs = new Dictionary<ClassType, int>();


            foreach (ClassLevel5e lvl in classLevels) {
                int count;
                if (uniqs.TryGetValue(lvl.classType, out count))
                {
                    uniqs[lvl.classType] = count + 1;
                }
                else {
                    uniqs.Add(lvl.classType, 1);
                }
            }

            return uniqs;
        } 


		public List<GenericFeature> AllMiscFeats(){ 
			List<GenericFeature> misc = new List<GenericFeature> ();

			foreach(ClassLevel5e lvl in classLevels) {
				foreach (GenericFeature f in lvl.features) {
					if (f.IsMisc)
						misc.Add (f);
				}
			}

			return misc;
		}


		//not serialized
		public AT.CharacterRules.Combat combatRules;
		public AT.CharacterRules.Abilities abilityRules;
		public AT.CharacterRules.Saves saveRules;
		private void InitRules() {
			combatRules = new AT.CharacterRules.Combat (this);
			abilityRules = new AT.CharacterRules.Abilities (this);
			saveRules = new AT.CharacterRules.Saves (this);
		}


		public Equipment Unequip(Equipment e) {
			return PaperDoll.Unequip (e, this);
		}


		public  void ActivateFeatures() {
			if(race != null) 
				race.ActivateFeatures (this);

			foreach (ClassLevel5e lvl in classLevels) {
				lvl.ActivateFeatures (this);
			}
		}

		public  void DeactivateFeatures() {
			if(race != null) 
				race.DeactivateFeatures (this);

			foreach (ClassLevel5e lvl in classLevels) {
				lvl.DeactivateFeatures (this);
			}
		}

		public ClassLevel5e LastClassLevelOfType(ClassType t) {
			for (int i = classLevels.Count - 1; i >= 0; i--) {
				if (classLevels [i].classType == t)
					return classLevels [i];
			}
			return null;
		}

		public delegate void EquippedAction(Sheet self, EquipmentSlotType slotType, Equipment equipment);
		public event EquippedAction OnEquipped;
		public void CallEquipped(EquipmentSlotType slotType, Equipment e) {
			if (OnEquipped != null)
				OnEquipped (this, slotType, e);
		}
		public void Equip(EquipmentSlotType slotType, Equipment e) {
			paperDoll.Equip (slotType, e, this);

		}

		public delegate void UnequippedAction(Sheet self, EquipmentSlotType slotType, Equipment equipped);
		public event UnequippedAction OnUnequipped;
		public void Unequip(EquipmentSlotType slotType, Equipment e) {
			paperDoll.Unequip (slotType, this);
		}
		public void CallUnequipped(EquipmentSlotType slotType, Equipment e) {
			if (OnUnequipped != null)
				OnUnequipped (this, slotType, e);
		}





		public delegate void DeathAction(Sheet s);
		public event DeathAction OnKilled;
		public void Died() {
			if (OnKilled != null) {
				OnKilled (this);
			}
		}

		public delegate void DamageAction(Damage effect, Action source = null);
		public event DamageAction OnDamaged;


		public delegate void WillBeDamagedAction(Damage effect);
		public event WillBeDamagedAction OnWillBeDamaged;

		public delegate void HealedAction(Healing effect, Action source = null);
		public event HealedAction OnHealed;


		public delegate void WillBeHealedAction(Healing effect);
		public event WillBeHealedAction OnWillBeHealed;


		public delegate void ProduceAttackDamageAction(PhysicalDamage effect, AttackSituation sit, bool offhand);
		public event ProduceAttackDamageAction OnAttackDamageProduced;



		public delegate void RollToHitAction(AttackSituation attSit, bool offhand);
		public event RollToHitAction OnToHitRoll;
		public void ProduceToHit(AttackSituation attSit, bool offhand=false) {
			
			if (OnToHitRoll != null) {
				OnToHitRoll (attSit, offhand);
			}
		}

		public delegate void ProduceACAction(AttackSituation attSit);
		public event ProduceACAction OnACProduced;
		public void ProduceAC(AttackSituation attSit) {
			if (OnACProduced != null) {
				OnACProduced (attSit);
			}
		}

		public int AttackAbilityModifier(string type) {
			int attackModifier = 0;
			if (type == "Dexterity") {
				attackModifier = Sheet.AbilityScoreModifierValue (GaugeByName("dexterity"));
			} else {
				attackModifier = Sheet.AbilityScoreModifierValue (GaugeByName("strength"));
			}
			return attackModifier;
		}



		public int ModifierValueFromWeapon(GenericWeapon w) {
			string abilityName = "Strength";
			if (w.IsRanged ()) {
				abilityName = "Dexterity";
			} else if ( w.Properties.Contains (WeaponProperty.FINESSE)) {
				int strMod = AttackAbilityModifier ("Strength");
				int dexMod = AttackAbilityModifier ("Dexterity");


				if (dexMod > strMod) {
					abilityName = "Dexterity";
				}
			}

			return AttackAbilityModifier (abilityName);
		}


		//serialized
		public  List<ClassLevel5e> classLevels;
        public Inventory inventory;


		public int ClassLevelIn(ClassType t, bool testMode=false) {
			int ret = 0;
			foreach (ClassLevel5e c in classLevels) {
				if (c.classType == t)
					ret++;
			}
			if (testMode) {
				return 11;
			}
			return ret;
		}

		public Equipment Equipment(EquipmentSlotType eqSlot) {
			return paperDoll.EquippedOn (eqSlot);
		}

	



		public int HypotheticalToHitBonus() {
			
			AttackSituation sample = new AttackSituation(this, this);
			sample.GetResult (true);
			return sample.ToHitGauge.ModifierSum;
		}

		public int HypotheticalSaveBonus(AbilityType at) {
			SaveSituation sample = new SaveSituation (this, this, at, SaveContext.SPELL);
			sample.GetResult (true);
			return sample.saveValue.ModifierSum;
		}


		public int HypotheticalOffhandToHitBonus() {
			if (OffHand () as GenericWeapon == null)
				return -1;
			Battle.Attack att = new Battle.Attack ();
			
			att.ForceFlagOffhand ();
			AttackSituation sample = new AttackSituation(this, this, att);
			sample.GetResult (true);
			return sample.ToHitGauge.ModifierSum;
		}




		public delegate void ProduceInitiativeAction(Gauge initiative);
		public event ProduceInitiativeAction OnProduceInitiative;
		public Gauge ProduceAndRollInitiative() {
			Gauge ret = new Gauge ("Initiative");
			ret.ChangeCurrentAndMax(DiceRoll (20));
			if (OnProduceInitiative != null) {
				OnProduceInitiative (ret);
			}
			return ret;
		}

		public int HypotheticalAC() {
			AttackSituation sample = new AttackSituation(this, this);
			sample.GetResult (true);
			return sample.ArmorClassGauge.ModifiedCurrent;
		}

		public delegate void AboutToAttackAction(AttackSituation attSit);
		public event AboutToAttackAction OnAboutToAttack;
		public void AboutToAttack(AttackSituation attSit) {
			if (OnAboutToAttack != null) {
				OnAboutToAttack (attSit);
			}
		}
		public delegate void DidAttackAction(AttackSituation attSit);
		public event DidAttackAction OnDidAttack;
		public void DidAttack(AttackSituation attSit) {
			if (OnDidAttack != null) {
				OnDidAttack (attSit);
			}
		}

		public delegate void AboutToBeAttackedAction(AttackSituation attSit);
		public event AboutToBeAttackedAction OnAboutToBeAttacked;
		public void AboutToBeAttacked(AttackSituation attSit) {
			if (OnAboutToBeAttacked != null) {
				OnAboutToBeAttacked (attSit);
			}
		}
		public delegate void WasAttackedAction(AttackSituation attSit);
		public event WasAttackedAction OnWasAttacked;
		public void WasAttacked(AttackSituation attSit) {
			if (OnWasAttacked != null) {
				OnWasAttacked (attSit);
			}
		}






		public delegate void ProduceBonusActionsAction(Actor self, List<Action> actions);
		public event ProduceBonusActionsAction OnProduceBonusActions;
		public void ProduceBonusActions(Actor actor, List<Action> actions) {
			if (OnProduceBonusActions != null) {
				OnProduceBonusActions (actor, actions);
			}
		}

		public delegate void ProduceActionsAction(Actor self, List<Action> actions);
		public event ProduceActionsAction OnProduceActions;
		public void ProduceActions(Actor actor, List<Action> actions) {
			if (OnProduceActions != null) {
				OnProduceActions (actor, actions);
			}
		}


		public delegate void TurnEndedAction(Actor a);
		public event TurnEndedAction OnTurnEnded;
		public void TurnEnded(Actor self) {
			if (OnTurnEnded != null) {

				OnTurnEnded (self);
			}
		}

		public delegate void TurnBeganAction(Actor a);
		public event TurnBeganAction OnTurnBegan;
		public void TurnBegan(Actor self) {
			if (OnTurnBegan != null) {
				OnTurnBegan (self);
			}
		}


		/// CHECK SITUATIONAL CODE
		/// 
		public delegate void ProduceCheckAction(CheckSituation request);
		public event ProduceCheckAction OnProduceCheck;
		public void ProduceCheck(CheckSituation request) {
			if (OnProduceCheck != null) {
				OnProduceCheck (request);
			}

		}

		public delegate void ProduceDcAction(CheckSituation request);
		public event ProduceDcAction OnProduceDcInCheck;
		public void ProduceDcInCheck(CheckSituation request) {
			if (OnProduceDcInCheck != null) {
				OnProduceDcInCheck (request);
			}
		}


		public delegate void WillResolveCheckAction(CheckSituation situation);
		/// <summary>
		/// Subscribe to a callback made by check situations right before calculating the success/failure
		/// Use this to create situational check bonuses
		/// </summary>
		public event WillResolveCheckAction OnAboutToResolveCheck;
		/// <summary>
		/// Called by check situations right before calculating the success/failure
		/// </summary>
		/// <param name="cs">Cs.</param>
		public void AboutToResolveCheck(CheckSituation cs) {
			if (OnAboutToResolveCheck != null) {
				OnAboutToResolveCheck (cs);
			}
		}

		public delegate void DidResolveCheckAction(CheckSituation situation);
		public event DidResolveCheckAction OnDidResolveCheck;
		public void DidResolveCheck(CheckSituation cs) {
			if (OnDidResolveCheck != null) {
				OnDidResolveCheck (cs);
			}
		}


		public delegate void AboutToResolveDcInCheckAction(CheckSituation sit);
        /// <summary>
        /// Event callback occuring just after producing dc in a check situation, and just before resolving the check.
        /// </summary>
		public event AboutToResolveDcInCheckAction OnAboutToResolveDcInCheck;
		public void AboutToResolveDcInCheck(CheckSituation sit) {
			if (OnAboutToResolveDcInCheck != null) {
				OnAboutToResolveDcInCheck (sit);
			}
		}

		public delegate void DidResolveDcInCheckAction(CheckSituation sit);
		public event DidResolveDcInCheckAction OnDidResolveDcInCheck;
		public void DidResolveDcInCheck(CheckSituation sit) {
			if (OnDidResolveDcInCheck != null) {
				OnDidResolveDcInCheck (sit);
			}
		}
		///CHECK SITUATIONAL CODE END









		/// SAVE SITUATIONAL CODE
		public delegate void ProduceSaveAction(SaveSituation request);
		public event ProduceSaveAction OnProduceSave;
		public void ProduceSave(SaveSituation request) {
			if (OnProduceSave != null) {
				OnProduceSave (request);
			}

		}

		public delegate void ProduceDcInSaveAction(SaveSituation request);
		public event ProduceDcInSaveAction OnProduceDcInSave;
		public void ProduceDcInSave(SaveSituation request) {
			if (OnProduceDcInSave != null) {
				OnProduceDcInSave (request);
			}
		}

		public delegate void WillResolveSaveAction(SaveSituation situation);
		/// <summary>
		/// Subscribe to a callback made by check situations right before calculating the success/failure
		/// Use this to create situational check bonuses
		/// </summary>
		public event WillResolveSaveAction OnAboutToResolveSave;
		/// <summary>
		/// Called by check situations right before calculating the success/failure
		/// </summary>
		/// <param name="cs">Cs.</param>
		public void AboutToResolveSave(SaveSituation cs) {
			if (OnAboutToResolveSave != null) {
				OnAboutToResolveSave (cs);
			}
		}

		public delegate void DidResolveSaveAction(SaveSituation situation);
		public event DidResolveSaveAction OnDidResolveSave;
		public void DidResolveSave(SaveSituation cs) {
			if (OnDidResolveSave != null) {
				OnDidResolveSave (cs);
			}
		}


		public delegate void AboutToResolveDcInSaveAction(SaveSituation sit);
		public event AboutToResolveDcInSaveAction OnAboutToResolveDcInSave;
		public void AboutToResolveDcInSave(SaveSituation sit) {
			if (OnAboutToResolveDcInSave != null) {
				OnAboutToResolveDcInSave (sit);
			}
		}

		public delegate void DidResolveDcInSaveAction(SaveSituation sit);
		public event DidResolveDcInSaveAction OnDidResolveDcInSave;
		public void DidResolveDcInSave(SaveSituation sit) {
			if (OnDidResolveDcInSave != null) {
				OnDidResolveDcInSave (sit);
			}
		}
        ///Save situational code end
        ///

        public delegate void WillPerformAction(Action a);
        public event WillPerformAction OnWillPerform;
        public void CallOnWillPerform(Action a)
        {

            if (OnWillPerform != null)
            {
                OnWillPerform(a);
            }
        }

        public delegate void DidPerformAction(Action a);
        public event DidPerformAction OnDidPerform;
        public void CallOnDidPerform(Action a)
        {
            if (OnDidPerform != null)
            {
                OnDidPerform(a);
            }
        }



        public override string ToString ()
		{
			string ret = "";
			if(Name != "") 
				ret += "--- " + Name + " ---\n\n";


			if (race != null && race.features.Count > 0 ) {
				ret += "-- " + Util.UtilString.EnumToReadable<RaceName>(race.name) + " Traits --\n";
				ret += "Speed: " + MovementSpeedAsFeet + "ft.\n";

				foreach (GenericFeature f in race.features) {
					ret += f.Name () + "\n"; 
				}
			}

			if (classLevels.Count > 0) {

				ret += "\n-- Class Levels --\n";
				Dictionary<ClassType, int> counts = ClassLevelCounts();
				foreach (ClassType t in counts.Keys) {
					ret += Util.UtilString.EnumToReadable<ClassType>(t) + "(" + counts[t] + ")";

				}
				ret += "\n";
			}


			ret += "\n-- Abilities --\n";
			ret += "Str: " + AbilityValuePlusBonusString(strength) + "\n";
			ret += "Dex: " + AbilityValuePlusBonusString(dexterity) + "\n";
			ret += "Con: " + AbilityValuePlusBonusString(constitution) + "\n";
			ret += "Int: " + AbilityValuePlusBonusString(intelligence) + "\n";
			ret += "Wis: " + AbilityValuePlusBonusString(wisdom) + "\n";
			ret += "Cha: " + AbilityValuePlusBonusString(charisma) + "\n";
			ret += "\n";

			ret += "Proficiency Bonus: " + ProficiencyModifier () + "\n\n";


			if (HitPoints >= 0) {
				ret += "-- Hit Points --\n";
				ret += HitPoints + "\n\n";

			}



			if (hitDice.Count > 0) {
				List<HitDice> available = AvailableHitDice ();
				if (available.Count > 0) {
					ret += "- Available Hit Dice -\n";
					foreach (HitDice hd in available) {
						ret += "d" + hd.sides + " ";
					}
					ret += "\n";
				}
				List<HitDice> used = UsedHitDice ();
				if (used.Count > 0) {
					ret += "- Used Hit Dice -\n";
					foreach (HitDice hd in used) {
						ret += hd.sides + " ";
					}
					ret += "\n";
				}
				ret += "\n";
			}



			if (armourProficiencies.Count > 0 ||
				skillProficiencies.Count > 0 ||
				saveProficiencies.Count > 0 ||
				weaponProficiencies.Count > 0) {

				ret += "-- Proficiencies --\n";

				if (saveProficiencies.Count > 0) {
					ret += "- Saves -" + "\n";
					foreach (SaveProficiency f in saveProficiencies) {
						ret += f.PresentableName + "\n";
					}
					ret += "\n";
				}

				if (armourProficiencies.Count > 0) {
					ret += "- Armour -" + "\n";
					foreach (ArmourProficiency f in armourProficiencies) {
						ret += f.PresentableName + "\n";
					}
					ret += "\n";
				}

				if (weaponProficiencies.Count > 0) {
					ret += "- Weapons -" + "\n";
					foreach (WeaponProficiency f in weaponProficiencies) {
						ret += f.PresentableName + "\n";
					}
					ret += "\n";
				}

				if (skillProficiencies.Count > 0) {
					ret += "- Skills -" + "\n";
					foreach (SkillProficiency f in skillProficiencies) {
						ret += f.PresentableName + "\n";
					}
					ret += "\n";
				}
			}


			List<GenericFeature> misc = AllMiscFeats ();
			if (misc.Count > 0) {
				ret += "-- Feats --\n";
				foreach(GenericFeature f in misc) {
					ret +=  f.Name () +  "\n";
				}
			}



			return ret;
		}



	}





}


