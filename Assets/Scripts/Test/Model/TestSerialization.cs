using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT.Serialization;
using AT.Character;
using AT.Character;
using System.IO;
using Dnd5eTest;

public class TestSerialization : TestModule {

	public TestSerialization() : base() {
       Test("That can serialize inventory items safely and correctly", () =>    {


           Sheet c = new Sheet();

           Assert(c.inventory != null);
           c.inventory.items[0] = new GenericArmour(EquipmentSubtype.ARMOUR_CHAINMAIL);

			c.inventory.items[1] = new GenericWeapon(EquipmentSubtype.SIMPLE_DAGGER);
           Manager.Serialize(c, TestPath);

           Sheet after = Manager.Deserialize<Sheet>(TestPath);
			Assert(after.inventory != null);
			Assert(after.inventory.items != null);
			GenericArmour arm = after.inventory.items[0] as GenericArmour;
			GenericWeapon wep = after.inventory.items[1] as GenericWeapon;
			Assert(arm.Subtype == EquipmentSubtype.ARMOUR_CHAINMAIL);
			Assert(wep.Subtype == EquipmentSubtype.SIMPLE_DAGGER);
       });
		
		Test ("That CanSerializeAFeatureFromAndToSubclass", () => {
			//create a subclass instance of a feature
			PerLevelHitPoints flhp = new PerLevelHitPoints (10, ClassType.FIGHTER);


			string path = Application.persistentDataPath + Path.DirectorySeparatorChar + "test123" ;

			Manager.Serialize (flhp, path);


			//System.Type t = System.Type.GetType ("AT.Characters.FirstLevelHitPointsWrapper");

			GenericFeature f = Manager.Deserialize<GenericFeature> (path);

			File.Delete (path);


			Sheet c = new Sheet ();
			int before = c.HitPoints;
			f.WhenActivatedOn (c);
			Assert (c.HitPoints != before);


			//create a subclass instance of a feature
			GaugeMod ability = new GaugeMod("dexterity", 10, "bullshit", true);
			Manager.Serialize (ability, TestPath);


			//System.Type t = System.Type.GetType ("AT.Characters.FirstLevelHitPointsWrapper");

			GenericFeature hamburgerFlip = Manager.Deserialize<GenericFeature> (TestPath);

			//		Debug.Log ("hf: " + hamburgerFlip.GetType());

			File.Delete (path);


			GaugeMod ab = (GaugeMod)hamburgerFlip;

			Assert(ab.isBase);


			Sheet cheese = new Sheet ();
			int before1 = cheese.Dexterity;

			ab.WhenActivatedOn (cheese);
			//		Debug.Log ("prop aft" + cheese.GaugeByName("dexterity").ModifiedCurrent);
			Assert (cheese.Dexterity != before1);
		});

		Test ("that CanSerializeFeatureBundle", () => {
			ClassLevel5e fb = new ClassLevel5e (ClassType.CLERIC, 0);
			fb.features.Add (new PerLevelHitPoints (10, ClassType.FIGHTER));
			fb.features.Add (new PerLevelHitPoints (4, ClassType.CLERIC));
			fb.features.Add (new PerLevelHitPoints (1, ClassType.ROGUE));


			string path = Application.persistentDataPath + Path.DirectorySeparatorChar + "Test";
			Manager.Serialize (fb, path);

			ClassLevel5e unse = Manager.Deserialize<ClassLevel5e> (path);
			File.Delete (path);

			Assert (unse.classType == fb.classType);

			for (int i = 0; i < fb.features.Count; i++) {
				PerLevelHitPoints before = (PerLevelHitPoints)fb.features [i];
				PerLevelHitPoints after = (PerLevelHitPoints)unse.features [i];
				Assert (before.amount == after.amount);
			}
		});

		Test ("that CanSerializeWeapon", () => {
			GenericWeapon sword = new Longsword ();
			Manager.Serialize (sword, TestPath);


			GenericWeapon after = Manager.Deserialize<GenericWeapon> (TestPath);
			File.Delete (TestPath);


			Assert (after.Dice[0] == sword.Dice[0]);
			Assert (after.DamageType == sword.DamageType);
			Assert (after.Subtype == sword.Subtype);
			Assert (after.Type == sword.Type);
			//after.WhenEquipped (new Sheet ());
		});

		Test ("that CanSerializeArmour", () => {
			GenericArmour armour = new PaddedArmour ();
 
			Manager.Serialize (armour, TestPath);


			GenericArmour after = Manager.Deserialize<GenericArmour> (TestPath);
			File.Delete (TestPath);

			Assert (armour.BaseAc == after.BaseAc);
			Assert (armour.Subtype == after.Subtype);
			Assert (armour.Type  == after.Type );
		});

		Test ("that CanSerializePaperDoll", () => {
			PaperDoll pd = new PaperDoll ();

			pd.slots.Add (EquipmentSlotType.BODY, new PaddedArmour ());
			pd.slots.Add (EquipmentSlotType.MAIN_HAND, new Longsword ());
			pd.slots.Add (EquipmentSlotType.OFF_HAND, new Dagger ());

			Manager.Serialize (pd, TestPath);


			PaperDoll after = Manager.Deserialize<PaperDoll> (TestPath);
			File.Delete (TestPath);

			Assert (after.slots.Count == after.slots.Count);
		});

		Test ("that CanSerializeGauge", () => {
			Gauge g = new Gauge ("hello");
			Manager.Serialize (g, TestPath);

			Gauge after = Manager.Deserialize<Gauge> (TestPath);
			File.Delete (TestPath);


			Assert (after.ModifiedCurrent == g.ModifiedCurrent);
			Assert (after.ModifiedMax == g.ModifiedMax);
			Assert (after.Name == g.Name);
		});

		Test ("that CanSerializeRace", () => {
			Race r = new Race (RaceName.TIEFLING);

			Manager.Serialize (r, TestPath);

			Race after = Manager.Deserialize<Race> (TestPath);

			Assert (after.name == r.name);

			Assert (after.speed.ModifiedCurrent == r.speed.ModifiedCurrent);

			int i = 0;

			foreach (GenericFeature f in after.features) {

				Assert (f.Name () == r.features [i].Name ());
				i++;
			}
		});

		Test ("that CanSerializeSheet", () => {
			Sheet c = new Sheet ();
			c.race = new Race (RaceName.TIEFLING);
			ClassLevel5e lvl1 = new ClassLevel5e (ClassType.FIGHTER, 0);
			lvl1.features.Add (new FighterLevels.ArcheryFightingStyle ());
			lvl1.features.Add(new FighterLevels.GreatWeaponFightingStyle());
			lvl1.features.Add(new FighterLevels.DuelingFightingStyle());
			lvl1.features.Add(new FighterLevels.DefenseFightingStyle());
			lvl1.features.Add(new FighterLevels.TwoWeaponFightingStyle());

			lvl1.InitDefaultFeatures();

			c.AddClassLevel (lvl1);

			c.ActivateFeatures ();

			Manager.Serialize (c, TestPath);

			//c.DeactivateFeatures ();

			Sheet after = Manager.Deserialize<Sheet> (TestPath);

			after.ActivateFeatures();


			Debug.Log ("rior " + c.HitPointsGauge.BaseModifierSum + ") " + c.HitPointsGauge.ToString() + " " +  c.ToString ());

			Debug.Log ("after " + after.HitPointsGauge.BaseModifierSum + ") "  + after.HitPointsGauge.ToString() + " " +  after.ToString ());




			Assert (after.ToString () == c.ToString ());
		});
	}

	public string TestPath { get {  return Application.persistentDataPath + Path.DirectorySeparatorChar + "Test"; } }


}
