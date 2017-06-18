using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace AT.Serialization {
	//classes will override this for their own wrappers...
	[Serializable]
	public class Wrapper {


		//perhaps heres where there could be a version?
		public int version = 0;


		public Wrapper() {
		}

		public virtual SerializedObject GetInstance() {
			throw new UnityException (this.GetType().ToString() + " should override GetInstance!");
		}


	}

}