using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Util {
	public class Reflection  {
		public static List<System.Type> GetSubclasses<T>() {
			return typeof(T).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(T))).ToList();
		}



	}



}