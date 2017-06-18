using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT.Character;
using AT.Character;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;
using AT.Serialization;
using Dnd5eTest;
using System.Linq;

public class TestDriver : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		


		foreach (System.Type t in Util.Reflection.GetSubclasses<TestModule> ()) {
			TestModule instance = (TestModule) System.Activator.CreateInstance(t);
			instance.DoTests ();
		}
		TestRun.Done ();
	}


}

namespace Dnd5eTest {

	public class TestModule {
		public delegate void TestAction();
		public Dictionary<string, TestAction> tests;

		public string currentTestDescription;

		public TestModule() {
			tests = new Dictionary<string, TestAction> ();
		}

		public virtual void Test(string thatSomethingHappensDescription, TestAction a) {

			tests.Add (thatSomethingHappensDescription, a);
		}

		public virtual void Setup() {
			//set stuff up
		}

		public void Assert(bool theTruth){
			TestRun.Assert (theTruth, currentTestDescription);
		}

		public virtual void DoTests() {
			Setup ();

			//loop through tests, calling each
			foreach(string desc in tests.Keys) {
				try {
					currentTestDescription = desc;
					TestRun.Log ("Testing " + desc);
					tests [desc].Invoke ();
				} catch (System.Exception e) {

					TestRun.LogError("Error Testing " + desc + ": " + e.ToString() + e.StackTrace);
				}
			}

			TearDown ();
		}

		public virtual void TearDown() {
			//cleanup
		}
	}

	public class TestRun {
		static int successful = 0;
		static int failed = 0;
		static int errors = 0;
		static List<TestModule> testModules;

		public static void Log(string s) {
			Debug.Log (s);
		}
		public static void LogError(string s) {
			Debug.LogError(s);
			errors++;
		}


		public static void RegisterTests(TestModule testModule) {
			testModules.Add (testModule);
		}


		public static void Done() {
			Debug.Log ("Done...  " + (successful + failed) + " total assertions. "  + successful + " successes, "+ errors + " errors,  and " + failed + " failed assertions!");
		}

		public static void Assert(bool theTruth, string desc) {
			if (!theTruth) {
				failed++;
				Debug.LogWarning("Assertion failed in test:" +  desc);
			} else {
				successful++;
			}
		}
	}

}