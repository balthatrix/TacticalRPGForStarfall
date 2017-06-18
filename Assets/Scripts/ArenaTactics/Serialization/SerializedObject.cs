using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using AT.Character;

namespace AT.Serialization {

	public interface  SerializedObject {
		Wrapper GetSerializableWrapper();
	}


	public class Manager {
		public static BinaryFormatter formatter = new BinaryFormatter();
		public static void Serialize(SerializedObject obj, string pathAndFile) {
			//create a subclass instance of a feature

			//get it's wrapper class
			Wrapper w = obj.GetSerializableWrapper ();
			Debug.Log ("Serializing " + obj.GetType() + " to PATH: " + pathAndFile);

			FileStream file = File.Create (pathAndFile);
			formatter.Serialize (file, w);
			file.Close ();
		}

		/// <summary>
		/// Deserialize the specified pathAndFile, which should point to a serialized wrapper object.
		/// If the name of the serializeable class changes, some kind of migration will need to occur
		/// For example: if you 
		/// 1. serialize a serializable object, 
		/// 2. close the run/program
		/// 3. change the class name of that object's type
		/// 4. open and try to deserialize, you will get an error with not being able to push into the type the data at the path
		/// 
		/// On the other hand, if you:
		/// 1. serialize a serializable object, 
		/// 2. close the run/program
		/// 3. add a property to that serializable object's class and wrapper
		/// 4. open and try to deserialize, you will have a wrapper with a null value for the newly added field.
		/// 

		/// 
		/// Basically: You will need to create a migration of sorts when you decide to change the name of a serialized class.
		/// You might need to create a migration of sorts if properties are added to a serialized object and legacy
		/// files are loaded.  The same case applies when changing the field names on the serializeable wrapper class.
		/// If the name of a field of a wrapper class is changed, there will need to be a migration of sorts.
		/// </summary>
		/// <param name="pathAndFile">Path and file.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T Deserialize<T>(string pathAndFile) where T : SerializedObject {
			FileStream read = File.Open (pathAndFile, FileMode.Open);


			var wrap = formatter.Deserialize(read);
			read.Close ();

//			Debug.Log ("type wrapper (GetType()): " + w.GetType());



			//all wrappers have a GetInstance method.... which returns an instance of the subclass it wrapped.
			MethodInfo method = wrap.GetType().GetMethod("GetInstance");
			T serializedObject = (T) method.Invoke (wrap, new object[0]);


			return serializedObject;
		}
	}
}